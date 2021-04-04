using AutoMapper;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.DeviceTypes.Requests;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.PlayLists.Entities;
using Project.Modules.PlayLists.Requests;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Requests;
using Project.Modules.Settings.Entitites;
using Project.Modules.Settings.Requests;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.TemplateDetails.Requests;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Modules.Medias.Entities;

namespace Project.App.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region Luyen
            CreateMap<StoreUserRequest, User>();
            CreateMap<UpdateUserRequest, User>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreGroupRequest, Group>();
            CreateMap<UpdateGroupRequest, Group>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreatePermissionRequest, Permission>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
         
            #endregion

            CreateMap<UpdateSettingRequest, Setting>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateDeviceTypeRequest, DeviceType>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateDevice, Device>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreDevice, Device>();
            CreateMap<Schedule, Schedule>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreScheduleRequest, Schedule>();
            CreateMap<StoreScheduleNonDeviceRequest, Schedule>();
            CreateMap<UpdateScheduleRequest, Schedule>();
            CreateMap<UpdateOldScheduleRequest, Schedule>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateDeviceTypeRequest, DeviceType>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreDevice, Device>();
            CreateMap<UpdateTemplate, Template>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateTemplatedetail, TemplateDetail>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateTemplateDetailVer2, TemplateDetail>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Template, Template>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateMediaTypeRequest, MediaType>();
            CreateMap<StoreUserRequest, User>();
            CreateMap<StorePlayListRequest, PlayList>();
            CreateMap<StorePlayListDetailMultipleRequest, PlayListDetail>();
            CreateMap<StorePlayListDetailRequest, PlayListDetail>();
            CreateMap<UpdatePlayListRequest, PlayList>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<UpdateSettingRequest, Setting>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Schedule, Schedule>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreScheduleRequest, Schedule>();
            CreateMap<UpdateScheduleRequest, Schedule>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateMediaTypeRequest, MediaType>();
            CreateMap<UpdateSettingRequest, Setting>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

        
            CreateMap<Schedule, Schedule>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreScheduleRequest, Schedule>();
            CreateMap<UpdateScheduleRequest, Schedule>();
            CreateMap<UpdateOldScheduleRequest, Schedule>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateDeviceTypeRequest, DeviceType>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateMediaRequest, Media>();


            CreateMap<UpdatePlayListDetailRequest, PlayListDetail>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateUserRequest, User>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateGroupRequest, Group>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreUserRequest, User>();
            CreateMap<UpdatePlayListDetailRequest, PlayListDetail>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateUserRequest, User>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
        
            CreateMap<UpdateDeviceTypeRequest, DeviceType>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
