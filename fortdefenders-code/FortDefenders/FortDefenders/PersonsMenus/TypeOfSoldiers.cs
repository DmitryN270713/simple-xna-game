using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders.PersonsMenus
{
    public sealed class TypeOfSoldiers
    {
        private Action addRequest;

        public TypeOfSoldiers(Action addRequest)
        {
            this.addRequest = addRequest;
        }

        public void InvokeAddRequest()
        {
            this.addRequest.Invoke();
        }
    }
}
