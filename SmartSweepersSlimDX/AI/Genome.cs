using System;
using System.Collections.Generic;

namespace SmartSweepersSlimDX.AI
{
    internal class Genome : IComparable
    {
        #region Private Variables

        private List<double> weights;

        #endregion

        #region Public Properties

        /// <summary>Gets the weights.</summary>
        /// <value>The weights.</value>
        public IList<double> Weights { get { return weights; } }

        /// <summary>Gets or sets the fitness.</summary>
        /// <value>The fitness.</value>
        public double Fitness { get; set; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Genome"/> class.</summary>
        public Genome()
        {
            weights = new List<double>();
            Fitness = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="Genome"/> class.</summary>
        /// <param name="weights">The weights.</param>
        /// <param name="fitness">The fitness.</param>
        public Genome(List<double> weights, double fitness)
        {
            this.weights = weights;
            this.Fitness = fitness;

            weights = new List<double>();
        }

        #endregion

        #region Public Methods

        /// <summary>Adds the weight.</summary>
        /// <param name="weight">The weight.</param>
        public void AddWeight(double weight)
        {
            weights.Add(weight);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="System.ArgumentException">Object is not a Genome.</exception>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            Genome otherGenome = obj as Genome;
            if (otherGenome != null)
            {
                return this.Fitness.CompareTo(otherGenome.Fitness);
            }

            throw new ArgumentException("Object is not a Genome.");
        }

        #endregion
    }
}
