using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataPlus.Web.UI
{
    public class TypeAccessor : AccessorBase
    {
        #region Private field region

        private readonly Dictionary<string, MemberAccessor> _members = new();
        private readonly Dictionary<string, ActionAccessor> _actions = new();
        private readonly ConcurrentDictionary<string, AttributeCollection> _customMemberAttributes = new();

        #endregion

        #region Private methods region

        private void Initialize(Type type)
        {
            foreach (Attribute attr in type.GetCustomAttributes(true))
            {
                switch (attr)
                {
                    case DisplayAttribute displayAttr:
                        DisplayAttribute ??= displayAttr;
                        break;

                    case DataTypeAttribute dataTypeAttr:
                        DataTypeAttribute ??= dataTypeAttr;
                        break;

                    case ValidationsAttribute validationsAttr:
                        ValidationsAttribute ??= validationsAttr;
                        break;

                    case DescriptionAttribute descriptionAttr:
                        DescriptionAttribute ??= descriptionAttr;
                        break;

                    case RenderAsAttribute renderAsAttr:
                        RenderAsAttribute ??= renderAsAttr;
                        break;

                    case GridAttribute gridAttr:
                        GridAttribute ??= gridAttr;
                        break;

                    case ElementAttribute elementAttr:
                        ElementAttribute ??= elementAttr;
                        break;

                    case FlagsAttribute flagsAttr:
                        FlagsAttribute ??= flagsAttr;
                        break;

                    default:
                        SetAttribute(-1, attr);
                        break;
                }
            }

            // Ensure DisplayAttribute is initialized
            DisplayAttribute ??= new();
            DisplayAttribute.Name ??= type.Name;

            // Ensure ValidationsAttribute is initialized
            ValidationsAttribute ??= new ValidationsAttribute();
        }

        #endregion

        #region Internal methods region

        internal void AddMembers(IEnumerable<MemberAccessor> members)
        {
            foreach (var member in members)
                _members.Add(member.Name, member);
        }

        internal void AddActions(IEnumerable<ActionAccessor> actions)
        {
            foreach (var action in actions)
                _actions.Add(action.Name, action);
        }

        #endregion

        #region Public constructors region

        public TypeAccessor(Type type)
            : base(type.FullName!)
        {
            BaseType = type ?? throw new ArgumentNullException(nameof(type));
            Initialize(type);
        }

        #endregion

        #region Public methods region

        /// <summary>
        /// Gets member accessors which has specifeid <paramref name="attributeType"/> attribute.
        /// </summary>
        /// <param name="attributeType">The sype of attribute to filter.</param>
        public IEnumerable<MemberAccessor> GetMembers(Type attributeType)
            => _members.Where(e => e.Value.HasAttribute(attributeType)).Select(e => e.Value);

        /// <summary>
        /// Gets action accessors which has specifeid <paramref name="attributeType"/> attribute.
        /// </summary>
        /// <param name="attributeType">The sype of attribute to filter.</param>
        public IEnumerable<ActionAccessor> GetActions(Type attributeType)
            => _actions.Where(e => e.Value.HasAttribute(attributeType)).Select(e => e.Value);

        /// <summary>
        /// Gets custom member attributes.
        /// </summary>
        /// <param name="member">A member name.</param>
        public AttributeCollection GetCustomAttributes(string member)
        {
            return _customMemberAttributes.GetOrAdd(member, m =>
            {
                var members = BaseType.GetMember(m);
                if (members == null || members.Length == 0)
                    throw new ArgumentException($"Invalid '{m} member.", nameof(member));
                return new AttributeCollection(members[0].GetCustomAttributes(false).Select(a => (Attribute)a).ToArray());
            });
        }

        #endregion

        #region Public properties region

        /// <summary>
        /// Gets underlied type.
        /// </summary>
        public Type BaseType { get; }

        /// <summary>
        /// Get member accessor by name.
        /// </summary>
        /// <param name="name">A name of the member.</param>
        public AccessorBase this[string name]
        {
            get
            {
                if (_members.TryGetValue(name, out var member))
                    return member;
                else if (_actions.TryGetValue(name, out var action))
                    return action;
                else
                    throw new KeyNotFoundException($"Could not found member or action '{name}' in the type '{Name}'.");
            }
        }

        /// <summary>
        /// Gets member accessors
        /// </summary>
        public IReadOnlyCollection<MemberAccessor> Members => _members.Values;

        /// <summary>
        /// Gets action accessors
        /// </summary>
        public IReadOnlyCollection<ActionAccessor> Actions => _actions.Values;

        #endregion

        #region Protected properties region

        protected FlagsAttribute? FlagsAttribute { get => (FlagsAttribute?)GetAttribute(CustomAttributeStartIndex); set => SetAttribute(CustomAttributeStartIndex, value!); }

        #endregion
    }
}
