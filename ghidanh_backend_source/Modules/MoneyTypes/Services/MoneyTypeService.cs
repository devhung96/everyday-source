using Project.App.DesignPatterns.Repositories;
using Project.Modules.MoneyTypes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.MoneyTypes.Services
{
    public interface IMoneyTypeService
    {
        /// <summary>
        /// Hiển thị tất cả mức thu học phí
        /// DevHung
        /// </summary>
        /// <returns></returns>
        public List<MoneyType> ShowAll();
    }
    public class MoneyTypeService : IMoneyTypeService
    {
        private readonly IRepositoryMariaWrapper _repositoryMariaWrapper;
        public MoneyTypeService( IRepositoryMariaWrapper repositoryMariaWrapper)
        {
            _repositoryMariaWrapper = repositoryMariaWrapper;
        }


        public List<MoneyType> ShowAll()
        {
            return _repositoryMariaWrapper.MoneyTypes.FindAll().ToList();
        }
    }
}
