using AutoMapper;
using FaceManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using Project.App.DesignPatterns.ObserverPatterns;
using Project.App.Helpers;
using Project.App.Requests;
using Project.Modules.Faces.FaceServices;
using Project.Modules.Groups.Enities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Requests;
using Project.Modules.Schedules.Services;
using Project.Modules.Tags.Enities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UserTagModes.Entities;
using RepositoriesManagement;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Modules.Users.Service
{
    public interface IEmployeeService
    {
        public (object data, string message) ImportEmployeesSync(ImportEmployeeRequest request);

        public (object data, string message) CreateUser(CreateUserRequest request);
        public (object data, string message) UpdateUser(UpdateUserRequest request, string userId);

        public (bool result, string message) DeleteUser(string userId);

        public object SyncAllUser();
        public List<object> DeleteUserAll();

        public (object data, string message) UpdateCustomer(UpdateCustomerRequest request, string userId);

        public (bool result, string message) DeleteOneFaceImage(string userId, string faceId);
    }
    public class EmployeeService : IEmployeeService
    {

        private readonly ISupportUserService _supportUserService;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IRepositoryMongoWrapper _repositoryMongo;
        private readonly IRepositoryWrapperMariaDB _repositoryWrapperMariaDB;
        private readonly IMeetingScheduleService _meetingScheduleService;
        private readonly IFaceGrpcService _faceGrpcService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        private readonly FaceManagementService.FaceManagementServiceClient _faceManagementServiceClient;
        private readonly RepositoryManagement.RepositoryManagementClient _repositoryManagementClient;

        public EmployeeService(RepositoryManagement.RepositoryManagementClient repositoryManagementClient, IConfiguration configuration, FaceManagementService.FaceManagementServiceClient faceManagementServiceClient, IFaceGrpcService faceGrpcService, ISupportUserService supportUserService, IRepositoryWrapperMariaDB repositoryWrapperMariaDB, IRepositoryMongoWrapper repositoryMongo, IMapper mapper, IMeetingScheduleService meetingScheduleService, IServiceScopeFactory scopeFactory)
        {
            _supportUserService = supportUserService;
            _repositoryWrapperMariaDB = repositoryWrapperMariaDB;
            _repositoryMongo = repositoryMongo;
            _mapper = mapper;
            _meetingScheduleService = meetingScheduleService;
            _faceGrpcService = faceGrpcService;
            this.scopeFactory = scopeFactory;
            _configuration = configuration;

            _faceManagementServiceClient = faceManagementServiceClient;
            _repositoryManagementClient = repositoryManagementClient;
        }



        public (List<User> datas, string message) ReadExcel(FileInfo fileInfo, List<string> existDB, List<string> emailDB)
        {
            // mở file excel
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var package = new ExcelPackage(fileInfo);

            // lấy ra sheet đầu tiên để thao tác
            ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

            List<User> employees = new List<User>();
            List<Group> groups = _repositoryWrapperMariaDB.Groups.FindAll().ToList();

            // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                try
                {
                    // biến j biểu thị cho một column trong file
                    int j = 1;

                    string groupCode = workSheet.Cells[i, 7].Value?.ToString();
                    string tagCode = workSheet.Cells[i, 8].Value?.ToString();

                    string UserCodeActive = workSheet.Cells[i, 1].Value?.ToString();
                    string UserFirstName = workSheet.Cells[i, 3].Value?.ToString();
                    string UserLastName = workSheet.Cells[i, 2].Value?.ToString();
                    string UserPhone = workSheet.Cells[i, 4].Value?.ToString();
                    string UserEmail = workSheet.Cells[i, 5].Value?.ToString();
                    string UserGender = workSheet.Cells[i, 6].Value?.ToString();

                    if (String.IsNullOrEmpty(UserCodeActive) && String.IsNullOrEmpty(UserEmail))
                    {
                        continue;
                    }

                    User newEmployee = new User()
                    {
                        UserCode = UserCodeActive,
                        UserFirstName = UserFirstName,
                        UserLastName = UserLastName,
                        UserPhone = UserPhone,
                        UserEmail = UserEmail,
                        TagCodeImport = tagCode,
                        GroupCodeImport = groupCode,
                        UserGender = !string.IsNullOrEmpty(UserGender) ? UserGender.ParseInt() : 0,
                        IsImportSuccess = true,
                    };


                    if (String.IsNullOrEmpty(newEmployee.UserCode))
                    {
                        newEmployee.IsImportSuccess = false;
                        newEmployee.ErrorImport.Add("UserCodeNotEmptyOrNotNull");
                    }
                    if (newEmployee.UserCode.Length <= 0 || newEmployee.UserCode.Length > 20)
                    {
                        newEmployee.IsImportSuccess = false;
                        newEmployee.ErrorImport.Add("UserCodeIsGreaterThan1AndLessThan20");
                    }

                    //validate
                    string groupId = groups.FirstOrDefault(x => x.GroupCode.Equals(groupCode))?.GroupId;
                    if (String.IsNullOrEmpty(groupId))
                    {
                        newEmployee.IsImportSuccess = false;
                        newEmployee.ErrorImport.Add("GroupCodeInvalid");
                    }
                    newEmployee.GroupId = groupId;

                    if (existDB.Contains(newEmployee.UserCode)) // thieu
                    {
                        newEmployee.IsImportSuccess = false;
                        newEmployee.ErrorImport.Add("UserCodeDuplicate");
                    }

                    if (emailDB.Contains(newEmployee.UserEmail))
                    {
                        newEmployee.IsImportSuccess = false;
                        newEmployee.ErrorImport.Add("UserEmailDuplicate");
                    }
                    Tag tag = _repositoryWrapperMariaDB.Tags.FindByCondition(x => !string.IsNullOrEmpty(tagCode) && x.TagCode.Equals(tagCode)).FirstOrDefault();

                    if (tag is null)
                    {
                        newEmployee.IsImportSuccess = false;
                        string tagCodeMessage = !string.IsNullOrEmpty(tagCode) ? "TagNotFound" : "FieldTagCodeNull";
                        newEmployee.ErrorImport.Add(tagCodeMessage);
                    }
                    else
                    {
                        List<string> tagIds = new List<string>() { tag.TagId };
                        newEmployee.UserTagIds = !string.IsNullOrEmpty(tagCode) ? JsonConvert.SerializeObject(tagIds) : null;
                        newEmployee.UserTags.Add(tag);
                    }

                    employees.Add(newEmployee);
                    existDB.Add(newEmployee.UserCode);
                    emailDB.Add(newEmployee.UserEmail);
                }
                catch (Exception exe)
                {
                    return (null, exe.Message);
                }
            }
            return (employees, "success");
        }

        public (object data, string message) ImportEmployeesSync(ImportEmployeeRequest request)
        {


            #region Giải nén
            (string zipDestination, string fileName, string messageUploadFile) = FilesExtensions.UploadFileLocalAndUnzipAsync(request.FileZip).Result;
            #endregion

            #region Đọc File Excel
            const string fileExcelName = "/UserData.xlsx";
            if (!File.Exists(zipDestination + fileExcelName))
            {

                return (null, "FileExcelNotExists");
            }
            FileInfo fileExcel = new FileInfo(zipDestination + fileExcelName);

            //Handle data
            List<string> userCodes = _repositoryWrapperMariaDB.Users.FindAll().Select(x => x.UserCode).ToList();
            List<string> emails = _repositoryWrapperMariaDB.Users.FindAll().Select(x => x.UserEmail).ToList();
            (List<User> users, string message) = ReadExcel(fileExcel, userCodes, emails);
            #endregion


            LogFile logFile = new LogFile
            {
                FileName = request.FileZip.FileName,
                ImportDate = DateTime.UtcNow,
                FilePath = $"upload/{fileName.Replace(".zip", "")}",
            };
            _repositoryMongo.LogFiles.Add(logFile);

            List<LogUserImport> logUserImports = new List<LogUserImport>();
            try
            {
                foreach (var item in users.ToList())
                {
                    LogUserImport logUserImport = new LogUserImport()
                    {
                        LogFileId = logFile.LogFileId,
                        UserId = item.UserId,
                        UserCode = item.UserCode,
                        GroupCode = item.GroupCodeImport,
                        TagCode = item.TagCodeImport,
                        LastName = item.UserLastName,
                        FirstName = item.UserFirstName,
                        Gender = item.UserGender,
                        Phone = item.UserPhone,
                        ErrorMessage = item.ErrorImport
                    };

                    if (item.IsImportSuccess)
                    {
                        #region Tạo User trong DB 
                        _repositoryWrapperMariaDB.Users.Add(item);
                        _repositoryWrapperMariaDB.SaveChanges();
                        #endregion
                        #region Ghi log
                        logUserImport.IsSuccess = true;
                        #endregion
                    }
                    logUserImports.Add(logUserImport);

                }
                if (logUserImports.Count > 0)
                {
                    _repositoryMongo.LogUserImport.AddRange(logUserImports);
                }

                Task.Run(() =>
                {
                    LogFile logFileUpdate = _repositoryMongo.LogFiles.FindByCondition(x => x.LogFileId.Equals(logFile.LogFileId)).FirstOrDefault();
                    foreach (var item in users.Where(x => x.IsImportSuccess).ToList())
                    {
                        FuncRegisterImage(item, zipDestination, item.UserTags.FirstOrDefault());
                    }
                    List<Tag> tagTrain = users.Where(x => x.IsImportSuccess).Select(x => x.UserTags.FirstOrDefault()).GroupBy(x => x.TagCode).Select(x => x.FirstOrDefault()).ToList();
                    foreach (var item in tagTrain)
                    {
                        try
                        {
                            var data = _repositoryManagementClient.trainAsync(new RepositoriesManagement.Repository { Id = item.RepositoryId });
                            logFileUpdate.TrainTags.Add(new TrainTag()
                            {
                                TagCode = item.TagCode,
                                IsTrainSuccess = true,
                                Message = "Success"
                            });
                        }
                        catch (Exception ex)
                        {
                            logFileUpdate.TrainTags.Add(new TrainTag()
                            {
                                TagCode = item.TagCode,
                                Message = ex.Message
                            });
                        }
                        _repositoryMongo.LogFiles.Update(logFileUpdate, x => x.LogFileId.Equals(logFileUpdate.LogFileId));
                    }

                });

                if (users.Count(x => x.IsImportSuccess) == 0)
                {
                    return (null, "AllDataInFileExcelInvalid");
                }
                return (users, "ImportSuccess");
            }

            catch (Exception ex)
            {
                return (null, $"Error:{ex.Message}");
            }

        }

        public void FuncRegisterImage(User item, string zipDestination, Tag tagDefault)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
                var unitOfWorkMonggo = scope.ServiceProvider.GetRequiredService<IRepositoryMongoWrapper>();

                LogUserImport logUserImport = unitOfWorkMonggo.LogUserImport.FindByCondition(x => x.UserId.Equals(item.UserId)).FirstOrDefault();


                #region Validate folder images
                // Lấy tất cả file hình trong Folder 
                var pathFolderImages = zipDestination + "/UserIMG" + $"/{item.UserCode}";
                bool folderImageExists = Directory.Exists(pathFolderImages);
                if (!folderImageExists)
                {
                    logUserImport.ErrorMessage.Add($"{pathFolderImages}:NotFound");
                    return;
                }
                List<string> images = Directory.GetFiles(pathFolderImages).ToList();
                if (images.Count == 0)
                {
                    logUserImport.ErrorMessage.Add($"NotImages");
                    return;
                }
                #endregion

                #region Đăng kí tất cả hình của User
                foreach (string image in images)
                {

                    if (Directory.Exists(image))
                    {
                        InfoImage infoImage = new InfoImage
                        {
                            path = image,
                            message = "Image Not Exist"
                        };

                        logUserImport.Images.Add(infoImage);
                        continue;
                    }

                    #region Check ContentType
                    if (!CheckContentType(image))
                    {

                        InfoImage infoImage = new InfoImage
                        {
                            path = image,
                            message = "Image Content Type Not Allower"
                        };

                        logUserImport.Images.Add(infoImage);
                        continue;
                    }
                    #endregion

                    using (var stream = System.IO.File.OpenRead(image))
                    {
                        IFormFile photo = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                        (User registerFace, string messageRegisterFace) = _supportUserService.RegisterFaceGrpcImport(item.UserId, photo, tagDefault);

                        InfoImage infoImage = new InfoImage
                        {
                            path = image,
                            message = messageRegisterFace

                        };

                        if (registerFace != null)
                        {
                            infoImage.isFace = true;
                            infoImage.pathSuccess = registerFace.UserImage;
                            Console.WriteLine(registerFace.UserImage);
                        }
                        logUserImport.Images.Add(infoImage);
                    }

                }

                #region Thêm Avt cho user
                string tmpImageRegisterSuccess = logUserImport.Images.FirstOrDefault(x => !String.IsNullOrEmpty(x.pathSuccess))?.pathSuccess;
                if (tmpImageRegisterSuccess != null)
                {
                    item.UserImage = tmpImageRegisterSuccess;
                    repository.Users.Update(item);
                    repository.SaveChanges();
                }
                #endregion

                #region Tạo 1 row trong UserTagMode nếu có Tag và đăng kí hình thành công
                if (!string.IsNullOrEmpty(item.UserTagIds))
                {
                    UserTagMode userTagMode = new UserTagMode()
                    {
                        UserId = item.UserId,
                        TagId = JsonConvert.DeserializeObject<List<string>>(item.UserTagIds).FirstOrDefault(),
                        ModeId = "Face_ID",
                    };
                    repository.UserTagModes.Add(userTagMode);
                    repository.SaveChanges();

                    RegisterMeetingWithTagMode register = new RegisterMeetingWithTagMode()
                    {
                        UserId = item.UserId,
                        TagModes = new List<TagMode>() { new TagMode
                                                        {
                                                            ModeId = "Face_ID",
                                                            TagId = JsonConvert.DeserializeObject<List<string>>(item.UserTagIds).FirstOrDefault() }
                                    }
                    };
                    var scheduleEmployee = _meetingScheduleService.RegisterMeetingWithTagModeImport(register);

                    if (scheduleEmployee.data is null)
                    {
                        item.ErrorImport.Add($"{scheduleEmployee.message}");
                        logUserImport.IsSchedule = false;
                        logUserImport.MessageSchedule = scheduleEmployee.message;
                    }
                    else
                    {
                        logUserImport.IsSchedule = true;
                    }

                }
                #endregion

                #endregion

                unitOfWorkMonggo.LogUserImport.Update(logUserImport, x => x.Id.Equals(logUserImport.Id));
            }
        }

        public bool CheckContentType(string file)
        {
            List<string> contentTypes = new List<string> { ".JPG", ".JPEG", ".PNG" };
            if (contentTypes.Contains(Path.GetExtension(file).ToUpperInvariant()))
            {
                return true;
            }
            return false;
        }


        public (object data, string message) CreateUser(CreateUserRequest request)
        {
            #region Validate tag 
            if (request.TagIdsParse is null || request.TagIdsParse.Any(x => string.IsNullOrEmpty(x))) request.TagIdsParse = new List<string>();
            foreach (var item in request.TagIdsParse)
            {
                Tag tmpTag = _repositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagId.Equals(item));
                if (tmpTag is null) return (null, $"TagIdNotFound");
            }

            Group group = _repositoryWrapperMariaDB.Groups.FirstOrDefault(x => x.GroupId.Equals(request.GroupId));
            if (group is null) return (null, $"GroupIdNotFound");
            #endregion

            List<string> codeExists = _repositoryWrapperMariaDB.Users.FindAll().Select(x => x.UserCode).ToList();
            var transaction = _repositoryWrapperMariaDB.BeginTransaction();
            try
            {
                #region Created user 
                User user = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserEmail == request.UserEmail).FirstOrDefault();
                if (user != null) return (null, "UserEmailAlreadyExists");

                User newUser = _mapper.Map<CreateUserRequest, User>(request);
                newUser.UserCode = GeneralHelper.RandomCode(6, codeExists);
                newUser.UserTagIds = JsonConvert.SerializeObject(request.TagIdsParse);

                _repositoryWrapperMariaDB.Users.Add(newUser);
                _repositoryWrapperMariaDB.SaveChanges();
                #endregion

                switch (request.ModeId)
                {
                    case null:
                    case "":
                        {
                            break;
                        }
                    case "Face_ID":
                        {
                            #region Register face nếu có
                            if (request.UserImages != null)
                            {
                                Tag tagDefault = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(request.TagIdsParse.FirstOrDefault())).FirstOrDefault();
                                foreach (var item in request.UserImages)
                                {
                                    (User tmpRegisterFace, string tmpMessageRegisterFace) = _supportUserService.RegisterFaceGrpc(newUser.UserId, item, tagDefault);
                                    if (tmpRegisterFace is null)
                                    {
                                        transaction.Rollback();
                                        return (null, tmpMessageRegisterFace);
                                    }
                                }


                                #region Handle add tags nếu có
                                List<UserTagMode> newUserTagModes = new List<UserTagMode>();
                                foreach (var item in request.TagIdsParse)
                                {
                                    newUserTagModes.Add(new UserTagMode
                                    {
                                        ModeId = request.ModeId,
                                        TagId = item,
                                        UserId = newUser.UserId,
                                    });
                                }
                                _repositoryWrapperMariaDB.UserTagModes.AddRange(newUserTagModes);
                                _repositoryWrapperMariaDB.SaveChanges();
                                #endregion

                                #region Đăng ký lịch theo mode 
                                //call fn : Đăng ký lịch họp cho user. 
                                (object registerMetting, string messageRegisterMetting) = _meetingScheduleService.SendKafkaForRegisterFaceId(newUser.UserId, request.ModeId);
                                if (registerMetting is null)
                                {
                                    transaction.Rollback();
                                    return (null, "Đăng ký lịch họp cho user error");
                                }
                                //call fn : Đăng ký user tag mode cho user
                                (object registerSchedule, string messageSchedule) = _meetingScheduleService.RegisterMeetingWithTagMode(new RegisterMeetingWithTagMode
                                {
                                    UserId = newUser.UserId,
                                    TagModes = _mapper.Map<List<TagMode>>(newUserTagModes)
                                });
                                if (registerSchedule is null)
                                {
                                    transaction.Rollback();
                                    return (null, " Đăng ký user tag mode cho user error");
                                }
                                #endregion
                            }
                            #endregion
                            break;
                        }

                    case "Card_ID":
                        {
                            //RegisterCardId
                            #region Register face nếu có
                            if (!String.IsNullOrEmpty(request.UserModeKeyCode))
                            {
                                Tag tagDefault = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(request.TagIdsParse.FirstOrDefault())).FirstOrDefault();
                                (User tmpRegisterCard, string tmpMessageRegisterCard) = _supportUserService.RegisterCardId(newUser.UserId, request.UserModeKeyCode, tagDefault);
                                if (tmpRegisterCard is null)
                                {
                                    transaction.Rollback();
                                    return (null, tmpMessageRegisterCard);
                                }


                                #region Handle add tags nếu có
                                List<UserTagMode> newUserTagModes = new List<UserTagMode>();
                                foreach (var item in request.TagIdsParse)
                                {
                                    newUserTagModes.Add(new UserTagMode
                                    {
                                        ModeId = request.ModeId,
                                        TagId = item,
                                        UserId = newUser.UserId,
                                    });
                                }
                                _repositoryWrapperMariaDB.UserTagModes.AddRange(newUserTagModes);
                                _repositoryWrapperMariaDB.SaveChanges();
                                #endregion

                                #region Đăng ký lịch theo mode 
                                //call fn : Đăng ký lịch họp cho user. 
                                (object registerMetting, string messageRegisterMetting) = _meetingScheduleService.SendKafkaForRegisterFaceId(newUser.UserId, request.ModeId);
                                if (registerMetting is null)
                                {
                                    transaction.Rollback();
                                    return (null, "Đăng ký lịch họp cho user error");
                                }
                                //call fn : Đăng ký user tag mode cho user
                                (object registerSchedule, string messageSchedule) = _meetingScheduleService.RegisterMeetingWithTagMode(new RegisterMeetingWithTagMode
                                {
                                    UserId = newUser.UserId,
                                    TagModes = _mapper.Map<List<TagMode>>(newUserTagModes)
                                });
                                if (registerSchedule is null)
                                {
                                    transaction.Rollback();
                                    return (null, " Đăng ký user tag mode cho user error");
                                }
                                #endregion
                            }
                            #endregion
                            break;
                        }
                    default:
                        {
                            return (null, "ModeNotSupport");
                        }
                }
                transaction.Commit();
                return (newUser, "Success");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, $"Error:{ex.Message}");
            }
        }

        public (object data, string message) UpdateUser(UpdateUserRequest request, string userId)
        {

            #region Validate tag 
            if (request.TagIdsParse is null || request.TagIdsParse.Any(x => string.IsNullOrEmpty(x))) request.TagIdsParse = new List<string>();
            foreach (var item in request.TagIdsParse)
            {
                Tag tmpTag = _repositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagId.Equals(item));
                if (tmpTag is null) return (null, $"TagIdNotFound");
            }
            #endregion

            var transaction = _repositoryWrapperMariaDB.BeginTransaction();
            try
            {
                #region Update user info
                User user = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
                if (user is null) return (null, "UserNotFound");

                User prepareDataUpdate = _mapper.Map<UpdateUserRequest, User>(request, user);
                prepareDataUpdate.UserTagIds = JsonConvert.SerializeObject(request.TagIdsParse);
                user = prepareDataUpdate;
                _repositoryWrapperMariaDB.SaveChanges();
                #endregion


                #region Delete schedule access control 
                (object deleteRegisterDetect, string deleteRegisterDetectMessage) = _meetingScheduleService.DeleteRegisterDetect(userId);
                if (deleteRegisterDetect is null)
                {
                    transaction.Rollback();
                    return (false, deleteRegisterDetectMessage); 
                }

                List<UserTagMode> oldUserTagModes = _repositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.UserId.Equals(userId)).ToList();
                _repositoryWrapperMariaDB.UserTagModes.RemoveRange(oldUserTagModes);
                _repositoryWrapperMariaDB.SaveChanges();
                #endregion

                switch (request.ModeId)
                {
                    case null:
                    case "":
                        {
                            break;
                        }
                    case "Face_ID":
                        {
                            #region Register face nếu có
                            if (request.UserImages != null)
                            {
                                var tagIdDefault = user.UserTagIdsParse.FirstOrDefault();
                                if (tagIdDefault is null) return (null, "TagNotFound");
                                Tag tagDefault = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(tagIdDefault)).FirstOrDefault();

                                foreach (var item in request.UserImages)
                                {
                                    (User tmpRegisterFace, string tmpMessageRegisterFace) = _supportUserService.RegisterFaceGrpc(user.UserId, item, tagDefault);
                                    if (tmpRegisterFace is null)
                                    {
                                        transaction.Rollback();
                                        return (null, tmpMessageRegisterFace);
                                    }
                                }  
                            }
                            #endregion
                            break;
                        }
                    case "Card_ID":
                        {

                            if (!String.IsNullOrEmpty(request.UserModeKeyCode))
                            {
                                Tag tagDefault = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(request.TagIdsParse.FirstOrDefault())).FirstOrDefault();

                                var allUserMode = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(user.UserId) && x.ModeId.Equals("Card_ID")).ToList();
                                _repositoryWrapperMariaDB.UserModes.RemoveRange(allUserMode);
                                (User tmpRegisterCard, string tmpMessageRegisterCard) = _supportUserService.RegisterCardId(user.UserId, request.UserModeKeyCode, tagDefault);
                                if (tmpRegisterCard is null)
                                {
                                    transaction.Rollback();
                                    return (null, tmpMessageRegisterCard);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            return (null, "ModeNotSupport");
                        }
                }




                #region Đăng ký lịch theo mode 
                //call fn : Đăng ký lịch họp cho user. 
                List<string> modeIds = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(userId)).Select(x => x.ModeId).Distinct().ToList();

                (object registerMettingScheduleTmp, string messageMettingScheduleTmp) = _meetingScheduleService.RegisterMeetingScheduleByUserId(userId, modeIds);
                if (registerMettingScheduleTmp is null)
                {
                    transaction.Rollback();
                    return (null, " Đăng ký user tag mode cho user error");
                }
                
                //(object registerMetting, string messageRegisterMetting) = _meetingScheduleService.SendKafkaForRegisterFaceId(user.UserId);
                //if (registerMetting is null)
                //{
                //    transaction.Rollback();
                //    return (null, "Đăng ký lịch họp cho user error");
                //}
                //call fn : Đăng ký user tag mode cho user
                List<UserTagMode> newUserTagModes = new List<UserTagMode>();
                foreach (var modeId in modeIds)
                {
                    foreach (var item in request.TagIdsParse)
                    {
                        newUserTagModes.Add(new UserTagMode
                        {
                            ModeId = modeId,
                            TagId = item,
                            UserId = user.UserId,
                        });
                    }
                }
               
                _repositoryWrapperMariaDB.UserTagModes.AddRange(newUserTagModes);
                _repositoryWrapperMariaDB.SaveChanges();
                List<UserTagMode> newUserTagModesTmp = _repositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.UserId.Equals(userId)).ToList();
                (object registerScheduleTmp, string messageScheduleTmp) = _meetingScheduleService.RegisterMeetingWithTagMode(new RegisterMeetingWithTagMode
                {
                    UserId = user.UserId,
                    TagModes = _mapper.Map<List<TagMode>>(newUserTagModesTmp)
                });
                if (registerScheduleTmp is null)
                {
                    transaction.Rollback();
                    return (null, " Đăng ký user tag mode cho user error");
                }
                #endregion


                transaction.Commit();
                return (user, "UpdateUserSuccess"); 
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, $"Error:{ex.Message}");
            }
        }

        public (object data, string message) UpdateCustomer(UpdateCustomerRequest request, string userId)
        {
            var transaction = _repositoryWrapperMariaDB.BeginTransaction();
            try
            {
                #region Update thông tin customer
                User user = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
                if (user is null) return (null, "CustomerNotFound");

                user.UserFirstName = request.CustomerFirstName;
                user.UserLastName = request.CustomerLastName;
                user.UserPhone = request.CustomerPhone;
                user.UserAddress = request.CustomerAddress;
                user.UserGender = request.CustomerGender.HasValue ? 0 : request.CustomerGender.Value;
                #endregion


                #region Delete schedule access control 
                (object deleteRegisterDetect, string deleteRegisterDetectMessage) = _meetingScheduleService.DeleteRegisterDetect(userId);
                if (deleteRegisterDetect is null)
                {
                    transaction.Rollback();
                    return (false, deleteRegisterDetectMessage);
                }

                List<UserTagMode> oldUserTagModes = _repositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.UserId.Equals(userId)).ToList();
                _repositoryWrapperMariaDB.UserTagModes.RemoveRange(oldUserTagModes);
                _repositoryWrapperMariaDB.SaveChanges();
                #endregion


                #region Xữ lý update mode và thông tin của mode đó.
                var tagIdDefault = user.UserTagIdsParse.FirstOrDefault();
                switch (request.ModeId)
                {
                    case null:
                    case "":
                        {
                            break;
                        }
                    case "Face_ID":
                        {
                            #region Register face nếu có
                            if (request.CustomerImages != null)
                            {
                                
                                Tag tagDefault = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(tagIdDefault)).FirstOrDefault();
                                if (tagDefault is null) return (null, "TagDefaultNotFound");

                                foreach (var item in request.CustomerImages)
                                {
                                    (User tmpRegisterFace, string tmpMessageRegisterFace) = _supportUserService.RegisterFaceGrpc(user.UserId, item, tagDefault);
                                    if (tmpRegisterFace is null)
                                    {
                                        transaction.Rollback();
                                        return (null, tmpMessageRegisterFace);
                                    }
                                }
                            }
                            #endregion
                            break;
                        }
                    case "Card_ID":
                        {
                            if (!String.IsNullOrEmpty(request.CustomerModeKeyCode))
                            {
                                Tag tagDefault = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(tagIdDefault)).FirstOrDefault();
                                if (tagDefault is null) return (null, "TagDefaultNotFound");

                                var allUserMode = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(user.UserId) && x.ModeId.Equals("Card_ID")).ToList();
                                _repositoryWrapperMariaDB.UserModes.RemoveRange(allUserMode);
                                (User tmpRegisterCard, string tmpMessageRegisterCard) = _supportUserService.RegisterCardId(user.UserId, request.CustomerModeKeyCode, tagDefault);
                                if (tmpRegisterCard is null)
                                {
                                    transaction.Rollback();
                                    return (null, tmpMessageRegisterCard);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            return (null, "ModeNotSupport");
                        }
                }
                #endregion


                #region Đăng ký lịch user
                List<string> modeIds = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(userId)).Select(x => x.ModeId).Distinct().ToList();
                // Đăng ký user tag mode cho user
                List<UserTagMode> newUserTagModes = new List<UserTagMode>();


                foreach (var modeId in modeIds)
                {
                    newUserTagModes.Add(new UserTagMode
                    {
                        ModeId = modeId,
                        TagId = tagIdDefault,
                        UserId = user.UserId,
                    });
                }
                _repositoryWrapperMariaDB.UserTagModes.AddRange(newUserTagModes);
                _repositoryWrapperMariaDB.SaveChanges();

                #region Lịch họp
                (object registerMettingScheduleTmp, string messageMettingScheduleTmp) = _meetingScheduleService.RegisterMeetingScheduleByUserId(userId, modeIds);
                if (registerMettingScheduleTmp is null)
                {
                    transaction.Rollback();
                    return (null, " Đăng ký user tag mode cho user error");
                }
                #endregion

                #region Đăng ký lịch normal
                List<UserTagMode> newUserTagModesTmp = _repositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.UserId.Equals(userId)).ToList();
                (object registerScheduleTmp, string messageScheduleTmp) = _meetingScheduleService.RegisterMeetingWithTagMode(new RegisterMeetingWithTagMode
                {
                    UserId = user.UserId,
                    TagModes = _mapper.Map<List<TagMode>>(newUserTagModesTmp)
                });
                if (registerScheduleTmp is null)
                {
                    transaction.Rollback();
                    return (null, " Đăng ký user tag mode cho user error");
                }
                #endregion


                #endregion

                _repositoryWrapperMariaDB.SaveChanges();
                transaction.Commit();
                return (new CustomerWelCome(user), "UpdateUserSuccess");

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, $"Error:{ex.Message}");
            }
        }


        public (bool result, string message) DeleteUser(string userId)
        {
            User user = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (user is null) return (false, "UserNotFound");

            var userModes = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(userId) && x.ModeId.Equals("Face_ID")).ToList();

            if (userModes.Count > 0)
            {
                (bool resutlRemoveFace, string messageRemoveFace) = _faceGrpcService.RemoveFaceByExternal(userModes);
                if (!resutlRemoveFace) return (false, messageRemoveFace);
            }
           

            (object deleteMeetingSchedule, string deleteMeetingScheduleMessage) = _meetingScheduleService.DeleteRegisterDetect(userId);
            if (deleteMeetingSchedule is null) return (false, deleteMeetingScheduleMessage);


            List<Schedule> oldSchedules = _repositoryWrapperMariaDB.Schedules.FindByCondition(x => x.UserId.Equals(userId)).ToList();
            _repositoryWrapperMariaDB.Schedules.RemoveRange(oldSchedules);

            List<UserTagMode> oldUserTagModes = _repositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.UserId.Equals(userId)).ToList();
            _repositoryWrapperMariaDB.UserTagModes.RemoveRange(oldUserTagModes);
            _repositoryWrapperMariaDB.SaveChanges();


            _repositoryWrapperMariaDB.Users.Remove(user);
            _repositoryWrapperMariaDB.SaveChanges();
            return (true, "DeleteUserSuccess");
        }


        public List<object> DeleteUserAll()
        {
            List<object> result = new List<object>();
            List<User> users = _repositoryWrapperMariaDB.Users.FindByCondition(x => true).ToList();
            foreach (var item in users)
            {
                (bool tmpResult, string tmpMessage) = DeleteUser(item.UserId);
                result.Add(new
                {
                    user = item,
                    result = tmpResult
                });

            }

            return result;
        }


        /// <summary>
        /// Cập nhật lại dữ liệu faceId khi user đã tồn tại.
        /// </summary>
        /// <returns></returns>
        public object SyncAllUser()
        {

            List<string> errors = new List<string>();
            List<object> added = new List<object>();
            List<UserMode> userModes = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.ModeId.Equals("Face_ID")).ToList();
            foreach (var userMode in userModes)
            {
                var faceRequest = new FaceRequest
                {
                    Bucket = _configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"],
                    ObjectName = userMode.UserModeImage,
                    ExternalId = userMode.UserId,
                    Repository = _configuration["OutsideSystems:FaceSettings:App"]
                };
                FaceResponse result = _faceManagementServiceClient.register(faceRequest);
                if (!String.IsNullOrEmpty(result.Id))
                {
                    userMode.UserModeKeyCode = result.Id;
                    added.Add(userMode);
                }
                else
                {
                    errors.Add($"Erro:UserModeID:{userMode.UserModeKeyCode}:{JsonConvert.SerializeObject(result)}");

                }
            }
            _repositoryWrapperMariaDB.SaveChanges();
            return new { errors = errors, added = added };
        }

        public (bool result, string message) DeleteOneFaceImage(string userId, string faceId)
        {
            UserMode userMode = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(userId) && x.UserModeKeyCode.Equals(faceId)).FirstOrDefault();
            if (userMode is null) return (false, "UserNotFound");

            FaceRemoveRequest requestRemoveFace = new FaceRemoveRequest
            {
                Id = faceId,
                Repository = userMode.RepositoryId
            };
            try
            {
                Common.HTTP resultRemoveImage = _faceManagementServiceClient.remove(requestRemoveFace);
                if (resultRemoveImage.Status != 200) return (false, resultRemoveImage.Message);
                _repositoryWrapperMariaDB.UserModes.Remove(userMode);
                _repositoryWrapperMariaDB.SaveChanges();
                return (true, "DeletedFaceImageSuccess");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }


        }


    }

}
