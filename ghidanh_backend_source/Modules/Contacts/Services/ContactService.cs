using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Contacts.Entities;
using Project.Modules.Contacts.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Contacts.Services
{
    public interface IContactService
    {
        public (object data, string message) ShowAllContact(RequestTable requestTable);
        public (Contact data, string message) CreateContact(CreateContactRequest valueInput);
        public (Contact data, string message) ShowDetailContact(string contactId);
    }
    public class ContactService : IContactService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        public ContactService(IRepositoryMariaWrapper _repositoryMariaWrapper)
        {
            repositoryMariaWrapper = _repositoryMariaWrapper;
        }
        public (Contact data, string message) CreateContact(CreateContactRequest valueInput)
        {
            Contact contact = new Contact()
            {
                ContactName = valueInput.ContactName,
                ContactPhone = valueInput.ContactPhone,
                ContactEmail = valueInput.ContactEmail,
                ContactContent = valueInput.ContactContent
            };
            repositoryMariaWrapper.Contacts.Add(contact);
            repositoryMariaWrapper.SaveChanges();
            return (contact, "CreateContactSuccess");
        }
        public (object data, string message) ShowAllContact(RequestTable requestTable)
        {
            List<Contact> contacts = repositoryMariaWrapper.Contacts.FindAll().OrderByDescending(x=>x.ContactCreatedAt).ToList();
            int totalRecord = contacts.Count();
            if(requestTable.Limit > 0 && requestTable.Page > 0)
            {
                contacts = contacts.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList();
            }
            ResponseTable responseTable = new ResponseTable()
            {
                Data = contacts,
                Info = new Info
                {
                    Limit = requestTable.Limit,
                    Page = requestTable.Page,
                    TotalRecord = totalRecord
                }
            };
            return (responseTable, "ShowAllContactSuccess");
        }
        public (Contact data, string message) ShowDetailContact(string contactId)
        {
            Contact contact = repositoryMariaWrapper.Contacts.FirstOrDefault(x => x.ContactId.Equals(contactId));
            if(contact == null)
            {
                return (contact, "ContactIdDoNotExists");
            }
            return (contact, "ShowDetailContactSuccess");
        }
    }
}
