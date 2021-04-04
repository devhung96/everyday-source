using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using SurveillanceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveillanceManagementServiceClient = SurveillanceManagement.SurveillanceManagement.SurveillanceManagementClient;
using RepositoriesManagements = RepositoriesManagement.RepositoryManagement.RepositoryManagementClient;
using Project.Modules.UserCodes.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : BaseController
    {
        private SurveillanceManagementServiceClient _surveillances;
        private RepositoriesManagements _repositories;
        private readonly IConfiguration _configuation;

        private readonly IUserCodeService _userCodeService;
        public SetupController(RepositoriesManagements repositories,SurveillanceManagementServiceClient surveillance, IConfiguration configuation, IUserCodeService userCodeService)
        {
            _configuation = configuation;
            _surveillances= surveillance;
            _repositories = repositories;
            _userCodeService = userCodeService;
        }
        [HttpGet("StoreSuriveillance")]
        public IActionResult StoreSurveilllance ()
        {
            
            try{
                RepositoriesManagement.Repository repository = _repositories.create(new RepositoriesManagement.RepositoryCreateRequest
                {
                    Name = "HDB_DEV_AC_DEFAULT",
                    Type = "VIP"
                });
                var data = new SurveillanceRequest()
                {
                    Name = "HDB_DEV_AC_DEFAULT",
                    PushChanel = _configuation["OutsideSystems:FaceSettings:PushChanel"],
                    SubChanel = _configuation["OutsideSystems:FaceSettings:PushChanel"]
                };
                data.Repositories.Add(repository.Id);
                Surveillance surveillance = _surveillances.create(data);
                return ResponseOk(new { SurveillanceId = surveillance.Id });
            }
            catch (Exception ex)
            {
                return ResponseBadRequest(ex.Message);
            }
        }


        [HttpGet("test")]
        public IActionResult Test()
        {
            _userCodeService.RemoveCodeExpire();
            return Ok();
        }
    }
}
