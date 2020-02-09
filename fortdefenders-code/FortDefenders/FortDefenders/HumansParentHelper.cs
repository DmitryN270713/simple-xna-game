using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders
{
    public sealed class HumansParentHelper
    {
        private static Dictionary<CellState, HumansRole> listOfPersons;

        public static Dictionary<CellState, HumansRole> GetList(Action<Type, Int16, Int16, CellState> objectBuilder)
        {
            if (listOfPersons == null)
            {
                listOfPersons = new Dictionary<CellState, HumansRole>()
                {
                    {CellState.Mayor, new HumansRole(typeof(Mayor), Mayor.START_X, Mayor.START_Z, CellState.Mayor, objectBuilder)},
                    {CellState.Knight, new HumansRole(typeof(Knight), Knight.START_X, Knight.START_Z, CellState.Knight, objectBuilder)},
                    {CellState.Archer, new HumansRole(typeof(Archer), Archer.START_X, Archer.START_Z, CellState.Archer, objectBuilder)},
                    {CellState.BatteringRam, new HumansRole(typeof(BatteringRam), BatteringRam.START_X, BatteringRam.START_Z, CellState.BatteringRam, objectBuilder)}
                };
            }

            return listOfPersons;
        }

    }
}
