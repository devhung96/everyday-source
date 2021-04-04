using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Entities;
using Project.Modules.Sells.Entities;
using System;

namespace Project.Modules.Seals.Response
{
    public class ReportExportResponse
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductUnit { get; set; }
        public DateTime? DateSell { get; set; }
        public string Invoice { get; set; }
        public string Note { get; set; }

        public ReportExportResponse(SellDetail sellDetail)
        {
            ProductName = sellDetail.Product?.ProductName;
            ProductCode = sellDetail.Product?.ProductCode;
            ProductQuantity = sellDetail.SoldNumber;
            ProductUnit = sellDetail.Product?.ProductUnit;
            DateSell = sellDetail.Sell.FlightDate;
            Invoice = sellDetail.Sell.InvoiceNo;
            Note = "Xuất bán";
        }
        public ReportExportResponse(DeClarationDetail deClarationDetail)
        {
            ProductName = deClarationDetail.Product.ProductName;
            ProductCode = deClarationDetail.Product.ProductCode;
            ProductQuantity = deClarationDetail.DeClaDetailQuantity;
            ProductUnit = deClarationDetail.Product.ProductUnit;
            DateSell = deClarationDetail.DeClaration.DeClaDateReData;
            Invoice = deClarationDetail.DeClaration.DeClaNumber;
            Note = "Tái xuất";
        }
        public ReportExportResponse(DestroyDetail destroyDetail)
        {
            ProductName = destroyDetail.Product.ProductName;
            ProductCode = destroyDetail.Product.ProductCode;
            ProductQuantity = destroyDetail.DestroyDetailQuantity;
            ProductUnit = destroyDetail.Product.ProductUnit;
            DateSell = destroyDetail.Destroy.DestroyDate;
            Invoice = destroyDetail.Destroy.DestroyCode;
            Note = "Xuất hủy";
        }
    }
}
