
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
    public class AddMenuButton : BaseButton
    {
        public override string Name { get; set; }

        public override ResponseModel GetResponse()
        {
            var message = "Указжите все пункты";

            var fromAndToButtons = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Откуда",
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
                            label = "Куда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "2"
                            })
                        }
                    }
                }.ToArray();

            var dateAndTimeButtons = 
                new List<Button>
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
                                button = "3"
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
                                button = "4"
                            })
                        }
                    }
                }.ToArray();

            var phoneAndDescriptionButtons = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Телефон",
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
                            label = "Комментарии",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "6"
                            })
                        }
                    }
                }.ToArray();

            var OkAndStartButtons = 
                new List<Button>
                {
                    new Button
                    {
                        color = "primary",
                        action = new Action
                        {
                            label = "Готово",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "7"
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
                                button = "8"
                            })
                        }
                    }
                }.ToArray();

            var keyboard = new Keyboard
            {
                OneTime = false,
                buttons = new[]
                {
                    fromAndToButtons,
                    dateAndTimeButtons,
                    phoneAndDescriptionButtons,
                    OkAndStartButtons
                }
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

        public ResponseModel GetResponse(int[] indexes)
        {
            var message = "Указжите все пункты";

            var fromAndToButtons = new List<Button>
                {
                    new Button
                    {
                        color = (indexes.FirstOrDefault(t => t == 1) == 0) ? "negative" : "default",
                        action = new Action
                        {
                            label = "Откуда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "1"
                            })
                        }
                    },
                    new Button
                    {
                        color = (indexes.FirstOrDefault(t => t == 2) == 0) ? "negative" : "default",
                        action = new Action
                        {
                            label = "Куда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "2"
                            })
                        }
                    }
                }.ToArray();

            var dateAndTimeButtons = (indexes.FirstOrDefault(t => t == 3) == 0) ?
                new List<Button>
                {
                    new Button
                    {
                        color = "negative", //(indexes.FirstOrDefault(t => t == 3) == 0) ? "negative" : "default",
                        action = new Action
                        {
                            label = "Когда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "3"
                            })
                        }
                    }
                }.ToArray() :
                new List<Button>
                { 
                    new Button
                    {
                        color = "default", //(indexes.FirstOrDefault(t => t == 3) == 0) ? "negative" : "default",
                        action = new Action
                        {
                            label = "Когда",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "3"
                            })
                        }
                    },
                    new Button
                    {
                        color = (indexes.FirstOrDefault(t => t == 4) == 0) ? "negative" : "default",
                        action = new Action
                        {
                            label = "Во сколько",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "4"
                            })
                        }
                    }
                }.ToArray();

            var phoneAndDescriptionButtons = new List<Button>
                {
                    new Button
                    {
                        color = (indexes.FirstOrDefault(t => t == 3) == 0) ? "negative" : "default",
                        action = new Action
                        {
                            label = "Телефон",
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
                            label = "Комментарии",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "6"
                            })
                        }
                    }
                }.ToArray();

            var OkAndStartButtons = (indexes.Length != 5) ?
                new List<Button>
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
                                button = "7"
                            })
                        }
                    }
                }.ToArray() :
                new List<Button>
                {
                    new Button
                    {
                        color = "primary",
                        action = new Action
                        {
                            label = "Готово",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "7"
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
                                button = "8"
                            })
                        }
                    }
                }.ToArray();

            var keyboard = new Keyboard
            {
                OneTime = false,
                buttons = new[] 
                {
                    fromAndToButtons,
                    dateAndTimeButtons,
                    phoneAndDescriptionButtons,
                    OkAndStartButtons
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