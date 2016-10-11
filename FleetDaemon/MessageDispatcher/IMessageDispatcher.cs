/* 
 * Description: IMessageDispatcher
 *              Interface for message dispatcher objects
 * Project: Fleet/FleetDaemon
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

using FleetIPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetDaemon.MessageDispatcher
{
    /// <summary>
    /// Interface for message dispatcher objects.
    /// Quite literally allows messages to be dispatched.
    /// </summary>
    public interface IMessageDispatcher
    {      
        void Dispatch(IPCMessage message);
    }
}
