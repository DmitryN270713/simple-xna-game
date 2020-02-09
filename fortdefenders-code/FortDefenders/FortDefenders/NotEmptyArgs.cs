using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FortDefenders
{
    public sealed class NotEmptyArgs : EventArgs
    {
        public Point Position { get; private set; }
        public CellState State { get; private set; }

        public NotEmptyArgs(Point Position, CellState State)
        {
            this.Position = Position;
            this.State = State;
        }
    }
}
