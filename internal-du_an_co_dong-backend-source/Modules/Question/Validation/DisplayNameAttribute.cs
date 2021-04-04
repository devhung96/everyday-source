using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Project.App.Database;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Project.Modules.Question.Validation
{
    public class DisplayNameAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public int Modules { get; set; }
        public int Level { get; set; }

        #region Override ActionFilterAttribute
         
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //int q = 5;
            //while(true)
            //{
            //    Console.WriteLine("haha");
            //}    
            string name = "sssss";
            if (string.IsNullOrEmpty(name))
                name = filterContext.Controller.GetType().Name;
            //bool hasAuthorizeAttribute = filterContext.ActionDescriptor.IsDe
            //.GetCustomAttributes(typeof(AuthorizeAttribute), false)
            //.Any();
            base.OnActionExecuting(filterContext);
        }
    
        #endregion

        #region Override IAuthorizationFilter
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            int modules = Modules;
            int LevelInt = Level;
            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (!IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (modules != 0 && LevelInt != 0)
            {
                bool isAuthorized = CheckPermission(context, modules, LevelInt);
                if (!isAuthorized)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
        #endregion
        public static bool CheckPermission(AuthorizationFilterContext context,int Modules,int level)
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

            #region Lấy tất cả Role thức tế trong Token
            List<int> RoleToken = new List<int>();
            var Roles = context.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).ToList();
            if (Roles.Count < Modules-1)
            {
                Console.WriteLine("token build rolse thieu. vi tri nhap vao in controller nho hon viu tri trong roles.");
                return false;
            }
            var LevelFromController = level;
            if(!int.TryParse(Roles[Modules+1].Value, out int LevelFromToken) || LevelFromToken == 0)
            {
                Console.WriteLine("Khong co quyen nao o modules nay.(null or 0)");
                return false;
            }
            #endregion

            #region Input
            List<string> listPermission = FindOperator(LevelFromController);
            if (listPermission.Any(x => x.Equals(LevelFromController.ToString())))
            {
                Console.WriteLine("Ghi sai quyen o controller");
            }
            #endregion

            //token vi du

            #region Check Quyền trong token
            #region get List Quyền trong token
            List<string> listPermissionToken = FindOperator(LevelFromToken);
            if (listPermissionToken.Any(x => x.Equals(LevelFromToken.ToString())))
            {
                Console.WriteLine("quyen trong token khong co hoac nhap sai");
            }
            #endregion
            foreach (var item in listPermission)
            {
                var existQuyen = listPermissionToken.Any(x => x.Equals(item));
                if (existQuyen == false)
                {
                    Console.WriteLine($"thieu quyen {item} trong modules số {Modules}");
                    return false;
                }
            }
            #endregion

            return true;
        }
        public static List<string> FindOperator(int PermissionInput)
        {
            var permissionCurrent = (PermissionUnix)PermissionInput;
            List<string> listPermission = new List<string>();
            string permission = Regex.Replace(permissionCurrent.ToString(), @"\s+", "");
            listPermission = permission.Split(',').ToList();
            return listPermission;
        }
        //public static List<string> DeQuyEnum(int position,List<string> input)
        //{

        //}
    }
    [Flags]
    public enum PermissionUnix
    {
        /// <summary>
        /// A => Đọc tất cả (All)
        /// D => Xem chi tiết (Detail)
        /// S => Thêm mới (Store)
        /// U => Update
        /// R => Remove
        /// </summary>
        A = 1,
        D = 2,
        S = 4,
        U = 8,
        R = 16
    }
}
