using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Ratings.Enities;
using Project.Modules.Ratings.Requests;
using Project.Modules.Ratings.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RobotController : BaseController
    {
        public readonly IConfiguration Configuration;
        public readonly IRatingService RatingService;
        public RobotController(IConfiguration configuration, IRatingService ratingService)
        {
            Configuration = configuration;
            RatingService = ratingService;
        }

        /// <summary>
        /// Hiển thị tất cả group (Có pagination) (Huỳnh Anh)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRobots([FromQuery] PaginationRequest request)
        {
            IQueryable<Robot> robots = RatingService.GetRobots();
            #region RequestTable
            //Sort
            robots = robots.ApplySort<Robot>(request.OrderByQuery);

            //Search
            robots = robots.Where(m => String.IsNullOrEmpty(request.SearchContent) ||
                                       (!String.IsNullOrEmpty(m.RobotName) && m.RobotName.ToLower().Contains(request.SearchContent)) 
                                       );

            
            var Pagination = PaginationHelper<Robot>.ToPagedList(robots, request.PageNumber, request.PageSize);
            PaginationResponse<Robot> paginationResponse = new PaginationResponse<Robot>(Pagination, Pagination.PageInfo);
            #endregion
            return ResponseOk(paginationResponse, "GetRobotsSuccess");
        }

        [HttpGet("{robotId}")]
        public async Task<IActionResult> GetById(string robotId)
        {
            Robot robot = RatingService.GetById(robotId);
            if (robot is null)
            {
                return ResponseBadRequest("RobotNotExists");
            }
            return ResponseOk(robot, "GetRobotSuccess");
        }

        [HttpPost]
        public async Task<IActionResult> InsertRobot([FromBody] InsertRobotRequest insertRobot)
        {
            Robot robot = new Robot
            {
                RobotName = insertRobot.RobotName
            };
            robot = RatingService.Insert(robot);
            return ResponseOk(robot, "InserRobotSuccess");
        }

        [HttpDelete("{robotId}")]
        public async Task<IActionResult> DeleteRobot(string robotId)
        {
            Robot robot = RatingService.GetById(robotId);
            if (robot is null)
            {
                return ResponseBadRequest("RobotNotExists");
            }
            RatingService.Delete(robot);
            return ResponseOk("DeleteRobotSuccess");
        }
    }
}
