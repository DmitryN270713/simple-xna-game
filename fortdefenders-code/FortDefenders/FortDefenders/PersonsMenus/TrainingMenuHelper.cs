using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders.PersonsMenus
{
    public sealed class TrainingMenuHelper
    {
        private static Dictionary<SoldierRole, TypeOfSoldiers> invokationList;

        public static void FillList(params Action[] listOfMethods)
        {
            if (invokationList == null)
            {
                invokationList = new Dictionary<SoldierRole, TypeOfSoldiers>()
                {
                    {SoldierRole.Swordsman, new TypeOfSoldiers(listOfMethods[0])     },
                    {SoldierRole.Archer, new TypeOfSoldiers(listOfMethods[1])       },
                    {SoldierRole.BatteringRam, new TypeOfSoldiers(listOfMethods[2]) }
                };
            }
        }

        public static Dictionary<SoldierRole, TypeOfSoldiers> GetListOfEvents()
        {
            return invokationList;
        }
    }
}
