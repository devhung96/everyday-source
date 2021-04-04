using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Requests
{
    public class BaseTicketTypeRequest
    {
        public string TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public string TicketTypeDescription { get; set; }
        public List<string> DeviceIds { get; set; }

        /// <summary>
        /// Dùng hiện thị danh sách lỗi cho insert many. Không dùng lưu dữ liệu.
        /// </summary>
        public List<string> MessageErrors { get; set; } = new List<string>();
    }

    public class AddTicketRequest : BaseTicketTypeRequest
    {

    }

    public class UpdateTicketRequet : BaseTicketTypeRequest
    {
    }

    public class DeleteTicketRequest : BaseTicketTypeRequest
    {
    }

    public class AddMutiTicketTypeRequest
    {
        public List<BaseTicketTypeRequest> TicketTypes { get; set; }
    }
}
