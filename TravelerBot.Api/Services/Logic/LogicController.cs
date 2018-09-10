using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TravelerBot.Api.Data.Models;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.ResourceModels;

namespace TravelerBot.Api.Services.Logic
{
    public class LogicController
    {
        private readonly ITripRepository _tripRepository;

        public LogicController(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        // Временный словарь
        private readonly Dictionary<int, Trip> trips = new Dictionary<int, Trip>();

        public ResponseModel Get(string buttonName, int accountVkontakteId)
        {
            var tripp = _tripRepository.Get(accountVkontakteId).GetAwaiter().GetResult();

            if (buttonName == "Начать")
            {
                var s = new TypeParticipantKeyboard();
                return s.Get();
            }

            if (buttonName == "Добавить поздку")
            {
                var s = new MenuKeyboard();
                return s.Get(new InboundButton[] { });
            }

            if (buttonName == "Водитель")
            {
                _tripRepository.Add(new Trip
                {
                    TypeParticipant = TypeParticipant.Driver,
                    AccountId = accountVkontakteId
                });

                //trips.Add(accountVkontakteId, new Trip
                //{
                //    TypeParticipant = TypeParticipant.Driver
                //});

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = 1
                    }
                });
            }

            if (buttonName == "Откуда")
            {
                var trip = trips[accountVkontakteId];
                trip.From = true;

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Куда")
            {
                var trip = trips[accountVkontakteId];
                trip.To = true;
            }

            if (buttonName == "Когда")
            {
                var trip = trips[accountVkontakteId];
                trip.Date = true;

                var s = new DateKeyboard();
                return s.Get();
            }

            if (buttonName == "Во сколько")
            {
                var trip = trips[accountVkontakteId];
                trip.Time = true;

                var s = new TimeKeyboard();
                return s.Get();
            }

            if (buttonName == "Готово")
            {
                if ((tripp.DateTime != null) && (!string.IsNullOrEmpty(tripp.FromString)) 
                    && (!string.IsNullOrEmpty(tripp.ToToString)) && (tripp.TimeSpan != null))
                {
                    tripp.IsPublished = true;
                    _tripRepository.Update(tripp);
                }
                else
                {
                    return new ResponseModel
                    {
                        Message = "Необходимо заполнить поля"
                    };
                }
            }
            
            if (tripp.From)
            {
                tripp.From = false;
                tripp.FromString = (buttonName == "Уфа") ? "Уфа" : "Караидель";

                _tripRepository.Update(tripp);

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = tripp.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.ToToString) ? 0 : 4
                    },
                    new InboundButton
                    {
                        Index = (tripp.DateTime == null) ? 0 : 5
                    },
                    new InboundButton
                    {
                        Index = 3
                    }
                });
            }

            if (tripp.To)
            {
                tripp.To = false;
                tripp.ToToString = buttonName;

                var s = new MenuKeyboard();
                s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = tripp.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = 4
                    }
                });
            }

            if (tripp.Date)
            {
                tripp.Date = false;
                switch(buttonName)
                {
                    case "Сегодня":
                        tripp.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                        break;
                    case "Завтра":
                        tripp.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
                        break;
                    default:
                        throw new ArgumentException("Необходимо указать дату");
                }

                _tripRepository.Update(tripp);

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = tripp.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.ToToString) ? 0 : 4
                    },
                    new InboundButton
                    {
                        Index = 5
                    }
                });
            }

            if (tripp.Time)
            {
                tripp.Time = false;

                var timespan = TimeSpan.Parse(buttonName);

                var s = new MenuKeyboard();
                s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = tripp.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(tripp.ToToString) ? 0 : 4
                    },
                    new InboundButton
                    {
                        Index = (tripp.DateTime == null) ? 0 : 5
                    },
                    new InboundButton
                    {
                        Index = 6
                    }
                });
            }

            return null;
        }
    }
}
