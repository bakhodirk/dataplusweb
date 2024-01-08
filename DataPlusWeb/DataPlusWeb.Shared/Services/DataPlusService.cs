using DataPlus.API.Contracts.Models;
using DataPlus.API.Contracts.Requests;
using DataPlus.API.Contracts.Responses;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace DataPlusWeb.Shared.Services
{
    public class DataPlusService(IHttpClientFactory httpClientFactory, ILogger<DataPlusService> logger) : IDataPlusService
    {
        public async Task<GetPractitionerListAPIResponse> GetPractitionerList(GetPractitionerListAPIRequest request)
        {
            using var httpClient = httpClientFactory.CreateClient("DataPlusAPI");
            try
            {
                var response = await httpClient.PostAsJsonAsync("/GetPractitionerList", request);
                var result = await response.Content.ReadFromJsonAsync<GetPractitionerListAPIResponse>();

                return result!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<GetPractitionerAPIResponse> GetPractitioner(long practitionId)
        {
            using var httpClient = httpClientFactory.CreateClient("DataPlusAPI");
            try
            {
                var response = await httpClient.GetAsync($"/GetPractitioner?id={practitionId}");
                var result = await response.Content.ReadFromJsonAsync<GetPractitionerAPIResponse>();

                return result!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<BaseUpdateAPIResponse<Practitioner>> CreateOrUpdatePractitioner(BaseAPIRequest<Practitioner> request)
        {
            using var httpClient = httpClientFactory.CreateClient("DataPlusAPI");
            try
            {
                if (request.Record.Id == 0)
                {
                    var response = await httpClient.PostAsJsonAsync("/CreatePractitioner", request);
                    var result = await response.Content.ReadFromJsonAsync<CreatePractitionerAPIResponse>();

                    return result!;
                }
                else
                {
                    var response = await httpClient.PutAsJsonAsync("/UpdatePractitioner", request);
                    var result = await response.Content.ReadFromJsonAsync<UpdatePractitionerAPIResponse>();

                    return result!;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<DeletePractitionerAPIResponse> DeletePractitioner(long practitionId, byte[] versionStamp)
        {
            using var httpClient = httpClientFactory.CreateClient("DataPlusAPI");
            try
            {
                var response = await httpClient.DeleteAsync($"/DeletePractitioner?id={practitionId}");
                var result = await response.Content.ReadFromJsonAsync<DeletePractitionerAPIResponse>();

                return result!;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
