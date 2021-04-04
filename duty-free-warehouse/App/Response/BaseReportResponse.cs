using System;

namespace Project.App.Response
{
    public class BaseReportResponse
    {
        public string TaxCode { get; set; } //Mã số thuế
        public string InvoiceNumber { get; set; } // Số phiếu
        public DateTime AccountingDate { get; set; } //Ngày hạch toán
        public string Currency { get; set; } // Tiền tệ
        public string Explain { get; set; } // // Diễn giải
        public string DeClaDetailProductNumber { get; set; } // Mã hs code
        public string ProductCode { get; set; } // Mã sản phẩm
        public string ProductName { get; set; } // Tên sản phẩm
        public string ProductUnit { get; set; } // Đơn vị sản phẩm
        public string DeClaNumber { get; set; } // Số tờ khai
        public DateTime DeClaDateRe { get; set; } // Ngày tờ khai
        public int DeClaDetailQuantity { get; set; } // Số lượng sản phẩm
        public double DeClaDetailInvoicePrice { get; set; } // Đơn giá sản phẩm
        public double DeClaDetailInvoiceValue { get; set; } // Giá trị sản phẩm
    }
}
