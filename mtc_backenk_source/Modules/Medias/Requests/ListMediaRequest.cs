using Project.App.Helpers;
using Project.Modules.Medias.Entities;

namespace Project.Modules.Medias.Requests
{
    public class ListMediaRequest : PaginationRequest
    {
        public bool? IsConfirm { get; set; }
        public string TypeMedia { get; set; }
        public string Date { get; set; }
        public MediaStatusEnum? Status { get; set; }
    }
}
