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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : BaseController
    {
        public readonly IConfiguration Configuration;
        public readonly IRatingService RatingService;
        public RatingController(IConfiguration configuration, IRatingService ratingService)
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
        public async Task<IActionResult> GetRatings([FromQuery] RatingDelegatePaginationRequest request)
        {

            //Filter
            Expression<Func<Rating, bool>> filter = x =>
                        (
                        (request.DateFrom != null
                        ? request.DateFrom <= x.CreatedAt
                        : true)
                        &&
                        (request.DateTo != null
                        ? request.DateTo >= x.CreatedAt
                        : true)
                        )
                        ;


            IQueryable<Rating> ratings = RatingService.GetRatings(filter);
            #region RequestTable
            //Sort
            ratings = ratings.ApplySort<Rating>(request.OrderByQuery);
            //Search
            ratings = ratings.Where(m => String.IsNullOrEmpty(request.SearchContent) ||
                                       (!String.IsNullOrEmpty(m.NameDisplay) && m.NameDisplay.ToLower().Contains(request.SearchContent))
                                       )
                                ;
            var Pagination = PaginationHelper<Rating>.ToPagedList(ratings, request.PageNumber, request.PageSize);
            PaginationResponse<Rating> paginationResponse = new PaginationResponse<Rating>(Pagination, Pagination.PageInfo);
            #endregion
            return ResponseOk(paginationResponse, "GetTagsSuccess");
        }
        
        /// <summary>
        /// (API APP) (Huỳnh Anh)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InsertRating([FromBody] InsertRatingRequest insertRating)
        {
            Rating rating = new Rating
            {
                RobotId = insertRating.RobotId,
                Star = insertRating.Star,
                UserId = insertRating.UserId,
                NameDisplay = insertRating.NameDisplay
            };
            rating = RatingService.Insert(rating);
            return ResponseOk(rating, "InsertRatingSuccess");
        }
    }
}
