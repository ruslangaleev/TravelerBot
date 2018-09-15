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
        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public  DateTime Convert(DateTime dateTime, string time)
        {
            var timeSpan = TimeSpan.Parse(time);
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeSpan.Hours, timeSpan.Minutes, 0);
        }

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