using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    public sealed class Cell
    {
        public CellState state { get; set; }
        public Int16 ind1 { get; set; }
        public Int16 ind2 { get; set; }
        public Int16 ind3 { get; set; }
        public Int16 ind4 { get; set; }
        public Int16 startX { get; set; }
        public Int16 startZ { get; set; }



        public Cell(Int16 ind1, Int16 ind2, Int16 ind3, Int16 ind4)
        {
            this.ind1 = ind1;
            this.ind2 = ind2;
            this.ind3 = ind3;
            this.ind4 = ind4;
            //the cell suppose to be empty. Thus, the construction coordinate is (-1, -1)
            this.startX = -1;
            this.startZ = -1;

            this.state = CellState.Free;
        }
    }
}
