
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public class EventId 
    {
        private  readonly IConfiguration _configuration;
        private static Random random = new Random();

        public EventId(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Value()
        {
            int length = _configuration["EventIdLength"].toEventId();
            List<string> type = new List<string>() { "numbers", "alphabets"};
            return GeneralHelper.GenerateId(type, length);




        }
    }
}
