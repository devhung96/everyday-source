using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations.Schema;

public static class Extension
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> entities, string orderByQueryString)
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

            if (objectProperty.GetCustomAttribute<ColumnAttribute>()?.Name is null)
            {
                Console.WriteLine($"Field {propertyFromQueryName} :Value is object not order by");
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

    public static bool TypeToken(this string Type)
    {
        if (Type != null && (Type.ToLower() == "admin" || Type.ToLower() == "h.anh"))
        {
            return true;
        }
        return false;
    }
}

