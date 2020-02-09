using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    public class NeededResources
    {
        private Func<Int16, Int16, Int16, Boolean> checker;
        private Action<Int16, Int16, Int16> setter;
        private Int16 wood;
        private Int16 stone;
        private Int16 gold;


        public NeededResources(Int16 wood, Int16 stone, Int16 gold, Func<Int16, Int16, Int16, Boolean> checker, Action<Int16, Int16, Int16> setter)
        {
            this.wood = wood;
            this.stone = stone;
            this.gold = gold;
            this.checker = checker;
            this.setter = setter;
        }

        public Boolean StartInvokation()
        {
            return this.checker.Invoke(wood, stone, gold);
        }

        public void StartSetterInvokation()
        {
            this.setter.Invoke(wood, stone, gold);
        }
    }
}
