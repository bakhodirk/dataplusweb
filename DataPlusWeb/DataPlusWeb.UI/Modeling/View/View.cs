
namespace DataPlus.Web.UI.Components
{
    public sealed class View : TypedBase
    {
        #region Public constructors region

        public View()
            : base(typeof(ViewComponent<>), typeof(ViewComponent))
        { }

        #endregion
    }
}
