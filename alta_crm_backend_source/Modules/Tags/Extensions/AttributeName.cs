using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Extensions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class AttributeName : Attribute
    {
        private string name;

        public string Name
        {
            get { return name; }
        }

        public AttributeName(string name)
        {
            this.name = name;
        }
    }
}
