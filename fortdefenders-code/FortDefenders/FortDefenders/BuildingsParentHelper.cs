using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FortDefenders.CityHall;
using FortDefenders.BatteringRamRangeFort;

namespace FortDefenders
{
    public sealed class BuildingsParentHelper
    {
        private static Dictionary<CellState, TypesOfBuildings> listOfTypesOfBuildingss;

        private BuildingsParentHelper()
        {

        }

        public static Dictionary<CellState, TypesOfBuildings> GetListOfTypesOfBuildings(Action<Type, Int16> methodPassed, 
                                                                    Func<Boolean> checker, Func<Int16, Boolean> isEnoughWorkers,
                                                                    Action<Int16> freeWorkers)
        {
            if (listOfTypesOfBuildingss == null)
            {
                listOfTypesOfBuildingss = new Dictionary<CellState, TypesOfBuildings>()
                {
                    {CellState.House, new TypesOfBuildings(House.NEEDS_HUMANS_TO_BUILD, House.WIDTH, House.HEIGHT, typeof(House), methodPassed, checker, isEnoughWorkers, freeWorkers)},
                    {CellState.Windmill, new TypesOfBuildings(Windmill.NEEDS_HUMANS_TO_BUILD, Windmill.WIDTH, Windmill.HEIGHT, typeof(Windmill), methodPassed, checker, isEnoughWorkers, freeWorkers) },
                    {CellState.Bank, new TypesOfBuildings(Bank.NEEDS_HUMANS_TO_BUILD, Bank.WIDTH, Bank.HEIGHT, typeof(Bank), methodPassed, checker, isEnoughWorkers, freeWorkers)},
                    {CellState.Marketplace, new TypesOfBuildings(Market.NEEDS_HUMANS_TO_BUILD, Market.WIDTH, Market.HEIGHT, typeof(Market), methodPassed, checker, isEnoughWorkers, freeWorkers) },
                    {CellState.CityHall, new TypesOfBuildings(Cityhall.NEEDS_HUMANS_TO_BUILD, Cityhall.WIDTH, Cityhall.HEIGHT, typeof(Cityhall), methodPassed, checker, isEnoughWorkers, freeWorkers)},
                    {CellState.Barracks, new TypesOfBuildings(Barracks.Barracks.NEEDS_HUMANS_TO_BUILD, Barracks.Barracks.WIDTH, Barracks.Barracks.HEIGHT, typeof(Barracks.Barracks), methodPassed, checker, isEnoughWorkers, freeWorkers)},
                    {CellState.ArcheryRange, new TypesOfBuildings(ArcheryRange.ArcheryRange.NEEDS_HUMANS_TO_BUILD, ArcheryRange.ArcheryRange.WIDTH, ArcheryRange.ArcheryRange.HEIGHT, typeof(ArcheryRange.ArcheryRange), methodPassed, checker, isEnoughWorkers, freeWorkers)},
                    {CellState.BatteringRamRange, new TypesOfBuildings(BatteringRamRange.NEEDS_HUMANS_TO_BUILD, BatteringRamRange.WIDTH, BatteringRamRange.HEIGHT, typeof(BatteringRamRange), methodPassed, checker, isEnoughWorkers, freeWorkers)}
                };
            }

            return listOfTypesOfBuildingss;
        }
    }
}
