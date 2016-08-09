using System;
using System.Collections.Generic;
using System.Text;

namespace FleetIPC
{
    public delegate void DaemonEvent(IPCMessage message);

    public class DaemonService:IDaemonIPC
    {
        public static event DaemonEvent OnRequest = delegate { };

        public void Request(IPCMessage message)
        {
            OnRequest(message);
        }
    }

    public delegate void ApplicationInformEvent(List<IPCMessage> message);
    public delegate void ApplicationDeliverEvent(IPCMessage message);

    public class ApplicationService:IApplicationIPC
    {
        public static event ApplicationInformEvent OnInform = delegate { };
        public static event ApplicationDeliverEvent OnDeliver = delegate { };

        public void Inform(List<IPCMessage> messages)
        {
            OnInform(messages);
        }

        public void Deliver(IPCMessage message)
        {
            OnDeliver(message);
        }
    }
}
