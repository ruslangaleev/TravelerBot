using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelerBot.Api.ResourceModels;
using TravelerBot.MVC.Models;
using Action = TravelerBot.Api.ResourceModels.Action;

namespace TravelerBot.MVC.Data.Models
{
    public class EditTimeButton : BaseButton
    {
        public override string Name { get; set; }

        public override ResponseModel GetResponse()
        {
            var message = "Укажите время в формате ЧЧ:ММ";

            var startKeyboard = new List<Button>
                    {
                        new Button
                        {
                            color = "default",
                            action = new Action
                            {
                                label = "Перейти на начало",
                                type = "text",
                                payload = JsonConvert.SerializeObject(new
                                {
                                    button = "3"
                                })
                            }
                        }
                    }.ToArray();

            var keyboard = new Keyboard
            {
                OneTime = false,
                buttons = new[] { startKeyboard }
            };

            return new ResponseModel
            {
                Message = message,
                Keyboard = keyboard
            };
        }

        public override ResponseModel GetResponse(IEnumerable<Trip> trips)
        {
            throw new NotImplementedException();
        }
    }
}