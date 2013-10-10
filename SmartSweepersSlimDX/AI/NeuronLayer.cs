using System.Collections.Generic;
using System.Linq;

namespace SmartSweepersSlimDX.AI
{
    class NeuronLayer
    {
        #region Private Variables

        /// <summary>The layer of neurons</summary>
        private List<Neuron> neurons;

        #endregion

        #region Public Properties

        /// <summary>Gets the neurons.</summary>
        /// <value>The neurons.</value>
        public IEnumerable<Neuron> Neurons { get { return neurons; } }

        /// <summary>Gets the <see cref="Neuron" /> at the specified index.</summary>
        /// <value>The <see cref="Neuron" />.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Neuron this[int index] { get { return neurons[index]; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuronLayer" /> class and 
        /// creates a layer of neurons of the required size by calling the Neuron ctor the rqd number of times.
        /// </summary>
        /// <param name="neuronCount">The neuron count.</param>
        /// <param name="inputCountPerNeuron">The input count per neuron.</param>
        public NeuronLayer(int neuronCount, int inputCountPerNeuron)
        {
            neurons = new List<Neuron>();

            for (int i = 0; i < neuronCount; ++i)
            {
                neurons.Add(new Neuron(inputCountPerNeuron));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Gets the number of weights.</summary>
        /// <returns></returns>
        public int GetNumberOfWeights()
        {
            return neurons
                .Select(neuron => neuron.InputCount)
                .Sum();
        }

        #endregion
    }
}
