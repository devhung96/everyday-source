using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Project.App.Paginations
{
    public class PaginationSort<T>
    {
        public static IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString)
        {
            if (!entities.Any())
            {
                return entities;
            }

            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }

            string[] orderParams = orderByQueryString.Trim().Split(',');
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            StringBuilder orderQueryBuilder = new StringBuilder();

            foreach (string param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                {
                    continue;
                }

                string propertyFromQueryName = param.Trim().Split(" ")[0];
                PropertyInfo objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null)
                {
                    continue;
                }
                string sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";

                orderQueryBuilder.Append($"{objectProperty.Name} {sortingOrder}, ");
            }

            string orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            if (string.IsNullOrEmpty(orderQuery))
            {
                return entities;
            }
            return entities.OrderBy(orderQuery);
        }
    }
}
