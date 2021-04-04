using Confluent.Kafka;
using FaceManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Project.Modules.Faces.FaceServices;
using Project.Modules.Kafka.Producer;
using Project.Modules.Medias.Services;
using Project.Modules.Schedules.Requests;
using Project.Modules.Schedules.Services;
using Project.Modules.Tags.Enities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.UserKafka;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UserTagModes.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Modules.Users.Service
{
    public interface ISupportUserService
    {
        (User data, string message) RegisterFaceIdWithEmployee(string userId, IFormFile image);
        (User data, string message) RegisterFaceGrpc(string userId, IFormFile image, Tag tagFirst);
        (User data, string message) RegisterFaceGrpcImport(string userId, IFormFile image, Tag tagFirst);
        (DetectFace data, string message) DetectFaceGrpc(IFormFile image);

        (User data, string message) RegisterCardId(string userId, string keyCode, Tag tagFirst);
    }
    public class SupportUserService : ISupportUserService
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        private readonly IFaceService faceService;
        private readonly IFaceGrpcService FaceProtoService;
        private readonly IServiceScopeFactory scopeFactory;


        public SupportUserService(
            IRepositoryWrapperMariaDB repositoryWrapperMariaDB,
            IFaceService _faceService,
            IFaceGrpcService faceProtoService,
           IServiceScopeFactory scopeFactory



            )
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
            faceService = _faceService;
            this.scopeFactory = scopeFactory;
            FaceProtoService = faceProtoService;
        }

        public (User data, string message) RegisterFaceIdWithEmployee(string userId, IFormFile image)
        {

            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (image != null)
            {
                (RegisterFace dataRegisterFace, string messageRegisterFace) = faceService.RegisterFace(image);
                if (dataRegisterFace is null)
                {
                    return (null, messageRegisterFace);
                }

                UserMode userMode = RepositoryWrapperMariaDB.UserModes.FirstOrDefault(x => x.ModeId.Equals("Face_ID") && x.UserModeKeyCode.Equals(dataRegisterFace.Id));
                if (userMode != null)
                {
                    return (null, messageRegisterFace);
                }

                userMode = new UserMode
                {
                    ModeId = "Face_ID",
                    UserId = userId,
                    UserModeKeyCode = dataRegisterFace.Id
                };

                RepositoryWrapperMariaDB.UserModes.Add(userMode);

                user.UserImage = dataRegisterFace.FullPath;
                RepositoryWrapperMariaDB.SaveChanges();
            }
            return (user, "Success");
        }

        public (User data, string message) RegisterFaceGrpc(string userId, IFormFile image , Tag tagFirst )
        {
            if (tagFirst is null) return (null, "TagNotFound");
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user is null)
            {
                return (null, "UserIdNotExist");
            }
            if (image != null)
            {
                (RegisterFace dataRegisterFace, string messageRegisterFace) = FaceProtoService.RegisterFace(image, userId , tagFirst.RepositoryId, train: true);
                if (dataRegisterFace is null)
                {
                    return (null, messageRegisterFace);
                }

                UserMode userMode = RepositoryWrapperMariaDB.UserModes.FirstOrDefault(x => x.ModeId.Equals("Face_ID") && x.UserModeKeyCode.Equals(dataRegisterFace.Id));
                if (userMode != null)
                {
                    return (null, messageRegisterFace);
                }

                userMode = new UserMode
                {
                    ModeId = "Face_ID",
                    UserId = userId,
                    UserModeKeyCode = dataRegisterFace.Id,
                    UserModeImage = dataRegisterFace.FullPath, 
                    RepositoryId = tagFirst.RepositoryId
                };

                RepositoryWrapperMariaDB.UserModes.Add(userMode);

                user.UserImage = string.IsNullOrEmpty(user.UserImage) ? dataRegisterFace.FullPath : user.UserImage;
                RepositoryWrapperMariaDB.SaveChanges();
            }
            return (user, "Success");
        }

        public (User data, string message) RegisterCardId(string userId, string keyCode, Tag tagFirst)
        {
            if (tagFirst is null) return (null, "TagNotFound");
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user is null) return (null, "UserIdNotExist");
           
            UserMode userMode = RepositoryWrapperMariaDB.UserModes.FirstOrDefault(x =>  x.ModeId.Equals("Card_ID") && x.UserModeKeyCode.Equals(keyCode) && !x.UserId.Equals(userId));
            if (userMode != null) return (null, "CardIdAlreadyExists");


            userMode = new UserMode
            {
                ModeId = "Card_ID",
                UserId = userId,
                UserModeKeyCode = keyCode,
                UserModeImage = "",
                RepositoryId = tagFirst.RepositoryId
            };

            RepositoryWrapperMariaDB.UserModes.Add(userMode);
            RepositoryWrapperMariaDB.SaveChanges();
            return (user, "Success");
        }


        public (User data, string message) RegisterFaceGrpcImport(string userId, IFormFile image , Tag tagFirst)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var repositoryNew = scope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

                User user = repositoryNew.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
                if (user is null)
                {
                    return (null, "UserIdNotExist");
                }
                if (image != null)
                {
                    (RegisterFace dataRegisterFace, string messageRegisterFace) = FaceProtoService.RegisterFace(image, userId , tagFirst.RepositoryId);
                    if (dataRegisterFace is null)
                    {
                        return (null, messageRegisterFace);
                    }
                    try
                    {
                        UserMode userMode = repositoryNew.UserModes.FindByCondition(x => x.ModeId.Equals("Face_ID") && x.UserModeKeyCode.Equals(dataRegisterFace.Id)).FirstOrDefault();
                        if (userMode != null)
                        {
                            return (null, messageRegisterFace);
                        }

                        userMode = new UserMode
                        {
                            ModeId = "Face_ID",
                            UserId = userId,
                            UserModeKeyCode = dataRegisterFace.Id,
                            UserModeImage = dataRegisterFace.FullPath,
                            RepositoryId = tagFirst.RepositoryId
                        }; 

                        repositoryNew.UserModes.Add(userMode);

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("--------------------------------" + ex.Message);
                    }
                    




                 

                    user.UserImage = string.IsNullOrEmpty(user.UserImage) ? dataRegisterFace.FullPath : user.UserImage;
                    repositoryNew.SaveChanges();
                }
                return (user, "Success");
            }
        }

        public (DetectFace data, string message) DetectFaceGrpc(IFormFile image)
        {
            (DetectFace dataDetectFace, string messageRegisterFace) = FaceProtoService.DetectFace(image);
            if (dataDetectFace is null)
            {
                return (null, messageRegisterFace);
            }

            return (dataDetectFace, "Success");
        }
    }
}
