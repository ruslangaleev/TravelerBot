﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            if (buttonName == "Начать")
            {
                var s = new OptionKeyboard();
                return s.Get();
            }

            var trips = _tripRepository.Get(accountVkontakteId);
            var trip = trips.FirstOrDefault();
            if (trip != null)
            {
                if (trip.TypeTransaction == MVC.Data.Models.TypeTransaction.Search)
                {
                    if (buttonName == "Добавить поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                        return AddTrip(buttonName, accountVkontakteId);
                    }

                    return SearchTrips(buttonName, accountVkontakteId, trip);
                }
                else
                {
                    if (buttonName == "Найти поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                    }

                    return AddTrip(buttonName, accountVkontakteId, trip);
                }
            }
            else
            {
                if (buttonName == "Добавить поездку")
                {
                    return AddTrip(buttonName, accountVkontakteId);
                }
                else
                {
                    return SearchTrips(buttonName, accountVkontakteId);
                }
            }
        }

        private ResponseModel SearchTrips(string buttonName, int accountVkontakteId, Trip trip = null)
        {
            if (trip == null)
            {
                _tripRepository.Add(new Trip
                {
                    TripId = Guid.NewGuid(),
                    AccountId = accountVkontakteId,
                    TypeTransaction = MVC.Data.Models.TypeTransaction.Search
                });

                var keyboard = new SearchMenuKeyboard();
                return keyboard.Get();
            }

            if (buttonName == "Откуда")
            {
                trip.From = true;
                _tripRepository.Update(trip);

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Куда")
            {
                trip.To = true;
                _tripRepository.Update(trip);

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Когда")
            {
                trip.Date = true;
                _tripRepository.Update(trip);

                var s = new DateKeyboard();
                return s.Get();
            }

            if (buttonName == "Показать")
            {
                var trips = _tripRepository.Get(trip.FromString, trip.ToToString, trip.DateTime);

                if (trips.Count() == 0)
                {
                    return new ResponseModel
                    {
                        Message = "Объявлений не найдено"
                    };
                }

                var tripsAsString = trips.Select(t =>
                {
                    return $"Кто: {t.AccountId}";
                });

                var message = string.Join("\r\n\r\n", tripsAsString);

                return new ResponseModel
                {
                    Message = message
                };
            }

            if (trip.From)
            {
                trip.From = false;
                trip.FromString = (buttonName == "Уфа") ? "Уфа" : "Караидель";

                _tripRepository.Update(trip);

                var s = new SearchMenuKeyboard();
                return s.Get();
            }

            if (trip.To)
            {
                trip.To = false;
                trip.ToToString = buttonName;

                _tripRepository.Update(trip);

                var s = new SearchMenuKeyboard();
                return s.Get();
            }

            if (trip.Date)
            {
                trip.Date = false;
                switch (buttonName)
                {
                    case "Сегодня":
                        trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                        break;
                    case "Завтра":
                        trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
                        break;
                    default:
                        throw new ArgumentException("Необходимо указать дату");
                }

                _tripRepository.Update(trip);

                var s = new SearchMenuKeyboard();
                return s.Get();
            }

            return null;
        }

        private ResponseModel AddTrip(string buttonName, int accountVkontakteId, Trip trip = null)
        {
            if (buttonName == "Добавить поездку")
            {
                if (trip != null)
                {
                    _tripRepository.Delete(trip.TripId);
                }

                _tripRepository.Add(new Trip
                {
                    TripId = Guid.NewGuid(),
                    TypeParticipant = TypeParticipant.Driver,
                    AccountId = accountVkontakteId,
                    TypeTransaction = MVC.Data.Models.TypeTransaction.Adding
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
                trip.From = true;
                _tripRepository.Update(trip);

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Куда")
            {
                trip.To = true;
                _tripRepository.Update(trip);

                var s = new PointKeyboard();
                return s.Get();
            }

            if (buttonName == "Когда")
            {
                trip.Date = true;
                _tripRepository.Update(trip);

                var s = new DateKeyboard();
                return s.Get();
            }

            if (buttonName == "Во сколько")
            {
                trip.Time = true;
                _tripRepository.Update(trip);

                var s = new TimeKeyboard();
                return s.Get();
            }

            if (buttonName == "Готово")
            {
                if ((trip.DateTime != null) && (!string.IsNullOrEmpty(trip.FromString))
                    && (!string.IsNullOrEmpty(trip.ToToString)) && (trip.TimeSpan != null))
                {
                    trip.IsPublished = true;
                    _tripRepository.Update(trip);

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

            if (trip.From)
            {
                trip.From = false;
                trip.FromString = (buttonName == "Уфа") ? "Уфа" : "Караидель";

                _tripRepository.Update(trip);

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = trip.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.ToToString) ? 0 : 4
                    },
                    new InboundButton
                    {
                        Index = (trip.DateTime == null) ? 0 : 5
                    },
                    new InboundButton
                    {
                        Index = 3
                    }
                });
            }

            if (trip.To)
            {
                trip.To = false;
                trip.ToToString = buttonName;

                _tripRepository.Update(trip);

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = trip.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = 4
                    }
                });
            }

            if (trip.Date)
            {
                trip.Date = false;
                switch (buttonName)
                {
                    case "Сегодня":
                        trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                        break;
                    case "Завтра":
                        trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
                        break;
                    default:
                        throw new ArgumentException("Необходимо указать дату");
                }

                _tripRepository.Update(trip);

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = trip.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.ToToString) ? 0 : 4
                    },
                    new InboundButton
                    {
                        Index = 5
                    }
                });
            }

            if (trip.Time)
            {
                trip.Time = false;
                var timespan = TimeSpan.Parse(buttonName);

                trip.TimeSpan = timespan;
                _tripRepository.Update(trip);

                var s = new MenuKeyboard();
                return s.Get(new InboundButton[]
                {
                    new InboundButton
                    {
                        Index = trip.TypeParticipant == TypeParticipant.Driver ? 1 : 0
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.FromString) ? 0 : 3
                    },
                    new InboundButton
                    {
                        Index = string.IsNullOrEmpty(trip.ToToString) ? 0 : 4
                    },
                    new InboundButton
                    {
                        Index = (trip.DateTime == null) ? 0 : 5
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
