namespace DataPlusWeb.Components.Account.Services
{
    public class AuthService : IAuthService
    {
        #region Public methods reigon

        public async Task<ApplicationUser> FindByUser(string username, string password)
        {
            return new ApplicationUser { UserName = "admin" };
        }

        #endregion
    }
}
