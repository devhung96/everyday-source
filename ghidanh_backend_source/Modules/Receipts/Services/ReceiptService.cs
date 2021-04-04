using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.MoneyTypes.Entities;
using Project.Modules.Receipts.Entities;
using Project.Modules.Receipts.Requests;
using Project.Modules.Students.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Project.Modules.Receipts.Services
{
    public interface IReceiptService
    {
        (Receipt receipt, string message) Store(ReceiptRequest request);
        (ReceiptResponse receipt, string message) Export(ReceiptRequest request);
        ResponseTable ReceiptExport(ReceiptExportRequest request);
        string NumberToText(string str);
    }
    public class ReceiptService : IReceiptService
    {
        private readonly IConfiguration configuration;
        private readonly IRepositoryMariaWrapper repository;
        private readonly IMapper mapper;
        public ReceiptService(IConfiguration configuration, IRepositoryMariaWrapper repository, IMapper mapper)
        {
            this.configuration = configuration;
            this.repository = repository;
            this.mapper = mapper;
        }

        public (Receipt receipt, string message) Store(ReceiptRequest request)
        {
            User user = repository.Users.FirstOrDefault(m => m.AccountId.Equals(request.AccountId));
            if (user is null)
            {
                return (null, "AccountNotExist");
            }
            RegistrationStudy registration = repository.RegistrationStudies.FirstOrDefault(m => m.ClassId.Equals(request.ClassId) && m.StudentId.Equals(request.StudentId));
            if (registration is null)
            {
                return (null, "StudentNotExistInClass");
            }
            string noBook = DateTime.UtcNow.AddHours(7).ToString("yyyyMM");
            int number = repository.Receipts.FindAll().ToList().Count + 1;
            Receipt receipt = mapper.Map<Receipt>(request);
            receipt.ClassAmount = repository.Classes.FirstOrDefault(m => m.ClassId.Equals(request.ClassId)).ClassAmount;
            // SỐ THU THỰC TẾ
            double amount = receipt.TypeAmount - receipt.DiscountAmount + receipt.SurchargeAmount;
            if (amount != request.Amount)
            {
                return (null, "AmountWrong");
            }
            receipt.NoBook = noBook;
            receipt.Numerical = number;
            receipt.Amount = amount;
            receipt.CreatedBy = user.UserId;
            receipt.UserName = user.UserName;
            repository.Receipts.Add(receipt);
            repository.SaveChanges();
            return (receipt, "Success");
        }
        public string NumberToText(string str)
        {
            str = str.Split(".")[0];
            string[] word = { "", " một", " hai", " ba", " bốn", " năm", " sáu", " bảy", " tám", " chín" };
            string[] million = { "", " mươi", " trăm", "" };
            string[] billion = { "", "", "", " nghìn", "", "", " triệu", "", "" };
            string result = "{0}";
            int count = 0;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (count > 0 && count % 9 == 0)
                    result = string.Format(result, "{0} tỷ");
                if (!(count < str.Length - 3 && count > 2 && str[i].Equals('0') && str[i - 1].Equals('0') && str[i - 2].Equals('0')))
                    result = string.Format(result, "{0}" + billion[count % 9]);
                if (!str[i].Equals('0'))
                    result = string.Format(result, "{0}" + million[count % 3]);
                else if (count % 3 == 1 && count > 1 && !str[i - 1].Equals('0') && !str[i + 1].Equals('0'))
                    result = string.Format(result, "{0} lẻ");
                int num;
                try
                {
                    num = Convert.ToInt16(str[i].ToString());
                }
                catch
                {
                    return "Wrong format";
                }
                result = string.Format(result, "{0}" + word[num]);
                count++;
            }
            result = result.Replace("{1}", "");
            result = result.Remove(0, 3);
            result = result.Trim();
            string fist = result[0].ToString().ToUpper();
            result = result.Remove(0, 1);
            result = result.Insert(0, fist);
            result = result.Insert(result.Length, " đồng.");
            return result.Trim();
        }

        public (ReceiptResponse receipt, string message) Export(ReceiptRequest request)
        {
            Receipt receipt = repository.Receipts.FindByCondition(m => m.ClassId.Equals(request.ClassId) && m.StudentId.Equals(request.StudentId))
                                                .FirstOrDefault();
            if (receipt is null)
            {
                return (null, "ReceiptNotExist");
            }

            receipt.Classes = repository.Classes.GetById(receipt.ClassId);
            receipt.Student = repository.Students.GetById(receipt.StudentId);
            string noPattern = configuration["Receipt:NoPattern"];
            string codeQHNS = configuration["Receipt:CodeQHNS"];
            string unit = configuration["Receipt:Unit"];
            string AmountToText = NumberToText(receipt.Amount.ToString());
            MoneyType moneyType = repository.MoneyTypes.FirstOrDefault(m => m.MoneyTypeId.Equals(receipt.MoneyTypeId));
            string moneyTypeName = moneyType != null ? moneyType.MoneyTypeName : "";
            ReceiptResponse receiptResponse = new ReceiptResponse(unit, codeQHNS, noPattern, receipt, AmountToText, moneyTypeName);
            receiptResponse.Surchages = repository.Surcharges
                                                  .FindByCondition(m => m.ClassId.Equals(request.ClassId))
                                                  .ToList();
            return (receiptResponse, "Success");
        }

        public ResponseTable ReceiptExport(ReceiptExportRequest request)
        {
            List<string> ClassIds = new List<string>();
            List<ReceiptExport> receipts = new List<ReceiptExport>();
            ClassIds = (string.IsNullOrEmpty(request.CourseId))
                      ? repository.Classes.FindAll().Select(m => m.ClassId).ToList()
                      : repository.Classes.FindByCondition(m => m.CourseID.Equals(request.CourseId)).Select(m => m.ClassId).ToList();
            foreach(string classId in ClassIds)
            {
                receipts.AddRange(repository.Receipts.FindByCondition(m => m.ClassId.Equals(classId))
                                                    .Include(m => m.Student)
                                                    .Include(m => m.Classes)
                                                    .Select(m=>new ReceiptExport(m))
                                                    .ToList());
            }



            receipts= receipts.Where(m=> !request.PaymentDate.HasValue || m.PaymentDate.Value.Date ==request.PaymentDate.Value.Date).OrderBy(m =>m.StudentFirstName +" "+m.StudentLastName).ToList();
            receipts = receipts.Where(m => String.IsNullOrEmpty(request.Search) || (
                                                     (!String.IsNullOrEmpty(m.StudentCode) && m.StudentCode.Contains(request.Search.ToUpper()))
                                                  || (m.StudentFirstName + " " + m.StudentLastName).ToUpper().Contains(request.Search.ToUpper())
                                                  || (!String.IsNullOrEmpty(m.StudentPhone) && m.StudentPhone.Contains(request.Search))
                                                  || (!String.IsNullOrEmpty(m.StudentEmail) && m.StudentEmail.ToUpper().Contains(request.Search.ToUpper()))   
                                                  || (!String.IsNullOrEmpty(m.ClassName) && m.ClassName.ToUpper().Contains(request.Search.ToUpper())) 
                                                  )).ToList();
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                receipts = receipts.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder)).ToList();
            }

            ResponseTable response = new ResponseTable
            {
                Data = request.Page == 0 ? receipts : receipts.Skip((request.Page - 1) * request.Limit).Take(request.Limit),
                Info = new Info
                {
                    Limit = request.Page == 0 ? receipts.Count : receipts.Skip((request.Page - 1) * request.Limit).Take(request.Limit).Count(),
                    Page = request.Page,
                    TotalRecord = receipts.Count,
                }
            };
            return response;
        }
    }
}
