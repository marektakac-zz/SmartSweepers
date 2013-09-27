using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SlimDX;
using SmartSweepersSlimDX.AI;

namespace SmartSweepersSlimDX
{
    class Minesweeper
    {
        #region Private Variables

        /// <summary>The minesweeper's neural net.</summary>
        private NeuralNet brain;

        /// <summary>Its position in the world.</summary>
        private Vector2 position;

        /// <summary>Direction sweeper is facing.</summary>
        public Vector2 lookAt;

        /// <summary>Its rotation (surprise surprise).</summary>
        public double rotation;

        /// <summary>The speed.</summary>
        public double speed;

        /// <summary>To store output from the ANN.</summary>
        private double lTrack;

        /// <summary>To store output from the ANN.</summary>
        private double rTrack;

        /// <summary>The sweeper's fitness score.</summary>
        private double fitness;

        /// <summary>The scale of the sweeper when drawn.</summary>
        private double scale;

        /// <summary>Index position of closest mine.</summary>
        private int closestMine;

        #endregion

        #region Public Methods

        /// <summary>Initializes a new instance of the <see cref="Minesweeper" /> class.</summary>
        public Minesweeper()
        {
            rotation = Utils.RandomDouble() * Math.PI * 2;
            lTrack = 0.16;
            rTrack = 0.16;
            fitness = 0;
            scale = Params.Instance.SweeperScale;
            closestMine = 0;

            brain = new NeuralNet();

            //create a random start position
            position = new Vector2(
                (float)(Utils.RandomDouble() * Params.Instance.WindowWidth),
                (float)(Utils.RandomDouble() * Params.Instance.WindowHeight));
        }

        /// <summary>
        /// Updates the ANN with information from the sweepers enviroment. First we take sensor readings and feed these into the sweepers brain.
        /// </summary>
        /// <param name="mines">The mines.</param>
        /// <returns></returns>
        /// <remarks>The inputs are:
        ///	A vector to the closest mine (x, y)
        ///	The sweepers 'look at' vector (x, y)
        ///	
        ///	We receive two outputs from the brain.. lTrack & rTrack.
        ///	So given a force for each track we calculate the resultant rotation 
        ///	and acceleration and apply to current velocity vector.</remarks>
        public bool Update(List<Vector2> mines)
        {
            //this will store all the inputs for the NN
            List<double> inputs = new List<double>();

            //get vector to closest mine
            Vector2 closestMine = GetClosestMine(mines);

            //normalise it
            closestMine.Normalize();

            //add in vector to closest mine
            inputs.Add(closestMine.X);
            inputs.Add(closestMine.Y);

            //add in sweepers look at vector
            inputs.Add(lookAt.X);
            inputs.Add(lookAt.Y);

            //update the brain and get feedback
            List<double> output = brain.Update(inputs);

            //make sure there were no errors in calculating the 
            //output
            if (output.Count < Params.Instance.OutputCount)
            {
                return false;
            }

            //assign the outputs to the sweepers left & right tracks
            lTrack = output[0];
            rTrack = output[1];

            //calculate steering forces
            double rottatingForce = lTrack - rTrack;

            //clamp rotation
            Utils.Clamp(ref rottatingForce, -Params.Instance.MaxTurnRate, Params.Instance.MaxTurnRate);

            rotation += rottatingForce;

            speed = (lTrack + rTrack);

            //update Look At 
            lookAt.X = (float)-Math.Sin(rotation);
            lookAt.Y = (float)Math.Cos(rotation);

            //update position
            position += (lookAt * (float)speed);

            //wrap around window limits
            if (position.X > Params.Instance.WindowWidth) { position.X = 0; }
            if (position.X < 0) { position.X = Params.Instance.WindowWidth; }
            if (position.Y > Params.Instance.WindowHeight) { position.Y = 0; }
            if (position.Y < 0) { position.Y = Params.Instance.WindowHeight; }

            return true;
        }

        /// <summary>Used to transform the sweepers vertices prior to rendering.</summary>
        /// <param name="sweeper">The sweeper.</param>
        public void WorldTransform(System.Drawing.PointF[] sweeper)
        {
            var matTransform = new System.Drawing.Drawing2D.Matrix();
            matTransform.Scale((float)scale, (float)scale, MatrixOrder.Append);
            matTransform.Rotate((float)RadToDeg(rotation), MatrixOrder.Append);
            matTransform.Translate(position.X, position.Y, MatrixOrder.Append);
            matTransform.TransformPoints(sweeper);
        }

        private double RadToDeg(double angle)
        {
            return angle * (180 / Math.PI);
        }

        private double DegToRad(double angle)
        {
            return Math.PI * angle / 180;
        }

        /// <summary>Returns a vector to the closest mine.</summary>
        /// <param name="mines">Mines.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Vector2 GetClosestMine(List<Vector2> mines)
        {
            double closestSoFar = 99999;

            Vector2 closestObject = new Vector2(0, 0);

            //cycle through mines to find closest
            for (int mineIndex = 0; mineIndex < mines.Count; mineIndex++)
            {
                float lengthToObject = (mines[mineIndex] - position).Length();

                if (lengthToObject < closestSoFar)
                {
                    closestSoFar = lengthToObject;
                    closestObject = position - mines[mineIndex];
                    closestMine = mineIndex;
                }
            }

            return closestObject;
        }

        /// <summary>Checks to see if the minesweeper has 'collected' a mine.</summary>
        /// <param name="mines">The mines.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public int CheckForMine(List<Vector2> mines, double size)
        {
            Vector2 distanceToObject = position - mines[closestMine];

            if (distanceToObject.Length() < size + 5)
            {
                return closestMine;
            }

            return -1;
        }

        /// <summary>Resets the sweepers position, fitness and rotation.</summary>
        public void Reset()
        {
            //reset the sweepers positions
            position = new Vector2(
                (float)(Utils.RandomDouble() * Params.Instance.WindowWidth),
                (float)(Utils.RandomDouble() * Params.Instance.WindowHeight));

            //and the fitness
            fitness = 0;

            //and the rotation
            rotation = Utils.RandomDouble() * Math.PI * 2;

            return;
        }

        #endregion

        #region Accessor functions

        /// <summary>Positions this instance.</summary>
        /// <returns></returns>
        public Vector2 Position()
        {
            return position;
        }

        /// <summary>Increments the fitness.</summary>
        public void IncrementFitness()
        {
            ++fitness;
        }

        /// <summary>Fitnesses this instance.</summary>
        /// <returns></returns>
        public double Fitness()
        {
            return fitness;
        }

        /// <summary>Puts the weights.</summary>
        /// <param name="w">The w.</param>
        public void PutWeights(IList<double> w)
        {
            brain.PutWeights(w);
        }

        /// <summary>Gets the number of weights.</summary>
        /// <returns></returns>
        public int GetNumberOfWeights()
        {
            return brain.GetNumberOfWeights();
        }

        #endregion
    }
}
