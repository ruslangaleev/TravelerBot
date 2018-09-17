using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.ResourceModels;
using TravelerBot.MVC.Models;
using Action = TravelerBot.Api.ResourceModels.Action;

namespace TravelerBot.MVC.Data.Models
{
    public class UserState
    {
        public Guid UserStateId { get; set; }

        /// <summary>
        /// Идентификатор внешнего аккаунта.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Операцию, которую сейчас выполняет пользователь.
        /// </summary>
        public TypeTransaction TypeTransaction { get; set; }

        /// <summary>
        /// Какая сейчас кнопка выбрана?
        /// </summary>
        public TypeButton TypeButton { get; set; }

        /// <summary>
        /// Характерна для поиска.
        /// </summary>
        public string Filter { get; set; }

        public Guid TripId { get; set; }
    }

    public enum TypeButton
    {
        EditButton = 0,
        EditMenuButton = 1,
        EditFromButton = 2,
        EditToButton = 3,
        EditDateButton = 4,
        EditTimeButton = 5,
        EditPhoneButton = 7,
        EditDescriptionButton = 8,

        AddMenuButton = 6,
    }

    public abstract class BaseButton
    {
        public abstract string Name { get; set; }

        public abstract ResponseModel GetResponse();

        public abstract ResponseModel GetResponse(IEnumerable<Trip> trips);
    }

    public class EditButton : BaseButton
    {
        public override string Name { get; set; }

        public override ResponseModel GetResponse(IEnumerable<Trip> trips)
        {
            var listButtons = new List<Button[]>();
            var tripsMessage = new List<string>();

            int step = -1;

            foreach (var entry in trips)
            {
                step++;
                var buttons = new List<Button>
                {
                    new Button
                    {
                        color = "positive",
                        action = new Action
                        {
                            label = $"Изменить {step}",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "1"
                            })
                        }
                    },
                    new Button
                    {
                        color = "negative",
                        action = new Action
                        {
                            label = $"Удалить {step}",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "2"
                            })
                        }
                    }
                };

                listButtons.Add(buttons.ToArray());
                tripsMessage.Add($"Поездка {step}\r\n\r\n" +
                    $"{entry.Whence} - {entry.Where}\r\n" +
                    $"{((DateTime)entry.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                    $"{((DateTime)entry.DateTime).ToString("hh:mm")}");
            }

            listButtons.Add(new List<Button>
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
            }.ToArray());

            var keyboard = new Keyboard
            {
                OneTime = false,
                buttons = listButtons.ToArray()
            };

            return new ResponseModel
            {
                Message = string.Join("\r\n\r\n", tripsMessage),
                Keyboard = keyboard
            };
        }

        public override ResponseModel GetResponse()
        {
            throw new NotImplementedException();
        }
    }
}