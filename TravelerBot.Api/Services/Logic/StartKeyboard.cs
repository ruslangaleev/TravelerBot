using Newtonsoft.Json;
using System.Collections.Generic;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class MainKeyboard : IKeyboard
    {
        public ResponseModel Get()
        {
            var message = "Найти поездку или добавить новую поездку?";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Найти поездку",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "1"
                            })
                        }
                    },
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Добавить поездку",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
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
                            Color = "default",
                            Action = new Action
                            {
                                Label = "Перейти на начало",
                                Type = "text",
                                Payload = JsonConvert.SerializeObject(new
                                {
                                    button = "3"
                                })
                            }
                        }
                    }.ToArray();

            var keyboard = new Keyboard
            {
                OneTime = false,
                Buttons = new[] { buttons, startKeyboard }
            };

            return new ResponseModel
            {
                Message = message,
                Keyboard = keyboard
            };
        }
    }
}
