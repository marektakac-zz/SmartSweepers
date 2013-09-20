using System.Collections.Generic;

namespace SmartSweepersSlimDX.AI
{
    internal class GeneticAlgorithm
    {
        #region Private Variables

        /// <summary>Holds the entire population of chromosomes.</summary>
        private List<Genome> genomes;

        /// <summary>Size of population.</summary>
        private int populationSize;

        /// <summary>Amount of weights per chromo.</summary>
        private int chromoLength;

        /// <summary>Total fitness of population.</summary>
        private double totalFitness;

        /// <summary>Best fitness this population.</summary>
        private double bestFitness;

        /// <summary>Average fitness.</summary>
        private double averageFitness;

        /// <summary>Worst fitness.</summary>
        private double worstFitness;

        /// <summary>Keeps track of the best genome.</summary>
        private int fittestGenome;

        /// <summary>probability that a chromosones bits will mutate.</summary>
        /// <remarks>Try figures around 0.05 to 0.3 ish</remarks>
        private double mutationRate;

        /// <summary>Probability of chromosones crossing over bits.</summary>
        /// <remarks>0.7 is pretty good.</remarks>
        private double crossoverRate;

        /// <summary>Generation counter.</summary>
        //private int generationCounter;

        #endregion

        #region Public Properties

        /// <summary>Gets the genomes.</summary>
        /// <value>The genomes.</value>
        public IEnumerable<Genome> Genomes { get { return genomes; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticAlgorithm" /> class and
        /// initialise population with chromosomes consisting of random weights and all fitnesses set to zero.
        /// </summary>
        /// <param name="populationSize">The size of population.</param>
        /// <param name="mutationRate">The mut rat.</param>
        /// <param name="crossoverRate">The crossover rate.</param>
        /// <param name="chromoLength">The length of chromo.</param>
        public GeneticAlgorithm(int populationSize, double mutationRate, double crossoverRate, int chromoLength)
        {
            this.populationSize = populationSize;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.chromoLength = chromoLength;

            this.totalFitness = 0;
            this.fittestGenome = 0;
            this.bestFitness = 0;
            this.worstFitness = double.MaxValue;
            this.averageFitness = 0;
            //this.generationCounter = 0;

            genomes = new List<Genome>();

            for (int genomeIndex = 0; genomeIndex < populationSize; ++genomeIndex)
            {
                genomes.Add(new Genome());

                for (int chromoIndex = 0; chromoIndex < chromoLength; ++chromoIndex)
                {
                    genomes[genomeIndex].AddWeight(Utils.RandomClamped());
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Takes a population of chromosones and runs the algorithm through one cycle.</summary>
        /// <param name="oldPopulation">The old_pop.</param>
        /// <returns>New population of chromosones.</returns>
        public List<Genome> Epoch(List<Genome> oldPopulation)
        {
            //assign the given population to the classes population
            genomes = oldPopulation;

            //reset the appropriate variables
            Reset();

            //sort the population (for scaling and elitism)
            genomes.Sort();

            //calculate best, worst, average and total fitness
            CalculateStatistics();

            //create a temporary list to store new chromosones
            List<Genome> newPopulation = new List<Genome>();

            //Now to add a little elitism we shall add in some copies of the
            //fittest genomes. Make sure we add an EVEN number or the roulette
            //wheel sampling will crash
            if ((Params.Instance.NumCopiesElite * Params.Instance.NumElite % 2) == 0)
            {
                GrabNBest(Params.Instance.NumElite, Params.Instance.NumCopiesElite, newPopulation);
            }

            //now we enter the GA loop

            //repeat until a new population is generated
            while (newPopulation.Count < populationSize)
            {
                //grab two chromosones
                Genome mum = GetChromoRoulette();
                Genome dad = GetChromoRoulette();

                //create some offspring via crossover
                List<double> baby1 = null;
                List<double> baby2 = null;

                Crossover(mum.Weights, dad.Weights, out baby1, out baby2);

                //now we mutate
                Mutate(baby1);
                Mutate(baby2);

                //now copy into vecNewPop population
                newPopulation.Add(new Genome(baby1, 0));
                newPopulation.Add(new Genome(baby2, 0));
            }

            //finished so assign new pop back into m_vecPop
            genomes = newPopulation;

            return genomes;
        }

        /// <summary>Averages the fitness.</summary>
        /// <returns></returns>
        public double AverageFitness()
        {
            return totalFitness / populationSize;
        }

        /// <summary>Bests the fitness.</summary>
        /// <returns></returns>
        public double BestFitness()
        {
            return bestFitness;
        }

        #endregion

        #region Private Methods

        /// <summary>Given parents and storage for the offspring this method performs crossover according to the GAs crossover rate.</summary>
        /// <param name="mum">The mum.</param>
        /// <param name="dad">The dad.</param>
        /// <param name="girl">The girl.</param>
        /// <param name="boy">The boy.</param>
        private void Crossover(IList<double> mum, IList<double> dad, out List<double> girl, out List<double> boy)
        {
            //just return parents as offspring dependent on the rate or if parents are the same
            if ((Utils.RandomDouble() > crossoverRate) || (mum == dad))
            {
                girl = new List<double>(mum);
                boy = new List<double>(dad);

                return;
            }

            girl = new List<double>();
            boy = new List<double>();

            //determine a crossover point
            int crossoverPoint = Utils.RandomInt(chromoLength - 1);

            //create the offspring
            for (int i = 0; i < crossoverPoint; ++i)
            {
                girl.Add(mum[i]);
                boy.Add(dad[i]);
            }

            for (int i = crossoverPoint; i < mum.Count; ++i)
            {
                girl.Add(dad[i]);
                boy.Add(mum[i]);
            }

            return;
        }

        /// <summary>
        /// Mutates the specified chromosome by perturbing its weights by an amount not greater than Params.MaxPerturbation.
        /// </summary>
        /// <param name="chromo">The chromosome.</param>
        private void Mutate(List<double> chromo)
        {
            //traverse the chromosome and mutate each weight dependent on the mutation rate
            for (int chromoIndex = 0; chromoIndex < chromo.Count; ++chromoIndex)
            {
                //do we perturb this weight?
                if (Utils.RandomDouble() < mutationRate)
                {
                    //add or subtract a small value to the weight
                    chromo[chromoIndex] += (Utils.RandomClamped() * Params.Instance.MaxPerturbation);
                }
            }
        }

        /// <summary>Gets a chromo based on roulette wheel sampling.</summary>
        /// <returns></returns>
        private Genome GetChromoRoulette()
        {
            //generate a random number between 0 & total fitness count
            double slice = (double)(Utils.RandomDouble() * totalFitness);

            //go through the chromosones adding up the fitness so far
            double fitnessSoFar = 0;

            for (int i = 0; i < populationSize; ++i)
            {
                fitnessSoFar += genomes[i].Fitness;

                //if the fitness so far > random number return the chromo at this point
                if (fitnessSoFar >= slice)
                {
                    return genomes[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Works like an advanced form of elitism by inserting NumCopies copies of the NBest most fittest genomes into a population vector.
        /// </summary>
        /// <param name="nBest">The N best.</param>
        /// <param name="copyCount">The count of copies.</param>
        /// <param name="population">The population.</param>
        private void GrabNBest(int nBest, int copyCount, List<Genome> population)
        {
            //add the required amount of copies of the n most fittest to the supplied vector
            while (nBest-- > 0)
            {
                for (int i = 0; i < copyCount; ++i)
                {
                    population.Add(genomes[(populationSize - 1) - nBest]);
                }
            }
        }

        /// <summary>Calculates the fittest and weakest genome and the average/total fitness scores</summary>
        private void CalculateStatistics()
        {
            totalFitness = 0;

            double highestSoFar = 0;
            double lowestSoFar = 9999999;

            for (int genomeIndex = 0; genomeIndex < populationSize; ++genomeIndex)
            {
                //update fittest if necessary
                if (genomes[genomeIndex].Fitness > highestSoFar)
                {
                    highestSoFar = genomes[genomeIndex].Fitness;
                    fittestGenome = genomeIndex;
                    bestFitness = highestSoFar;
                }

                //update worst if necessary
                if (genomes[genomeIndex].Fitness < lowestSoFar)
                {
                    lowestSoFar = genomes[genomeIndex].Fitness;
                    worstFitness = lowestSoFar;
                }

                totalFitness += genomes[genomeIndex].Fitness;
            }

            averageFitness = totalFitness / populationSize;
        }

        /// <summary>Resets all the relevant variables ready for a new generation.</summary>
        private void Reset()
        {
            totalFitness = 0;
            bestFitness = 0;
            worstFitness = 9999999;
            averageFitness = 0;
        }

        #endregion
    }
}
