using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelerBot.MVC.ResourceModels
{
    public class Message
    {
        public string Type { get; set; }

        public string Secret { get; set; }

        [JsonProperty("object")]
        public ObjectMessage ObjectMessage { get; set; }

        [JsonProperty("group_id")]
        public int GroupId { get; set; }
    }

    public class ObjectMessage
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        public string Body { get; set; }
    }
}