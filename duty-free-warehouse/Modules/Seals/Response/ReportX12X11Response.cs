using Microsoft.Extensions.Configuration;
using Project.App.Response;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Seals.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Response
{
    public class ReportX12X11Response : BaseReportResponse
    {
        public string WarehoustCode { get; set; } // MA_KHO_NHAP
        public string WarehouseCodeOut { get; set; } // MA_KHO_XUAT
        public string UserInput { get; set; } // NGUOI_XUAT
        public string ShopCode { get; set; } // MA_CUA_HANG_NHAP
        public string ShopCodeOut { get; set; } // MA_CUA_HANG_XUAT
        public string UserOutput { get; set; } // NGUOI_NHAP
        public string FlightCode { get; set; } // SO_HIEU_PHUONG_TIEN
        public DateTime FlightDate { get; set; } // NGAY_DI
        public string AddressFlight { get; set; } // DIA_CHI_BAY
        public string SealNumber { get; set; } // SO_HIEU_XE_HANG
        public string SealNumberReturn { get; set; } // SO_HIEU_XE_HANG_VE
        public DateTime? DateExport { get; set; } // GIAO_HANG_TU_NGAY
        public DateTime? DateImport { get; set; } // THOI_GIAN_NHAN_HANG
        public string Route { get; set; } // TUYEN_DUONG_VAN_CHUYEN

        public ReportX12X11Response() { }

        public List<ReportX12X11Response> GenerateData(Seal seal, IConfiguration configuration, int typeReport = 11)
        {
            List<ReportX12X11Response> reportX12Responses = new List<ReportX12X11Response>();
            foreach (var item in seal.SealDetails)
            {
                DeClarationDetail deClarationDetail = item.DeClaration.DeClarationDetails
                    .FirstOrDefault(x => x.ProductCode.Equals(item.ProductCode));

                int quanlity;
                string invoiceNumber;
                if(typeReport == 11)
                {
                    quanlity = item.QuantityExport;
                    invoiceNumber = "X11" + seal.ExportDate.GetValueOrDefault().ToString("yyyyMMdd") + item.SealDetailId;
                }
                else
                {
                    quanlity = item.QuantityInventory;
                    invoiceNumber = "X12" + seal.ExportDate.GetValueOrDefault().ToString("yyyyMMdd") + item.SealDetailId;
                }
                    
                reportX12Responses.Add(new ReportX12X11Response
                {
                    UserInput = null,
                    UserOutput = null,
                    Route = seal.Route,
                    Explain = null,
                    TaxCode = configuration["Export:TaxCode"],
                    InvoiceNumber = invoiceNumber,
                    WarehoustCode = configuration["Export:WHCode"],
                    WarehouseCodeOut = configuration["Export:OutCode"],
                    AccountingDate = DateTime.Now,
                    ShopCode = configuration["Export:ShopCode"],
                    ShopCodeOut = configuration["Export:ShopCodeOut"],
                    FlightCode = seal.AcReg,
                    FlightDate = seal.FlightDate,
                    SealNumber = seal.SealNumber,
                    SealNumberReturn = seal.Return,
                    DeClaDateRe = item.DeClaration.DeClaDateReData,
                    DeClaNumber = item.DeClaration.DeClaNumber,
                    DeClaDetailProductNumber = deClarationDetail?.DeClaDetailProductNumber,
                    DeClaDetailInvoicePrice = deClarationDetail != null ? deClarationDetail.DeClaDetailInvoicePrice : 0,
                    DeClaDetailInvoiceValue = (deClarationDetail != null ? deClarationDetail.DeClaDetailInvoicePrice : 0) * quanlity,
                    DeClaDetailQuantity = quanlity,
                    DateExport = seal.ExportDate,
                    DateImport = seal.ImportDate,
                    ProductCode = item.Product.ProductCode,
                    ProductName = item.Product.ProductName,
                    ProductUnit = item.Product.ProductUnit,
                    Currency = "USD",
                    AddressFlight = configuration["Export:AddressFlight"]
                });
            }
            return reportX12Responses;
        }
    }
}
