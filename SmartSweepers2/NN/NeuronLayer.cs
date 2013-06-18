using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepers2.NN
{
    class NeuronLayer
    {
        //the number of neurons in this layer
        public int NeuronCount { get; private set; }

        //the layer of neurons
        private List<Neuron> neurons;

        /// <summary>Gets the neurons.</summary>
        /// <value>The neurons.</value>
        public IEnumerable<Neuron> Neurons { get { return neurons; } }

        /// <summary>Gets the <see cref="Neuron" /> at the specified index.</summary>
        /// <value>The <see cref="Neuron" />.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Neuron this[int index]
        {
            get { return neurons[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuronLayer" /> class and 
        /// creates a layer of neurons of the required size by calling the Neuron ctor the rqd number of times.
        /// </summary>
        /// <param name="neuronCount">The neuron count.</param>
        /// <param name="inputCountPerNeuron">The input count per neuron.</param>
        public NeuronLayer(int neuronCount, int inputCountPerNeuron)
        {
            NeuronCount = neuronCount;

            neurons = new List<Neuron>();

            for (int i = 0; i < NeuronCount; ++i)
            {
                neurons.Add(new Neuron(inputCountPerNeuron));
            }
        }
    }
}
