using Project.Modules.Classes.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Slots.Entities;
using Project.Modules.Subjects.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.ClassSchedules.Entities
{
    public class ClassScheduleOrigin
    {
        [Column("class_schedule_id")]
        [Key]
        public string ClassScheduleId { get; set; } = Guid.NewGuid().ToString();
        [Column("lecturer_id")]
        public string LecturerId { get; set; }
        [Column("subject_id")]
        public string SubjectId { get; set; }
        [Column("class_id")]
        public string ClassId { get; set; }
        [Column("schedule_type")]
        public SCHEDULE_TYPE ScheduleType { get; set; }
        [Column("class_room")]
        public string ClassRoom { get; set; }
        [Column("day_of_week")]
        public string DayOfWeek { get; set; }
        [Column("date_start")]
        public DateTime DateStart { get; set; }
        [Column("date_end")]
        public DateTime DateEnd { get; set; }
        [Column("repeat_lesson")]
        public string StepRepeat { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public Class Class { get; set; }
        public Subject Subject { get; set; }
        public Lecturer Lecturer { get; set; }
    }

    [Table("rc_class_schedules")]
    public class ClassSchedule : ClassScheduleOrigin
    {
        [Column("time_start")]
        public long TimeStart { get; set; }
        [Column("time_end")]
        public long TimeEnd { get; set; }
    }

    public class ClassScheduleResponse : ClassScheduleOrigin
    {
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Error { get; set; }
        public ClassScheduleResponse(ClassSchedule classSchedule, string message = null)
        {
            ClassScheduleId = classSchedule.ClassScheduleId;
            LecturerId = classSchedule.LecturerId;
            SubjectId = classSchedule.SubjectId;
            ClassId = classSchedule.ClassId;
            ScheduleType = classSchedule.ScheduleType;
            ClassRoom = classSchedule.ClassRoom;
            DateStart = classSchedule.DateStart;
            DateEnd = classSchedule.DateEnd;
            CreatedAt = classSchedule.CreatedAt;
            StepRepeat = classSchedule.StepRepeat;
            Class = classSchedule.Class;
            Subject = classSchedule.Subject;
            Lecturer = classSchedule.Lecturer;
            DayOfWeek = classSchedule.DayOfWeek;
            Error = message;

            TimeStart = new TimeSpan(classSchedule.TimeStart).ToString(@"hh\:mm\:ss");
            TimeEnd = new TimeSpan(classSchedule.TimeEnd).ToString(@"hh\:mm\:ss");
        }
    }

    public class RepeatLesson
    {
        public TYPE_REPEAT TypeRepeat { get; set; }
        public string ValueRepeat { get; set; }
    }
        

    public enum SCHEDULE_TYPE
    {
        ONLINE = 1,
        OFFLINE = 0
    }

    public enum TYPE_REPEAT
    {
        //DAILY = 1,
        WEEKLY = 1,
        //MOTHLY = 3
    }
}
