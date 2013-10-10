using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSweepersSlimDX.AI
{
    class NeuralNet
    {
        #region Private Variables

        private List<NeuronLayer> layers;
        private int inputCount;
        private int weightCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralNet" /> class.
        /// </summary>
        public NeuralNet(int inputCount, int outputCount, int hiddenLayerCount, int neuronsPerHiddenLayer)
        {
            this.inputCount = inputCount;

            layers = new List<NeuronLayer>();

            //create the layers of the network
            if (hiddenLayerCount > 0)
            {
                //create first hidden layer
                layers.Add(new NeuronLayer(neuronsPerHiddenLayer, inputCount));

                //create hidden layers
                for (int index = 0; index < hiddenLayerCount - 1; ++index)
                {
                    layers.Add(new NeuronLayer(neuronsPerHiddenLayer, neuronsPerHiddenLayer));
                }

                //create output layer
                layers.Add(new NeuronLayer(outputCount, neuronsPerHiddenLayer));
            }
            else
            {
                //create output layer
                layers.Add(new NeuronLayer(outputCount, inputCount));
            }

            this.weightCount = GetNumberOfWeights();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets total number of weights in net.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfWeights()
        {
            return layers
                .Select(layer => layer.GetNumberOfWeights())
                .Sum();
        }

        /// <summary>Replaces the weights with new ones.</summary>
        /// <param name="weights">The weights.</param>
        /// <exception cref="System.ArgumentException">If the count of weights is different then expected, then this excpetion is thrown with some details.</exception>
        public void PutWeights(IList<double> weights)
        {
            //first check that we have the correct amount of weights
            if (weights.Count != weightCount)
            {
                throw new ArgumentException(string.Format("The count of weights should be {0} instead of {1}.", weightCount, weights.Count));
            }

            int weightIndex = 0;

            foreach (var layer in layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    for (int index = 0; index < neuron.InputCount; ++index)
                    {
                        neuron[index] = weights[weightIndex++];
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Calculates the outputs from a set of inputs.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <returns></returns>
        public List<double> Update(List<double> inputs)
        {
            int weight = 0;
            List<double> outputs = new List<double>();

            if (inputs.Count != inputCount)
            {
                //first check that we have the correct amount of inputs and just return an empty vector if incorrect.
                return outputs;
            }

            foreach (var layer in layers)
            {
                if (layers.IndexOf(layer) > 0)
                {
                    inputs = new List<double>(outputs);
                }

                outputs.Clear();
                weight = 0;

                //for each neuron sum the (inputs * corresponding weights).Throw 
                //the total at our sigmoid function to get the output.
                foreach (var neuron in layer.Neurons)
                {
                    double netInput = 0;

                    for (int weightIndex = 0; weightIndex < neuron.InputCount - 1; ++weightIndex)
                    {
                        //sum the weights with inputs
                        netInput += neuron[weightIndex] * inputs[weight++];
                    }

                    //add in the bias
                    netInput += neuron[neuron.InputCount - 1] * Params.Instance.Bias;

                    //we can store the outputs from each layer as we generate them. 
                    //The combined activation is first filtered through the sigmoid function
                    outputs.Add(Sigmoid(netInput, Params.Instance.ActivationResponse));

                    weight = 0;
                }
            }

            return outputs;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sigmoid response curve.
        /// </summary>
        /// <param name="activation">The activation.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private double Sigmoid(double activation, double response)
        {
            return (1 / (1 + Math.Exp(-activation / response)));
        }

        #endregion
    }
}
