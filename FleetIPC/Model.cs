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

    }
}
