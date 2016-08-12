using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FleetIPC
{
    [DataContract]
    public class IPCMessage
    {
        [DataMember]
        public String ApplicaitonSenderID { get; set; }

        [DataMember]
        public String ApplicationRecipientID { get; set; }

        [DataMember]
        public String Content { get; set; }
        // Having content as a JSON string is a very javascript concept.
        // It would probably be better to have a Dictionary<String, String>
        // Easier to use, will not have to parse ever message.
        // More inline with C#, and same concept. 
    }
}
