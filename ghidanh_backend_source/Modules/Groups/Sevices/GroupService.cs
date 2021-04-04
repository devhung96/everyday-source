using Project.App.DesignPatterns.Repositories;
using Project.Modules.Accounts.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Sevices
{
    public interface IGroupService
    {
        List<Group> ShowAll();
        List<Module> GetAllModule();
        List<Permission> GetAllPermissions();
        (List<GroupPermission> groupPermissions, string message) AddPermissionGroup(AddPermissionGroup request);
        (Group group, string message) Detail(string groupId);
        (object data, string message) Update(GroupRequest request, string groupId);
    }
    public class GroupService : IGroupService
    {
        private readonly IRepositoryMariaWrapper repository;
        public GroupService(IRepositoryMariaWrapper repositoryMaria)
        {
            repository = repositoryMaria;
        }

        public (List<GroupPermission> groupPermissions, string message) AddPermissionGroup(AddPermissionGroup request)
        {
            Group group = repository.Groups.GetById(request.GroupId);
            if(group is null)
            {
                return (null, "GroupNotExist");
            }
            List<GroupPermission> permissionGroups = new List<GroupPermission>();
            foreach(  string permissionCode in request.PermissionCodes)
            {
                permissionGroups.Add(new GroupPermission() { GroupId = group.GroupId, PermissionCode = permissionCode }); 
            }
            repository.GroupPermissions.AddRange(permissionGroups);
            repository.SaveChanges();
            return (permissionGroups, "Success");
        }

        public (Group group, string message) Detail(string groupId)
        {
            Group group = repository.Groups.GetById(groupId);
            if (group is null)
            {
                return (null, "GroupNotExist");
            }
            List<string> permissionCodes = repository.GroupPermissions.FindByCondition(m => m.GroupId.Equals(group.GroupId)).Select(m => m.PermissionCode).ToList();
            group.Permissions = repository.Permissions.FindByCondition(m => permissionCodes.Contains(m.PermissionCode)).ToList();
            return (group, "Success");
        }

        public List<Permission> GetAllPermissions()
        {
            return repository.Permissions.FindAll().ToList();
        }
        public List<Module> GetAllModule()
        {
            List<Module> modules = repository.Modules.FindAll().ToList();
            foreach (Module module in modules)
            {
                module.Permissions = repository.Permissions.FindByCondition(m => m.ModuleCode.Equals(module.ModuleCode)).ToList();
            }
            return modules;
        }

        public List<Group> ShowAll()
        {
            List<Group> groups = repository.Groups.FindByCondition(m=>! m.GroupId.Equals(PERMISSION_DEFAULT.Lecturer)&&! m.GroupId.Equals(PERMISSION_DEFAULT.Student)).ToList();
            foreach (Group group in groups)
            {
               group.PermissionCodes = repository.GroupPermissions.FindByCondition(m => m.GroupId.Equals(group.GroupId)).Select(m => m.PermissionCode).ToList();
            }    
            return groups;
        }

        public (object data, string message) Update(GroupRequest request, string groupId)
        {
            Group group = repository.Groups.GetById(groupId);
            if (group is null)
            {
                return (group, "GroupNotExist");
            }
            using var transaction = repository.BeginTransaction();
            try
            {
                repository.Groups.UpdateMaria(group);
                List<string> permissionDb = repository.GroupPermissions
                                                      .FindByCondition(m => m.GroupId.Equals(groupId))
                                                      .Select(m => m.PermissionCode)
                                                      .ToList();
                List<string> accountIds = repository.Users.FindByCondition(m => m.GroupId.Equals(groupId)).Select(m => m.AccountId).ToList();

                List<string> removePermission = permissionDb.Except(permissionDb.Intersect(request.PermissionCodes)).ToList();

                List<AccountPermission> removePermissionAccount = repository.AccountPermissions.FindByCondition(m => accountIds.Contains(m.AccountId) && removePermission.Contains(m.PermissionCode)).ToList();

                List<GroupPermission> groupPermissions = repository.GroupPermissions
                                                                   .FindByCondition(m => m.GroupId.Equals(groupId) && removePermission.Contains(m.PermissionCode))
                                                                   .ToList();
                repository.AccountPermissions.RemoveRangeMaria(removePermissionAccount);
                repository.GroupPermissions.RemoveRangeMaria(groupPermissions);
                repository.SaveChanges();
                List<string> Add = request.PermissionCodes.Except(permissionDb.Intersect(request.PermissionCodes)).ToList();
                List<AccountPermission> accountPermissionStores = new List<AccountPermission>();
                foreach (string permissionCode in Add)
                {
                    foreach (string accountId in accountIds)
                    {
                        accountPermissionStores.Add(new AccountPermission(accountId, permissionCode));
                    }
                }

                List<GroupPermission> groupPermissionsAdd = new List<GroupPermission>();
                foreach (string permissionCode in Add)
                {
                    groupPermissionsAdd.Add(new GroupPermission() { GroupId = groupId, PermissionCode = permissionCode });
                }
                repository.GroupPermissions.AddRange(groupPermissionsAdd);
                repository.AccountPermissions.AddRange(accountPermissionStores);
                repository.SaveChanges();
                transaction.Commit();
                return (group, "Success");
            }
            catch
            {
                transaction.Rollback();
                return (null, "UpdateGroupFaild");
            }
           
        }
    }
}
