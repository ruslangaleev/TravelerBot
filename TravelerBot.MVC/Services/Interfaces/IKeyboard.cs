using TravelerBot.Api.ResourceModels;

namespace TravelerBot.Api.Services.Interfaces
{
    public interface IKeyboard
    {
        ResponseModel Get();

        ResponseModel Get(string buttonName);

        ResponseModel Get(InboundButton[] inboundButtons);
    }
}
