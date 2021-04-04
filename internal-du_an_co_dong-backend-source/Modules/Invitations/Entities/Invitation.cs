using Project.Modules.Events.Entities;
using Project.Modules.Users.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Invitations.Entities
{
    [Table("shareholder_invitation")]
    public class Invitation
    {
        [Key]
        [Column("invitation_id")]
        public string InvitationId { get; set; } = Guid.NewGuid().ToString();
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("template_id")]
        public string TemplateId { get; set; }
        [Column("invitation_status")]
        public INVITATION_STATUS InvitationStatus { get; set; } = INVITATION_STATUS.NOT_SEND;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public User User { get; set; }
        public Template Template { get; set; }
        public Event Event { get; set; }
    }

    public enum INVITATION_STATUS
    {
        NOT_SEND = 0,
        SENDING = 1,
        SENT = 2
    }
}
