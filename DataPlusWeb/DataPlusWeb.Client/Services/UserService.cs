using DataPlus.Auth.API.Contracts.Requests;
using DataPlus.Auth.API.Contracts.Responses;
using DataPlusWeb.Client.Provider;
using DataPlusWeb.Shared.Models;
using System.Net.Http.Json;

namespace DataPlusWeb.Client.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserService( IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<AuthenticateAPIResponse> LoginUser(LoginModel loginModel)
        {
            var httpClient = httpClientFactory.CreateClient("AuthAPI");
            var request = new AuthenticateAPIRequest();
            var result = new AuthenticateAPIResponse();

            try
            {
                request.Record = new DataPlus.Auth.API.Contracts.Models.User()
                {
                    Username = loginModel.Username,
                    Password = loginModel.Password
                };

                var response = await httpClient.PostAsJsonAsync("Auth", request);
                result = await response.Content.ReadFromJsonAsync<AuthenticateAPIResponse>();
            }
            catch (Exception ex)
            {

                throw;
            }
            
            return result!;
        }
    }
}
