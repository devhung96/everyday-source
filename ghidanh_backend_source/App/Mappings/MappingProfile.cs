using AutoMapper;
using Project.Modules.Classes.Entities;
using Project.Modules.Classes.Requests;
using Project.Modules.Lecturers;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Slots.Entities;
using Project.Modules.Slots.Requests;
using Project.Modules.Students.Entities;
using Project.Modules.SubjectGroups.Entities;
using Project.Modules.SubjectGroups.Requests;
using Project.Modules.Subjects.Entities;
using Project.Modules.Subjects.Requests;
using Project.Modules.Students.Requests;
using Project.Modules.Courses.Requests;
using Project.Modules.Courses.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Receipts.Entities;
using Project.Modules.Receipts.Requests;

namespace Project.App.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {            
            CreateMap<StoreSlotRequest, Slot>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StoreClassRequest, Class>();
            CreateMap<CollectedTuitionRequest, ReceiptRequest>();
            CreateMap<ReceiptRequest, Receipt>();
            CreateMap<AddStudent, Student>();
            CreateMap<AddUserRequest, User>();
            CreateMap<UpdateStudentRequest, Student>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AddLecturerRequest, Lecturer>();
            CreateMap<UpdateLecturerRequest, Lecturer>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StoreSubjectGroupRequest, SubjectGroup>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<StoreSubjectRequest, Subject>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateClassRequest, Class>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<StoreCourseRequest, Course>().ForAllMembers(x => x.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
