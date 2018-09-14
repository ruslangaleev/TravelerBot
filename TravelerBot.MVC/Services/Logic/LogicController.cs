using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.ResourceModels;
using TravelerBot.MVC.Data.Models;
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
            if (buttonName == "Начать" || buttonName == "Перейти на начало")
            {
                var s = new OptionKeyboard();
                return s.Get();
            }

            var trips = _tripRepository.Get(accountVkontakteId, false);
            var trip = trips.FirstOrDefault();

            if (buttonName == "Редактировать поездки")
            {
                trip.TypeTransaction = TypeTransaction.Edit;
                _tripRepository.Update(trip);
                return GetKeyboardMenuEdit(accountVkontakteId);
            }
            if (trip.TypeTransaction == TypeTransaction.Edit)
            {

            }


            if (trip != null)
            {
                if (trip.TypeTransaction == MVC.Data.Models.TypeTransaction.Search)
                {
                    if (buttonName == "Добавить поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                        return AddTrip(buttonName, accountVkontakteId);
                    }

                    if (buttonName == "Найти поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                        return SearchTrips(buttonName, accountVkontakteId);
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
                if (buttonName == "Найти поездку")
                {
                    return SearchTrips(buttonName, accountVkontakteId);
                }
            }

            return null;
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

            if (buttonName == "Найти поездку")
            {
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
                    return $"Водитель\r\n" +
                    $"https://vk.com/id{t.AccountId}\r\n" +
                    $"{t.FromString} - {t.ToToString}\r\n" +
                    $"{((DateTime)t.DateTime).ToString("dd.MM.yyyy")}" +
                    $"{((DateTime)t.DateTime).ToString("hh:mm")}";
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
                return s.Get(new InboundButton[]
                    {
                        new InboundButton
                        {
                            Index = 3
                        },
                        new InboundButton
                        {
                            Index = (string.IsNullOrEmpty(trip.ToToString)) ? 0 : 4
                        },
                        new InboundButton
                        {
                            Index = (trip.DateTime == null) ? 0 : 5
                        }
                    });
            }

            if (trip.To)
            {
                trip.To = false;
                trip.ToToString = buttonName;

                _tripRepository.Update(trip);

                var s = new SearchMenuKeyboard();
                return s.Get(new InboundButton[]
                    {
                        new InboundButton
                        {
                            Index = (string.IsNullOrEmpty(trip.FromString)) ? 0 : 3
                        },
                        new InboundButton
                        {
                            Index = 4
                        },
                        new InboundButton
                        {
                            Index = (trip.DateTime == null) ? 0 : 5
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

                var s = new SearchMenuKeyboard();
                return s.Get(new InboundButton[]
                    {
                        new InboundButton
                        {
                            Index = (string.IsNullOrEmpty(trip.FromString)) ? 0 : 3
                        },
                        new InboundButton
                        {
                            Index = (string.IsNullOrEmpty(trip.ToToString)) ? 0 : 4
                        },
                        new InboundButton
                        {
                            Index = 5
                        }
                    });
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

                    return s.Get("Объявление успешно опубликовано:\r\n" +
                        "Водитель\r\n" +
                        $"{trip.FromString} - {trip.ToToString}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((TimeSpan)trip.TimeSpan).ToString(@"hh\:mm")}");
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
                        Index = 3
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
                        Index = (trip.TimeSpan == null) ? 0 : 6
                    },
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
                    },
                    new InboundButton
                    {
                        Index = (trip.DateTime == null) ? 0 : 5
                    },
                    new InboundButton
                    {
                        Index = (trip.TimeSpan == null) ? 0 : 6
                    },
                });
            }

            if (trip.Date)
            {
                trip.Date = false;

                if (buttonName == "Сегодня" && trip.TimeSpan == null)
                {
                    trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                }
                if (buttonName == "Сегодня" && trip.TimeSpan != null)
                {
                    trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ((TimeSpan)trip.TimeSpan).Hours, ((TimeSpan)trip.TimeSpan).Minutes, 0);
                }
                if (buttonName == "Завтра" && trip.TimeSpan == null)
                {
                    trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
                }
                if (buttonName == "Завтра" && trip.TimeSpan != null)
                {
                    trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ((TimeSpan)trip.TimeSpan).Hours, ((TimeSpan)trip.TimeSpan).Minutes, 0).AddDays(1);
                }

                //switch (buttonName)
                //{
                //    case "Сегодня":
                //        trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                //        break;
                //    case "Завтра":
                //        trip.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddDays(1);
                //        break;
                //    default:
                //        throw new ArgumentException("Необходимо указать дату");
                //}

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
                    },
                    new InboundButton
                    {
                        Index = (trip.TimeSpan == null) ? 0 : 6
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

        private ResponseModel GetKeyboardMenuEdit(int accountVkontakteId)
        {
            var trips = _tripRepository.Get(accountVkontakteId, true);
            if (trips == null || trips.Count() == 0)
            {
                return new ResponseModel
                {
                    Message = "У вас нет объявлений"
                };
            }

            var listButtons = new List<Button[]>();
            var tripsMessage = new List<string>();

            int step = -1;

            foreach(var entry in trips)
            {
                step++;
                var buttons = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new ResourceModels.Action
                        {
                            label = $"Объявление {step}",
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
                        action = new ResourceModels.Action
                        {
                            label = "Удалить",
                            type = "text",
                            payload = JsonConvert.SerializeObject(new
                            {
                                button = "2"
                            })
                        }
                    }
                };

                listButtons.Add(buttons.ToArray());
                tripsMessage.Add($"ИД: {step}\r\n" +
                    $"{entry.FromString} - {entry.ToToString}\r\n" +
                    $"{((DateTime)entry.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                    $"{((DateTime)entry.DateTime).ToString("hh:mm")}");
            }

            listButtons.Add(new List<Button>
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
            }.ToArray());

            var keyboard = new Keyboard
            {
                OneTime = false,
                buttons = listButtons.ToArray()
            };

            return new ResponseModel
            {
                Message =string.Join("\r\n\r\n", tripsMessage),
                Keyboard = keyboard
            };
        }

        private ResponseModel GetKeyboardEdit(string buttonValue, int accountVkontakteId)
        {
            // Объявление 1
            if (buttonValue.Length == 12)
            {
                throw new ArgumentNullException();
            }

            var value = buttonValue.EndsWith(" ");

            var trips = _tripRepository.Get(accountVkontakteId, true).ToArray();

            var buttonsFromTo = new List<Button>
                {
                    new Button
                    {
                        color = "default",
                        action = new ResourceModels.Action
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
                        action = new ResourceModels.Action
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
                        action = new ResourceModels.Action
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
                        action = new ResourceModels.Action
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
                        new Button
                        {
                            color = "default",
                            action = new ResourceModels.Action
                            {
                                label = "Готово",
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
                            action = new ResourceModels.Action
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
