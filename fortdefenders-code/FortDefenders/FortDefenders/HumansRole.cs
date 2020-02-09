using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    public sealed class HumansRole
    {
        private Type type;
        private Action<Type, Int16, Int16, CellState> objectBuilder;
        private Int16 X;
        private Int16 Z;
        private CellState role;

        public HumansRole(Type type, Int16 X, Int16 Z, CellState role, Action<Type, Int16, Int16, CellState> objectBuilder)
        {
            this.type = type;
            this.X = X;
            this.Z = Z;
            this.objectBuilder = objectBuilder;
            this.role = role;
        }

        public void InvokeObjectCreation()
        {
            this.objectBuilder.Invoke(this.type, this.X, this.Z, role);
        }
    }
}
