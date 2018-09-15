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
    public class EditDateButton : BaseButton
    {
        public override string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DateTime Convert(string when)
        {
            if (when == "Сегодня")
            {
                return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            }

            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0).AddDays(1);
        }

        public override ResponseModel GetResponse()
        {
            var message = "Выберите дату";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Сегодня",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "1"
                            })
                        }
                    },
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Завтра",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "2"
                            })
                        }
                    }
                }.ToArray();

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
                buttons = new[] { buttons, startKeyboard }
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