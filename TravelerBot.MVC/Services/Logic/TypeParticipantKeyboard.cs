using Newtonsoft.Json;
using System.Collections.Generic;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class TypeParticipantKeyboard : IKeyboard
    {
        public ResponseModel Get()
        {
            var message = "Добавить или найти поездку?";

            var buttons = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new Action
                        {
                            label = "Найти поездку",
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
                            label = "Добавить поездку",
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

        public ResponseModel Get(string buttonName)
        {
            throw new System.NotImplementedException();
        }

        public ResponseModel Get(InboundButton[] inboundButtons)
        {
            throw new System.NotImplementedException();
        }
    }
}
