using AutoMapper;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.Logs.Entities;
using Project.Modules.Logs.Requests;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.RegisterDetects.Requests;
using Project.Modules.Tags.Entities;
using Project.Modules.Tags.Requests;
using Project.Modules.Tickets.Entities;
using Project.Modules.TicketTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<AddDeviceRequest,Device>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateDeviceRequest,Device>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddTagRequest,Tag>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddTagRequest,UpdateTagRequest>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateTagRequest, Tag>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateDeviceRequest,Tag>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateTicketTypeRequest, TicketType>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddTicketTypeRequest, PublishTicketTypeRequest>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CrmAddTag, AddTagRequest>();
            CreateMap<RegisterUserDetectRequest, RegisterDetect>();        
            CreateMap<AppLogRequest, AppLog>();
        }
        
    }
}
