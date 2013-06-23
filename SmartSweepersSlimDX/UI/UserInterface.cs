using System;

namespace SmartSweepersSlimDX.UI
{
    /// <summary>
    /// Encapsulates logical user interface state.
    /// </summary>
    public class UserInterface
    {
        /// <summary>
        /// Gets or sets the interface's element container.
        /// </summary>
        public ElementContainer Container { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInterface"/> class.
        /// </summary>
        public UserInterface()
        {
            Container = new ElementContainer();
        }
    }
}
