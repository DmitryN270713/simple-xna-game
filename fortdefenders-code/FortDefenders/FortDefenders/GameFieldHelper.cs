using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FortDefenders.CityHall;
using FortDefenders.BatteringRamRangeFort;

namespace FortDefenders
{
    public sealed class GameFieldHelper
    {
        private static Dictionary<CellState, NeededResources> resourcesNeeded;

        private GameFieldHelper()
        { }

        public static Dictionary<CellState, NeededResources> GetResourcesNeeded(Func<Int16, Int16, Int16, Boolean> checker, Action<Int16, Int16, Int16> setter)
        {
            if (resourcesNeeded == null)
            {
                resourcesNeeded = new Dictionary<CellState, NeededResources>()
                {
                    {CellState.House, new NeededResources(-House.WOOD, -House.STONE, -House.GOLD, checker, setter)},
                    {CellState.CityHall, new NeededResources(-Cityhall.WOOD, -Cityhall.STONE, -Cityhall.GOLD, checker, setter)},
                    {CellState.Windmill, new NeededResources(-Windmill.WOOD, -Windmill.STONE , -Windmill.GOLD, checker, setter)},
                    {CellState.Bank, new NeededResources(-Bank.WOOD, -Bank.STONE, -Bank.GOLD, checker, setter)},
                    {CellState.Marketplace, new NeededResources(-Market.WOOD, -Market.STONE, -Market.GOLD, checker, setter)},
                    {CellState.Barracks, new NeededResources(-Barracks.Barracks.WOOD, -Barracks.Barracks.STONE, -Barracks.Barracks.GOLD, checker, setter)},
                    {CellState.ArcheryRange, new NeededResources(-ArcheryRange.ArcheryRange.WOOD, -ArcheryRange.ArcheryRange.STONE, -ArcheryRange.ArcheryRange.GOLD, checker, setter)},
                    {CellState.BatteringRamRange, new NeededResources(-BatteringRamRange.WOOD, -BatteringRamRange.STONE, -BatteringRamRange.GOLD, checker, setter)},
                };
            }

            return resourcesNeeded;
        }
    }
}

