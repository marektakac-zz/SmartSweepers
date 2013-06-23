using System.Collections.Generic;
using SmartSweepersSlimDX.UI.Bindings;

namespace SmartSweepersSlimDX.UI
{   /// <summary>
    /// Provides basic logical UI component functionality.
    /// </summary>
    public class Element
    {
        private Dictionary<string, Binding> bindings = new Dictionary<string, Binding>();

        /// <summary>
        /// Gets or sets the element's label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>Sets the binding.</summary>
        /// <param name="targetName">Name of the target.</param>
        /// <param name="source">The source.</param>
        public void SetBinding(string targetName, object source)
        {
            bindings[targetName] = new Binding(targetName, source);
        }

        /// <summary>Updates this instance.</summary>
        public void Update()
        {
            foreach (Binding binding in bindings.Values)
            {
                binding.Update(this);
            }
        }
    }
}
