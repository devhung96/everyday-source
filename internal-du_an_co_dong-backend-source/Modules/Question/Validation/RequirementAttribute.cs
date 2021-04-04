using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Services.ActivityStatistic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.FormulaParsing.Utilities;
using Project.App.Database;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public enum TypePermission
    {
        PERMISSION,
        PROVIP
    }

    public class ControllerDisplayNameAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public string Name { get; set; }
        public string Permission { get; set; }

        #region Override ActionFilterAttribute

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string name = Name;
            if (string.IsNullOrEmpty(name))
                name = filterContext.Controller.GetType().Name;

            //filterContext.Controller.ViewData["ControllerDisplayName"] = Name;
            //filterContext.HttpContext.
            base.OnActionExecuting(filterContext);
        }
        #endregion

        #region Override IAuthorizationFilter
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //RequirementAttribute requirement = new RequirementAttribute();

            //var www = HoanAnh;
            //RequirementAttribute requirement = new RequirementAttribute();
            //string aaa = requirement.Permiss;
            //var a = requirement.ReturnValue();
            string PermissionString = "CM-MIC.31";
            bool isAuthorized = CheckPermission(context, PermissionString); // :)
            //bool isAuthorized = false; // :)

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
        #endregion
        public static bool CheckPermission(AuthorizationFilterContext context, string PermissionString)
        {
            var ID = "1";
            try
            {
                ID = context.HttpContext.User.FindFirstValue("UserID");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "token khong co userid");
            }

            IConfiguration _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            var _dbContext = new MariaDBContext(optionsBuilder.Options);

            User user = _dbContext.Users.Find(ID);


            #region Lấy tất cả Role của User trong Db
            //List<string> RoleDB = new List<string>();
            //List<PermissionUser> permissionUsers = _dbContext.PermissionUsers.Where(m => m.UserId.Equals(ID)).ToList();
            //if (!(permissionUsers is null))
            //{
            //    foreach (var item in permissionUsers)
            //    {
            //        RoleDB.Add(item.PermissionCode);
            //    }
            //}

            #endregion

            #region Lấy tất cả Role thức tế trong Token
            //List<string> RoleToken = new List<string>();
            //var Roles = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            //foreach (var item in Roles)
            //{
            //    RoleToken.Add(item.Value);
            //}
            #endregion

            //if ((RoleDB.Count() != RoleToken.Count()) || (RoleDB.Except(RoleToken).Count() != 0) || (RoleToken.Except(RoleDB).Count() != 0))
            //{
            //    return false;
            //}

            //Console.WriteLine("SET CACHED AND CURRENT FLAGS");

            //PermissionString = "EV-ADM.31";
            #region Input
            PermissionString = "EV-ADM.31";

            //cat chuoi input
            List<string> alanWalker = new List<string>();
            alanWalker = PermissionString.Split('.').ToList();

            //de check sau
            if (alanWalker.Count < 2)
            {
                return false;
            }

            //check Interger ???
            Dictionary<string, int> LimitedInput = new Dictionary<string, int>();
            LimitedInput.Add(alanWalker[0], int.Parse(alanWalker[1]));
            int PermissionInput = int.Parse(alanWalker[1]);
            List<string> listPermission = FindOperator(PermissionInput);
            if (listPermission.Any(x => x.Equals(PermissionInput.ToString())))
            {
                Console.WriteLine("Ghi sai quyen o controller");
            }
            #endregion

            //token vi du

            #region Check Quyền trong token
            Dictionary<string, int> types = new Dictionary<string, int>()
            {
                        {"EV-IE", 26},
                        {"CM-MIC", 37}
            };
            //check X
            int value;
            var checkExists = types.TryGetValue("EV-IE", out value);
            if (!checkExists)
            {
                Console.WriteLine("Sai Ten x");
                return false;
            }
            #region get List Quyền trong token
            List<string> listPermissionToken = FindOperator(value);
            if (listPermissionToken.Any(x => x.Equals(PermissionInput.ToString())))
            {
                Console.WriteLine("quyen trong token khong co hoac nhap sai");
            }
            #endregion
            foreach (var item in listPermission)
            {
                var existQuyen = listPermissionToken.Any(x => x.Equals(item));
                if (existQuyen == false)
                {
                    Console.WriteLine($"thieu quyen {item}");
                    return false;
                }
            }
            #endregion

            #region Check Tập Con
            //c1
            //List<PermissionUnix> listEnums = new List<PermissionUnix>();
            #region Check Số đó có thuộc trong Enum hay không. Không tính tổng.
            //var attributes = PermissionUnix.A | PermissionUnix.D | PermissionUnix.S | PermissionUnix.U | PermissionUnix.R;
            //if ((int)(attributes & PermissionUnix.A) == text)
            //{
            //    Console.WriteLine("AAAAAAAAA");
            //    listEnums.Add(PermissionUnix.A);
            //}
            //if ((int)(attributes & PermissionUnix.D) == text)
            //{
            //    Console.WriteLine("DDDDDDDDDD");
            //    listEnums.Add(PermissionUnix.D);
            //}
            //if ((int)(attributes & PermissionUnix.R) == text)
            //{
            //    Console.WriteLine("RRRRRRRRRR");
            //    listEnums.Add(PermissionUnix.R);
            //}
            //if ((int)(attributes & PermissionUnix.S) == text)
            //{
            //    Console.WriteLine("SSSSSSSSSS");
            //    listEnums.Add(PermissionUnix.S);
            //}
            //if ((int)(attributes & PermissionUnix.U) == text)
            //{
            //    Console.WriteLine("UUUUUUUUUU");
            //    listEnums.Add(PermissionUnix.D);
            //}
            #endregion
            //or

            //foreach (PermissionUnix item in (PermissionUnix[])Enum.GetValues(typeof(PermissionUnix)))
            //{
            //    if (PermissionInput > (int)item)
            //    {
            //        listEnums.Add(item);
            //    }
            //}
            #endregion

            return true;
        }
        public static List<string> FindOperator(int PermissionInput)
        {
            //int PermissionInput = int.Parse(alanWalker[1]);
            //Console.WriteLine((PermissionUnix)PermissionInput);
            var permissionCurrent = (PermissionUnix)PermissionInput;
            List<string> listPermission = new List<string>();
            //if ((int)permissionCurrent != PermissionInput)
            //{
            //    return listPermission;
            //}
            string permission = Regex.Replace(permissionCurrent.ToString(), @"\s+", "");
            listPermission = permission.Split(',').ToList();
            return listPermission;
            //int count = 2;
            #region TEST
            //while (true)
            //{
            //    var flag = false;
            //    int i = 0;
            //    int a = 0;
            //    foreach (var item in enums)
            //    {

            //        foreach (var item2 in enums)
            //        {
            //            int b = a + 1;
            //            if (enums[b+1] is null)
            //            {

            //            }
            //            if (count == 2)
            //            {
            //                if ((int)enums[a] + (int)(enums[b + 1]) == PermissionInput)
            //                {
            //                    Console.WriteLine($"RIGHT : {enums[a]}     {enums[b + 1]}");
            //                    flag = true;
            //                }
            //            }
            //            b++;
            //        }
            //    }
            //    if (flag == false)
            //    {
            //        count++;
            //    }
            //}
            #endregion
            //return true;
        }
    }
    #region TypeFilterAttribute Test
    public class RequirementAttribute: TypeFilterAttribute
    {
        //public string Permiss { get; set; }//* = "keyboard";
        public string Name { get; set; }

        public RequirementAttribute() : base(typeof(ActionFilterAttribute))
        {
            //Arguments = new object[] { type, permission };
            this.Name = Name;

        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    string name = Name;
        //    if (string.IsNullOrEmpty(name))
        //        name = filterContext.Controller.GetType().Name;

        //    //filterContext.Controller.ViewData["ControllerDisplayName"] = Name;
        //    //filterContext.HttpContext.
        //    base.OnActionExecuting(filterContext);
        //}

        public RequirementAttribute(TypePermission type,string permission) : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { type, permission };

        }
        #region Ke thua
        //public RequirementAttribute(string Permission)
        //: base(typeof(AuthorizeActionFilter))
        //{
        //    //Arguments = new object[] { Permission };
        //}
        #endregion
    }

    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        //public AuthorizeActionFilter()
        //{
        //    HoanAnh = Permission111;
        //}
        //public string Permission { get; set; }
        //public string Value { get; set; }
        //public readonly AT aT;
        //public string AT { get; set; }
        public TypePermission Type;
        public string Permission { get; set; }
        public AuthorizeActionFilter(TypePermission type ,string _Permission)
        {
            
            Type = type;
            Permission = _Permission;
        }
        //public RequiredAttribute(string ha)
        //{
        //    HoanAnh = ha;
        //}
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //RequirementAttribute requirement = new RequirementAttribute();

            //var www = HoanAnh;
            //RequirementAttribute requirement = new RequirementAttribute();
            //string aaa = requirement.Permiss;
            //var a = requirement.ReturnValue();
            string PermissionString = "CM-MIC.31";
            bool isAuthorized = CheckPermission(context, PermissionString); // :)

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
        public static bool CheckPermission(AuthorizationFilterContext context,string PermissionString)
        {
            var ID = "1";
            try
            {
                ID = context.HttpContext.User.FindFirstValue("UserID");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "token khong co userid");
            }

            IConfiguration _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(_config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            var _dbContext = new MariaDBContext(optionsBuilder.Options);

            User user = _dbContext.Users.Find(ID);


            #region Lấy tất cả Role của User trong Db
            //List<string> RoleDB = new List<string>();
            //List<PermissionUser> permissionUsers = _dbContext.PermissionUsers.Where(m => m.UserId.Equals(ID)).ToList();
            //if (!(permissionUsers is null))
            //{
            //    foreach (var item in permissionUsers)
            //    {
            //        RoleDB.Add(item.PermissionCode);
            //    }
            //}

            #endregion

            #region Lấy tất cả Role thức tế trong Token
            //List<string> RoleToken = new List<string>();
            //var Roles = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            //foreach (var item in Roles)
            //{
            //    RoleToken.Add(item.Value);
            //}
            #endregion

            //if ((RoleDB.Count() != RoleToken.Count()) || (RoleDB.Except(RoleToken).Count() != 0) || (RoleToken.Except(RoleDB).Count() != 0))
            //{
            //    return false;
            //}
            
            //Console.WriteLine("SET CACHED AND CURRENT FLAGS");

            //PermissionString = "EV-ADM.31";
            #region Input
            PermissionString = "EV-ADM.31";

            //cat chuoi input
            List<string> alanWalker = new List<string>();
            alanWalker = PermissionString.Split('.').ToList();

            //de check sau
            if (alanWalker.Count < 2)
            {
                return false;
            }

            //check Interger ???
            Dictionary<string, int> LimitedInput = new Dictionary<string, int>();
            LimitedInput.Add(alanWalker[0], int.Parse(alanWalker[1]));
            int PermissionInput = int.Parse(alanWalker[1]);
            List<string> listPermission = FindOperator(PermissionInput);
            if (listPermission.Any(x => x.Equals(PermissionInput.ToString())))
            {
                Console.WriteLine("Ghi sai quyen o controller");
            }
            #endregion

            //token vi du

            #region Check Quyền trong token
            Dictionary<string, int> types = new Dictionary<string, int>()
            {
                        {"EV-IE", 26},
                        {"CM-MIC", 37}
            };
            //check X
            int value;
            var checkExists = types.TryGetValue("EV-IE", out value);
            if (!checkExists)
            {
                Console.WriteLine("Sai Ten x");
                return false;
            }
            #region get List Quyền trong token
            List<string> listPermissionToken = FindOperator(value);
            if (listPermissionToken.Any(x => x.Equals(PermissionInput.ToString())))
            {
                Console.WriteLine("quyen trong token khong co hoac nhap sai");
            }
            #endregion
            foreach (var item in listPermission)
            {
                var existQuyen = listPermissionToken.Any(x => x.Equals(item));
                if (existQuyen == false)
                {
                    Console.WriteLine($"thieu quyen {item}");
                    return false;
                }
            }
            #endregion

            #region Check Tập Con
            //c1
            //List<PermissionUnix> listEnums = new List<PermissionUnix>();
            #region Check Số đó có thuộc trong Enum hay không. Không tính tổng.
            //var attributes = PermissionUnix.A | PermissionUnix.D | PermissionUnix.S | PermissionUnix.U | PermissionUnix.R;
            //if ((int)(attributes & PermissionUnix.A) == text)
            //{
            //    Console.WriteLine("AAAAAAAAA");
            //    listEnums.Add(PermissionUnix.A);
            //}
            //if ((int)(attributes & PermissionUnix.D) == text)
            //{
            //    Console.WriteLine("DDDDDDDDDD");
            //    listEnums.Add(PermissionUnix.D);
            //}
            //if ((int)(attributes & PermissionUnix.R) == text)
            //{
            //    Console.WriteLine("RRRRRRRRRR");
            //    listEnums.Add(PermissionUnix.R);
            //}
            //if ((int)(attributes & PermissionUnix.S) == text)
            //{
            //    Console.WriteLine("SSSSSSSSSS");
            //    listEnums.Add(PermissionUnix.S);
            //}
            //if ((int)(attributes & PermissionUnix.U) == text)
            //{
            //    Console.WriteLine("UUUUUUUUUU");
            //    listEnums.Add(PermissionUnix.D);
            //}
            #endregion
            //or

            //foreach (PermissionUnix item in (PermissionUnix[])Enum.GetValues(typeof(PermissionUnix)))
            //{
            //    if (PermissionInput > (int)item)
            //    {
            //        listEnums.Add(item);
            //    }
            //}
            #endregion

            return true;
        }
        public static List<string> FindOperator(int PermissionInput)
        {
            //int PermissionInput = int.Parse(alanWalker[1]);
            //Console.WriteLine((PermissionUnix)PermissionInput);
            var permissionCurrent = (PermissionUnix)PermissionInput;
            List<string> listPermission = new List<string>();
            //if ((int)permissionCurrent != PermissionInput)
            //{
            //    return listPermission;
            //}
            string permission = Regex.Replace(permissionCurrent.ToString(), @"\s+", "");
            listPermission = permission.Split(',').ToList();
            return listPermission;
            //int count = 2;
            #region TEST
            //while (true)
            //{
            //    var flag = false;
            //    int i = 0;
            //    int a = 0;
            //    foreach (var item in enums)
            //    {

            //        foreach (var item2 in enums)
            //        {
            //            int b = a + 1;
            //            if (enums[b+1] is null)
            //            {

            //            }
            //            if (count == 2)
            //            {
            //                if ((int)enums[a] + (int)(enums[b + 1]) == PermissionInput)
            //                {
            //                    Console.WriteLine($"RIGHT : {enums[a]}     {enums[b + 1]}");
            //                    flag = true;
            //                }
            //            }
            //            b++;
            //        }
            //    }
            //    if (flag == false)
            //    {
            //        count++;
            //    }
            //}
            #endregion
            //return true;
        }
    }
    #endregion
}
