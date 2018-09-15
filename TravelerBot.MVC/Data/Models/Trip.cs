using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TravelerBot.MVC.Data.Models;

namespace TravelerBot.MVC.Models
{
    public class Trip
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key]
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

        /// <summary>
        /// Какое действие сейчас выполняется?
        /// </summary>
        public TypeTransaction TypeTransaction { get; set; }

        /// <summary>
        /// Характерно для редактирования. Указываем какое объявление редактируется.
        /// </summary>
        public Guid EditTripId { get; set; }

        public string Phone { get; set; }

        public string Description { get; set; }
    }
}