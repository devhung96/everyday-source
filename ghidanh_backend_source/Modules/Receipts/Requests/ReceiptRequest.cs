using Project.App.Helpers;
using System;

namespace Project.Modules.Receipts.Requests
{
    // [ValidateAddReceipt]
    public class ReceiptRequest
    {
        public string ClassId { get; set; }
        public string StudentId { get; set; }
        public string AccountId { get; set; }
        public double TypeAmount { get; set; } // số tiền theo loại
        public string MoneyTypeId { get; set; }// loại thu
        public double SurchargeAmount { get; set; } // tiền phụ thu
        public double DiscountAmount { get; set; } // tiền giảm giá
        public string DiscountNote { get; set; } // nội dung giảm giá
        public double Amount { get; set; } // Tổng thu
    }
    public class ReceiptExportRequest:RequestTable
    {
          public string CourseId { get; set; }
          public DateTime ? PaymentDate { get; set; }
    }
}
