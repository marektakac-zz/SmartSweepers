using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using SmartSweepers2.AI;

namespace SmartSweepers2
{
    class Controller
    {
        #region Private Variables

        /// <summary>The num sweeper verts</summary>
        private const int NumSweeperVerts = 16;

        /// <summary>The sweeper</summary>
        private List<System.Drawing.Point> sweeper = new List<System.Drawing.Point>
        { 
            new System.Drawing.Point(-4, -4),
            new System.Drawing.Point(-4, 4),
	        new System.Drawing.Point(-2, 4),
            new System.Drawing.Point(-2, -4),
            
            new System.Drawing.Point(2, -4),
            new System.Drawing.Point(4, -4),
            new System.Drawing.Point(4, 4),
            new System.Drawing.Point(2, 4),

            new System.Drawing.Point(-2, -2),
            new System.Drawing.Point(2, -2),

            new System.Drawing.Point(-2, 2),
            new System.Drawing.Point(-1, 2),
            new System.Drawing.Point(-1, 7),
            new System.Drawing.Point(1, 7),
            new System.Drawing.Point(1, 2),
            new System.Drawing.Point(2, 2)
        };

        /// <summary>The num mine verts</summary>
        private const int NumMineVerts = 4;

        /// <summary>The mine</summary>
        private List<System.Drawing.Point> mine = new List<System.Drawing.Point>
        {
            new System.Drawing.Point(-1, -1),
	        new System.Drawing.Point(-1, 1),
	        new System.Drawing.Point(1, 1),
	        new System.Drawing.Point(1, -1)
        };

        /// <summary>Storage for the population of genomes.</summary>
        private List<Genome> population = new List<Genome>();

        /// <summary>Storage for the minesweepers.</summary>
        private List<Minesweeper> sweepers = new List<Minesweeper>();

        /// <summary>Storage for the mines.</summary>
        private List<Vector> mines = new List<Vector>();

        /// <summary>The Genetic algorithm.</summary>
        private GeneticAlgorithm geneticAlgorithm;

        /// <summary>The count of sweepers.</summary>
        private int sweepersCount;

        /// <summary>The count of mines.</summary>
        private int minesCount;

        /// <summary>The count of weights in NN.</summary>
        private int weightsInNN;

        /// <summary>Vertex buffer for the sweeper shape's vertices.</summary>
        private List<System.Drawing.Point> sweeperVertexBuffer = new List<System.Drawing.Point>();

        /// <summary>Vertex buffer for the mine shape's vertices.</summary>
        private List<System.Drawing.Point> mineVertexBuffer = new List<System.Drawing.Point>();

        /// <summary>Stores the average fitness per generation for use in graphing.</summary>
        private List<double> averageFitness = new List<double>();

        /// <summary>Stores the best fitness per generation.</summary>
        private List<double> bestFitness = new List<double>();

        /// <summary>The red pen.</summary>
        private Pen redPen;

        /// <summary>The blue pen.</summary>
        private Pen bluePen;

        /// <summary>The green pen.</summary>
        private Pen greenPen;

        /// <summary>The font</summary>
        private Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

        /// <summary>Toggles the speed at which the simulation runs.</summary>
        private bool fastRender;

        /// <summary>Cycles per generation.</summary>
        private int ticks;

        /// <summary>Generation counter.</summary>
        private int generations;

        /// <summary>Window dimension.</summary>
        private int clientWidth;

        /// <summary>Window dimension.</summary>
        private int clientHeight;

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="Controller"/> class.</summary>
        public Controller()
        {
            sweepersCount = Params.Instance.NumSweepers;
            fastRender = false;
            ticks = 0;
            minesCount = Params.Instance.NumMines;
            generations = 0;
            clientWidth = Params.Instance.WindowWidth;
            clientHeight = Params.Instance.WindowHeight;

            int i = 0;

            //let's create the mine sweepers
            for (i = 0; i < sweepersCount; ++i)
            {
                sweepers.Add(new Minesweeper());
            }

            //get the total number of weights used in the sweepers NN so we can initialise the GA
            weightsInNN = sweepers[0].GetNumberOfWeights();

            //initialize the Genetic Algorithm class
            geneticAlgorithm = new GeneticAlgorithm(sweepersCount, Params.Instance.MutationRate, Params.Instance.CrossoverRate, weightsInNN);

            //Get the weights from the GA and insert into the sweepers brains
            population = geneticAlgorithm.Genomes.ToList();

            for (i = 0; i < sweepersCount; i++)
            {
                sweepers[i].PutWeights(population[i].Weights);
            }

            //initialize mines in random positions within the application window
            for (i = 0; i < minesCount; ++i)
            {
                mines.Add(new Vector(Utils.RandomDouble() * clientWidth, Utils.RandomDouble() * clientHeight));
            }

            //create a pen for the graph drawing
            bluePen = new Pen(new SolidBrush(Color.FromArgb(0, 0, 255)));
            redPen = new Pen(new SolidBrush(Color.FromArgb(255, 0, 0)));
            greenPen = new Pen(new SolidBrush(Color.FromArgb(0, 150, 0)));

            //fill the vertex buffers
            for (i = 0; i < NumSweeperVerts; ++i)
            {
                sweeperVertexBuffer.Add(sweeper[i]);
            }

            for (i = 0; i < NumMineVerts; ++i)
            {
                mineVertexBuffer.Add(mine[i]);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Renders the specified surface.</summary>
        /// <param name="surface">The surface.</param>
        public void Render(Graphics surface)
        {
            surface.Clear(SystemColors.Control);

            //render the stats
            surface.DrawString(string.Format("Generation: {0}", generations), font, bluePen.Brush, 10, 10);

            int i = 0;

            //do not render if running at accelerated speed
            if (!fastRender)
            {
                //render the mines
                for (i = 0; i < minesCount; ++i)
                {
                    //grab the vertices for the mine shape
                    var mineVB = mineVertexBuffer.ToArray();

                    WorldTransform(mineVB, mines[i]);

                    //draw the mines
                    surface.DrawRectangle(
                        greenPen,
                        (float)mineVB[0].X,
                        (float)mineVB[0].Y,
                        (float)mineVB[2].X - (float)mineVB[0].X,
                        (float)mineVB[2].Y - (float)mineVB[0].Y);
                }

                //render the sweepers
                for (i = 0; i < sweepersCount; i++)
                {
                    var pen = bluePen;

                    if (i == Params.Instance.NumElite)
                    {
                        pen = redPen;
                    }

                    //grab the sweeper vertices
                    var sweeperVB = sweeperVertexBuffer.ToArray();

                    //transform the vertex buffer
                    sweepers[i].WorldTransform(sweeperVB);

                    if (i == 0)
                    {
                        var msg = string.Format("[{0:F0} , {1:F0}] {2:F0} {3:F0} [{4:F0} , {5:F0}]",
                            sweepers[i].Position().X,
                            sweepers[i].Position().Y,
                            sweepers[i].speed,
                            sweepers[i].rotation,
                            sweepers[i].lookAt.X,
                            sweepers[i].lookAt.Y);
                        surface.DrawString(msg, font, bluePen.Brush, 10, 380);
                    }

                    //draw the sweeper left track
                    surface.DrawLines(pen, sweeperVB.Take(4).ToArray());
                    surface.DrawLine(pen, sweeperVB[0], sweeperVB[3]);

                    //draw the sweeper right track
                    surface.DrawLines(pen, sweeperVB.Skip(4).Take(4).ToArray());
                    surface.DrawLine(pen, sweeperVB[4], sweeperVB[7]);

                    surface.DrawLine(pen, sweeperVB[8], sweeperVB[9]);

                    surface.DrawLines(pen, sweeperVB.Skip(10).ToArray());
                }
            }
            else
            {
                PlotStats(surface);
            }
        }

        /// <summary>
        /// Sets up the translation matrices for the mines and applies the world transform to each vertex 
        /// in the vertex buffer passed to this method..
        /// </summary>
        /// <param name="VBuffer">The V buffer.</param>
        /// <param name="vPos">The v pos.</param>
        public void WorldTransform(System.Drawing.Point[] VBuffer, Vector vPos)
        {
            Matrix transformMatrix = new Matrix();
            transformMatrix.Scale((float)Params.Instance.MineScale, (float)Params.Instance.MineScale);
            transformMatrix.Translate((float)vPos.X, (float)vPos.Y);
            transformMatrix.TransformPoints(VBuffer);
        }

        /// <summary>This is the main workhorse. The entire simulation is controlled from here.</summary>
        /// <returns></returns>
        public bool Update()
        {
            //run the sweepers through Params.Instance.iNumTicks amount of cycles. During
            //this loop each sweepers NN is constantly updated with the appropriate
            //information from its surroundings. The output from the NN is obtained
            //and the sweeper is moved. If it encounters a mine its fitness is
            //updated appropriately,
            if (ticks++ < Params.Instance.NumTicks)
            {
                for (int i = 0; i < sweepersCount; ++i)
                {
                    //update the NN and position
                    if (!sweepers[i].Update(mines))
                    {
                        //error in processing the neural net
                        MessageBox.Show("Wrong amount of NN inputs!", "Error", MessageBoxButtons.OK);

                        return false;
                    }

                    //see if it's found a mine
                    int GrabHit = sweepers[i].CheckForMine(mines, Params.Instance.MineScale);

                    if (GrabHit >= 0)
                    {
                        //we have discovered a mine so increase fitness
                        sweepers[i].IncrementFitness();

                        //mine found so replace the mine with another at a random 
                        //position
                        mines[GrabHit] = new Vector(Utils.RandomDouble() * clientWidth, Utils.RandomDouble() * clientHeight);
                    }

                    //update the chromos fitness score
                    population[i].Fitness = sweepers[i].Fitness();
                }
            }
            else
            {
                //Another generation has been completed.
                //Time to run the GA and update the sweepers with their new NNs

                //update the stats to be used in our stat window
                averageFitness.Add(geneticAlgorithm.AverageFitness());
                bestFitness.Add(geneticAlgorithm.BestFitness());

                //statistics to debug window
                System.Diagnostics.Debug.WriteLine("Epoch {0} - Fitness Avg / Best: {1,8:N2} / {2,7:N}", generations, averageFitness[generations], bestFitness[generations]);

                //increment the generation counter
                ++generations;

                //reset cycles
                ticks = 0;

                //run the GA to create a new population
                population = geneticAlgorithm.Epoch(population);

                //insert the new (hopefully)improved brains back into the sweepers
                //and reset their positions etc
                for (int i = 0; i < sweepersCount; ++i)
                {
                    sweepers[i].PutWeights(population[i].Weights);
                    sweepers[i].Reset();
                }
            }

            return true;
        }

        /// <summary>Fasts the render.</summary>
        /// <returns></returns>
        public bool FastRender()
        {
            return fastRender;
        }

        /// <summary>Fasts the render.</summary>
        /// <param name="arg">if set to <c>true</c> [arg].</param>
        public void FastRender(bool arg)
        {
            fastRender = arg;
        }

        /// <summary>Fasts the render toggle.</summary>
        public void FastRenderToggle()
        {
            fastRender = !fastRender;
        }

        #endregion

        #region Private Methods

        /// <summary>Plots a graph of the average and best fitnesses over the course of a run.</summary>
        /// <param name="surface">The surface.</param>
        private void PlotStats(Graphics surface)
        {
            string s = string.Format("Best Fitness: {0}", geneticAlgorithm.BestFitness());
            surface.DrawString(s, font, bluePen.Brush, 5, 20);

            s = string.Format("Average Fitness: {0}",geneticAlgorithm.AverageFitness());
            surface.DrawString(s, font, bluePen.Brush, 5, 40);

            //render the graph
            float HSlice = (float)clientWidth / (generations + 1);
            float VSlice = (float)clientHeight / (((float)geneticAlgorithm.BestFitness() + 1) * 2);

            //plot the graph for the best fitness
            /*
            float x = 0;
            int i = 0;

            MoveToEx(surface, 0, cyClient, NULL);

            for (i = 0; i < m_vecBestFitness.size(); ++i)
            {
                LineTo(surface, x, cyClient - VSlice * m_vecBestFitness[i]);

                x += HSlice;
            }

            //plot the graph for the average fitness
            x = 0;

            SelectObject(surface, m_BluePen);

            MoveToEx(surface, 0, cyClient, NULL);

            for (i = 0; i < m_vecAvFitness.size(); ++i)
            {
                LineTo(surface, (int)x, (int)(cyClient - VSlice * m_vecAvFitness[i]));

                x += HSlice;
            }
             */ 
        }

        #endregion
    }
}
