using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.RegisterDetects.Requests;
using Project.Modules.RegisterDetects.Validations;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.Tickets.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Services
{
    public interface IRegisterDetectService
    {
        public (object resutl, string message) RegisterUserDetect(RegisterUserDetectRequest request);
        public (object resutl, string message) RegisterUserDetectMutil(RegisterUserDetectMutilRequest request);

        public (bool resutl, string message) UnRegisterUserDetect(UnRegisterUserDetectRequest request);
        public (bool result, string message) UnRegisterUserMutil(UnRegisterUserDetectMutilRequest request);
        public (bool result, string message) UnRegisterUserWithUserId(string userId);

    }
    public class RegisterDetectService : IRegisterDetectService
    {
        private readonly IRepositoryWrapperMariaDB _repositoryMariaWrapper;
        private readonly IServiceScopeFactory _servirceScopeFatory;

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public RegisterDetectService(IRepositoryWrapperMariaDB repositoryMariaWrapper, IConfiguration configuration, IMapper mapper, IServiceScopeFactory servirceScopeFatory)
        {
            _repositoryMariaWrapper = repositoryMariaWrapper;
            _servirceScopeFatory = servirceScopeFatory;

            _configuration = configuration;
            _mapper = mapper;
        }

        public (object resutl, string message) RegisterUserDetect(RegisterUserDetectRequest request)
        {
            #region Validations
            RegisterUserDetectValidation validationRules = new RegisterUserDetectValidation(_repositoryMariaWrapper);
            var resultValidationRules = validationRules.Validate(request);
            if (resultValidationRules.Errors.Count > 0)
            {
                return (null, resultValidationRules.Errors.FirstOrDefault().ErrorMessage);
            }
            #endregion  End Validations

            #region Register service fatory
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB repositoryMariaWrapper = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
            #endregion

            #region Handel
            List<TicketTypeDevice> ticketDevices = repositoryMariaWrapper.TicketTypeDevices.FindByCondition(x => x.TicketTypeId == request.TicketTypeId).ToList();
            if (ticketDevices is null) return (null, "TicketTypeNotFound");
           
            using var transaction = repositoryMariaWrapper.BeginTransaction();
            try
            {

                RegisterDetect registerDetect = repositoryMariaWrapper.RegisterDetects.FindByCondition(x =>
                                                    x.TicketTypeId.Equals(request.TicketTypeId) && x.ModeId.Equals(request.ModeId)
                                                    && x.RgDectectKey.Equals(request.RgDectectKey)
                                                    && x.TagCode.Equals(request.TagCode)
                                                    && x.ModeId.Equals(request.ModeId)
                                                    ).FirstOrDefault();

                if(registerDetect is null)
                {
                     RegisterDetect newRegisterDetect = new RegisterDetect
                     {
                         TicketTypeId = request.TicketTypeId,
                         ModeId = request.ModeId,
                         RgDectectKey = request.RgDectectKey,
                         RgDectectExtension = request.RgDectectExtension,
                         RgDectectUserId = request.RgDectectUserId,
                         TagCode = request.TagCode
                     };
                    repositoryMariaWrapper.RegisterDetects.Add(newRegisterDetect);
                    repositoryMariaWrapper.SaveChanges();
                    registerDetect = newRegisterDetect;
                }

               

                foreach (var item in request.RegisterDettectDetailRequests)
                {
                    // handel detail
                    List<RegisterDetectDetail> newRegisterDetectDetails = new List<RegisterDetectDetail>();
                    foreach (var registerDettectDetail in request.RegisterDettectDetailRequests)
                    {
                        RegisterDetectDetail newRegisterDetectDetail = new RegisterDetectDetail
                        {
                            RegisterDetectId = registerDetect.RegisterDetectId,
                            RgDectectDetailTimeBegin = registerDettectDetail.RgDectectDetailTimeBegin.ParseStringToTimeSpan(),
                            RgDectectDetailTimeEnd = registerDettectDetail.RgDectectDetailTimeEnd.ParseStringToTimeSpan(),

                            RgDectectDetailRepeat = registerDettectDetail.RgDectectDetailRepeat,
                            RgDectectDetailRepeatValue = JsonConvert.SerializeObject(registerDettectDetail.RgDectectDetailRepeatValueData),

                            RgDectectDetailDateBegin = registerDettectDetail.RgDectectDetailDateBegin.ParseStringGMTToDatime(),
                            RgDectectDetailDateEnd = registerDettectDetail.RgDectectDetailDateEnd.ParseStringGMTToDatime()
                        };
                        newRegisterDetectDetails.Add(newRegisterDetectDetail);
                    }
                    repositoryMariaWrapper.RegisterDetectDetails.AddRange(newRegisterDetectDetails);
                    repositoryMariaWrapper.SaveChanges();

                }
                transaction.Commit();
                return (registerDetect, "RegisterUserDetectSuccess");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, $"Error:Exception:{ex.StackTrace} + {ex.InnerException} + {ex.Message}");
            }
            #endregion
        }

       
        public (object resutl, string message) RegisterUserDetectMutil(RegisterUserDetectMutilRequest request)
        {
            #region Validations
            #endregion

            #region Register service fatory
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB repositoryMariaWrapper = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
            #endregion


            using var transaction = _repositoryMariaWrapper.BeginTransaction();

            try
            {
                foreach (var item in request.RegisterUserDetects)
                {
                    List<TicketTypeDevice> ticketDevices = repositoryMariaWrapper.TicketTypeDevices.FindByCondition(x => x.TicketTypeId == item.TicketTypeId).ToList();
                    if (ticketDevices is null) return (null, "TicketTypeNotFound");


                    RegisterDetect registerDetect = repositoryMariaWrapper.RegisterDetects.FindByCondition(x =>
                                                    x.TicketTypeId.Equals(item.TicketTypeId) && x.ModeId.Equals(item.ModeId)
                                                    && x.RgDectectKey.Equals(item.RgDectectKey)
                                                    && x.TagCode.Equals(item.TagCode)
                                                    && x.ModeId.Equals(item.ModeId)
                                                    ).FirstOrDefault();

                    if (registerDetect is null)
                    {
                        RegisterDetect newRegisterDetect = new RegisterDetect
                        {
                            TicketTypeId = item.TicketTypeId,
                            ModeId = item.ModeId,
                            RgDectectKey = item.RgDectectKey,
                            RgDectectExtension = item.RgDectectExtension,
                            RgDectectUserId = item.RgDectectUserId,
                            TagCode = item.TagCode
                        };
                        repositoryMariaWrapper.RegisterDetects.Add(newRegisterDetect);
                        repositoryMariaWrapper.SaveChanges();
                        registerDetect = newRegisterDetect;
                    }

                    

                    // handel detail
                    List<RegisterDetectDetail> newRegisterDetectDetails = new List<RegisterDetectDetail>();
                    foreach (var registerDettectDetail in item.RegisterDettectDetailRequests)
                    {
                        RegisterDetectDetail newRegisterDetectDetail = new RegisterDetectDetail
                        {
                            RegisterDetectId = registerDetect.RegisterDetectId,
                            RgDectectDetailTimeBegin = registerDettectDetail.RgDectectDetailTimeBegin.ParseStringToTimeSpan(),
                            RgDectectDetailTimeEnd = registerDettectDetail.RgDectectDetailTimeEnd.ParseStringToTimeSpan(),

                            RgDectectDetailRepeat = registerDettectDetail.RgDectectDetailRepeat,
                            RgDectectDetailRepeatValue = JsonConvert.SerializeObject(registerDettectDetail.RgDectectDetailRepeatValueData),

                            RgDectectDetailDateBegin = registerDettectDetail.RgDectectDetailDateBegin.ParseStringGMTToDatime(),
                            RgDectectDetailDateEnd = registerDettectDetail.RgDectectDetailDateEnd.ParseStringGMTToDatime()
                        };
                        newRegisterDetectDetails.Add(newRegisterDetectDetail);
                    }
                    repositoryMariaWrapper.RegisterDetectDetails.AddRange(newRegisterDetectDetails);
                    repositoryMariaWrapper.SaveChanges();
                }
                transaction.Commit();
                return (request, "RegisterUserDetectSuccess");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, $"Error:Exception:{ex.StackTrace} + {ex.InnerException} + {ex.Message}");
            }
        }


        public (bool resutl, string message) UnRegisterUserDetect(UnRegisterUserDetectRequest request)
        {
            #region Validations
            UnRegisterUserDetectValidation validationRules = new UnRegisterUserDetectValidation();
            var resultValidationRules = validationRules.Validate(request);
            if (resultValidationRules.Errors.Count > 0)
            {
                return (false, resultValidationRules.Errors.FirstOrDefault().ErrorMessage);
            }
            #endregion End Validations

            #region Register service fatory
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB repositoryMariaWrapper = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
            #endregion


            try
            {
                RegisterDetect registerDetect = repositoryMariaWrapper.RegisterDetects.FindByCondition(x =>
                                                x.RgDectectKey.Equals(request.RgDectectKey)
                                                && x.RgDectectUserId.Equals(request.RgDectectUserId)
                                                && x.TagCode.Equals(request.TagCode)
                                                && x.ModeId.Equals(request.ModeId)
                                                && x.TicketTypeId.Equals(request.TicketTypeId)
                                                ).FirstOrDefault();
                if(registerDetect is null) return (false,"RegisterDetectNotFound");




                //prepare data
                var dateBegin = request.RgDectectDetailDateBegin.ParseStringGMTToDatime();
                var dateEnd = request.RgDectectDetailDateEnd.ParseStringGMTToDatime();
                var timeBegin = request.RgDectectDetailTimeBegin.ParseStringToTimeSpan();
                var timeEnd = request.RgDectectDetailTimeEnd.ParseStringToTimeSpan();

                List<RegisterDetectDetail> registerDetectDetails = repositoryMariaWrapper.RegisterDetectDetails.FindByCondition(x =>
                                                                    x.RegisterDetectId.Equals(registerDetect.RegisterDetectId)
                                                                    && x.RgDectectDetailDateBegin.Equals(dateBegin)
                                                                    && x.RgDectectDetailDateEnd.Equals(dateEnd)
                                                                    && x.RgDectectDetailTimeBegin.Equals(timeBegin)
                                                                    && x.RgDectectDetailTimeBegin.Equals(timeEnd)
                                                                    ).ToList();
                repositoryMariaWrapper.RegisterDetectDetails.RemoveRange(registerDetectDetails);
                repositoryMariaWrapper.SaveChanges();

                // Triger hỗ trợ nếu deatil.count = 0 thì thằng cha tự động xóa đi.
                return (true, "UnRegisterSuccess");
            }
            catch (Exception ex)
            {
                return (false, $"Erro:Exception:{ex.Message}");
            }
        }

        public (bool result, string message) UnRegisterUserMutil(UnRegisterUserDetectMutilRequest request)
        {
            #region Validations
            UnRegisterUserDetectMutilValidation validationRules = new UnRegisterUserDetectMutilValidation();
            var resultValidationRules = validationRules.Validate(request);
            if (resultValidationRules.Errors.Count > 0)
            {
                return (false,resultValidationRules.Errors.FirstOrDefault().ErrorMessage);
            }
            #endregion


            #region Register service fatory
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB repositoryMariaWrapper = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
            #endregion


            using var transaction = repositoryMariaWrapper.BeginTransaction();
            try
            {
                var prepareRequest = request.UnRegisterUserDetects.GroupBy(x => new { ModeId = x.ModeId, RgDectectKey = x.RgDectectKey, TagCode = x.TagCode, TicketTypeId = x.TicketTypeId }).Select(x => new
                {
                    ModeId = x.Key.ModeId,
                    RgDectectKey = x.Key.RgDectectKey,
                    TagCode = x.Key.TagCode,
                    TicketTypeId = x.Key.TicketTypeId,
                    Items = x.ToList()
                }).ToList();

                foreach (var item in prepareRequest)
                {
                    RegisterDetect tmpRegisterDetect = repositoryMariaWrapper.RegisterDetects.FindByCondition(x =>
                                                     x.RgDectectKey.Equals(item.RgDectectKey)
                                                     && x.TagCode.Equals(item.TagCode)
                                                     && x.ModeId.Equals(item.ModeId)
                                                     && x.TicketTypeId.Equals(item.TicketTypeId)
                                                     ).FirstOrDefault();
                    if (tmpRegisterDetect is null)
                    {
                        transaction.Rollback();
                        return (result: false, message: "RegisterDetectNotFound");
                    }

                    foreach (var detail in item.Items)
                    {
                        //prepare data
                        var dateBegin = detail.RgDectectDetailDateBegin.ParseStringGMTToDatime();
                        var dateEnd = detail.RgDectectDetailDateEnd.ParseStringGMTToDatime();
                        var timeBegin = detail.RgDectectDetailTimeBegin.ParseStringToTimeSpan();
                        var timeEnd = detail.RgDectectDetailTimeEnd.ParseStringToTimeSpan();
                        List<RegisterDetectDetail> registerDetectDetails = _repositoryMariaWrapper.RegisterDetectDetails.FindByCondition(x =>
                                                                    x.RegisterDetectId == tmpRegisterDetect.RegisterDetectId
                                                                    && x.RgDectectDetailDateBegin.Equals(dateBegin)
                                                                    && x.RgDectectDetailDateEnd.Equals(dateEnd)
                                                                    && x.RgDectectDetailTimeBegin.Equals(timeBegin)
                                                                    && x.RgDectectDetailTimeEnd.Equals(timeEnd)
                                                                    ).ToList();

                        repositoryMariaWrapper.RegisterDetectDetails.RemoveRange(registerDetectDetails);
                        repositoryMariaWrapper.SaveChanges();
                    }

                    // Triger hỗ trợ nếu deatil.count = 0 thì thằng cha tự động xóa đi.
                }
                transaction.Commit();
                return (result: true, message: "UnRegisterSuccess");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (false, $"Erro:Exception:{ex.Message}");
            }
        }
    
        public (bool result , string message) UnRegisterUserWithUserId(string userId)
        {

            #region Register service fatory
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB repositoryMariaWrapper = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();
            #endregion

            List<RegisterDetect> registerDetects = repositoryMariaWrapper.RegisterDetects.FindByCondition(x => x.RgDectectUserId.Equals(userId)).ToList();
            if(registerDetects.Count > 0 )
            {
                List<RegisterDetectDetail> registerDetectDetails = repositoryMariaWrapper.RegisterDetectDetails.FindByCondition(x => registerDetects.Select(y=> y.RegisterDetectId).Contains(x.RegisterDetectId)).ToList();
                repositoryMariaWrapper.RegisterDetectDetails.RemoveRange(registerDetectDetails);
                repositoryMariaWrapper.SaveChanges();
            }
            return (true, "Success");
        }
    
    }
}
