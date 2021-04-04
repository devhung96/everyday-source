using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Entities
{
    public class MediaResponse
    {
        public string MediaID { get; set; }
        public string MediaName { get; set; }
        public string MediaPath { get; set; }
        public string MediaTitle { get; set; }
        public string MediaTag { get; set; }
        public string MediaDescription { get; set; }
        public string MediaFullPath { get; set; }
        public DateTime CreatedAt { get; set; }

        public MediaResponse(Media media)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            MediaID = media.MediaID;
            MediaName = media.MediaName;
            MediaPath = media.MediaPath;
            MediaTitle = media.MediaTitle;
            MediaTag = media.MediaTag;
            MediaDescription = media.MediaDescription;
            MediaFullPath = configuration["OutsideSystems:Media:Url"] + "api/media/getFile?link=" + media.MediaPath;
            CreatedAt = media.CreatedAt;
        }
    }
}
