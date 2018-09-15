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
    public class EditMenuButton : BaseButton
    {
        public override string Name { get; set; }

        public override ResponseModel GetResponse(IEnumerable<Trip> trips)
        {
            throw new NotImplementedException();
        }
        

        public override ResponseModel GetResponse()
        {
            var buttonsFromTo = new List<Button>
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
                                button = "3"
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
                        //new Button
                        //{
                        //    color = "default",
                        //    action = new Action
                        //    {
                        //        label = "Готово",
                        //        type = "text",
                        //        payload = JsonConvert.SerializeObject(new
                        //        {
                        //            button = "8"
                        //        })
                        //    }
                        //},
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
                Message = "Редактирование",
                Keyboard = keyboard
            };
        }
    }
}