using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.ResourceModels;
using TravelerBot.MVC.Data.Models;
using TravelerBot.MVC.Data.Repositories.Interfaces;
using TravelerBot.MVC.Models;
using TravelerBot.MVC.Services.Logic;

namespace TravelerBot.Api.Services.Logic
{
    public class LogicController
    {
        private readonly ITripRepository _tripRepository;

        private readonly IUserRepository _userRepository;

        public LogicController(ITripRepository tripRepository, IUserRepository userRepository)
        {
            _tripRepository = tripRepository;
            _userRepository = userRepository;
        }

        public ResponseModel Get(string buttonName, int accountId)
        {
            var userState = _userRepository.GetUserState(accountId);
            if (userState == null)
            {
                _userRepository.AddUserState(new UserState
                {
                    AccountId = accountId
                });
            }

            if (buttonName == "Начать" || buttonName == "Перейти на начало")
            {
                var s = new OptionKeyboard();
                return s.Get();
            }

            // TODO: Чтобы возвращаело только один
            var trips = _tripRepository.Get(accountId, false);
            var trip = trips.FirstOrDefault();

            // Будет выводить список существующих поездок
            if (buttonName == "Мои поездки")
            {
                trips = _tripRepository.Get(accountId, true);
                if (trips == null || trips.Count() == 0)
                {
                    return new ResponseModel
                    {
                        Message = "У вас нет объявлений"
                    };
                }

                // Включаем режим редактирования.
                userState.TypeTransaction = TypeTransaction.Edit;
                userState.TypeButton = TypeButton.EditButton;
                _userRepository.Update(userState);

                var button = new EditButton();
                return button.GetResponse(trips);
            }

            if (buttonName == "Добавить поездку")
            {
                trips = _tripRepository.Get(accountId, false);
                foreach(var entry in trips.Select(t => t.TripId))
                {
                    _tripRepository.Delete(entry);
                }

                trip = new Trip
                {
                    TripId = Guid.NewGuid(),
                    TypeParticipant = TypeParticipant.Driver
                };

                _tripRepository.Add(trip);

                _tripRepository.SaveChanges();

                userState.TypeTransaction = TypeTransaction.Add;
                userState.TypeButton = TypeButton.AddMenuButton;
                userState.TripId = trip.TripId;
                _userRepository.Update(userState);

                var button = new AddMenuButton();
                return button.GetResponse();
            }

            if (buttonName == "Найти поездку")
            {
                userState.TypeTransaction = TypeTransaction.Search;
                //userState.TypeButton = TypeButton.AddMenuButton;
                _userRepository.Update(userState);
            }

            // Будет выводит меню для существующей поездки
            if (userState.TypeTransaction == TypeTransaction.Edit)
            {
                if (userState.TypeButton == TypeButton.EditButton)
                {
                    trips = _tripRepository.Get(accountId, true);
                    if (buttonName.Length == 10)
                    {
                        trip = trips.ToArray()[Convert.ToInt32(buttonName.Substring(9, 1))];

                        userState.TripId = trip.TripId;

                        // Выводим список объявлений
                        userState.TypeButton = TypeButton.EditMenuButton;
                        _userRepository.Update(userState);

                        var button = new EditMenuButton();
                        return button.GetResponse();
                    }
                    if (buttonName.Length == 9)
                    {
                        trip = trips.ToArray()[Convert.ToInt32(buttonName.Substring(8, 1))];
                        _tripRepository.Delete(trip.TripId);
                        _tripRepository.SaveChanges();

                        trips = _tripRepository.Get(accountId, true);
                        if (trips.Count() == 0)
                        {
                            var s = new OptionKeyboard();
                            var result = s.Get();
                            result.Message = "У вас нет больше объявлений";
                            return result;
                        }

                        var button = new EditButton();
                        return button.GetResponse(trips);
                    }
                }

                if (userState.TypeButton == TypeButton.EditMenuButton)
                {
                    if (buttonName == "Откуда")
                    {
                        userState.TypeButton = TypeButton.EditFromButton;
                        _userRepository.Update(userState);

                        var button = new EditFromButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Куда")
                    {
                        userState.TypeButton = TypeButton.EditToButton;
                        _userRepository.Update(userState);

                        var button = new EditFromButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Когда")
                    {
                        userState.TypeButton = TypeButton.EditDateButton;
                        _userRepository.Update(userState);

                        var button = new EditDateButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Во сколько")
                    {
                        userState.TypeButton = TypeButton.EditTimeButton;
                        _userRepository.Update(userState);

                        var button = new EditTimeButton();
                        return button.GetResponse();
                    }
                }

                if (userState.TypeButton == TypeButton.EditFromButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.FromString = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message = 
                        $"{trip.FromString} - {trip.ToToString}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.ToToString = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.FromString} - {trip.ToToString}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    
                    trip.DateTime = new EditDateButton().Convert(buttonName);
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.FromString} - {trip.ToToString}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditTimeButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.DateTime = new EditTimeButton().Convert((DateTime)trip.DateTime, buttonName);
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.FromString} - {trip.ToToString}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }
            }

            if (userState.TypeTransaction == TypeTransaction.Add)
            {
                if (userState.TypeButton == TypeButton.AddMenuButton)
                {
                    if (buttonName == "Откуда")
                    {
                        userState.TypeButton = TypeButton.EditFromButton;
                        _userRepository.Update(userState);

                        var button = new EditFromButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Куда")
                    {
                        userState.TypeButton = TypeButton.EditToButton;
                        _userRepository.Update(userState);

                        var button = new EditFromButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Когда")
                    {
                        userState.TypeButton = TypeButton.EditDateButton;
                        _userRepository.Update(userState);

                        var button = new EditDateButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Во сколько")
                    {
                        userState.TypeButton = TypeButton.EditTimeButton;
                        _userRepository.Update(userState);

                        var button = new EditTimeButton();
                        return button.GetResponse();
                    }

                    if (buttonName == "Телефон")
                    {
                        userState.TypeButton = TypeButton.EditPhoneButton;
                        _userRepository.Update(userState);

                        return new ResponseModel
                        {
                            Message = "Укажите номер"
                        };
                    }

                    if (buttonName == "Комментарии")
                    {
                        userState.TypeButton = TypeButton.EditDescriptionButton;
                        _userRepository.Update(userState);

                        return new ResponseModel
                        {
                            Message = "Укажите комментарии"
                        };
                    }

                    if (buttonName == "Готово")
                    {
                        trip = _tripRepository.Get(userState.TripId);
                        trip.IsPublished = true;

                        _tripRepository.Update(trip);

                        var s = new OptionKeyboard();
                        return s.Get();
                    }
                }

                if (userState.TypeButton == TypeButton.EditFromButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.FromString = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.FromString) ? "?" : trip.FromString) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.ToToString) ? "?" : trip.ToToString) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон : " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Description}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.ToToString = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.FromString) ? "?" : trip.FromString) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.ToToString) ? "?" : trip.ToToString) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Description}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditTimeButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    var time = TimeSpan.Parse(buttonName);
                    if (trip.DateTime == null)
                    {
                        trip.DateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Hours, time.Minutes, 0);
                    }
                    else
                    {
                        trip.DateTime = new DateTime(((DateTime)trip.DateTime).Year, ((DateTime)trip.DateTime).Month, ((DateTime)trip.DateTime).Day, time.Hours, time.Minutes, 0);
                    }
                    
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.FromString) ? "?" : trip.FromString) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.ToToString) ? "?" : trip.ToToString) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Description}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    var now = default(DateTime);
                    if (buttonName == "Сегодня")
                    {
                        now = DateTime.UtcNow;
                    }
                    if (buttonName == "Завтра")
                    {
                        now = DateTime.UtcNow.AddDays(1);
                    }

                    if (trip.DateTime == null)
                    {
                        trip.DateTime = now;
                    }
                    else
                    {
                        var time = (DateTime)trip.DateTime;
                        trip.DateTime = new DateTime(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0);
                    }

                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.FromString) ? "?" : trip.FromString) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.ToToString) ? "?" : trip.ToToString) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Description}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditPhoneButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.Phone = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.FromString) ? "?" : trip.FromString) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.ToToString) ? "?" : trip.ToToString) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Description}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditPhoneButton)
                {
                    trip = _tripRepository.Get(userState.TripId);
                    trip.Description = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.FromString) ? "?" : trip.FromString) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.ToToString) ? "?" : trip.ToToString) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Description}";

                    return result;
                }
            }

            if (trip != null)
            {
                if (trip.TypeTransaction == MVC.Data.Models.TypeTransaction.Search)
                {
                    if (buttonName == "Добавить поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                        return AddTrip(buttonName, accountId);
                    }

                    if (buttonName == "Найти поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                        return SearchTrips(buttonName, accountId);
                    }

                    return SearchTrips(buttonName, accountId, trip);
                }
                else
                {
                    if (buttonName == "Найти поездку")
                    {
                        _tripRepository.Delete(trip.TripId);
                    }

                    return AddTrip(buttonName, accountId, trip);
                }
            }
            else
            {
                if (buttonName == "Добавить поездку")
                {
                    return AddTrip(buttonName, accountId);
                }
                if (buttonName == "Найти поездку")
                {
                    return SearchTrips(buttonName, accountId);
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
                    TypeTransaction = MVC.Data.Models.TypeTransaction.Add
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

        //private ResponseModel GetKeyboardMenuEdit(int accountVkontakteId)
        //{
        //    var trips = _tripRepository.Get(accountVkontakteId, true);
        //    if (trips == null || trips.Count() == 0)
        //    {
        //        return new ResponseModel
        //        {
        //            Message = "У вас нет объявлений"
        //        };
        //    }

        //    var listButtons = new List<Button[]>();
        //    var tripsMessage = new List<string>();

        //    int step = -1;

        //    foreach(var entry in trips)
        //    {
        //        step++;
        //        var buttons = new List<Button>
        //        {
        //            new Button
        //            {
        //                color = "default",
        //                action = new ResourceModels.Action
        //                {
        //                    label = $"Объявление {step}",
        //                    type = "text",
        //                    payload = JsonConvert.SerializeObject(new
        //                    {
        //                        button = "1"
        //                    })
        //                }
        //            },
        //            new Button
        //            {
        //                color = "negative",
        //                action = new ResourceModels.Action
        //                {
        //                    label = "Удалить",
        //                    type = "text",
        //                    payload = JsonConvert.SerializeObject(new
        //                    {
        //                        button = "2"
        //                    })
        //                }
        //            }
        //        };

        //        listButtons.Add(buttons.ToArray());
        //        tripsMessage.Add($"ИД: {step}\r\n" +
        //            $"{entry.FromString} - {entry.ToToString}\r\n" +
        //            $"{((DateTime)entry.DateTime).ToString("dd.MM.yyyy")}\r\n" +
        //            $"{((DateTime)entry.DateTime).ToString("hh:mm")}");
        //    }

        //    listButtons.Add(new List<Button>
        //    {
        //        new Button
        //        {
        //            color = "default",
        //            action = new ResourceModels.Action
        //            {
        //                label = "Перейти на начало",
        //                type = "text",
        //                payload = JsonConvert.SerializeObject(new
        //                {
        //                    button = "3"
        //                })
        //            }
        //        }
        //    }.ToArray());

        //    var keyboard = new Keyboard
        //    {
        //        OneTime = false,
        //        buttons = listButtons.ToArray()
        //    };

        //    return new ResponseModel
        //    {
        //        Message =string.Join("\r\n\r\n", tripsMessage),
        //        Keyboard = keyboard
        //    };
        //}

        //private ResponseModel GetKeyboardEdit(string buttonValue, int accountVkontakteId)
        //{

        //}
    }
}
