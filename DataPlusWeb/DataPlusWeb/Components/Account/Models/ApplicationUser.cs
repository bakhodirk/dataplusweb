using System.Diagnostics.CodeAnalysis;

namespace DataPlusWeb.Components.Account;

public class ApplicationUser
{
    [AllowNull]
    public string UserName { get; set; }
}
