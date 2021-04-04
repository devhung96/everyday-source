
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.Modules.Reports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project.Modules.Events.Services
{

    public interface ISoketIO
    {
        public  Task ForwardAsync(string eventId, object data, string token, string message = "", List<string> members = null, string admin = "1", string member = "0", string projector="0");
    }
    public class SoketIO: ISoketIO
    {
        public string _endpoint { get; set; }
        public readonly IConfiguration _config;

        public SoketIO(IConfiguration config)
        {
            _config = config;
            _endpoint = _config["SocketClient"].toDefaultUrl();
        }

        public async Task ForwardAsync(string eventId, object data, string token, string message = "", List<string> members = null , string admin = "0", string member = "0", string projector = "0")
        {
            if (members is null) members = new List<string>() { "" };

            var url = _endpoint + "/forward";
            try
            {
                var body = new ChartRealTime
                {
                    eventID = eventId,
                    message = message,
                    admin = admin,
                    member = member,
                    projector = projector,
                    members = members,
                    data = data
                };
                Dictionary<string, object> headers = new Dictionary<string, object>
                {
                    { "Authorization", token }
                };

                (string responseData, int? responseStatusCode) = await HttpMethod.Post.SendRequestAsync2(url, body, headers);
                if (responseStatusCode == 200)
                {
                    Console.WriteLine(responseData);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
