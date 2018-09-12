using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class DateKeyboard : IKeyboard
    {
        public ResponseModel Get()
        {
            var message = "Выберите дату";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        Color = "default",
                        Action = new ResourceModels.Action
                        {
                            Label = "Сегодня",
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
                        Action = new ResourceModels.Action
                        {
                            Label = "Завтра",
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
                            Action = new ResourceModels.Action
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

        public ResponseModel Get(string buttonName)
        {
            throw new NotImplementedException();
        }

        public ResponseModel Get(InboundButton[] inboundButtons)
        {
            throw new NotImplementedException();
        }
    }
}
