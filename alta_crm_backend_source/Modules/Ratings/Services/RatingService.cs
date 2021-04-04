using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Modules.Ratings.Enities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Services
{
    public interface IRatingService
    {
        Robot Insert(Robot robot);
        void Delete(Robot robot);
        Robot Update(Robot robot);
        IQueryable<Robot> GetRobots(Expression<Func<Robot, bool>> expression = null);
        Robot GetById(string Id);
        Rating Insert(Rating rating);
        void Delete(Rating rating);
        Rating Update(Rating rating);
        IQueryable<Rating> GetRatings(Expression<Func<Rating, bool>> expression = null);
        Rating GetRatingById(string Id);
    }
    public class RatingService : IRatingService
    {
        public readonly IConfiguration Configuration;
        public readonly IRepositoryWrapperMariaDB MariabDb;
        public RatingService(IConfiguration configuration, IRepositoryWrapperMariaDB mariaDb)
        {
            Configuration = configuration;
            MariabDb = mariaDb;
        }

        #region Robot
        public Robot Insert(Robot robot)
        {
            MariabDb.Robots.Add(robot);
            MariabDb.SaveChanges();
            return robot;
        }

        public void Delete(Robot robot)
        {
            MariabDb.Robots.Remove(robot);
            MariabDb.SaveChanges();
        }

        public Robot Update(Robot robot)
        {
            MariabDb.Robots.Update(robot);
            MariabDb.SaveChanges();
            return robot;
        }

        public IQueryable<Robot> GetRobots(Expression<Func<Robot, bool>> expression = null)
        {
            return MariabDb.Robots.FindByCondition(expression)
                .Include(x => x.Ratings)
                ;
        }
        public Robot GetById(string Id)
        {
            return MariabDb.Robots.FindByCondition(x => x.RobotId == Id)
                .Include(x => x.Ratings)
                .FirstOrDefault();
        }
        #endregion

        #region Rating
        public Rating Insert(Rating rating)
        {
            MariabDb.Ratings.Add(rating);
            MariabDb.SaveChanges();
            return rating;
        }

        public void Delete(Rating rating)
        {
            MariabDb.Ratings.Remove(rating);
            MariabDb.SaveChanges();
        }

        public Rating Update(Rating rating)
        {
            MariabDb.Ratings.Update(rating);
            MariabDb.SaveChanges();
            return rating;
        }

        public IQueryable<Rating> GetRatings(Expression<Func<Rating, bool>> expression = null)
        {
            return MariabDb.Ratings.FindByCondition(expression);
        }
        public Rating GetRatingById(string Id)
        {
            return MariabDb.Ratings.FindByCondition(x => x.RatingId == Id).FirstOrDefault();
        }
        #endregion
    }
}
