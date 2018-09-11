using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelerBot.MVC.Models
{
    public class Trip
    {
        public Guid TripId { get; set; }

        /// <summary>
        ///  Тип участника.
        /// </summary>
        public TypeParticipant TypeParticipant { get; set; }

        public bool From { get; set; }

        public string FromString { get; set; }

        public bool To { get; set; }

        public string ToToString { get; set; }

        public bool Date { get; set; }

        public bool Time { get; set; }

        public DateTime? DateTime { get; set; }

        public TimeSpan? TimeSpan { get; set; }

        public int AccountId { get; set; }

        /// <summary>
        /// Опубликовано ли объявление?
        /// </summary>
        public bool IsPublished { get; set; }
    }
}