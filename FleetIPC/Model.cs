using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FleetIPC
{
    [DataContract]
    public class IPCMessage
    {
        public IPCMessage()
        {
            this.Content = new Dictionary<String, String>();
            this.IsRemoteMessage = false;
        }

        [DataMember]
        public String ApplicaitonSenderID { get; set; }

        [DataMember]
        public String ApplicationRecipientID { get; set; }

        [DataMember]
        public Dictionary<String,String> Content { get; set; }
        
        [DataMember]
        public bool IsRemoteMessage { get; internal set; }

    }
}
