using System;
using System.Configuration;
using System.Globalization;

namespace SmartSweepersSlimDX
{
    /// <summary>
    /// 
    /// </summary>
    internal class Params
    {
        #region General parameters

        /// <summary>Gets the width of the window.</summary>
        /// <value>The width of the window.</value>
        public int WindowWidth { get { return GetInt("WindowWidth"); } }

        /// <summary>Gets the height of the window.</summary>
        /// <value>The height of the window.</value>
        public int WindowHeight { get { return GetInt("WindowHeight"); } }

        #endregion

        #region Neural network parameters

        /// <summary>Gets the num inputs.</summary>
        /// <value>The num inputs.</value>
        public int InputCount { get { return GetInt("InputCount"); } }

        /// <summary>Gets the num hidden.</summary>
        /// <value>The num hidden.</value>
        public int HiddenLayerCount { get { return GetInt("HiddenLayerCount"); } }

        /// <summary>Gets the neurons per hidden layer.</summary>
        /// <value>The neurons per hidden layer.</value>
        public int NeuronsPerHiddenLayer { get { return GetInt("NeuronsPerHiddenLayer"); } }

        /// <summary>Gets the num outputs.</summary>
        /// <value>The num outputs.</value>
        public int OutputCount { get { return GetInt("OutputCount"); } }

        /// <summary>Gets the activation response used for tweeking the sigmoid function.</summary>
        /// <value>The activation response.</value>
        public double ActivationResponse { get { return GetDouble("ActivationResponse"); } }

        /// <summary>Gets the bias.</summary>
        /// <value>The bias.</value>
        public double Bias { get { return GetDouble("Bias"); } }

        #endregion

        #region Sweepers parameters

        /// <summary>Gets the max turn rate.</summary>
        /// <value>The max turn rate.</value>
        public double MaxTurnRate { get { return GetDouble("MaxTurnRate"); } }

        /// <summary>Gets the sweeper scale.</summary>
        /// <value>The sweeper scale.</value>
        public int SweeperScale { get { return GetInt("SweeperScale"); } }

        #endregion

        #region Controller parameters

        /// <summary>Gets the num sweepers.</summary>
        /// <value>The num sweepers.</value>
        public int NumSweepers { get { return GetInt("NumSweepers"); } }

        /// <summary>Gets the num mines.</summary>
        /// <value>The num mines.</value>
        public int NumMines { get { return GetInt("NumMines"); } }

        /// <summary>Gets the num ticks.</summary>
        /// <value>The num ticks.</value>
        public int NumTicks { get { return GetInt("NumTicks"); } }

        /// <summary>Gets the mine scale.</summary>
        /// <value>The mine scale.</value>
        public double MineScale { get { return GetDouble("MineScale"); } }

        #endregion

        #region GA parameters

        /// <summary>Gets the crossover rate.</summary>
        /// <value>The crossover rate.</value>
        public double CrossoverRate { get { return GetDouble("CrossoverRate"); } }

        /// <summary>Gets the mutation rate.</summary>
        /// <value>The mutation rate.</value>
        public double MutationRate { get { return GetDouble("MutationRate"); } }

        /// <summary>Gets the max perturbation.</summary>
        /// <value>The max perturbation.</value>
        public double MaxPerturbation { get { return GetDouble("MaxPerturbation"); } }

        /// <summary>Gets the num elite.</summary>
        /// <value>The num elite.</value>
        public int NumElite { get { return GetInt("NumElite"); } }

        /// <summary>Gets the num copies elite.</summary>
        /// <value>The num copies elite.</value>
        public int NumCopiesElite { get { return GetInt("NumCopiesElite"); } }

        #endregion

        private static Params instance = null;

        public static Params Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Params();
                }

                return instance;
            }
        }

        /// <summary>Gets an int value from the configuration file specified by the name.</summary>
        /// <param name="name">The name of the configuration value.</param>
        /// <returns></returns>
        private int GetInt(string name)
        {
            int value = 0;
            Int32.TryParse(ConfigurationManager.AppSettings[name], out value);
            return value; // parsed value or 0 if any problem occurred
        }

        /// <summary>Gets a double value from the configuration file specified by the name.</summary>
        /// <param name="name">The name of the configuration value.</param>
        /// <returns></returns>
        private double GetDouble(string name)
        {
            double value = 0;
            
            Double.TryParse(
                ConfigurationManager.AppSettings[name], 
                NumberStyles.Number | NumberStyles.AllowDecimalPoint, 
                CultureInfo.CreateSpecificCulture("en-US"), 
                out value);

            return value; // parsed value or 0 if any problem occurred
        }
    }
}
