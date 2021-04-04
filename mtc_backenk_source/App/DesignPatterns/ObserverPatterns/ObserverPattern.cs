using System.Collections.Generic;

namespace Project.App.DesignPatterns.ObserverPatterns
{
    public class ObserverPattern
    {
        public static ObserverPattern Instance { get; } = new ObserverPattern();
        public delegate void UpdateFunc(object data);
        private readonly Dictionary<string, List<UpdateFunc>> observers;

        public ObserverPattern()
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
