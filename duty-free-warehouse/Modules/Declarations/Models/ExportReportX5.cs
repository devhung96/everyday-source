using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Modules.Declarations.Models;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public static class Extensions
    {
        private static IConfiguration Configuration;
        public static string PersonalCodeStatic(this TypeSchedule value)
        {
            switch (value)
            {
                case TypeSchedule.EntryCountry: return "EntryCountry";
                case TypeSchedule.ExitCountry: return "ExitCountry";
                default: throw new ArgumentOutOfRangeException("value");
            }
        }
        public static IConfiguration SetUtilsProviderConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            Configuration = configuration;
            return Configuration;
        }

        public static List<ExportReportN2> HandleList(this DeClaration deClaration, User user)
        {
            string InvoiceNumberHash = "N2" + DateTime.UtcNow.ToString("yyyyMMdd"); //( loại chuẩn + ngày tháng năm + 00001)
            List<ExportReportN2> N2Temp = new List<ExportReportN2>();
            foreach (DeClarationDetail deClarationDetail in deClaration.DeClarationDetails)
            {
                ExportReportN2 reportN2 = new ExportReportN2
                {
                    TaxCode = Configuration["Export:TaxCode"],
                    InvoiceNumber = InvoiceNumberHash + DateTimeOffset.UtcNow.Ticks,
                    AccountingDate = DateTime.Now,
                    EntryCode = Configuration["Export:WHCode"],
                    EntryGoodsPersonal = user != null ? user.FullName : null,
                    Supplier = deClaration.Content["supplier"] is null ? "" : deClaration.Content["supplier"].ToString(),
                    ContractNumber = null,
                    Currency = "USD",
                    Explain = null,
                    DeClaDetailProductNumber = deClarationDetail.DeClaDetailProductNumber,
                    ProductCode = deClarationDetail.Product.ProductCode,
                    ProductName = deClarationDetail.Product.ProductName,
                    ProductUnit = deClarationDetail.Product.ProductUnit,
                    DeClaNumber = deClarationDetail.DeClaNumber,
                    DeClaDateRe = deClaration.DeClaDateReData,
                    DeClaDetailQuantity = deClarationDetail.DeClaDetailQuantity,
                    DeClaDetailInvoicePrice = deClarationDetail.DeClaDetailInvoicePrice,
                    DeClaDetailInvoiceValue = deClarationDetail.DeClaDetailInvoiceValue
                };
                N2Temp.Add(reportN2);
            }
            return N2Temp;
        }
    }
    public class ExportReportX5
    {
        public string TaxCode { get; set; } //Mã số thuế
        public string InvoiceNumber { get; set; } //Số phiếu
        public string DeClaDateRe { get; set; } //Ngày Hạch Toán
        //public string EntryCode { get; set; } //Mã nhập kho
        //public string EntryGoodsPersonal { get; set; } //Người Nhập
        //public string Supplier { get; set; } //Nhà cung cấp
        //public string ContractNumber { get; set; } //Số hợp đồng
        public string CurrencyType { get; set; } //Loại ngoại tệ
        public string Explain { get; set; } //Diễn giải


        public string PersonalCode { get; set; } //Mã đối tượng
        public string CustomerName { get; set; } //Tên khách hàng
        public string Passport { get; set; }//CMND -  Hộ chiếu
        public string Nationality { get; set; }// Quốc tịch
        public string FlightCode { get; set; }// Số hiệu phương tiện
        public string TicketFlight { get; set; }// Thẻ lên tàu bay



        public string DeClaDetailProductNumber { get; set; } //Mã HS
        public string ProductCode { get; set; } //Mã Hàng
        public string ProductName { get; set; } //Tên Hàng
        public string ProductUnit { get; set; } //Đơn vị tính
        public string DeClaNumber { get; set; } //Số tờ khai
        public string DateAdded { get; set; } //Ngày tờ khai
        public string DeClaDetailQuantity { get; set; } //Số lượng
        public double DeClaDetailInvoicePrice { get; set; } //Đơn giá
        public double DeClaDetailInvoiceValue { get; set; } //Trị giá
    }

    public class CityPairSchedule
    {
        public string departure { get; set; }
        public string arrival { get; set; }
        public TypeSchedule typeSchedule { get; set; }
        public string route { get; set; }
        public string flightNumber { get; set; }
        public int status { get; set; }
        public List<string> flightTime { get; set; } = new List<string>();
    }
    public enum TypeSchedule
    {
        ExitCountry = 0,
        EntryCountry = 1
    }
}
