using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FortDefenders.PathFinder
{
    public interface INode
    {
        Int32 F { get; set; }
        Int32 G { get; set; }
        Int32 H { get; set; }
        PathNode Parent { get; set; }
        Point CellCoordinates { get; set; }
    }
}
