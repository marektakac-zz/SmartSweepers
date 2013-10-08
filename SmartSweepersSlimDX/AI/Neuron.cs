using System.Collections.Generic;
using SmartSweepersSlimDX.AI.Utils;

namespace SmartSweepersSlimDX.AI
{
    class Neuron
    {
        #region Private Variables

        /// <summary>
        /// The weights for each input.
        /// </summary>
        private List<double> weights;

        #endregion

        #region Public Properties

        /// <summary>
        /// The number of inputs into the neuron.
        /// </summary>
        public int InputCount { get; private set; }

        /// <summary>Gets the weights.</summary>
        /// <value>The weights.</value>
        public IEnumerable<double> Weights { get { return weights; } }

        /// <summary>Gets the <see cref="System.Double" /> at the specified index.</summary>
        /// <value>The <see cref="System.Double" />.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public double this[int index]
        {
            get { return weights[index]; }
            set { weights[index] = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Neuron" /> class.
        /// </summary>
        /// <param name="inputCount">The input count.</param>
        public Neuron(int inputCount)
        {
            //we need an additional weight for the bias hence the +1
            this.InputCount = inputCount + 1;

            weights = new List<double>();

            for (int i = 0; i < this.InputCount; ++i)
            {
                //set up the weights with an initial random value
                weights.Add(RandomNumbers.Clamped());
            }
        }

        #endregion
    }
}
