using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //List<string> maze = GetMaze("");
                Maze maze = new Maze(args[0]);

                maze.solveMaze(maze, maze.StartBlock);

                maze.AppendSolution();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            
        }

        public static List<string> GetMaze(string theFile)
        {
            List<string> mazeArray = new List<string>();

            int counter = 0;
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(@"C:\Projects\MazeSolver\MazeSolver\MazeFiles\Test.txt");
            //"C:\\Projects\\MazeSolver\\MazeSolver\\MazeFiles\\Test.txt"
            while ((line = file.ReadLine()) != null)
            {
                mazeArray.Add(line);

                Console.WriteLine(line);
                counter++;
            }

            file.Close();

            // Suspend the screen.
            //Console.ReadLine();

            return mazeArray;
        }
    }
}
