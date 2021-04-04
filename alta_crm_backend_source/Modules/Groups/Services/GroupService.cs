using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Modules.Faces.FaceServices;
using Project.Modules.Groups.Enities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Services;
using Project.Modules.Users.Entities;
using Project.Modules.UserTagModes.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Services
{
    public interface IGroupService
    {
        #region Group
        Group InsertGroup(Group group);
        void DeleteGroup(Group group);
        Group UpdateGroup(Group group);
        IQueryable<Group> GetAllGroup(Expression<Func<Group, bool>> expression = null);
        Group GetGroupById(string groupId);
        #endregion

        IQueryable<ModeAuthentication> GetModeAuthentications(Expression<Func<ModeAuthentication, bool>> expression = null);
    }
    public class GroupService : IGroupService
    {
        private readonly IRepositoryWrapperMariaDB MariabDb;
        private readonly IConfiguration Configuration;

        private readonly IMeetingScheduleService _meetingScheduleService;
        private readonly IFaceGrpcService _faceGrpcService;

        public GroupService(IRepositoryWrapperMariaDB mariaDb, IConfiguration configuration, IFaceGrpcService faceGrpcService, IMeetingScheduleService meetingScheduleService)
        {
            MariabDb = mariaDb;
            Configuration = configuration;

            _faceGrpcService = faceGrpcService;
            _meetingScheduleService = meetingScheduleService;
        }

        #region Group
        public Group InsertGroup(Group group)
        {
            MariabDb.Groups.Add(group);
            MariabDb.SaveChanges();
            return group;
        }

        public void DeleteGroup(Group group)
        {
            List<User> users = MariabDb.Users.FindByCondition(x => x.GroupId.Equals(group.GroupId)).ToList();
            foreach (var user in users)
            {
                var userModes = MariabDb.UserModes.FindByCondition(x => x.UserId.Equals(user.UserId)).ToList();
                if (userModes.Count > 0)
                {
                    (bool resutlRemoveFace, string messageRemoveFace) = _faceGrpcService.RemoveFaceByExternal(userModes);
                    if (!resutlRemoveFace)
                    {
                        Console.WriteLine($"Delete group error:  Delete face by userId:{user.UserId}");
                    }

                    (object deleteRegisterDetect, string deleteRegisterDetectMessage) = _meetingScheduleService.DeleteRegisterDetect(user.UserId);
                    if (deleteRegisterDetect is null) Console.WriteLine($"DeleteRegisterDetect error:  Delete face by userId:{user.UserId} Message: {deleteRegisterDetectMessage}");
                }
            }

            List<Schedule> oldSchedules = MariabDb.Schedules.FindByCondition(x => users.Select(y=> y.UserId).Contains(x.UserId)).ToList();
            MariabDb.Schedules.RemoveRange(oldSchedules);

            List<UserTagMode> oldUserTagModes = MariabDb.UserTagModes.FindByCondition(x => users.Select(y => y.UserId).Contains(x.UserId)).ToList();
            MariabDb.UserTagModes.RemoveRange(oldUserTagModes);
            MariabDb.SaveChanges();

            MariabDb.Users.RemoveRange(users);
            MariabDb.SaveChanges();






            // xoa face 
            // xoa lich
            // user mode 
            // user tagmode


            MariabDb.Groups.Remove(group);
            MariabDb.SaveChanges();
        }

        public Group UpdateGroup(Group group)
        {
            group.UpdatedAt = DateTime.Now;
            MariabDb.Groups.Update(group);
            MariabDb.SaveChanges();
            return group;
        }

        public IQueryable<Group> GetAllGroup(Expression<Func<Group, bool>> expression = null)
        {
            return MariabDb.Groups.FindByCondition(expression)
                ;
        }

        public Group GetGroupById(string groupId)
        {
            return MariabDb.Groups.FindByCondition(x => x.GroupId == groupId)
                .FirstOrDefault()
                ;
        }
        #endregion

        public IQueryable<ModeAuthentication> GetModeAuthentications(Expression<Func<ModeAuthentication, bool>> expression = null)
        {
            return MariabDb.ModeAuthentications.FindByCondition(expression);
        }
    }
}
