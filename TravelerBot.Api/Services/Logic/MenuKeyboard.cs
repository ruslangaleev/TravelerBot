using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class MenuKeyboard : IKeyboard
    {
        public ResponseModel Get(string buttonName)
        {
            var message = "Укажите все необходимые пункты";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        Color = (buttonName == "Водитель") ? "positive" : "default",
                        Action = new Action
                        {
                            Label = "Водитель",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "1"
                            })
                        }
                    },
                    new Button
                    {
                        Color = (buttonName == "Пассажир") ? "positive" : "default",
                        Action = new Action
                        {
                            Label = "Пассажир",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "2"
                            })
                        }
                    }
                }.ToArray();

            var buttonsFromTo = new List<Button>
                {
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Откуда",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "3"
                            })
                        }
                    },
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Куда",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "4"
                            })
                        }
                    }
                }.ToArray();

            var buttonsDateTime = new List<Button>
                {
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Когда",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "5"
                            })
                        }
                    },
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Во сколько",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "6"
                            })
                        }
                    }
                }.ToArray();

            var buttonsDescription = new List<Button>
                {
                    new Button
                    {
                        Color = "default",
                        Action = new Action
                        {
                            Label = "Комментарии",
                            Type = "text",
                            Payload = JsonConvert.SerializeObject(new
                            {
                                button = "7"
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
                                    button = "8"
                                })
                            }
                        }
                    }.ToArray();

            var keyboard = new Keyboard
            {
                OneTime = false,
                Buttons = new[]
                {
                    buttons,
                    buttonsFromTo,
                    buttonsDateTime,
                    startKeyboard
                }
            };

            return new ResponseModel
            {
                Message = message,
                Keyboard = keyboard
            };
        }

        public ResponseModel Get()
        {
            throw new System.NotImplementedException();
        }
    }
}
