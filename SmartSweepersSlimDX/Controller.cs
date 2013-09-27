using SlimDX;
using SlimDX.Direct2D;
using SmartSweepersSlimDX.AI;
using SmartSweepersSlimDX.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SlimDX.DirectWrite;

namespace SmartSweepersSlimDX
{
    public class Controller
    {
        #region Private Variables

        /// <summary>The num sweeper verts</summary>
        private const int NumSweeperVerts = 16;

        /// <summary>The sweeper</summary>
        private List<System.Drawing.PointF> sweeper = new List<System.Drawing.PointF>
        { 
            new System.Drawing.PointF(-1, -1),
	        new System.Drawing.PointF(-1, 1),
	        new System.Drawing.PointF(-0.5f, 1),
	        new System.Drawing.PointF(-0.5f, -1),

	        new System.Drawing.PointF(0.5f, -1),
	        new System.Drawing.PointF(1, -1),
	        new System.Drawing.PointF(1, 1),
	        new System.Drawing.PointF(0.5f, 1),

	        new System.Drawing.PointF(-0.5f, -0.5f),
	        new System.Drawing.PointF(0.5f, -0.5f),

	        new System.Drawing.PointF(-0.5f, 0.5f),
	        new System.Drawing.PointF(-0.25f, 0.5f),
	        new System.Drawing.PointF(-0.25f, 1.75f),
	        new System.Drawing.PointF(0.25f, 1.75f),
	        new System.Drawing.PointF(0.25f, 0.5f),
	        new System.Drawing.PointF(0.5f, 0.5f)
        };

        /// <summary>The num mine verts</summary>
        private const int NumMineVerts = 4;

        /// <summary>The mine</summary>
        private List<System.Drawing.PointF> mine = new List<System.Drawing.PointF>
        {
            new System.Drawing.PointF(-1, -1),
	        new System.Drawing.PointF(-1, 1),
	        new System.Drawing.PointF(1, 1),
	        new System.Drawing.PointF(1, -1)
        };

        /// <summary>Storage for the population of genomes.</summary>
        private List<Genome> population = new List<Genome>();

        /// <summary>Storage for the minesweepers.</summary>
        private List<Minesweeper> sweepers = new List<Minesweeper>();

        /// <summary>Storage for the mines.</summary>
        private List<Vector2> mines = new List<Vector2>();

        /// <summary>The Genetic algorithm.</summary>
        private GeneticAlgorithm geneticAlgorithm;

        /// <summary>The count of sweepers.</summary>
        private int sweepersCount;

        /// <summary>The count of mines.</summary>
        private int minesCount;

        /// <summary>The count of weights in NN.</summary>
        private int weightsInNN;

        /// <summary>Vertex buffer for the sweeper shape's vertices.</summary>
        private List<System.Drawing.PointF> sweeperVertexBuffer = new List<System.Drawing.PointF>();

        /// <summary>Vertex buffer for the mine shape's vertices.</summary>
        private List<System.Drawing.PointF> mineVertexBuffer = new List<System.Drawing.PointF>();

        /// <summary>Stores the average fitness per generation for use in graphing.</summary>
        private List<double> averageFitness = new List<double>();

        /// <summary>Stores the best fitness per generation.</summary>
        private List<double> bestFitness = new List<double>();

        private Color4 redColor;

        private Color4 blueColor;

        private Color4 greenColor;

        /// <summary>The font</summary>
        //private Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular);

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

        private WindowRenderTarget renderTarget;
        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="Controller"/> class.</summary>
        public Controller(WindowRenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;

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
                mines.Add(new Vector2(
                    (float)(Utils.RandomDouble() * clientWidth),
                    (float)(Utils.RandomDouble() * clientHeight)));
            }

            //create a pen for the graph drawing
            blueColor = new Color4(System.Drawing.Color.DeepSkyBlue);
            redColor = new Color4(System.Drawing.Color.MistyRose);
            greenColor = new Color4(System.Drawing.Color.Aquamarine);

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
        public void Render()
        {
            //render the stats
            //surface.DrawString(string.Format("Generation: {0}", generations), font, bluePen.Brush, 10, 10);

            int i = 0;

            string stats = string.Format("Generation: {0}\nBest Fitness: {1}\nAverage Fitness: {2:0.00}\nCycles: {3}",
                generations, 
                geneticAlgorithm.BestFitness(), 
                geneticAlgorithm.AverageFitness(), 
                ticks);

            using (var factory = new SlimDX.DirectWrite.Factory())
            using (var format = new TextFormat(factory, "Arial", FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, 14, "arial"))
            using (var brush = new SolidColorBrush(renderTarget, blueColor))
            {
                renderTarget.DrawText(stats, format, new System.Drawing.Rectangle(5, 5, 300, 100), brush);
            }

            //do not render if running at accelerated speed
            if (!fastRender)
            {
                //render the mines
                for (i = 0; i < minesCount; ++i)
                {
                    //grab the vertices for the mine shape
                    var mineVB = mineVertexBuffer.ToArray();

                    WorldTransform(mineVB, mines[i]);

                    using (var brush = new SolidColorBrush(renderTarget, greenColor))
                    {
                        //draw the mines
                        renderTarget.DrawRectangle(brush, new System.Drawing.RectangleF(
                            (float)mineVB[0].X,
                            (float)mineVB[0].Y,
                            (float)mineVB[2].X - (float)mineVB[0].X,
                            (float)mineVB[2].Y - (float)mineVB[0].Y));
                    }
                }

                //render the sweepers
                for (i = 0; i < sweepersCount; i++)
                {
                    var color = blueColor;

                    if (i <= Params.Instance.NumElite)
                    {
                        color = redColor;
                    }

                    //grab the sweeper vertices
                    var sweeperVB = sweeperVertexBuffer.ToArray();

                    //transform the vertex buffer
                    sweepers[i].WorldTransform(sweeperVB);

                    if (i == 0)
                    {
                        var msg = string.Format("[{0:0.0} , {1:0.0}] {2:0.0} {3:0.0} [{4:0.0} , {5:0.0}]",
                            sweepers[i].Position().X,
                            sweepers[i].Position().Y,
                            sweepers[i].speed,
                            sweepers[i].rotation,
                            sweepers[i].lookAt.X,
                            sweepers[i].lookAt.Y);

                        using (var factory = new SlimDX.DirectWrite.Factory())
                        using (var format = new TextFormat(factory, "Arial", FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, 14, "arial"))
                        using (var brush = new SolidColorBrush(renderTarget, blueColor))
                        {
                            renderTarget.DrawText(msg, format, new System.Drawing.Rectangle(5, 570, 300, 30), brush);
                        }
                    }

                    //draw the sweeper's left track
                    using (var geometry = new PathGeometry(renderTarget.Factory))
                    {
                        using (GeometrySink sink = geometry.Open())
                        {
                            sink.BeginFigure(sweeperVB[0], FigureBegin.Filled);
                            sink.AddLines(sweeperVB.Take(4).ToArray());
                            sink.AddLine(sweeperVB[0]);
                            sink.EndFigure(FigureEnd.Closed);
                            sink.Close();
                        }

                        using (var brush = new SolidColorBrush(renderTarget, color))
                        {
                            renderTarget.FillGeometry(geometry, brush);
                        }
                    }

                    //draw the sweeper's right track
                    using (var geometry = new PathGeometry(renderTarget.Factory))
                    {
                        using (GeometrySink sink = geometry.Open())
                        {
                            sink.BeginFigure(sweeperVB[4], FigureBegin.Filled);
                            sink.AddLines(sweeperVB.Skip(4).Take(4).ToArray());
                            sink.AddLine(sweeperVB[4]);
                            sink.EndFigure(FigureEnd.Closed);
                            sink.Close();
                        }

                        using (var brush = new SolidColorBrush(renderTarget, color))
                        {
                            renderTarget.FillGeometry(geometry, brush);
                        }
                    }

                    //draw the sweeper's body
                    using (var geometry = new PathGeometry(renderTarget.Factory))
                    {
                        using (GeometrySink sink = geometry.Open())
                        {
                            sink.BeginFigure(sweeperVB[8], FigureBegin.Filled);
                            sink.AddLines(sweeperVB.Skip(7).ToArray());
                            sink.EndFigure(FigureEnd.Closed);
                            sink.Close();
                        }

                        using (var brush = new SolidColorBrush(renderTarget, color))
                        {
                            renderTarget.FillGeometry(geometry, brush);
                        }
                    }
                }
            }
            else
            {
                PlotStats();
            }
        }

        /// <summary>
        /// Sets up the translation matrices for the mines and applies the world transform to each vertex 
        /// in the vertex buffer passed to this method..
        /// </summary>
        /// <param name="VBuffer">The V buffer.</param>
        /// <param name="vPos">The v pos.</param>
        public void WorldTransform(System.Drawing.PointF[] VBuffer, Vector2 vPos)
        {
            var transformMatrix = new System.Drawing.Drawing2D.Matrix();
            transformMatrix.Scale((float)Params.Instance.MineScale, (float)Params.Instance.MineScale, System.Drawing.Drawing2D.MatrixOrder.Append);
            transformMatrix.Translate((float)vPos.X, (float)vPos.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
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
                        mines[GrabHit] = new Vector2(
                            (float)(Utils.RandomDouble() * clientWidth),
                            (float)(Utils.RandomDouble() * clientHeight));
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
        private void PlotStats()
        {
            //render the graph
            float HSlice = (float)clientWidth / (generations + 1);
            float VSlice = (float)clientHeight / (((float)geneticAlgorithm.BestFitness() + 1) * 2);

            //plot the graph for the best fitness
            float x = 0;
            float y = clientHeight;
            int i = 0;

            for (i = 0; i < bestFitness.Count; ++i)
            {
                var tempX = x;
                var tempY = (float)(clientHeight - VSlice * bestFitness[i]);
                x += HSlice;

                using (var brush = new SolidColorBrush(renderTarget, redColor))
                {
                    renderTarget.DrawLine(brush, tempX, y, x, tempY);
                }

                y = tempY;
            }

            //plot the graph for the average fitness
            x = 0;
            y = clientHeight;

            for (i = 0; i < averageFitness.Count; ++i)
            {
                var tempX = x;
                var tempY = (float)(clientHeight - VSlice * averageFitness[i]);
                x += HSlice;

                using (var brush = new SolidColorBrush(renderTarget, greenColor))
                {
                    renderTarget.DrawLine(brush, tempX, y, x, tempY);
                }

                y = tempY;
            }
        }

        #endregion
    }
}
