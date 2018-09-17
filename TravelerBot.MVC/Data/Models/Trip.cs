using System;
using TravelerBot.MVC.Data.Models;

namespace TravelerBot.MVC.Models
{
    public class Trip
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid TripId { get; set; }

        /// <summary>
        ///  Тип участника.
        /// </summary>
        public TypeParticipant TypeParticipant { get; set; }

        /// <summary>
        /// Откуда.
        /// </summary>
        public string Whence { get; set; }

        /// <summary>
        /// Куда.
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// Дата и время выезда.
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// Опубликовано ли объявление?
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Комментарии.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public Guid UserStateId { get; set; }

        public virtual UserState UserState { get; set; }
    }
}