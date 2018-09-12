using Newtonsoft.Json;

namespace TravelerBot.Api.ResourceModels
{
    public class Keyboard
    {
        [JsonProperty("one_time")]
        public bool OneTime { get; set; }

        public Button[][] buttons { get; set; }
    }

    public class Button
    {
        public Action action { get; set; }

        public string color { get; set; }
    }

    public class Action
    {
        public string type { get; set; }

        public string payload { get; set; }

        public string label { get; set; }
    }
}
