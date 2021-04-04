using AutoMapper;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Settings.Entitites;
using Project.Modules.Settings.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Settings.Services
{

    public interface ISettingService
    {
        (Setting data, string message) Store(Setting setting);
        (Setting data, string message) Update(UpdateSettingRequest setting, string key);
        (Setting, string) Delete(Setting setting);
        (Setting setting, string message) ShowByWithKey(string key);
        (PaginationResponse<SettingReponse>, string) ShowAll(PaginationRequest paginationRequest);

    }
    public class SettingService : ISettingService
    {
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;
        private readonly IMapper mapper;
        public SettingService(IRepositoryWrapperMariaDB _repositoryWrapperMariaDB, IMapper _mapper)
        {
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
            mapper = _mapper;
        }

        public (Setting data, string message) Store(Setting setting)
        {
            Setting checkSetting = repositoryWrapperMariaDB.Settings.FindByCondition(x => x.SettingKey == setting.SettingKey).FirstOrDefault();
            if(checkSetting != null)
            {
                return (null, "SettingKeyExists");
            }
            setting.CreatedAt = DateTime.UtcNow.AddHours(7);
            repositoryWrapperMariaDB.Settings.Add(setting);
            repositoryWrapperMariaDB.SaveChanges();
            return (setting, "Created setting success!");
        }

        public (Setting data, string message) Update(UpdateSettingRequest newSetting, string key)
        {
            Setting checkSetting = repositoryWrapperMariaDB.Settings.FindByCondition(x => x.SettingKey == key).FirstOrDefault();
            if (checkSetting == null) return (null, $"Setting key : {key} not found.");
            string value = "";
            if(!checkSetting.SettingKey.Equals(newSetting.SettingKey))
            {
                if (repositoryWrapperMariaDB.Settings.FindByCondition(x => x.SettingKey.Equals(newSetting.SettingKey)).Any())
                {
                    return (null, "SettingKeyExists");
                }
            }
            if (checkSetting.SettingType == (int)EnumSettingType.String)
            {
                value = newSetting.SettingValue.ToString();
            }
            else
            {
                value = JsonConvert.SerializeObject(newSetting.SettingValue);
            }
            checkSetting.SettingValue = value;
            checkSetting = mapper.Map(newSetting, checkSetting);
            checkSetting.UpdatedAt = DateTime.UtcNow.AddHours(7);
            repositoryWrapperMariaDB.Settings.Update(checkSetting);
            repositoryWrapperMariaDB.SaveChanges();
            return (checkSetting, $"Updated key: {checkSetting.SettingKey} success");
        }

        public (Setting, string) Delete(Setting setting)
        {
            repositoryWrapperMariaDB.Settings.Remove(setting);
            repositoryWrapperMariaDB.SaveChanges();
            return (null, "DeleteSettingSuccess");
        }

        public (Setting setting, string message) ShowByWithKey(string key)
        {
            Setting setting = repositoryWrapperMariaDB.Settings.FindByCondition(x => x.SettingKey == key).FirstOrDefault();
            if (setting == null) return (null, "Setting key not found!");
            return (setting, "Show all success");
        }

        public (PaginationResponse<SettingReponse>, string) ShowAll(PaginationRequest paginationRequest)
        {
            IQueryable<Setting> settings = repositoryWrapperMariaDB.Settings.FindByCondition(x => string.IsNullOrWhiteSpace(paginationRequest.SearchContent) || x.SettingKey.ToLower().Contains(paginationRequest.SearchContent.ToLower()));
            PaginationHelper<Setting> settingInfo = PaginationHelper<Setting>.ToPagedList(settings, paginationRequest.PageNumber, paginationRequest.PageSize);
            PaginationResponse<SettingReponse> paginationResponse = new PaginationResponse<SettingReponse>(settingInfo.Select(x => new SettingReponse(x)), settingInfo.PageInfo);
            return (paginationResponse, "ShowAllSettingSuccess");
        }
    }
}
