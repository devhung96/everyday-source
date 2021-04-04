using AutoMapper;
using Project.Modules.Events.Entities;
using Project.Modules.Events.Requests;
using Project.Modules.Events.Validations;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Requests;
using Project.Modules.Sessions.Entities;
using Project.Modules.Sessions.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public class MappingProfile: AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrganizeRequest,Organize>();
            CreateMap<UpdateOrganizeRequest,Organize>();
            CreateMap<CreateEventRequest, Event>();
            CreateMap<UpdateEventRequest, Event>();
            CreateMap<CreateSessionRequest, Session>();
            CreateMap<UpdateSessionRequest, Session>();
            CreateMap<UpdateSessionRequest,User>();
            CreateMap<CreateUserAdminRequest, UserSuper>();

        }
    }
}
