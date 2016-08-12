using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace FleetIPC
{
    [ServiceContract]
    public interface IApplicationIPC
    {
       
        /*This Interface is what the application exposes */
        [OperationContract]
        void Inform(List<IPCMessage> messages);

        [OperationContract]
        void Deliver(IPCMessage message);
    }

    [ServiceContract]
    public interface IDaemonIPC
    {
        /*This interface is what the Daemon exposes */
        [OperationContract]
        void Request(IPCMessage message);

        // We may want a register and deregister method so the daemon knows that
        // applications exist, and will provide a way of knowing which pipe to use
        // to contact the application
    }
}
