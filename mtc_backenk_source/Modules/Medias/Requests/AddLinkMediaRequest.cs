using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Medias.Requests
{
    public class AddLinkMediaRequest
    {
        public string MediaName { get; set; }
        public string MediaUrl { get; set; }
        public string MediaComment { get; set; }
        public string GroupIds { get; set; }

        public string MediaExtend { get; set; }
    }
}
