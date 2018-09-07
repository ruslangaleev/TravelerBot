using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelerBot.Api.ResourceModels
{
    public class ResponseModel
    {
        public string Message { get; set; }

        public Keyboard Keyboard { get; set; }
    }
}
