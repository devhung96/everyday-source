using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Project.App.Database;
using Project.App.Helpers;
using Project.Modules.DeClarations.Entites;
using Project.Modules.FileUploads.Entities;
using Project.Modules.FileUploads.Request;
using Project.Modules.Products.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Services;
using Project.Modules.SellSeals;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FileUploads.Service
{
    public interface IFileUploadService
    {
        Task<(object data, string message)> UploadFileAsync(RequestUploadFile requestUploadFile, string token);
        (object data, string message) GetAllFileWithType(FileTye fileTye,string hostRequest);
    }
    public class FileUploadService: IFileUploadService
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public readonly ISealImportService sealImportService;
        public readonly IServiceUser serviceUser;
        public readonly ISellSealSerivce sellSealSerivce;

        public FileUploadService(IConfiguration _config, MariaDBContext _dBContext, ISealImportService _sealImportService, IServiceUser _serviceUser, ISellSealSerivce _sellSealSerivce)
        {
            config = _config;
            dBContext = _dBContext;
            sealImportService = _sealImportService;
            serviceUser = _serviceUser;
            sellSealSerivce = _sellSealSerivce;
        }

        public (object data, string message) GetAllFileWithType(FileTye fileTye, string hostRequest)
        {
            var listFile = dBContext.FileUploads.Include(x=>x.User).Where(x => x.FileType == fileTye).OrderByDescending(x => x.CreatedAt).Select(x => new {
                fileid = x.FileID,
                filename = x.FileName,
                filepath = hostRequest + "/" + x.FilePath,
                createdAt = x.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                userupload = new
                {
                    userid = x.UserID,
                    username = x.User.UserName
                }
            });

            return (listFile, "Lấy danh sách file thành công.");
        }


        public class Tmp
        {
            public string STK { get; set; }
            public string NgayTK { get; set; }
            public string TenHang { get; set; }
            public string MaHang { get; set; }
            public string DVT { get; set; }
            public double Gia { get; set; }
            public HangNhapBan HangNhap { get; set; }
            public HangNhapBan HangBan { get; set; }
            public HangXuat HangXuat { get; set; }

            public Tmp()
            {
                HangNhap = new HangNhapBan();
                HangBan = new HangNhapBan();
                HangXuat = new HangXuat();
            }

        }

        public class HangNhapBan
        {
            public int SoLuong { get; set; }
            public double TriGia { get; set; }
        }
        public class HangXuat
        {
            public int SoLuong { get; set; }
            public double TriGia { get; set; }
            public string STk { get; set; }
            public string NgayTK { get; set; }
        }

        public (SealImportRequest, SellData, string) ImportData(RequestUploadFile requestUploadFile, ExcelWorksheet worksheet, int row, FileInfo fileInfo)
        {
            if (string.IsNullOrEmpty(worksheet.Cells[row, 2].Text.Trim()) && row == 3)
            {
                return (null, null, "File không có dữ liệu.");
            }

            if (requestUploadFile.fileType == Entities.FileTye.Sell)
            {
                SellData sellData = new SellData();
                try
                {
                    int soluong = int.Parse(worksheet.Cells[row, 4].Text.Trim());
                    sellData.Number = soluong;
                }
                catch
                {
                    fileInfo.Delete();
                    return (null, null, "Số lượng không đúng định dạng.");
                }
                try
                {
                    DateTime dateTime = DateTime.ParseExact(worksheet.Cells[row, 8].Text.Trim(), "dd/MM/yyyy", null);
                    sellData.FlightDate = dateTime;
                }
                catch
                {
                    fileInfo.Delete();
                    return (null, null, "Định dạy ngày không hợp lệ. Định dạng đúng là dd/MM/yyyy");
                }

                try
                {
                    string priceStr = worksheet.Cells[row, 5].Text.Trim();
                    double? price = null;
                    if (!String.IsNullOrEmpty(priceStr))
                    {
                        price = double.Parse(priceStr);
                    }
                    sellData.Price = price;
                }
                catch
                {
                    fileInfo.Delete();
                    return (null, null, "Giá bán không đúng định dạng số.");
                }
                sellData.JDECode = worksheet.Cells[row, 2].Text.Trim();
                sellData.FlightNo = worksheet.Cells[row, 7].Text.Trim();
                sellData.Invoice = worksheet.Cells[row, 9].Text.Trim();
                sellData.Customer = worksheet.Cells[row, 10].Text.Trim();
                sellData.Passport = worksheet.Cells[row, 11].Text.Trim();
                sellData.Nationality = worksheet.Cells[row, 12].Text.Trim();
                sellData.SeatNumber = worksheet.Cells[row, 13].Text.Trim();
                sellData.Currency = worksheet.Cells[row, 6].Text.Trim();
                return (null, sellData, "Success");
            }
            else
            {
                SealImportRequest importRequest = new SealImportRequest
                {
                    SealNumber = worksheet.Cells[row, 2].Text.Trim(),
                    FlightNumber = worksheet.Cells[row, 3].Text.Trim()
                };

                try
                {
                    DateTime dateTime = DateTime.ParseExact(worksheet.Cells[row, 4].Text.Trim(), "dd/MM/yyyy", null);
                    importRequest.FlightDate = dateTime;
                }
                catch
                {
                    fileInfo.Delete();
                    return (null, null, "Định dạng ngày không hợp lệ. Định dạng đúng là dd/MM/yyyy");
                }
                importRequest.AcReg = worksheet.Cells[row, 5].Text.Trim();
                importRequest.SealNumberReturn = worksheet.Cells[row, 6].Text.Trim();

                return (importRequest, null, "Success");
            }
        }

        public async Task<(object data, string message)> UploadFileAsync(RequestUploadFile requestUploadFile, string token)
        {
            try
            {
                string path = requestUploadFile.fileType == Entities.FileTye.Seal ? "Seal" : "Sell";
                string fileFullName = await GeneralHelper.UploadFile(requestUploadFile.fileUpload, path);
                string file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path, fileFullName);
                FileInfo fileInfo = new FileInfo(file);

                List<SealImportRequest> sealImportRequests = new List<SealImportRequest>();
                List<SellData> sellDatas = new List<SellData>();

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    int sheet = 0;
                    int columns = 5;
                    if (requestUploadFile.fileType == Entities.FileTye.Sell)
                    {
                        sheet = 1;
                        columns = 11;
                    }

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheet];
                    if (worksheet.Dimension.Columns < columns)
                        return (null, "File không đúng định dạng.");
                    for (int row = 3; row <= worksheet.Dimension.Rows; row++)
                    {
                        if(string.IsNullOrEmpty(worksheet.Cells[row, 2]?.Text) && string.IsNullOrEmpty(worksheet.Cells[row, 3]?.Text))
                        {
                            continue;
                        }

                        (SealImportRequest sealImportRequest, SellData sellData, string messageImport) = 
                            ImportData(requestUploadFile, worksheet, row, fileInfo);

                        if(sealImportRequest == null && sellData == null)
                        {
                            return (null, messageImport);
                        }    

                        if(sealImportRequest != null)
                        {
                            sealImportRequests.Add(sealImportRequest);
                        }

                        if (sellData != null)
                        {
                            sellDatas.Add(sellData);
                        }
                    }
                }
                (object data, string message) = (null, null);
                if (requestUploadFile.fileType == Entities.FileTye.Seal)
                {
                    (data, message) = sealImportService.Import(sealImportRequests);
                }
                else
                {
                    (data, message) = sellSealSerivce.UpdateSellData(sellDatas);
                }

                if (data == null)
                {
                    fileInfo.Delete();
                    return (null, message);
                }
                User user = serviceUser.DecodeTokenGetIdUser(token);
                FileUpload fileUpload = new FileUpload
                {
                    FileType = requestUploadFile.fileType,
                    FilePath = Path.Combine(path, fileFullName),
                    FileName = fileInfo.Name,
                    UserID = user.UserId
                };
                dBContext.FileUploads.Add(fileUpload);
                dBContext.SaveChanges();
                return (data, message);
            }
            catch
            {
                return (null, "File không đúng định dạng.");
            }
        }
    }

    public class SellData
    {
        public string JDECode { get; set; }
        public int Number { get; set; }
        public double? Price { get; set; }
        public string Currency { get; set; }
        public string FlightNo { get; set; }
        public DateTime FlightDate { get; set; }

        public string Invoice { get; set; }
        public string Customer { get; set; }
        public string Passport { get; set; }

        public string Nationality { get; set; }
        public string SeatNumber { get; set; }
    }

}
