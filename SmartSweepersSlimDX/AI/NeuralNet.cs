using System;
using System.Collections.Generic;

namespace SmartSweepersSlimDX.AI
{
    class NeuralNet
    {
        private int inputCount;
        private int outputCount;
        private int hiddenLayerCount;
        private int neuronsPerHiddenLayer;

        List<NeuronLayer> layers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NeuralNet" /> class.
        /// </summary>
        public NeuralNet()
        {
            inputCount = Params.Instance.InputCount;
            outputCount = Params.Instance.OutputCount;
            hiddenLayerCount = Params.Instance.HiddenLayerCount;
            neuronsPerHiddenLayer = Params.Instance.NeuronsPerHiddenLayer;

            layers = new List<NeuronLayer>();

            CreateNet();
        }

        /// <summary>Builds the ANN. The weights are all initially set to random values -1 < w < 1.</summary>
        private void CreateNet()
        {
            //create the layers of the network
            if (hiddenLayerCount > 0)
            {
                //create first hidden layer
                layers.Add(new NeuronLayer(neuronsPerHiddenLayer, inputCount));

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
        }

        /// <summary>
        /// Gets the weights from the NN.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> GetWeights()
        {
            //this will hold the weights
            List<double> weights = new List<double>();

            //for each layer
            for (int layerIndex = 0; layerIndex < hiddenLayerCount + 1; ++layerIndex)
            {
                //for each neuron
                for (int neuronIndex = 0; neuronIndex < layers[layerIndex].NeuronCount; ++neuronIndex)
                {
                    //for each weight
                    for (int weightIndex = 0; weightIndex < layers[layerIndex][neuronIndex].InputCount; ++weightIndex)
                    {
                        weights.Add(layers[layerIndex][neuronIndex][weightIndex]);
                    }
                }
            }

            return weights;
        }

        /// <summary>
        /// Gets total number of weights in net.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfWeights()
        {
            int weights = 0;

            //for each layer
            for (int layerIndex = 0; layerIndex < hiddenLayerCount + 1; ++layerIndex)
            {
                //for each neuron
                for (int neuronIndex = 0; neuronIndex < layers[layerIndex].NeuronCount; ++neuronIndex)
                {
                    //for each weight
                    for (int weightIndex = 0; weightIndex < layers[layerIndex][neuronIndex].InputCount; ++weightIndex)
                    {
                        weights++;
                    }
                }
            }

            return weights;
        }


        /// <summary>
        /// Replaces the weights with new ones.
        /// </summary>
        /// <param name="weights">The weights.</param>
        public void PutWeights(IList<double> weights)
        {
            int weight = 0;

            //for each layer
            for (int layerIndex = 0; layerIndex < hiddenLayerCount + 1; ++layerIndex)
            {
                //for each neuron
                for (int neuronIndex = 0; neuronIndex < layers[layerIndex].NeuronCount; ++neuronIndex)
                {
                    //for each weight
                    for (int weightIndex = 0; weightIndex < layers[layerIndex][neuronIndex].InputCount; ++weightIndex)
                    {
                        layers[layerIndex][neuronIndex][weightIndex] = weights[weight++];
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
            //stores the resultant outputs from each layer
            List<double> outputs = new List<double>();

            int weight = 0;

            //first check that we have the correct amount of inputs
            if (inputs.Count != inputCount)
            {
                //just return an empty vector if incorrect.
                return outputs;
            }

            //For each layer....
            for (int layerIndex = 0; layerIndex < hiddenLayerCount + 1; ++layerIndex)
            {
                if (layerIndex > 0)
                {
                    inputs = new List<double>(outputs);
                }

                outputs.Clear();

                weight = 0;

                //for each neuron sum the (inputs * corresponding weights).Throw 
                //the total at our sigmoid function to get the output.
                for (int neuronIndex = 0; neuronIndex < layers[layerIndex].NeuronCount; ++neuronIndex)
                {
                    double netInput = 0;

                    int numInputs = layers[layerIndex][neuronIndex].InputCount;

                    //for each weight
                    for (int weightIndex = 0; weightIndex < numInputs - 1; ++weightIndex)
                    {
                        //sum the weights x inputs
                        netInput += layers[layerIndex][neuronIndex][weightIndex] * inputs[weight++];
                    }

                    //add in the bias
                    netInput += layers[layerIndex][neuronIndex][numInputs - 1] * Params.Instance.Bias;

                    //we can store the outputs from each layer as we generate them. 
                    //The combined activation is first filtered through the sigmoid function
                    outputs.Add(Sigmoid(netInput, Params.Instance.ActivationResponse));

                    weight = 0;
                }
            }

            return outputs;
        }

        /// <summary>
        /// Sigmoid response curve.
        /// </summary>
        /// <param name="activation">The activation.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public double Sigmoid(double activation, double response)
        {
            return (1 / (1 + Math.Exp(-activation / response)));
        }
    }
}
