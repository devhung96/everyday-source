using Project.Modules.Groups.Entities;

namespace Project.Modules.Medias.Response
{
    public class MediaGroupResponse
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
        public bool IsAssign { get; set; }
        public MediaGroupResponse(Group group, bool isAssign)
        {
            GroupId = group.GroupId;
            GroupName = group.GroupName;
            GroupCode = group.GroupCode;
            IsAssign = isAssign;
        }
    }
}
