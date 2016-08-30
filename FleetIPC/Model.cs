using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FleetIPC
{
    [DataContract]
    public class IPCMessage
    {

        public enum MessageTarget
        {
            Local,
            Remote,
            Daemon
        };

        public IPCMessage()
        {
            this.Content = new Dictionary<string, string>();
        }

        [DataMember]
        public string ApplicaitonSenderID { get; set; }

        [DataMember]
        public string ApplicationRecipientID { get; set; }

        [DataMember]
        public Dictionary<string, string> Content { get; set; }

        [DataMember]
        public MessageTarget Target { get; set; }

        [DataMember]
        public String Type { get; set; }

        [DataMember]
        public bool SkipSelector { get; set; }

    }

}
