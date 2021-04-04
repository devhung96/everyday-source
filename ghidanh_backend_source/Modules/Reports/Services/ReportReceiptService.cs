using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.Receipts.Entities;
using Project.Modules.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Services
{
    public interface IReportReceiptService
    {
        ReportReceipt ReportReceipt(DateTime receiptDate);
    }
    public class ReportReceiptService : IReportReceiptService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        private readonly IConfiguration configuration;
        public ReportReceiptService(IRepositoryMariaWrapper repositoryMariaWrapper, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.repositoryMariaWrapper = repositoryMariaWrapper;
        }
        public ReportReceipt ReportReceipt(DateTime receiptDate)
        {
            ReportReceipt reportReceipt = new ReportReceipt();
            List<Receipt> receipt = repositoryMariaWrapper.Receipts.FindByCondition(x => x.CreatedAt.Date.Equals(receiptDate.ToUniversalTime().Date)).ToList();
            if(receipt.Count > 0)
            {
                reportReceipt.Department = "Giáo vụ";
                reportReceipt.ReceiptSymbol = configuration["Receipt:NoPattern"];
                reportReceipt.NoBook = receipt.FirstOrDefault().NoBook;
                reportReceipt.FromReceiptNumerical = receipt.Min(x => x.Numerical).ToString();
                reportReceipt.ToReceiptNumerical = receipt.Max(x => x.Numerical).ToString();
                reportReceipt.TotalAmount = receipt.Sum(x => x.Amount);
            }
            return reportReceipt;
        }
    }
}
