using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TravelerBot.Api.Services.Logic;
using TravelerBot.MVC.Data.Repositories;
using TravelerBot.MVC.Data.Repositories.Logic;
using TravelerBot.MVC.ResourceModels;

namespace TravelerBot.MVC.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly string _secret = "q11w22e33r44";

        private readonly string _token = "195b278ff87d740cd559f757783973b80a545b828dd9ca102f9e100c5593290afb30cc38e00e6a714f06a";

        private readonly static HttpClient _httpClient = new HttpClient();

        // POST api/values
        public async Task<object> Post([FromBody]Message message)
        {
            if (message.Type == "confirmation")
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("6bc74ded", Encoding.UTF8, "text/plain");
                return response;
            }

            if (message.Secret != _secret)
            {
                return BadRequest("Не верный секретный ключ.");
            }

            if (message.Type == "message_new")
            {
                var tripRepository = new TripRepository();
                var userRepository = new UserRepository();

                var logicController = new LogicController(tripRepository, userRepository);

                var responseModel = logicController.Get(message.ObjectMessage.Body, message.ObjectMessage.UserId);

                //string responseMessage = "Привет, я бот. Чем могу помочь?";

                var request = string.Empty;
                if (responseModel.Keyboard == null && !string.IsNullOrEmpty(responseModel.Message))
                {
                    request = $"https://api.vk.com/method/messages.send?user_id={message.ObjectMessage.UserId}&group_id={message.GroupId}&message={responseModel.Message}&v=5.80&access_token={_token}";
                }
                if (responseModel.Keyboard != null && string.IsNullOrEmpty(responseModel.Message))
                {
                    request = $"https://api.vk.com/method/messages.send?user_id={message.ObjectMessage.UserId}&group_id={message.GroupId}&keyboard={JsonConvert.SerializeObject(responseModel.Keyboard)}&v=5.80&access_token={_token}";
                }
                if (responseModel.Keyboard != null && !string.IsNullOrEmpty(responseModel.Message))
                {
                    request = $"https://api.vk.com/method/messages.send?user_id={message.ObjectMessage.UserId}&group_id={message.GroupId}&message={responseModel.Message}&keyboard={JsonConvert.SerializeObject(responseModel.Keyboard)}&v=5.80&access_token={_token}";
                }


                await _httpClient.GetAsync(request);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StringContent("ok", Encoding.UTF8, "text/plain");
                return response;
            }

            return BadRequest("тип события не распознан.");
        }
    }
}
