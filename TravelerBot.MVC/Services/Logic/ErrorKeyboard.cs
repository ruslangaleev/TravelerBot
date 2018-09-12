using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class ErrorKeyboard : IKeyboard
    {
        public ResponseModel Get(string buttonName)
        {
            throw new System.NotImplementedException();
        }

        public ResponseModel Get()
        {
            throw new System.NotImplementedException();
        }

        public ResponseModel Get(InboundButton[] inboundButtons)
        {
            var message = "Необходимо указать все необходимые пункты";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        color = (inboundButtons.First(t => t.Index == 1) != null) ? "positive" : "default",
                        action = new Action
                        {
                            label = "Водитель",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "1"
                            })
                        }
                    },
                    new Button
                    {
                        color = (inboundButtons.First(t => t.Index == 2) != null) ? "positive" : "default",
                        action = new Action
                        {
                            label = "Пассажир",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
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
                        color = (inboundButtons.First(t => t.Index == 3) != null) ? "positive" : "default",
                        action = new Action
                        {
                            label = (inboundButtons.First(t => t.Index == 3) != null) ? $"Откуда - {inboundButtons.First(t => t.Index == 3).Value}" : "Откуда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "3"
                            })
                        }
                    },
                    new Button
                    {
                        color = (inboundButtons.First(t => t.Index == 4) != null) ? "positive" : "default",
                        action = new Action
                        {
                            label = (inboundButtons.First(t => t.Index == 4) != null) ? $"Куда - {inboundButtons.First(t => t.Index == 4)}" : "Куда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
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
                        color = "default",
                        action = new Action
                        {
                            label = "Когда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "5"
                            })
                        }
                    },
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Во сколько",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "6"
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
                                label = "Готово",
                                type = "text",
                                payload = JsonConvert.SerializeObject(new
                                {
                                    button = "8"
                                })
                            }
                        },
                        new Button
                        {
                            color = "default",
                            action = new Action
                            {
                                label = "Перейти на начало",
                                type = "text",
                                payload = JsonConvert.SerializeObject(new
                                {
                                    button = "9"
                                })
                            }
                        }
                    }.ToArray();

            var keyboard = new Keyboard
            {
                OneTime = false,
                buttons = new[]
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
    }
}
