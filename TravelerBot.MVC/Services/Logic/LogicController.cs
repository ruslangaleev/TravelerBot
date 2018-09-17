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
            var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);

            // Будет выводить список существующих поездок
            if (buttonName == "Мои поездки")
            {
                var trips = _tripRepository.GetTripsByUserStateId(userState.UserStateId);
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
                trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                if (trip != null)
                {
                    _tripRepository.Delete(trip.TripId);
                }

                trip = new Trip
                {
                    TripId = Guid.NewGuid(),
                    TypeParticipant = TypeParticipant.Driver,
                    UserStateId = userState.UserStateId
                };

                _tripRepository.Add(trip);

                _tripRepository.SaveChanges();

                userState.TypeTransaction = TypeTransaction.Add;
                userState.TypeButton = TypeButton.AddMenuButton;
                _userRepository.Update(userState);

                var button = new AddMenuButton();
                return button.GetResponse();
            }

            if (buttonName == "Найти поездку")
            {
                userState.TypeTransaction = TypeTransaction.Search;
                userState.TypeButton = TypeButton.AddMenuButton;
                userState.Filter = null;
                _userRepository.Update(userState);

                var button = new SearchMenuButton();
                return button.GetResponse();
            }

            // Будет выводит меню для существующей поездки
            if (userState.TypeTransaction == TypeTransaction.Edit)
            {
                if (userState.TypeButton == TypeButton.EditButton)
                {
                    var trips = _tripRepository.GetTripsByUserStateId(userState.UserStateId);
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

                        trips = _tripRepository.GetTripsByUserStateId(userState.UserStateId);
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
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    trip.Whence = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    trip.Where = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);

                    trip.DateTime = new EditDateButton().Convert(buttonName);
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"{((DateTime)trip.DateTime).ToString("hh:mm")}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditTimeButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    trip.DateTime = new EditTimeButton().Convert((DateTime)trip.DateTime, buttonName);
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
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
                        trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                        if (string.IsNullOrEmpty(trip.Whence) || string.IsNullOrEmpty(trip.Where)
                            || string.IsNullOrEmpty(trip.Phone) || trip.DateTime == null)
                        {
                            return new ResponseModel
                            {
                                Message = "Необходимо указать все пункты"
                            };
                        }

                        trip.IsPublished = true;
                        _tripRepository.Update(trip);

                        var s = new OptionKeyboard();
                        return s.Get();
                    }
                }

                if (userState.TypeButton == TypeButton.EditFromButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    trip.Whence = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.Whence) ? "?" : trip.Whence) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.Where) ? "?" : trip.Where) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон : " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.TripId);
                    trip.Where = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.Whence) ? "?" : trip.Whence) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.Where) ? "?" : trip.Where) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditTimeButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                        "Откуда: " + (string.IsNullOrEmpty(trip.Whence) ? "?" : trip.Whence) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.Where) ? "?" : trip.Where) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                        "Откуда: " + (string.IsNullOrEmpty(trip.Whence) ? "?" : trip.Whence) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.Where) ? "?" : trip.Where) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditPhoneButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    trip.Phone = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.Whence) ? "?" : trip.Whence) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.Where) ? "?" : trip.Where) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditPhoneButton)
                {
                    trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    trip.Comments = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.AddMenuButton;
                    _userRepository.Update(userState);

                    var button = new AddMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        "Откуда: " + (string.IsNullOrEmpty(trip.Whence) ? "?" : trip.Whence) + "\r\n" +
                        "Куда: " + (string.IsNullOrEmpty(trip.Where) ? "?" : trip.Where) + "\r\n" +
                        "Когда: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("dd.MM.yyyy")) + "\r\n" +
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("hh:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }
            }

            if (userState.TypeTransaction == TypeTransaction.Search)
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

                    if (buttonName == "Искать")
                    {
                        
                        var tripOptionDess = JsonConvert.DeserializeObject<Trip>(userState.Filter);

                        var trips = _tripRepository.Get(tripOptionDess.Whence, tripOptionDess.Where, tripOptionDess.DateTime);

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
                            $"https://vk.com/id{t.UserState.AccountId}\r\n" +
                            $"{t.Whence} - {t.Where}\r\n" +
                            $"{((DateTime)t.DateTime).ToString("dd.MM.yyyy")}" +
                            $"{((DateTime)t.DateTime).ToString("hh:mm")}";
                        });

                        var message = string.Join("\r\n\r\n", tripsAsString);

                        return new ResponseModel
                        {
                            Message = message
                        };
                    }
                }

                //var searchOption = _searchRepository.Get(userState.SearchOptionId);
                if (string.IsNullOrEmpty(userState.Filter))
                {
                    trip = new Trip();

                    userState.Filter = JsonConvert.SerializeObject(trip);
                }

                var tripOptionDes = JsonConvert.DeserializeObject<Trip>(userState.Filter);
                if (userState.TypeButton == TypeButton.EditFromButton)
                {
                    tripOptionDes.Whence = buttonName;
                }
                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    tripOptionDes.Where = buttonName;
                }
                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    tripOptionDes.DateTime = (buttonName == "Сегодня") ? DateTime.Now : DateTime.Now.AddDays(1);
                }

                userState.Filter = JsonConvert.SerializeObject(tripOptionDes);
                _userRepository.Update(userState);

                userState.TypeButton = TypeButton.AddMenuButton;
                _userRepository.Update(userState);

                var newbutton = new SearchMenuButton();
                return newbutton.GetResponse();
            }

            return null;
        }
    }
}
