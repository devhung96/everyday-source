using Project.App.Database;
using Project.Modules.Exchangerates.Entities;
using Project.Modules.Exchangerates.Requests;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Exchangerates.Services
{
    public interface IExchangerateService
    {
        (Exchangerate data, string message) CreateExchangerate(ExchangerateCreateRequest valueInput);
        (Exchangerate data, string message) GetExchangerateCode(string exchangerateCode);
        List<Exchangerate> GetAll();

    }
    public class ExchangerateService : IExchangerateService
    {
        private readonly MariaDBContext mariaDBContext;
        public ExchangerateService(MariaDBContext _mariaDBContext)
        {
            mariaDBContext = _mariaDBContext;
        }

        public (Exchangerate data, string message) CreateExchangerate(ExchangerateCreateRequest valueInput)
        {
            int exchangerateOrder = GetAll().OrderByDescending(x => x.ExchangerateOrder).Select(x => x.ExchangerateOrder).FirstOrDefault();
            Exchangerate exchangerate = new Exchangerate()
            {
                ExchangerateCode = valueInput.ExchangerateCode,
                ExchangerateRate = valueInput.ExchangerateRate,
                ExchangerateOrder = exchangerateOrder + 1
            };
            mariaDBContext.Exchangerates.Add(exchangerate);
            mariaDBContext.SaveChanges();
            return (exchangerate, "CreateExchangerateSuccess");
        }

        public List<Exchangerate> GetAll()
        {
            return mariaDBContext.Exchangerates.ToList();
        }
        public (Exchangerate data, string message) GetById(string exchangerateId)
        {
            Exchangerate exchangerate = mariaDBContext.Exchangerates.FirstOrDefault(x=>x.ExchangerateId.Equals(exchangerateId));
            if(exchangerate == null)
            {
                return (null, "exchangerateIdDoNotExists");
            }
            return (exchangerate, "GetByIdSuccess");
        }

        public (Exchangerate data, string message) GetExchangerateCode(string exchangerateCode)
        {
            Exchangerate exchangerate = GetAll().FirstOrDefault(x=>x.ExchangerateCode.Equals(exchangerateCode));
            if(exchangerate == null)
            {
                return (exchangerate, "exchangerateIdDoNotExists");
            }
            return (exchangerate, "GetByIdSuccess");
        }
    }
}
