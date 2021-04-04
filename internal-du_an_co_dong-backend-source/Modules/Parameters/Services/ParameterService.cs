
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Parameters.Entities;
using Project.Modules.Sessions.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Parameters.Services
{
    public interface IParameterService
    {
        IEnumerable<ParameterResponse> ListParameter();
        Parameter DetailParameter(string key);
        Parameter AddParameter(Parameter parameter);
        Parameter EditParameter(Parameter parameter);
        Parameter GetParamReport(Parameter parameter, string eventId);
    }
    public class ParameterService : IParameterService
    {
        private readonly MariaDBContext dBContext;
        public ParameterService(MariaDBContext DBContext)
        {
            dBContext = DBContext;
        }

        public IEnumerable<ParameterResponse> ListParameter()
        {
            return dBContext.Parameters.Where(x => x.DeletedAt == null).Select(x => new ParameterResponse(x));
        }

        public Parameter DetailParameter(string key)
        {
            return dBContext.Parameters.FirstOrDefault(x => x.DeletedAt == null && x.ParameterKey.Equals(key));
        }

        public Parameter AddParameter(Parameter parameter)
        {
            dBContext.Parameters.Add(parameter);
            dBContext.SaveChanges();
            return parameter;
        }

        public Parameter EditParameter(Parameter parameter)
        {
            dBContext.Parameters.Update(parameter);
            dBContext.SaveChanges();
            return parameter;
        }


        public Parameter GetParamReport(Parameter parameter, string eventId)
        {
            IEnumerable<Session> sessions = dBContext.Sessions.Where(x => x.EventId.Equals(eventId)).OrderBy(x => x.SessionSort);
            JToken jData = JToken.Parse(parameter.ParameterValue);
            List<ParameterValue> parameterValues = JsonConvert.DeserializeObject<IEnumerable<ParameterValue>>(jData[1]["params"].ToString()).ToList();

            int key = 0;
            foreach (var item in sessions)
            {
                parameterValues.Add(new ParameterValue
                {
                    key = $"session_title_{key}",
                    name = $"Tiêu đề chương trình thứ {key + 1}"
                });

                parameterValues.Add(new ParameterValue
                {
                    key = $"session_description_{key}",
                    name = $"Nội dung chương trình thứ {key + 1}"
                });

                parameterValues.Add(new ParameterValue
                {
                    key = $"session_question_{key}",
                    name = $"Câu hỏi chương trình thứ {key++ + 1}"
                });
            }

            jData[1]["params"] = JToken.FromObject(parameterValues);
            parameter.ParameterValue = JsonConvert.SerializeObject(jData);
            return parameter;
        }
    }

    public class ParameterValue
    {
        public string key { get; set; }
        public string name { get; set; }
    }
}
