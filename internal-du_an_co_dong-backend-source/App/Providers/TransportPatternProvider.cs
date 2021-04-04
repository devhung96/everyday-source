using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Providers
{
    public class TransportPatternProvider
    {
        public static TransportPatternProvider Instance { get; } = new TransportPatternProvider();
        public delegate void UpdateFunc(object data);
        private readonly Dictionary<string, List<UpdateFunc>> observers;

        public TransportPatternProvider()
        {
            this.observers = new Dictionary<string, List<UpdateFunc>>();
        }

        public void On(string chanel, UpdateFunc func)
        {
            if (!this.observers.ContainsKey(chanel))
            {
                this.observers.Add(chanel, new List<UpdateFunc>());

            }
            this.observers[chanel].Add(func);
        }

        public void Off(string chanel, UpdateFunc func)
        {
            if (this.observers.ContainsKey(chanel))
            {
                this.observers[chanel].Remove(func);
            }
        }

        public void Emit(string chanel, object data)
        {
            if (this.observers.ContainsKey(chanel))
            {
                foreach (var observer in this.observers[chanel])
                {
                    observer.Invoke(data);
                }
            }
        }
    }
}
