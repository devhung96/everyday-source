using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.Modules.Events.Entities;
using Project.Modules.Parameters.Entities;
using Project.Modules.Parameters.Requests;
using Project.Modules.Parameters.Services;
using System;

namespace Project.Modules.Parameters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametersController : BaseController
    {
        private readonly IParameterService parameterService;
        public ParametersController(IParameterService ParameterService)
        {
            parameterService = ParameterService;
        }

        [HttpGet]
        public IActionResult ListParams()
        {
            return ResponseOk(parameterService.ListParameter(), "Show List Success");
        }

        [HttpGet("{paramKey}")]
        public IActionResult DetailParam(string paramKey)
        {
            string eventId = Request.Query["eventId"].ToString();
            if(paramKey == "TemplateType" && string.IsNullOrWhiteSpace(eventId))
            {
                return ResponseBadRequest("Mã sự kiện trong danh sách biểu mẫu bắt buộc");
            }
                
            Parameter parameter = parameterService.DetailParameter(paramKey);

            if(parameter is null)
            {
                return ResponseBadRequest("Tham số không tìm thấy");
            }

            parameter = parameterService.GetParamReport(parameter, eventId);
            return ResponseOk(new ParameterResponse(parameter));
        }

        [HttpPost]
        public IActionResult AddParameter([FromBody]AddParameterRequest request)
        {
            Parameter parameterCheck = parameterService.DetailParameter(request.ParameterKey);

            if (parameterCheck != null)
            {
                return ResponseBadRequest("Parammeter Is Exists");
            }
            Parameter parameter;
            if (request.ParameterType == TYPE_PARAMS.TEXT)
            {
                parameter = parameterService.AddParameter(new Parameter
                {
                    ParameterKey = request.ParameterKey,
                    ParameterName = request.ParameterName,
                    Type = request.ParameterType,
                    ParameterValue = request.ParameterValue as string,
                });
            }
            else
            {
                parameter = parameterService.AddParameter(new Parameter
                {
                    ParameterKey = request.ParameterKey,
                    ParameterName = request.ParameterName,
                    Type = request.ParameterType,
                    ParameterValue = JsonConvert.SerializeObject(request.ParameterValue),
                });
            }
            
            return ResponseOk(new ParameterResponse(parameter), "Add Parameter Success");
        }

        [HttpPut("{paramKey}")]
        public IActionResult EditParameter([FromBody]EditParameterRequest request, string paramKey)
        {
            Parameter parameterCheck = parameterService.DetailParameter(paramKey);

            if (parameterCheck is null)
            {
                return ResponseBadRequest("Parammeter Not Found");
            }

            if(request.ParameterValue != null)
            {
                if(parameterCheck.Type == TYPE_PARAMS.TEXT)
                {
                    parameterCheck.ParameterValue = request.ParameterValue as string;
                }
                else
                {
                    parameterCheck.ParameterValue = JsonConvert.SerializeObject(request.ParameterValue);
                }
            }    

            Parameter parameter = parameterService.EditParameter(parameterCheck);

            return ResponseOk(new ParameterResponse(parameter), "Edit Parameter Success");
        }

        [HttpDelete("{paramKey}")]
        public IActionResult DeleteParameter(string paramKey)
        {
            Parameter parameterCheck = parameterService.DetailParameter(paramKey);

            if (parameterCheck is null)
            {
                return ResponseBadRequest("Parammeter Not Found");
            }

            parameterCheck.DeletedAt = DateTime.Now;

            Parameter parameter = parameterService.EditParameter(parameterCheck);
            return ResponseOk(parameter, "Delete Parameter Success");
        }
    }
}