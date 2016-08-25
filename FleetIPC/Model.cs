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
        public MessageLocationHandle LocationHandle { get; set; }

        [DataMember]
        public MessageType Type { get; set; }

    }

    public enum MessageType
    {
        
    }
}
