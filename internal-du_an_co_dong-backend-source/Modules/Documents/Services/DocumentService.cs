using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Documents.Entities;
using Project.Modules.Documents.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Documents.Services
{
    public interface IDocumentService
    {
        DocumentFile Stored(StoredDocumentRequest storedDocumentRequest,string userId);
        DocumentFile Delete(DocumentParameterRequest deleteRequest);
        object ShowAll(RequestTable requestTable,string eventId);
        DocumentFile FindId(string documentId,string eventId);
        DocumentFile Update(string documentId,UpdateDocumentRequest updateDocumentRequest);
    }
    public class DocumentService : IDocumentService
    {
        private readonly MariaDBContext _mariaDBContext;
        private readonly IConfiguration _configuration;
        public DocumentService(MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;
        }

        public DocumentFile Delete(DocumentParameterRequest deleteRequest)
        {
            DocumentFile documentFile = FindId(deleteRequest.DocumentId, deleteRequest.EventId);
            if (documentFile == null)
            {
                return null;
            }
            _mariaDBContext.DocumentFiles.Remove(documentFile);
            _mariaDBContext.SaveChanges();
            return documentFile;
        }
        public DocumentFile FindId(string documentId, string eventId)
        {
            DocumentFile documentFile = _mariaDBContext.DocumentFiles.FirstOrDefault(x => x.DocumentID.Equals(documentId) && x.EventId.Equals(eventId));
            if (documentFile == null)
                return null;
            return documentFile;
        }

        public object ShowAll(RequestTable requestTable, string eventId)
        {
            List<DocumentFile> documentFiles = _mariaDBContext.DocumentFiles.Where(x => x.EventId.Equals(eventId)).ToList();
            foreach (var documentFile in documentFiles)
            {
                User user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(documentFile.UserId));
                documentFile.UserName = user?.FullName;
            }
            documentFiles = documentFiles.Where(x => x.DocumentName.Contains(requestTable.Search)).ToList();
            documentFiles.ForEach(x => x.DocumentLink = _configuration["MediaService:MediaUrl"] +"/"+ x.DocumentLink);
            ResponseTable response = new ResponseTable
            {
                DateResult = requestTable.Page == 0 ?
                            documentFiles :
                            documentFiles.Skip((requestTable.Page - 1) * requestTable.Results).Take(requestTable.Results),
                Info = new Info
                {
                    Page = requestTable.Page,
                    Results = requestTable.Results,
                    TotalRecord = documentFiles.Count()
                }
            };
            return response;
        }

        public DocumentFile Stored(StoredDocumentRequest storedDocumentRequest, string userId)
        {
            Uri linkDocument = new Uri(storedDocumentRequest.DocumentLink);
            DocumentFile documentFile = new DocumentFile()
            {
                EventId = storedDocumentRequest.EventId,
                DocumentType = storedDocumentRequest.DocumentType,
                DocumentLink = linkDocument.PathAndQuery.Substring(1),
                DocumentName = storedDocumentRequest.DocumentName,
                UserId = userId,
            };
            _mariaDBContext.DocumentFiles.Add(documentFile);
            _mariaDBContext.SaveChanges();
            User user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            documentFile.UserName = user?.FullName;
            return documentFile;
        }

        public DocumentFile Update(string documentId, UpdateDocumentRequest updateDocumentRequest)
        {
            DocumentFile documentFile = FindId(documentId, updateDocumentRequest.EventId);
            if (documentFile == null)
            {
                return null;
            }
            Uri linkDocument = new Uri(updateDocumentRequest.DocumentLink);
            documentFile.DocumentLink = linkDocument.PathAndQuery.Substring(1);
            documentFile.DocumentType = updateDocumentRequest.DocumentType;
            documentFile.DocumentName = updateDocumentRequest.DocumentName;
            _mariaDBContext.SaveChanges();
            return documentFile;
        }
    }
}
