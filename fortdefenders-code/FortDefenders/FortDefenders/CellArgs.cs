using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    public sealed class CellArgs : EventArgs 
    {
        public Int16 X { get; private set; }
        public Int16 Z { get; private set; }

        public CellArgs(Int16 X, Int16 Z)
        {
            this.X = X;
            this.Z = Z;
        }
    }
}
