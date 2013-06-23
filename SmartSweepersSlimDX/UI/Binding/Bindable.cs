using System;

namespace SmartSweepersSlimDX.UI.Bindings
{
    /// <summary>
    /// A wrapper for value types that enable their use in UI element bindings.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    class Bindable<T> : IBindable where T : struct
    {
        private T value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if (!object.Equals(this.value, value))
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bindable&lt;T&gt;"/> class.
        /// </summary>
        public Bindable()
            : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bindable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Bindable(T value)
        {
            this.value = value;
        }

        object IBindable.GetValue()
        {
            return Value;
        }
    }
}
