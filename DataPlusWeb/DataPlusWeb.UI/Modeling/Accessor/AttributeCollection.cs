using System;
using System.Collections.Generic;

namespace DataPlus.Web.UI
{
    public class AttributeCollection
    {
        #region Private fields region

        private Attribute[] _attributes;
        private Dictionary<Type, int>? _attributesPerTypes;

        #endregion

        #region Private methods region

        private int FindIndex(Type type)
        {
            if (_attributesPerTypes != null)
            {
                if (_attributesPerTypes.TryGetValue(type, out var index))
                    return index;
            }
            else if (_attributes.Length < 6)
            {
                for (int i = 0; i < _attributes.Length; i++)
                {
                    if (_attributes[i].GetType() == type)
                        return i;
                }
            }
            else
            {
                var perTypes = new Dictionary<Type, int>();
                var index = -1;
                for (int i = 0; i < _attributes.Length; i++)
                {
                    var attType = _attributes[i].GetType();
                    if (index != -1 && attType == type) index = i;
                    perTypes.Add(attType, i);
                }
                _attributesPerTypes = perTypes;
                return index;
            }
            return -1;
        }

        #endregion

        #region Public constructors region

        public AttributeCollection(Attribute[] attributes)
            : base()
        {
            _attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        }

        #endregion

        #region Public methods region

        public bool Contains(Type attributeType) => FindIndex(attributeType) != -1;

        #endregion

        #region Public properties region

        public Attribute? this[Type attributeType]
        {
            get
            {
                var index = FindIndex(attributeType);
                if (index == -1) return null;
                return _attributes[index];
            }
        }

        #endregion
    }

    public static class AttributeCollectionExtensions
    {
        #region Public methods region

        public static TAttribute? GetAttribute<TAttribute>(this AttributeCollection source) where TAttribute : Attribute => (TAttribute?)source[typeof(TAttribute)];

        #endregion
    }
}
