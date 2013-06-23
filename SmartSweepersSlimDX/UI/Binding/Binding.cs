using System.Reflection;

namespace SmartSweepersSlimDX.UI.Bindings
{
    class Binding
    {
        private string targetName;
        private object source;

        /// <summary>Initializes a new instance of the <see cref="Binding"/> class.</summary>
        /// <param name="targetName">Name of the target.</param>
        /// <param name="source">The source.</param>
        public Binding(string targetName, object source) {
            this.targetName = targetName;
            this.source = source;
        }

        /// <summary>Updates the specified target.</summary>
        /// <param name="target">The target.</param>
        public void Update(object target) {
            PropertyInfo property = target.GetType().GetProperty(targetName);

            if (property == null) {
                return;
            }

            object value = source;
            IBindable bindable = source as IBindable;
            
            if (bindable != null) {
                value = bindable.GetValue();
            }

            if (property.PropertyType.IsAssignableFrom(value.GetType())) 
            {
                property.SetValue(target, value, null);
            } else if (property.PropertyType == typeof(string)) 
            {
                property.SetValue(target, value.ToString(), null);
            }
        }
    }
}
