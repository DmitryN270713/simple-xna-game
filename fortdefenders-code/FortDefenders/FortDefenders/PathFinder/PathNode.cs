using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FortDefenders.PathFinder
{
    public sealed class PathNode : INode
    {
        public Int32 F { get; set; }
        public Int32 G { get; set; }
        public Int32 H { get; set; }
        public PathNode Parent { get; set; }
        public Point CellCoordinates { get; set; }
    }
}
