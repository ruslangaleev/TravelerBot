using Moq;
using NUnit.Framework;
using System;
using TravelerBot.Api.Data.Models;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.Services.Logic;

namespace TravelerBot.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void ReturnsTypeParticipantKeyboard()
        {
            var tripRepository = new Mock<ITripRepository>();

            var logicController = new LogicController(tripRepository.Object);

            var result = logicController.Get("Начать", 123456);

            Assert.AreEqual(2, result.Keyboard.Buttons[0].Length);
            Assert.AreEqual(1, result.Keyboard.Buttons[1].Length);
            Assert.AreEqual("Найти поездку", result.Keyboard.Buttons[0][0].Action.Label);
            Assert.AreEqual("Добавить поездку", result.Keyboard.Buttons[0][1].Action.Label);
            Assert.AreEqual("Перейти на начало", result.Keyboard.Buttons[1][0].Action.Label);
        }

        [Test]
        public void ReturnsMenuKeyboard()
        {
            Trip trip = null;

            var tripRepository = new Mock<ITripRepository>();
            tripRepository.Setup(t => t.Add(It.IsAny<Trip>())).Callback((Trip t) =>
            {
                trip = t;
            });

            var logicController = new LogicController(tripRepository.Object);

            var result = logicController.Get("Водитель", 123456);

            // Воидтель
            Assert.AreEqual(2, result.Keyboard.Buttons[0].Length);
            // Откуда или куда
            Assert.AreEqual(2, result.Keyboard.Buttons[1].Length);
            //Когда и во сколько
            Assert.AreEqual(2, result.Keyboard.Buttons[2].Length);
            // На начало
            Assert.AreEqual(2, result.Keyboard.Buttons[3].Length);

            // Водитель
            Assert.AreEqual("positive", result.Keyboard.Buttons[0][0].Color);

            Assert.IsNotNull(trip);
        }

        [Test]
        public void ReturnsDate()
        {
            Trip trip = new Trip
            {
                Date = true,
                TypeParticipant = TypeParticipant.Driver,
                AccountId = 123456
            };

            var tripRepository = new Mock<ITripRepository>();
            tripRepository.Setup(t => t.Get(It.IsAny<int>())).ReturnsAsync(trip);
            tripRepository.Setup(t => t.Update(It.IsAny<Trip>())).Callback((Trip param) =>
            {
                trip = param;
            });

            var logicController = new LogicController(tripRepository.Object);

            var result = logicController.Get("Сегодня", 123456);

            // Воидтель
            Assert.AreEqual(2, result.Keyboard.Buttons[0].Length);
            // Откуда или куда
            Assert.AreEqual(2, result.Keyboard.Buttons[1].Length);
            //Когда и во сколько
            Assert.AreEqual(2, result.Keyboard.Buttons[2].Length);
            // На начало
            Assert.AreEqual(2, result.Keyboard.Buttons[3].Length);

            // Водитель
            Assert.AreEqual("positive", result.Keyboard.Buttons[0][0].Color);

            // Когда
            Assert.AreEqual("positive", result.Keyboard.Buttons[2][0].Color);

            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            Assert.AreEqual(date, trip.DateTime);
        }
    }
}
