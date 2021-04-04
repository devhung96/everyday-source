using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Project.App.Request
{
    public class BaseRequest<T>
    {
        public T ToObject() {
            JObject data = JObject.FromObject(this);
            return data.ToObject<T>();
        }
        public object MergeData(object originData)
        {
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                if (propertyInfo.GetValue(this, null) == null && originData.GetType().GetProperty(propertyInfo.Name) != null)
                {
                    propertyInfo.SetValue(this, originData.GetType().GetProperty(propertyInfo.Name)
                    .GetValue(originData, null));
                }
            }
            return this;
        }
    }
}
