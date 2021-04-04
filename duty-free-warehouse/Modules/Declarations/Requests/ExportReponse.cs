using Project.App.Response;
using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Entities;
using Project.Modules.Inventories.Entites;
using Project.Modules.Products.Entities;
using System;

namespace Project.Modules.Declarations.Requests
{
    public class ExportReponse : BaseReportResponse
    {
        public string WarehouseCode { get; set; }// Mã kho nhập
        public string WarehouseCodeOut { get; set; }// Mã kho xuất
        public string ShopCode { get; set; } // Mã cửa hàng nhập
        public ExportReponse() { }
        public ExportReponse(DeClaration de, DeClarationDetail detail, InforExport infor, string noBull)
        {
            TaxCode = infor.TaxCode;
            WarehouseCode = infor.WarehouseCode;
            ShopCode = infor.ShopCode;
            AccountingDate = infor.AccountingDate;
            InvoiceNumber = noBull;
            DeClaNumber = de.DeClaNumber;
            DeClaDateRe = de.DeClaDateReData;
            DeClaDetailProductNumber = detail.DeClaDetailProductNumber;
            DeClaDetailQuantity = detail.DeClaDetailQuantity;
            DeClaDetailInvoicePrice = detail.DeClaDetailInvoicePrice;
            DeClaDetailInvoiceValue = detail.DeClaDetailQuantity*detail.DeClaDetailInvoicePrice;
            ProductCode = detail.Product.ProductCode;
            ProductName = detail.Product.ProductName;
            ProductUnit = detail.Product.ProductUnit;
            Currency = "USD";
        }

        public ExportReponse(DestroyDetail detail, Product product,InforExport infor, string noBull, DeClaration de)
        {
            TaxCode = infor.TaxCode;
            WarehouseCode = infor.WarehouseCode;
            WarehouseCodeOut = infor.WarehouseCodeOut;
            InvoiceNumber = noBull;
            AccountingDate = infor.AccountingDate;
            Explain = detail.DestroyDetailNote;
            DeClaNumber = de.DeClaNumber;
            DeClaDateRe = de.DeClaDateReData;
            DeClaDetailProductNumber = detail.ProductCode;
            DeClaDetailQuantity = detail.DestroyDetailQuantity;
            DeClaDetailInvoicePrice = detail.ProductPirce;
            DeClaDetailInvoiceValue = detail.DestroyDetailQuantity * detail.ProductPirce;
            ProductCode = product.ProductCode;
            ProductName = product.ProductName;
            ProductUnit = product.ProductUnit;
            Currency = "USD";
        }
        public struct InforExport
        {
            public string WarehouseCode { get; set; }// Mã kho nhập
            public string WarehouseCodeOut { get; set; }// Mã kho xuẩ
            public string ShopCode { get; set; } // Mã cửa hàng nhập
            public string TaxCode { get; set; } //Mã số thuế
            public DateTime AccountingDate { get; set; } // Ngày hoạch toán
        }
    }
}
