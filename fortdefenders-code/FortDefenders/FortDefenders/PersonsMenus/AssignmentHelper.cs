using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders.PersonsMenus
{
    public class AssignmentHelper
    {
        private static Dictionary<WorkerRole, TypesOfWorkers> invokationList;

        public static void FillList(params Action[] listOfMethods)
        {
            if (invokationList == null)
            {
                invokationList = new Dictionary<WorkerRole, TypesOfWorkers>()
                {
                    {WorkerRole.Builder, new TypesOfWorkers(listOfMethods[0], listOfMethods[1])},
                    {WorkerRole.Lumberjack, new TypesOfWorkers(listOfMethods[2], listOfMethods[3])},
                    {WorkerRole.Miner, new TypesOfWorkers(listOfMethods[4], listOfMethods[5])},
                    {WorkerRole.Farmer, new TypesOfWorkers(listOfMethods[6], listOfMethods[7])},
                    {WorkerRole.Banker, new TypesOfWorkers(listOfMethods[8], listOfMethods[9])},
                    {WorkerRole.Seller, new TypesOfWorkers(listOfMethods[10], listOfMethods[11])}
                };
            }
        }

        public static Dictionary<WorkerRole, TypesOfWorkers> GetListOfEvents()
        {
            return invokationList;
        }
    }
}
