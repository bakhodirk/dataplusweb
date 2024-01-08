using System;
using System.Runtime.CompilerServices;

namespace DataPlus.Web.UI
{
    public abstract class ContextualModelBase<TKey>
    {
        private class States<T>
        {
            #region Private fields region

            private const int _capacity = 4;
            private T[] _states = new T[_capacity];

            #endregion

            #region Private methods region

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private T Get(int index)
            {
                if (index < _states.Length) return _states[index];
                throw new IndexOutOfRangeException();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Set(int index, T value)
            {
                if (index >= _states.Length)
                    Array.Resize(ref _states, (index / _capacity + 1) * _capacity);
                _states[index] = value;
            }

            #endregion

            #region Public properties region

            public T this[int index] { get => Get(index); set => Set(index, value); }

            #endregion
        }

        #region Private fields region

        private readonly States<ContextualModelState> _states = new();

        #endregion

        #region Public constructors region

        public ContextualModelBase()
            : this(default!)
        { }

        public ContextualModelBase(TKey key)
            : base()
        {
            Key = key;
        }

        #endregion

        #region Public properties region

        public virtual TKey Key { get; }

        #endregion

        #region Protected methods region

        protected void SetModified(int index)
        {
            _states[index] = ContextualModelState.Modified;
        }

        protected void SetUnchanged(int index)
        {
            _states[index] = ContextualModelState.Unchanged;
        }

        protected ContextualModelState GetState(int index)
        {
            return _states[index];
        }

        #endregion
    }

    public enum ContextualModelState : byte
    {
        #region Enums

        /// <summary>
        /// No changes.
        /// </summary>
        Unchanged = 0,
        /// <summary>
        /// Modified.
        /// </summary>
        Modified = 1

        #endregion
    }
}
