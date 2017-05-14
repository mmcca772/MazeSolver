using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    class Block
    {
        #region Properties

        //public struct Types
        //{
        //    Char START = 'S';
        //    Char END = 'E';
        //    Char WALL = 'X';
        //    Char OPEN = 'O';
        //}

        public string Type = "";
        public int Row = 0;
        public int Column = 0;
        public bool IsPath = false;
        public bool IsChecked = false;

        #endregion

        #region Constructor

        public Block()
        {

        }

        public Block(string type, int column, int row)
        {
            this.Type = type;
            this.Column = column;
            this.Row = row;
        }
        #endregion
    }
}
