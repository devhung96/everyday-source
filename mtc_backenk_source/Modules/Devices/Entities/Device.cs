using Newtonsoft.Json;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Entities
{
    [Table("mtc_device_tbl")]
    public class Device
    {
        [Key]
        [Column("device_id")]
        public string DeviceId { get; set; } = Guid.NewGuid().ToString();
        [Column("device_name")]
        public string DeviceName { get; set; }
        [Column("device_photo")]
        public string DevicePhoto { get; set; }
        [Column("device_login_name")]
        public string LoginName { get; set; }
        [Column("device_sku")]
        public string DeviceSku { get; set; }
        [Column("device_pass")]
        [JsonIgnore]
        [DataType(DataType.Password)]
        public string DevicePass { get; set; }
        [Column("device_saft")]
        [JsonIgnore]
        public string DeviceSaft { get; set; }
        [Column("device_status")]
        [EnumDataType(typeof(DEVICESTATUS))]
        public DEVICESTATUS DeviceStatus { get; set; }
        [EnumDataType(typeof(ENUMDEVICELOCK))]
        [Column("device_power")]
        public ENUMDEVICEPOWER DevicePower { get; set; }
        [Column("device_lock")]
        [EnumDataType(typeof(ENUMDEVICELOCK))]
        public ENUMDEVICELOCK DeviceLock { get; set; }
        [Column("device_comment")]
        public string DeviceComment { get; set; }
        [Column("color_code")]
        public string ColorCode { get; set; }
        [Column("device_memory")]
        public float? DeviceMemory { get; set; } = 0;
        [Column("device_user_memory")]
        public float? DeviceUserMemory { get; set; } = 0;
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("device_type_id")]
        public string DeviceTypeId { get; set; }
        [Column("device_mac_address")]
        public string DeviceMACAddress { get; set; }
        [Column("device_setting")]
        public string DeviceSetting { get; set; }
        [Column("device_expired")]
        public DateTime DeviceExpired { get; set; }
        [Column("device_warranty_expires_date")]
        public DateTime DeviceWarrantyExpiresDate { get; set; }
        [Column("device_location")]
        public string DeviceLocation { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public User User { get; set; }
        public Group Group { get; set; }
        [JsonIgnore]
        public List<DeviceTokens> DeviceTokens { get; set; }
        public DeviceType DeviceType { get; set; }

    }

    public class DeviceResponse
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string LoginName { get; set; }
        public string DevicePhoto { get; set; }
        public string DeviceSku { get; set; }
        public DEVICESTATUS DeviceStatus { get; set; }
        public ENUMDEVICERESTART DeviceRestart { get; set; }
        public ENUMDEVICEPOWER DevicePower { get; set; }
        public ENUMDEVICELOCK DeviceLock { get; set; }
        public string DeviceComment { get; set; }
        public string ColorCode { get; set; }
        public object DeviceSetting { get; set; }
        public float? DeviceMemory { get; set; }
        public string UserId { get; set; }
        public string DeviceTypeId { get; set; }
        public string DeviceMACAddress { get; set; }
        public DateTime DeviceExpiredDateTime { get; set; }
        public int? DeviceExpired { get; set; }
        public DateTime DeviceWarrantyExpiresDate { get; set; }
        public int? DeviceWarrantyExpires { get; set; }
        public string DeviceLocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public List<DeviceTokens> DeviceTokens { get; set; }
        public DeviceType DeviceType { get; set; }
        public string GroupUserName { get; set; }
        public string GroupId { get; set; }
        public DeviceResponse(Device device, string url = null, Group group = null)
        {
            DeviceId = device.DeviceId;
            DeviceName = device.DeviceName;
            LoginName = device.LoginName;
            DeviceStatus = device.DeviceStatus;
            DeviceComment = device.DeviceComment;
            ColorCode = device.ColorCode;
            DeviceMemory = device.DeviceMemory;
            UserId = device.DeviceId;
            DeviceTypeId = device.DeviceTypeId;
            DeviceMACAddress = device.DeviceMACAddress;
            DeviceExpiredDateTime = device.DeviceExpired;
            DeviceExpired = device.DeviceExpired < DateTime.Now ? 1 : 0; // 1 là đã hết hạng , 0 là chưa hết hạng
            DeviceWarrantyExpiresDate = device.DeviceWarrantyExpiresDate;
            DeviceWarrantyExpires = device.DeviceWarrantyExpiresDate < DateTime.Now ? 1 : 0; // 1 là đã hết hạng bảo hành , 0 là chưa hết hạng
            DeviceLocation = device.DeviceLocation;
            CreatedAt = device.CreatedAt;
            User = device.User;
            DeviceSku = device.DeviceSku;
            if (url != null)
            {
                DevicePhoto = device.DevicePhoto != null ? GeneralHelper.UrlCombine(url, device.DevicePhoto) : device.DevicePhoto;
            }
            else
            {
                DevicePhoto = null;
            }
            DeviceTokens = device.DeviceTokens;
            DeviceType = device.DeviceType;
            DeviceLock = device.DeviceLock;
            DevicePower = device.DevicePower;
            DeviceSetting = device.DeviceSetting == null ? null : JsonConvert.DeserializeObject(device.DeviceSetting);
            GroupUserName = group?.GroupName;
            GroupId = device.GroupId;
        }
    }
    public enum DEVICESTATUS
    {
        ACTIVED = 1,
        DEACTIVATED = 2,
        DELETE = 3
    }
    public enum ENUMDEVICELOCK
    {
        LOCK = 1,
        UNLOCK = 0
    }
    public enum ENUMDEVICEPOWER
    {
        ON = 1,
        OFF = 0
    }
    public enum ENUMDEVICERESTART
    {
        ON = 1,
        OFF = 0
    }
}
