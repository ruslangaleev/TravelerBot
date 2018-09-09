using Moq;
using NUnit.Framework;
using System;
using TravelerBot.Api.Data.Repositories;
using TravelerBot.Api.Services.Logic;

namespace TravelerBot.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Unknown()
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
    }
}
