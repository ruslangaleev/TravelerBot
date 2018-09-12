using Newtonsoft.Json;

namespace TravelerBot.Api.ResourceModels
{
    public class Keyboard
    {
        [JsonProperty("one_time")]
        public bool OneTime { get; set; }

        public Button[][] Buttons { get; set; }
    }

    public class Button
    {
        public Action Action { get; set; }

        public string Color { get; set; }
    }

    public class Action
    {
        public string Type { get; set; }

        public string Payload { get; set; }

        public string Label { get; set; }
    }
}
