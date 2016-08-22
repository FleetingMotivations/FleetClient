using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FleetIPC
{
    [DataContract]
    public class IPCMessage
    {
        public enum MessageLocationHandle
        {
            LOCAL,
            REMOTE,
            DAEMON
        };

        public IPCMessage()
        {
            this.Content = new Dictionary<String, String>();
        }

        [DataMember]
        public String ApplicaitonSenderID { get; set; }

        [DataMember]
        public String ApplicationRecipientID { get; set; }

        [DataMember]
        public Dictionary<String,String> Content { get; set; }

        [DataMember]
        public MessageLocationHandle LocationHandle { get; set; }

        [DataMember]
        public String Type { get; set; }

    }
}
