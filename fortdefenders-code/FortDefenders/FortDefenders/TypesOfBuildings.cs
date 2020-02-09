using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    public delegate void Action<T1, T2>(T1 t1, params T2[] list);

    public sealed class TypesOfBuildings
    {
        private Int16 width;
        private Int16 height;
        private Type type;
        private Int16 neededWorkers;
        private Action<Type, Int16> methodToInvoke;
        private Func<Boolean> checker;
        private Func<Int16, Boolean> isEnoughWorkers;
        private Action<Int16> freeWorkers;

        public TypesOfBuildings(Int16 neededWorkers, Int16 width, Int16 height, 
                                Type type, Action<Type, Int16> methodToInvoke, Func<Boolean> checker, 
                                Func<Int16, Boolean> isEnoughWorkers, Action<Int16> freeWorkers)
        {
            this.width = width;
            this.height = height;
            this.type = type;
            this.methodToInvoke = methodToInvoke;
            this.checker = checker;
            this.isEnoughWorkers = isEnoughWorkers;
            this.neededWorkers = neededWorkers;
            this.freeWorkers = freeWorkers;
        }

        public Boolean StartCheckerInvokation()
        {
            return this.checker();
        }

        public void StartConstructBuildingInvokation(Int16 X, Int16 Z)
        {
            this.methodToInvoke.Invoke(this.type, X, Z, this.width, this.height);
        }

        public Boolean IsEnoughWorkersInvokation()
        {
            return this.isEnoughWorkers.Invoke(neededWorkers);
        }

        public void FreeWorkersInvocation()
        {
            this.freeWorkers.Invoke(neededWorkers);
        }
    }
}
