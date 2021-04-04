using Project.Modules.Classes.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.Users.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Project.Modules.Students.Entities.Student;

namespace Project.Modules.Receipts.Entities
{
    [Table("rc_receipts")]
    public class Receipt
    {
        [Key]
        [Column("receipt_id")]
        public string ReceiptId { get; set; } = Guid.NewGuid().ToString();
        [Column("class_id")]
        public string ClassId { get; set; }
        [Column("student_id")]
        public string StudentId { get; set; }
        [Column("receipt_no_book")]
        public string NoBook { get; set; }
        [Column("receipt_numerical")]
        public int Numerical { get; set; }
        [Column("receipt_amount")]
        public double Amount { get; set; }
        [Column("receipt_class")]
        public double ClassAmount { get; set; }
        [Column("receipt_type")]
        public double TypeAmount { get; set; }
        [Column("type_id")]
        public string MoneyTypeId { get; set; }
        [Column("receipt_surcharge")]
        public double SurchargeAmount { get; set; }
        [Column("receipt_discount")]
        public double DiscountAmount { get; set; }
        [Column("receipt_discount_note")]
        public string DiscountNote { get; set; }
        [Column("user_id")]
        public string CreatedBy { get; set; } 
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("receipt_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Student Student { get; set; }
        public Class Classes { get; set; }
    }
    public class ReceiptResponse
    {
        public string Unit { get; set; } // Đơn vị 
        public string CodeQHNS { get; set; } // Mã QHNS
        public string NoPattern { get; set; } // Mẫu số
        public string NoBook { get; set; } // Quyển số
        public int Numerical { get; set; } // Số
        public string StudentName { get; set; } // Tên HV
        public string StudentAddress { get; set; } // Địa chỉ HV
        public string ClassName { get; set; } // Tên lớp đóng HP
        public double Amount { get; set; } // Số tiền cần đóng
        public string AmountToText { get; set; } // Số tiền viết bằng chữ
        public string CreatedByUser { get; set; } // Người thu tiền

        public double TypeAmount { get; set; } // số tiền theo loại
        public string MoneyTypeName { get; set; }// loại thu
        public double SurchargeAmount { get; set; } // tiền phụ thu
        public double DiscountAmount { get; set; } // tiền giảm giá
        public string DiscountNote { get; set; } // nội dung giảm giá
        public double ClassAmount { get; set; } // Số tiền học phí của lớp
        public List<Surcharge> Surchages { get; set; } // Tất cả thụ thu theo lớp
        public ReceiptResponse()
        {
        }
        public ReceiptResponse(string unit,string codeQHNS, string noPattern, Receipt receipt, string text, string moneyTypeName =null)
        {
            Unit = unit;
            CodeQHNS = codeQHNS;
            NoPattern = noPattern;
            Numerical = receipt.Numerical;
            NoBook = receipt.NoBook;
            StudentName = receipt.Student.StudentFirstName + " " + receipt.Student.StudentLastName;
            StudentAddress = receipt.Student.StudentAddress;
            ClassName = receipt.Classes.ClassName;
            Amount = receipt.Amount;
            AmountToText = text;
            CreatedByUser =receipt.UserName;
            TypeAmount = receipt.TypeAmount;
            MoneyTypeName = moneyTypeName;
            SurchargeAmount = receipt.SurchargeAmount;
            DiscountAmount = receipt.DiscountAmount;
            DiscountNote = receipt.DiscountNote;
            ClassAmount = receipt.ClassAmount;

        }
    }

    public class ReceiptExport
    {
        public string StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentLastName { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentEmail { get; set; }
        public DateTime StudentBirthday { get; set; }
        public GENDER StudentGender { get; set; }
        public string StudentAddress { get; set; }
        public string StudentPhone { get; set; }
        public string StudentImage { get; set; }
        public string ParentName { get; set; }
        public string ClassName { get; set; }
        public DateTime? PaymentDate { get; set; }
        public ReceiptExport() { }
        public ReceiptExport(Receipt re)
        {
            StudentId = re.Student.StudentId;
            StudentCode = re.Student.StudentCode;
            StudentLastName = re.Student.StudentLastName;
            StudentFirstName = re.Student.StudentFirstName;
            StudentEmail = re.Student.StudentEmail;
            StudentBirthday = re.Student.StudentBirthday;
            StudentGender = re.Student.StudentGender;
            StudentAddress = re.Student.StudentAddress;
            StudentPhone = re.Student.StudentPhone;
            StudentImage = re.Student.StudentImage;
            ParentName = re.Student.ParentName;
            ClassName = re.Classes.ClassName;
            PaymentDate = re.CreatedAt;
        }
    }


}
 