using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSweepers2.AI
{
    internal class Genome : IComparable
    {
        private List<double> weights;

        public IList<double> Weights { get { return weights; } }

        public double Fitness { get; set; }

        public Genome()
        {
            weights = new List<double>();
            Fitness = 0;
        }

        public Genome(List<double> weights, double fitness)
        {
            this.weights = weights;
            this.Fitness = fitness;

            weights = new List<double>();
        }

        public void AddWeight(double weight)
        {
            weights.Add(weight);
        }

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
    }
}
