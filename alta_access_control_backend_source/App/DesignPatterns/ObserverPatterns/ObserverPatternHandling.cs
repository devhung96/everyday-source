using Microsoft.Extensions.Configuration;

namespace Project.App.DesignPatterns.ObserverPatterns
{
    public class ObserverPatternHandling
    {
        public static readonly ObserverPatternHandling instance = new ObserverPatternHandling();
        public static ObserverPatternHandling Instance => instance;
        public static IConfiguration Configuration;
        public ObserverPatternHandling() { }
        public void Connection(IConfiguration configuration)
        {
            Configuration = configuration;
            Handling();
        }
        public void Handling()
        {
            ObserverPattern.Instance.On("EventName", (data) =>
            {

            });
        }
    }
}
