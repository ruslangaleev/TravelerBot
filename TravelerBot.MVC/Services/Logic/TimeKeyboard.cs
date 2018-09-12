using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelerBot.Api.ResourceModels;
using TravelerBot.Api.Services.Interfaces;

namespace TravelerBot.Api.Services.Logic
{
    public class TimeKeyboard : IKeyboard
    {
        public ResponseModel Get()
        {
            var message = "Укажите время в формате ЧЧ:ММ";

            //var buttons = new List<Button>
            //    {
            //        new Button
            //        {
            //            Color = "default",
            //            Action = new ResourceModels.Action
            //            {
            //                Label = "Сегодня",
            //                Type = "text",
            //                Payload = JsonConvert.SerializeObject(new
            //                {
            //                    button = "1"
            //                })
            //            }
            //        },
            //        new Button
            //        {
            //            Color = "default",
            //            Action = new ResourceModels.Action
            //            {
            //                Label = "Завтра",
            //                Type = "text",
            //                Payload = JsonConvert.SerializeObject(new
            //                {
            //                    button = "2"
            //                })
            //            }
            //        }
            //    }.ToArray();

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
                buttons = new[] { startKeyboard }
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
