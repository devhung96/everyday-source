using Project.App.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public class ExportReportN2 : BaseReportResponse
    {
        public string EntryCode { get; set; } //Mã nhập kho
        public string EntryGoodsPersonal { get; set; } //Người Nhập
        public string Supplier { get; set; } //Nhà cung cấp
        public string ContractNumber { get; set; } //Số hợp đồng (HD)
    }
}
