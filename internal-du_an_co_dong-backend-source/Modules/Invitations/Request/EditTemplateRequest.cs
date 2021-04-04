using Project.Modules.Invitations.Entities;

namespace Project.Modules.Invitations.Request
{
    public class EditTemplateRequest
    {
        public string Title { get; set; }
        public int? Type { get; set; }
        public string Content { get; set; }
        public TEMPLATE_STATUS? Status { get; set; }
    }
}
