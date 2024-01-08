using DataPlus.API.Contracts.Models;
using DataPlus.API.Contracts.Requests;
using DataPlus.API.Contracts.Responses;

namespace DataPlusWeb.Shared.Services
{
    public interface IDataPlusService
    {
        Task<GetPractitionerListAPIResponse> GetPractitionerList(GetPractitionerListAPIRequest request);

        Task<GetPractitionerAPIResponse> GetPractitioner(long practitionId);

        Task<BaseUpdateAPIResponse<Practitioner>> CreateOrUpdatePractitioner(BaseAPIRequest<Practitioner> request);

        Task<DeletePractitionerAPIResponse> DeletePractitioner(long practitionId, byte[] versionStamp);
    }
}
