using System;
using System.Collections.Generic;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.ResourceModels;
using TravelerBot.MVC.Models;
using TravelerBot.MVC.Services.Logic;

namespace TravelerBot.Api.Services.Logic
{
    public class LogicController
    {
        private readonly ITripRepository _tripRepository;

        public LogicController(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public ResponseModel Get(string buttonName, int accountVkontakteId)
        {
            var tripp = _tripRepository.Get(accountVkontakteId);

            if (buttonName == "Начать")
            {
                var s = new OptionKeyboard();
                return s.Get();
            }

            if (buttonName == "Добавить поездку")
            {
                var s = new TypeParticipantKeyboard();
                return s.Get();
            }

            if (buttonName == "Водитель")
            {
                if (tripp != null)
                {
                    _tripRepository.Delete(tripp.TripId);
                }

                _tripRepository.Add(new Trip
                {
                    TripId = Guid.NewGuid(),
                    TypeParticipant = TypeParticipant.Driver,
                    AccountId = accountVkontakteId
                });

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
                tripp.From = true;
                _tripRepository.Update(tripp);

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Куда")
            {
                tripp.To = true;
                _tripRepository.Update(tripp);

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Когда")
            {
                tripp.Date = true;
                _tripRepository.Update(tripp);

                var s = new DateKeyboard();
                return s.Get();
            }

            if (buttonName == "Во сколько")
            {
                tripp.Time = true;
                _tripRepository.Update(tripp);

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

                    var s = new OptionKeyboard();
                    return s.Get("Объявление успешно добавлено");
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

                tripp.TimeSpan = timespan;
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
                        Index = 6
                    }
                });
            }

            return null;
        }
    }
}
