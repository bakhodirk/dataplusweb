namespace DataPlus.Web.UI;

public sealed class ContextManager
{
    #region Private fields region

    private readonly object _context;
    private TypeAccessor? _accessor;

    #endregion

    #region Internal constructors region

    internal ContextManager(object context)
        : base()
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #endregion

    #region Public methods region

    /// <summary>
    /// Gets members which defined sepcifeid attribute.
    /// </summary>
    /// <param name="attributeType">The type of attribute to filter.</param>
    public IEnumerable<MemberAccessor> GetMembers(Type? attributeType) => attributeType == null ? Accessor.Members : Accessor.GetMembers(attributeType);

    /// <summary>
    /// Gets members which defined sepcifeid attribute.
    /// </summary>
    /// <param name="type">The type accessor to select members.</param>
    /// <typeparam name="TAttribute">The type of attribute to filter.</typeparam>
    public IEnumerable<MemberAccessor> GetMembers<TAttribute>() where TAttribute : Attribute => GetMembers(typeof(TAttribute));

    /// <summary>
    /// Gets actions which defined sepcifeid attribute.
    /// </summary>
    /// <param name="attributeType">The type of attribute to filter.</param>
    public IEnumerable<ActionAccessor> GetActions(Type? attributeType) => attributeType == null ? Accessor.Actions : Accessor.GetActions(attributeType);

    /// <summary>
    /// Gets actions which defined sepcifeid attribute.
    /// </summary>
    /// <param name="type">The type accessor to select members.</param>
    /// <typeparam name="TAttribute">The type of attribute to filter.</typeparam>
    public IEnumerable<ActionAccessor> GetActions<TAttribute>(TypeAccessor type) where TAttribute : Attribute => GetActions(typeof(TAttribute));

    #endregion

    #region Public properties region

    /// <summary>
    /// Gets the instance of the context.
    /// </summary>
    public object Context => _context;

    /// <summary>
    /// Gets the accessor to the context.
    /// </summary>
    public TypeAccessor Accessor { get => _accessor ??= AccessorHelper.GetTypeAccessor(_context.GetType()); }

    #endregion
}
