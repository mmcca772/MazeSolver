using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace MazeSolver
{
    class Maze
    {
        #region Properties

        public string File = "";
        public List<string> UnsolvedMaze = new List<string>();
        public List<string> SolvedMaze = new List<string>();
        public List<Block> Blocks = new List<Block>();
        public Block StartBlock = new Block();

        public List<List<Block>> Paths = new List<List<Block>>();
        public int Width = 3;
        public int Height = 0;
        public int PathLength = 0;

        #endregion

        #region Constructors

        public Maze(string file)
        {
            this.File = file;
            CreateMaze(this.File);
            if (Blocks != null)
                SetStartBlock();
        }
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void SetStartBlock()
        {
            var S = Blocks.Where(b => b.Type == "S").ToList();
            if (S.Count == 1)
            {
                StartBlock = S[0];
                StartBlock.IsChecked = true;
            }
            else if (S.Count == 0)
                //throw exception
                throw new Exception("Maze Structural Error: " + "No Starting point");
            else
                //throw exception for too many S points
                throw new Exception("Maze Structural Error: " + S.Count + " starts, maze should only have 1");
        }

        /// <summary>
        /// reads in text file to find maze and maps each
        /// character (X,S,E,space) into a grid - collection of Block 
        /// objects
        /// this function also sets the maze objects width and height
        /// </summary>
        /// <param name="theFile">String</param>
        public void CreateMaze(string theFile)
        {
            int counter = 0;
            string line;
            bool start = false;
            bool end = false;

            // Read the file 
            if (!System.IO.File.Exists(theFile))
            {
                throw new FileNotFoundException("File Not Found Error");
            }
            StreamReader file =
               new StreamReader(theFile);

            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                //int xCount = line.Count(x => x == 'X');

                while (!start && !end)
                {

                    if (IsHorizantalWall(line))
                    {
                        this.Width = line.Length;
                        start = true;
                        if (this.Width < 3)
                            throw new InvalidDataException("Invalid Width Exception: Width Frame needs to be greater than 3");
                        if (this.Width > 255)
                            throw new InvalidDataException("Invalid Width Error: Width Frame needs to be less than 255");
                    }

                }

                if (start)
                {
                    //grid all blocks
                    for (int i = 0; i < line.Length; i++)
                    {
                        Block b = new Block();
                        b.Type = line[i].ToString().Trim();
                        if (b.Type.Equals(""))
                            b.Type = "O";
                        b.Row = counter;
                        b.Column = i;

                        this.Blocks.Add(b);
                    }

                    this.UnsolvedMaze.Add(line);
                    this.Height++;
                    
                    if (IsHorizantalWall(line) && this.Height > 2)
                        end = true;
                }

                if (end)
                {
                    break;
                }

                counter++;
            }

            file.Close(); 
        }

        /// <summary>
        /// helps find a horizantal wall frame for a maze in 
        /// a string. Compares if total characters of 'X' equal 
        /// the total length of string passed in
        /// </summary>
        /// <param name="line">string</param>
        /// <returns>bool</returns>
        public bool IsHorizantalWall(string line)
        {
            bool isWall = false;
            int xCount = line.Count(x => x == 'X');
            if (xCount == line.Length)
                isWall = true;

            return isWall;
        }

        /// <summary>
        /// finds all paths to solve the Maze. Firstly, finds
        /// the 'S', start point, and then goes into a recursive
        /// function to track all paths
        /// </summary>
        /// <param name="maze">Maze</param>
        /// <param name="block">Block</param>
        public void solveMaze(Maze maze, Block block)
        {
            //map blocks sorrounding S block passed in
            // migrate this query into built in function 'FindSorroundingOpenBlocks'
            int topRowBlock = block.Row - 1;
            int bottomRowBlock = block.Row + 1;
            int rightColumnBlock = block.Column + 1;
            int leftColumnBlock = block.Column - 1;
            //linq query to find open blocks around S block
            List<Block> openBlocksAroundS =
                maze.Blocks.Where(b => (!b.Type.Contains("X")) &&
                ((b.Row == topRowBlock && b.Column == block.Column) ||
                (b.Row == bottomRowBlock && b.Column == block.Column) ||
                (b.Column == leftColumnBlock && b.Row == block.Row) ||
                (b.Column == rightColumnBlock && b.Row == block.Row))).ToList();

            //set initial paths from start
            List<List<Block>> newPaths = new List<List<Block>>();
            int startPathCount = openBlocksAroundS.Count();
            foreach (Block b in openBlocksAroundS)
            {
                List<Block> path = new List<Block>();
                path.Add(b);
                newPaths.Add(path);
            }

            //recursive function
            for (int i = 0; i < startPathCount; i++)
            {
                newPaths = FindPaths(newPaths, newPaths[i]);
            }

            Paths = newPaths;
        }

        /// <summary>
        /// Recursive function
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="path"></param>
        /// <returns>paths List<List<Block>></returns>
        public List<List<Block>> FindPaths(List<List<Block>> paths, List<Block> path)
        {
            //get last block in path
            Block block = path.Last();

            block.IsChecked = true;

            if (block.Type.Equals("O"))
            {
                //find sorrounding blocks
                List<List<Block>> tempPaths = new List<List<Block>>();
                List<Block> openBlocks = FindSorroundingOpenBlocks(block);
                if (openBlocks.Count > 1)
                {
                    foreach (Block b in openBlocks)
                    {
                        //deep copy of paths at an intersection
                        List<Block> newPath = new List<Block>();
                        newPath = path.ConvertAll(el => new Block(el.Type, el.Column, el.Row));
                        tempPaths.Add(newPath);
                    }
                }
                if (openBlocks.Count > 0)
                {
                    for (int i = 0; i < openBlocks.Count; i++)
                    {
                        if (openBlocks[i].Type.Equals("O"))
                        {
                            if (i == 0)
                            {
                                path.Add(openBlocks[i]);
                                paths = FindPaths(paths, path);
                            }
                            else
                            {
                                List<Block> newPath = tempPaths[i];
                                newPath.Add(openBlocks[i]);
                                paths.Add(newPath);
                                paths = FindPaths(paths, newPath);
                            }
                        }
                        else
                        {
                            path.Add(openBlocks[i]);
                            paths = FindPaths(paths, path);
                        }

                    }
                }
                else
                {
                    //dead end: remove path form list
                    paths.Remove(path);
                }
            }
            else if (block.Type.Equals("E"))
            {
                block.IsPath = true;
                block.IsChecked = false;
            }

            return paths;
        }

        /// <summary>
        /// finds all open blocks, including 'E' blocks, around a 
        /// given block
        /// - LINQ query
        /// </summary>
        /// <param name="block"></param>
        /// <returns>List<Block></Block></returns>
        public List<Block> FindSorroundingOpenBlocks(Block block)
        {
            List<Block> blocks = new List<Block>();

            int topRowBlock = block.Row - 1;
            int bottomRowBlock = block.Row + 1;
            int rightColumnBlock = block.Column + 1;
            int leftColumnBlock = block.Column - 1;
            //linq query to find open blocks
            blocks =
                Blocks.Where(b => (!b.Type.Contains("X")) &&
                (b.IsChecked == false) &&
                ((b.Row == topRowBlock && b.Column == block.Column) ||
                (b.Row == bottomRowBlock && b.Column == block.Column) ||
                (b.Column == leftColumnBlock && b.Row == block.Row) ||
                (b.Column == rightColumnBlock && b.Row == block.Row))).ToList();

            return blocks;
        }

        /// <summary>
        /// 
        /// </summary>
        public void AppendSolution()
        {
            //replace solution path blocks with '.'
            foreach (Block block in Blocks)
            {
                foreach (Block pBlock in Paths[0])
                {
                    if (block.Row == pBlock.Row && block.Column == pBlock.Column
                        && block.Type != "E")
                        block.Type = ".";
                    else if (block.Type.Equals("O"))
                        block.Type = " ";
                }
                
            }

            using (StreamWriter sw =
                System.IO.File.AppendText(this.File))
            {
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("MazeSolver Solution:");
                sw.WriteLine();
                
                string line = "";
                foreach (Block b in Blocks)
                {
                    line += b.Type;
                    if (line.Length == this.Width)
                    {
                        sw.WriteLine(line);
                        line = "";
                    }
                    
                }
            }
        }

        #endregion
    }
}