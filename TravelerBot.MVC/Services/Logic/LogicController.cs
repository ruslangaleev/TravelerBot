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

        private readonly ISearchRepository _searchRepository;

        public LogicController(ITripRepository tripRepository, IUserRepository userRepository,
            ISearchRepository searchRepository)
        {
            _tripRepository = tripRepository;
            _userRepository = userRepository;
            _searchRepository = searchRepository;
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
                foreach (var entry in trips.Select(t => t.TripId))
                {
                    _tripRepository.Delete(entry);
                }

                trip = new Trip
                {
                    TripId = Guid.NewGuid(),
                    TypeParticipant = TypeParticipant.Driver,
                    AccountId = accountId
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
                var search = _searchRepository.Get(userState.SearchOptionId);
                if (search != null)
                {
                    _searchRepository.Delete(search.SearchOptionsId);
                }
                search = new SearchOptions
                {
                    SearchOptionsId = Guid.NewGuid()
                };
                _searchRepository.Add(search);
                _searchRepository.SaveChanges();

                userState.TypeButton = TypeButton.AddMenuButton;
                userState.SearchOptionId = search.SearchOptionsId;
                _userRepository.Update(userState);

                var button = new SearchMenuButton();
                return button.GetResponse();
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
                        //trip = _tripRepository.Get(userState.TripId);
                        //trip.IsPublished = true;

                        //_tripRepository.Update(trip);

                        //var s = new OptionKeyboard();
                        //return s.Get();
                        var search = _searchRepository.Get(userState.SearchOptionId);
                        var tripOptionDess = JsonConvert.DeserializeObject<Trip>(search.Filter);

                        trips = _tripRepository.Get(tripOptionDess.FromString, tripOptionDess.ToToString, tripOptionDess.DateTime);

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
                }

                var searchOption = _searchRepository.Get(userState.SearchOptionId);
                if (string.IsNullOrEmpty(searchOption.Filter))
                {
                    trip = new Trip();

                    searchOption.Filter = JsonConvert.SerializeObject(trip);
                }

                var tripOptionDes = JsonConvert.DeserializeObject<Trip>(searchOption.Filter);
                if (userState.TypeButton == TypeButton.EditFromButton)
                {
                    tripOptionDes.FromString = buttonName;
                }
                if (userState.TypeButton == TypeButton.EditToButton)
                {
                    tripOptionDes.ToToString = buttonName;
                }
                if (userState.TypeButton == TypeButton.EditDateButton)
                {
                    tripOptionDes.DateTime = (buttonName == "Сегодня") ? DateTime.Now : DateTime.Now.AddDays(1);
                }

                searchOption.Filter = JsonConvert.SerializeObject(tripOptionDes);
                _searchRepository.Update(searchOption);
                _searchRepository.SaveChanges();

                userState.TypeButton = TypeButton.AddMenuButton;
                _userRepository.Update(userState);

                var newbutton = new SearchMenuButton();
                return newbutton.GetResponse();
            }

            return null;
        }
    }
}
