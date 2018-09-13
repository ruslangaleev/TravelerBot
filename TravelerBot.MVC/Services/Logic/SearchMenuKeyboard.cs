using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.MVC.Services.Logic
{
    public class SearchMenuKeyboard : IKeyboard
    {
        public ResponseModel Get(string buttonName)
        {
            throw new System.NotImplementedException();
        }

        public ResponseModel Get()
        {
            var message = "Укажите параметры для поиска";

            var buttonsFromTo = new List<Button>
                {
                    new Button
                    {
                        color ="default",
                        action = new Action
                        {
                            label = "Откуда", //(inboundButtons?.FirstOrDefault(t => t.Index == 3) != null) ? $"Откуда" : "Откуда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "3"
                            })
                        }
                    },
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Куда", //(inboundButtons?.FirstOrDefault(t => t.Index == 4) != null) ? $"Куда" : "Куда",
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
                    //new Button
                    //{
                    //    color = "default",
                    //    action = new Action
                    //    {
                    //        label = "Во сколько",
                    //        type = "text",
                    //        payload = JsonConvert.SerializeObject(new
                    //        {
                    //            button = "6"
                    //        })
                    //    }
                    //}
                }.ToArray();

            var startKeyboard = new List<Button>
                    {
                        new Button
                        {
                            color = "default",
                            action = new Action
                            {
                                label = "Показать",
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

        public ResponseModel Get(InboundButton[] inboundButtons)
        {
            var message = "Укажите параметры для поиска";

            var buttonsFromTo = new List<Button>
                {
                    new Button
                    {
                        color = (inboundButtons?.FirstOrDefault(t => t.Index == 3) != null) ? "positive" : "default",
                        action = new Action
                        {
                            label = "Откуда", //(inboundButtons?.FirstOrDefault(t => t.Index == 3) != null) ? $"Откуда" : "Откуда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "3"
                            })
                        }
                    },
                    new Button
                    {
                        color = (inboundButtons?.FirstOrDefault(t => t.Index == 4) != null) ? "positive" : "default",
                        action = new Action
                        {
                            label = "Куда", //(inboundButtons?.FirstOrDefault(t => t.Index == 4) != null) ? $"Куда" : "Куда",
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
                        color = (inboundButtons?.FirstOrDefault(t => t.Index == 5) != null) ? "positive" : "default",
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
                    //new Button
                    //{
                    //    color = "default",
                    //    action = new Action
                    //    {
                    //        label = "Во сколько",
                    //        type = "text",
                    //        payload = JsonConvert.SerializeObject(new
                    //        {
                    //            button = "6"
                    //        })
                    //    }
                    //}
                }.ToArray();

            var startKeyboard = new List<Button>
                    {
                        new Button
                        {
                            color = "primary",
                            action = new Action
                            {
                                label = "Показать",
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