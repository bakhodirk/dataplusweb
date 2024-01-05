namespace DataPlusWeb.Components.Account.Services;

public interface IAuthService
{
    #region Public methods reigon

    public Task<ApplicationUser> FindByUser(string username, string password);

    #endregion
}
