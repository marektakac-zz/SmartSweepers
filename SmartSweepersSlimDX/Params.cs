using System;

namespace SmartSweepersSlimDX
{
    /// <summary>
    /// 
    /// </summary>
    class Params
    {
        #region General parameters

        /// <summary>Gets the width of the window.</summary>
        /// <value>The width of the window.</value>
        public int WindowWidth { get; private set; }

        /// <summary>Gets the height of the window.</summary>
        /// <value>The height of the window.</value>
        public int WindowHeight { get; private set; }

        #endregion

        #region Neural network parameters

        /// <summary>Gets the num inputs.</summary>
        /// <value>The num inputs.</value>
        public int InputCount { get; private set; }

        /// <summary>Gets the num hidden.</summary>
        /// <value>The num hidden.</value>
        public int HiddenLayerCount { get; private set; }

        /// <summary>Gets the neurons per hidden layer.</summary>
        /// <value>The neurons per hidden layer.</value>
        public int NeuronsPerHiddenLayer { get; private set; }

        /// <summary>Gets the num outputs.</summary>
        /// <value>The num outputs.</value>
        public int OutputCount { get; private set; }

        /// <summary>Gets the activation response used for tweeking the sigmoid function.</summary>
        /// <value>The activation response.</value>
        public double ActivationResponse { get; private set; }

        /// <summary>Gets the bias.</summary>
        /// <value>The bias.</value>
        public double Bias { get; private set; }

        #endregion

        #region Sweepers parameters

        /// <summary>Gets the max turn rate.</summary>
        /// <value>The max turn rate.</value>
        public double MaxTurnRate { get; private set; }

        /// <summary>Gets the sweeper scale.</summary>
        /// <value>The sweeper scale.</value>
        public int SweeperScale { get; private set; }

        #endregion

        #region Controller parameters

        /// <summary>Gets the num sweepers.</summary>
        /// <value>The num sweepers.</value>
        public int NumSweepers { get; private set; }

        /// <summary>Gets the num mines.</summary>
        /// <value>The num mines.</value>
        public int NumMines { get; private set; }

        /// <summary>Gets the num ticks.</summary>
        /// <value>The num ticks.</value>
        public int NumTicks { get; private set; }

        /// <summary>Gets the mine scale.</summary>
        /// <value>The mine scale.</value>
        public double MineScale { get; private set; }

        #endregion

        #region GA parameters

        /// <summary>Gets the crossover rate.</summary>
        /// <value>The crossover rate.</value>
        public double CrossoverRate { get; private set; }

        /// <summary>Gets the mutation rate.</summary>
        /// <value>The mutation rate.</value>
        public double MutationRate { get; private set; }

        /// <summary>Gets the max perturbation.</summary>
        /// <value>The max perturbation.</value>
        public double MaxPerturbation { get; private set; }

        /// <summary>Gets the num elite.</summary>
        /// <value>The num elite.</value>
        public int NumElite { get; private set; }

        /// <summary>Gets the num copies elite.</summary>
        /// <value>The num copies elite.</value>
        public int NumCopiesElite { get; private set; }

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

        /// <summary>Initializes a new instance of the <see cref="Params" /> class.</summary>
        private Params()
        {
            WindowWidth = 800;
            WindowHeight = 600;

            //LoadInParameters("params.ini");

            InputCount = 4;
            HiddenLayerCount = 1;
            NeuronsPerHiddenLayer = 8;
            OutputCount = 2;
            ActivationResponse = 1;
            Bias = -1;
            MaxTurnRate = 0.3;

            SweeperScale = 5;
            MineScale = 2;
            
            NumMines = 50;
            NumSweepers = 30;
            NumTicks = 4000;
            CrossoverRate = 0.7;
            MutationRate = 0.1;
            MaxPerturbation = 0.3;
            NumElite = 4;
            NumCopiesElite = 1;
        }
    }
}
