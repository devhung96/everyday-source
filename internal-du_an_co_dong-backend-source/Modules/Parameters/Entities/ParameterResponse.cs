using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Parameters.Entities
{
    public class ParameterResponse
    {
        public string ParameterKey { get; set; }
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public TYPE_PARAMS Type { get; set; }
        public DateTime? CreatedAt { get; set; }

        public ParameterResponse(Parameter parameter)
        {
            ParameterKey = parameter.ParameterKey;
            ParameterName = parameter.ParameterName;
            Type = parameter.Type;
            if(Type == TYPE_PARAMS.TEXT)
            {
                ParameterValue = parameter.ParameterValue as string;
            }    
            else
            {
                ParameterValue = JsonConvert.DeserializeObject(parameter.ParameterValue);
            }
            CreatedAt = parameter.CreatedAt;
        }
    }
}
