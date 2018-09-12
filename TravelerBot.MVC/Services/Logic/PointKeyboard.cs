using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class PointKeyboard : IKeyboard
    {
        public ResponseModel Get()
        {
            var message = "Выберите пункт";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new ResourceModels.Action
                        {
                            label = "Уфа",
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
                        action = new ResourceModels.Action
                        {
                            label = "Караидель",
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
                            action = new ResourceModels.Action
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
