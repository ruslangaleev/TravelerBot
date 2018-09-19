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
                var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                if (trip != null)
                {
                    _tripRepository.Delete(trip.TripId);
                }

                var trips = _tripRepository
                    .GetTripsByUserStateId(userState.UserStateId)
                    .Where(t => t.DateTime >= DateTime.Now);

                if (trips.Count() > 2)
                {
                    return new ResponseModel
                    {
                        Message = "Пока нельзя публиковать более 2 поездок. Сори=("
                    };
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
            
            if (userState.TypeTransaction == TypeTransaction.Edit)
            {
                if (userState.TypeButton == TypeButton.EditButton)
                {
                    var trips = _tripRepository.GetTripsByUserStateId(userState.UserStateId);
                    if (buttonName.Length == 10)
                    {
                        var value = buttonName.Substring(9, 1);
                        int number = 0;
                        if (!int.TryParse(value, out number))
                        {
                            return new ResponseModel
                            {
                                Message = "Команда не распознана"
                            };
                        }
                        var trip = trips.ToArray()[number - 1];

                        userState.TripId = trip.TripId;

                        // Выводим список объявлений
                        userState.TypeButton = TypeButton.EditMenuButton;
                        _userRepository.Update(userState);

                        var button = new EditMenuButton();
                        return button.GetResponse();
                    }
                    if (buttonName.Length == 9)
                    {
                        var value = buttonName.Substring(8, 1);
                        int number = 0;
                        if (!int.TryParse(value, out number))
                        {
                            return new ResponseModel
                            {
                                Message = "Команда не распознана"
                            };
                        }
                        var trip = trips.ToArray()[number - 1];

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

                    if (buttonName == "Телефон")
                    {
                        userState.TypeButton = TypeButton.EditPhoneButton;
                        _userRepository.Update(userState);

                        var button = new EditTimeButton();
                        var result = button.GetResponse();
                        result.Message = "Укажите номер телефона";
                    }

                    if (buttonName == "Комментарии")
                    {
                        userState.TypeButton = TypeButton.EditDescriptionButton;
                        _userRepository.Update(userState);

                        var button = new EditTimeButton();
                        var result = button.GetResponse();
                        result.Message = "Укажите комментарии";
                    }
                }

                if (userState.TypeButton == TypeButton.EditFromButton)
                {
                    var trip = _tripRepository.GetTrip(userState.TripId);
                    trip.Whence = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"Когда: {((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"Во сколько: {((DateTime)trip.DateTime).ToString("HH:mm")}\r\n" +
                        $"Телефон: {trip.Phone}\r\n" +
                        ((string.IsNullOrEmpty(trip.Comments)) ? null : $"Комментарии: {trip.Comments}");

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    var trip = _tripRepository.GetTrip(userState.TripId);
                    trip.Where = buttonName;
                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"Когда: {((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"Во сколько: {((DateTime)trip.DateTime).ToString("HH:mm")}\r\n" +
                        $"Телефон: {trip.Phone}\r\n" +
                        ((string.IsNullOrEmpty(trip.Comments)) ? null : $"Комментарии: {trip.Comments}");

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    var trip = _tripRepository.GetTrip(userState.TripId);
                    
                    if (buttonName == "Сегодня")
                    {
                        var now = DateTime.UtcNow.AddHours(5);
                        var time = (DateTime)trip.DateTime;
                        trip.DateTime = new DateTime(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0);
                    }
                    else
                    if (buttonName == "Завтра")
                    {
                        var now = DateTime.UtcNow.AddHours(5).AddDays(1);
                        var time = (DateTime)trip.DateTime;
                        trip.DateTime = new DateTime(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0);
                    }
                    else
                    {
                        return new ResponseModel
                        {
                            Message = "Команда не распознана"
                        };
                    }

                     new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0).AddDays(1);

                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"Когда: {((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"Во сколько: {((DateTime)trip.DateTime).ToString("HH:mm")}\r\n" +
                        $"Телефон: {trip.Phone}\r\n" +
                        ((string.IsNullOrEmpty(trip.Comments)) ? null : $"Комментарии: {trip.Comments}");

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditTimeButton)
                {
                    var trip = _tripRepository.GetTrip(userState.TripId);
                    var timeSpan = TimeSpan.Parse(buttonName);
                    var date = (DateTime)trip.DateTime;
                    trip.DateTime = new DateTime(date.Year, date.Month, date.Day, timeSpan.Hours, timeSpan.Minutes, 0);

                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"Когда: {((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"Во сколько: {((DateTime)trip.DateTime).ToString("HH:mm")}\r\n" +
                        $"Телефон: {trip.Phone}\r\n" +
                        ((string.IsNullOrEmpty(trip.Comments)) ? null : $"Комментарии: {trip.Comments}");

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditPhoneButton)
                {
                    var trip = _tripRepository.GetTrip(userState.TripId);
                    trip.Phone = buttonName;

                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"Когда: {((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"Во сколько: {((DateTime)trip.DateTime).ToString("HH:mm")}\r\n" +
                        $"Телефон: {trip.Phone}\r\n" +
                        ((string.IsNullOrEmpty(trip.Comments)) ? null : $"Комментарии: {trip.Comments}");

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDescriptionButton)
                {
                    var trip = _tripRepository.GetTrip(userState.TripId);
                    trip.Comments = buttonName;

                    _tripRepository.Update(trip);

                    userState.TypeButton = TypeButton.EditMenuButton;
                    _userRepository.Update(userState);

                    var button = new EditMenuButton();
                    var result = button.GetResponse();
                    result.Message =
                        $"{trip.Whence} - {trip.Where}\r\n" +
                        $"Когда: {((DateTime)trip.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                        $"Во сколько: {((DateTime)trip.DateTime).ToString("HH:mm")}\r\n" +
                        $"Телефон: {trip.Phone}\r\n" +
                        ((string.IsNullOrEmpty(trip.Comments)) ? null : $"Комментарии: {trip.Comments}");

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
                        var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                    var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("HH:mm")) + "\r\n" +
                        "Телефон : " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("HH:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditTimeButton)
                {
                    var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    var time = TimeSpan.Parse(buttonName);
                    if (trip.DateTime == null)
                    {
                        var now = DateTime.UtcNow.AddHours(5);
                        var date = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, 0);
                        if (now > date)
                        {
                            date = date.AddDays(1);
                        }

                        trip.DateTime = date;
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
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("HH:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
                    var now = default(DateTime);
                    if (buttonName == "Сегодня")
                    {
                        now = DateTime.UtcNow.AddHours(5);
                    }
                    if (buttonName == "Завтра")
                    {
                        now = DateTime.UtcNow.AddDays(1).AddHours(5);
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
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("HH:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditPhoneButton)
                {
                    var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("HH:mm")) + "\r\n" +
                        "Телефон: " + (string.IsNullOrEmpty(trip.Phone) ? "?" : trip.Phone) + "\r\n" +
                        $"Комментарии {trip.Comments}";

                    return result;
                }

                if (userState.TypeButton == TypeButton.EditDescriptionButton)
                {
                    var trip = _tripRepository.GetTripByUserStateId(userState.UserStateId);
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
                        "Во сколько: " + (trip.DateTime == null ? "?" : ((DateTime)trip.DateTime).ToString("HH:mm")) + "\r\n" +
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
                        if (string.IsNullOrEmpty(userState.Filter))
                        {
                            return new ResponseModel
                            {
                                Message = "Необходимо указать хотя бы один пункт"
                            };
                        }

                        var tripOptionDess = JsonConvert.DeserializeObject<Trip>(userState.Filter);

                        var trips = _tripRepository.Get(tripOptionDess.Whence, tripOptionDess.Where, tripOptionDess.DateTime);

                        if (trips.Count() == 0)
                        {
                            return new ResponseModel
                            {
                                Message = "Объявлений не найдено"
                            };
                        }

                        var result = new List<string>();

                        foreach (var entry in trips)
                        {
                            result.Add($"{entry.Whence} - {entry.Where}\r\n" +
                                $"Когда: {((DateTime)entry.DateTime).ToString("dd.MM.yyyy")}\r\n" +
                                $"Во сколько: {((DateTime)entry.DateTime).ToString("HH:mm")}\r\n" +
                                $"Телефон: {entry.Phone}\r\n" +
                                $"Страница: https://vk.com/id{entry.UserState.AccountId}\r\n" +
                                ((string.IsNullOrEmpty(entry.Comments)) ? null : $"Комментарии {entry.Comments}"));
                        }

                        var message = string.Join("\r\n\r\n", result);

                        return new ResponseModel
                        {
                            Message = message
                        };
                    }
                }
                
                if (string.IsNullOrEmpty(userState.Filter))
                {
                    var trip = new Trip();

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

            return new ResponseModel
            {
                Message = "Команда не распознана"
            };
        }
    }
}
