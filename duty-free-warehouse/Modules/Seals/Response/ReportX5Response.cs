using Microsoft.Extensions.Configuration;
using Project.App.Response;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Seals.Entity;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Response
{
    public class ReportX5Response : BaseReportResponse
    {
        public string ShopCodeOut { get; set; } //Mã cửa hàng xuất
        public string UserInput { get; set; } //Người Nhập
        public string PersonalCode { get; set; } //Mã đối tượng
        public string CustomerName { get; set; } //Tên khách hàng
        public string Passport { get; set; }//CMND -  Hộ chiếu
        public string Nationality { get; set; }// Quốc tịch
        public string FlightCode { get; set; }// Số hiệu phương tiện
        public string TicketFlight { get; set; }// Thẻ lên tàu bay

        public ReportX5Response() { }

        public List<ReportX5Response> GenerateData(Seal seal, Sell sell, int? typeDoiTuong, IConfiguration configuration)
        {
            List<ReportX5Response> reportX5Responses = new List<ReportX5Response>();
            foreach (var item in sell.SellDetails)
            {
                DeClaration deClaration = null;
                if(seal != null)
                {
                    deClaration = seal.SealDetails.FirstOrDefault(x => x.ProductCode.Equals(item.ProductCode))?.DeClaration;
                }    

                DeClarationDetail deClarationDetail = null;
                if(deClaration != null)
                {
                    deClarationDetail = deClaration.DeClarationDetails.FirstOrDefault(x => x.ProductCode.Equals(item.ProductCode));
                }

                int quantity = item.SoldNumber;
                reportX5Responses.Add(new ReportX5Response
                {
                    UserInput = "",
                    Currency = "USD",
                    Explain = null,
                    TaxCode = configuration["Export:TaxCode"],
                    InvoiceNumber = "X5" + sell.FlightDate.ToString("yyyyMMdd") + item.ID,
                    //InvoiceNumber = sell.InvoiceNo,
                    AccountingDate = DateTime.Now,
                    ShopCodeOut = configuration["Export:ShopCodeOut"],

                    PersonalCode = typeDoiTuong == 9 ? "DT9" : "DT10",
                    CustomerName = sell.CustomerName,
                    Passport = sell.PassportNumber,
                    Nationality = sell.Nationality,
                    FlightCode = seal?.AcReg,
                    //TicketFlight = sell.SeatNumber,
                    TicketFlight = sell.FlightNumberDetail,

                    ProductCode = item.ProductCode,
                    ProductName = item.Product.ProductName,
                    ProductUnit = item.Product.ProductUnit,

                    DeClaDateRe = deClaration != null ? deClaration.DeClaDateReData : DateTime.MinValue,
                    DeClaNumber = deClaration?.DeClaNumber,
                    DeClaDetailProductNumber = deClarationDetail?.DeClaDetailProductNumber,
                    DeClaDetailInvoicePrice = deClarationDetail != null ? deClarationDetail.DeClaDetailInvoicePrice : 0,
                    DeClaDetailInvoiceValue = (deClarationDetail != null ? deClarationDetail.DeClaDetailInvoicePrice : 0) * quantity,
                    DeClaDetailQuantity = quantity,
                });
            }
            return reportX5Responses;
        }
    }
}