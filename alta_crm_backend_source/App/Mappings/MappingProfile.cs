using AutoMapper;
using Project.Modules.Schedules.Requests;
using Project.Modules.Tags.Enities;
using Project.Modules.Tags.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.UserTagModes.Entities;
using System.Collections.Generic;

namespace Project.App.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserStoredRequest, User>();
            CreateMap<CreateUserRequest, User>();
            CreateMap<UserEditRequest, User>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UserTagMode,TagMode>();
            CreateMap<UpdateUserRequest, User>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            #region Ticket Type
            CreateMap<AddTicketRequest, TicketType>();
            CreateMap<UpdateTicketRequet, TicketType>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<BaseTicketTypeRequest, TicketType>();
            #endregion


            #region Tag
            CreateMap<InsertTagRequest, Tag>();
            CreateMap<UpdateTagRequest, Tag>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            #endregion




        }
    }
}
