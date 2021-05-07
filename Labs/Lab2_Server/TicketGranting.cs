using System;

namespace Lab2_Server
{
    [Serializable]
    class TicketGranting
    {
        public TicketGranting() { }
        public string ClientIdentity { get; set; }
        public string ServiceIdentity { get; set; }
        public DateTime IssuingTime { get; set; }
        public long Duration { get; set; }
        public string Key { get; set; }
    }
}
