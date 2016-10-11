/* 
 * Description: IControlMessageManager
 *              Interface for the control message manager object
 * Project: Fleet/FleetDaemon
 * Last modified: 11 October 2016
 * Last Author: Hayden Cheers
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetServer;

namespace FleetDaemon
{
    /// <summary>
    /// Public interface for the control message amanger
    /// </summary>
    interface IControlMessageManager
    {
        /// <summary>
        /// Instruct the manager to handle any control message udpates
        /// </summary>
        /// <param name="token"></param>
        void HandleControlMessageUpdate(FleetClientToken token);
    }
}
