using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Detections.Services
{
    public class HandleTask<T>
    {
        public Dictionary<string, bool> HandleTasks { get; set; } = new Dictionary<string, bool>();
        public Dictionary<string, T> ResponseTasks { get; set; } = new Dictionary<string, T>();
        public HandleTask()
        {

        }
        public void Action(string key,T value)
        {
            if (HandleTasks.ContainsKey(key))
            {
                HandleTasks[key] = true;

                if(!ResponseTasks.ContainsKey(key))
                {
                    ResponseTasks.Add(key, value);
                }
            }
        }

        public bool Get(string key)
        {
            if (HandleTasks.ContainsKey(key))
            {
                return HandleTasks[key];
            }
            return false;
        }

        public T GetData(string key)
        {
            if(ResponseTasks.ContainsKey(key))
            {
                return ResponseTasks[key];
            }
            return default(T);
        }

        public void Remove(string key)
        {
            if (HandleTasks.ContainsKey(key))
            {
                HandleTasks.Remove(key);
            }

            if (ResponseTasks.ContainsKey(key))
            {
                ResponseTasks.Remove(key);
            }
        }
    }
}
