using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.MoneyTypes.Entities;
using Project.Modules.MoneyTypes.Services;

namespace Project.Modules.MoneyTypes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyTypeController : BaseController
    {
        private readonly IMoneyTypeService _moneyTypeService;
        public MoneyTypeController(IMoneyTypeService moneyTypeService)
        {
            _moneyTypeService = moneyTypeService;
        }


        /// <summary>
        /// Hiển thị tất cả mức thu học phí
        /// DevHung
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ShowAll()
        {
            List<MoneyType> moneyTypes = _moneyTypeService.ShowAll();
            return ResponseOk(moneyTypes);
        }
    }
}