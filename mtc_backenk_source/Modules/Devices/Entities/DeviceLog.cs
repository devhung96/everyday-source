using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Entities
{
    public class DeviceLog
    {
        [BsonId]
        [BsonElement("device_log_id")]
        public string DeviceLogId { get; set; } = Guid.NewGuid().ToString();
        [BsonElement("device_log_value")]
        public string DeviceLogValue { get; set; }
        [BsonElement("device_id")]
        public string DeviceId { get; set; }
        [BsonElement("device_log_type")]
        public int DeviceLogType { get; set; }
        [BsonElement("userId")]
        public string UserId { get; set; }
        [BsonElement("device_log_created_at")]
        public DateTime DeviceLogCreatedAt { get; set; }
        public User User { get; set; }
    }
    public class ResponseDeviceLog
    {
        public string DeviceLogId { get; set; } 
        public JArray DeviceLogValue { get; set; }
        public int DeviceLogType { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public DateTime DeviceLogCreatedAt { get; set; }
        public ResponseDeviceLog(DeviceLog deviceLog, User user, Device device)
        {
            DeviceLogId = deviceLog.DeviceLogId;
            DeviceLogValue = JArray.Parse(deviceLog.DeviceLogValue);
            DeviceLogType = deviceLog.DeviceLogType;
            UserId = deviceLog.UserId;
            DeviceLogCreatedAt = deviceLog.DeviceLogCreatedAt;
            UserName = user?.UserName;
            DeviceId = deviceLog.DeviceId;
            DeviceName = device?.DeviceName;
            UserAvatar = user?.UserImage;
        }
    }
    public enum ENUMDEVICELOGTYPE
    {
        UPDATE = 0,
        DELETE = 1,
        CREATE = 2
    }
}
