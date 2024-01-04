using DataPlus.Auth.API.Contracts.Responses;
using DataPlusWeb.Shared.Models;

namespace DataPlusWeb.Client.Services
{
    public interface IUserService
    {
        Task<AuthenticateAPIResponse> LoginUser(LoginModel loginModel);
    }
}
