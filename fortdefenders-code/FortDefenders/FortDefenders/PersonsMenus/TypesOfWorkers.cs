using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortDefenders.PersonsMenus
{
    public delegate void Action();

    public sealed class TypesOfWorkers
    {
        private Action recallRequest;
        private Action addRequest;

        public TypesOfWorkers(Action recallRequest, Action addRequest)
        {
            this.recallRequest = recallRequest;
            this.addRequest = addRequest;
        }

        public void InvokeRecallRequest()
        {
            this.recallRequest.Invoke();
        }

        public void InvokeAddRequest()
        {
            this.addRequest.Invoke();
        }
    }
}
