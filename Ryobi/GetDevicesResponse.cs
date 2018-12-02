using System;

namespace gdo.Ryobi
{
    public class GetDeviceResponseSys
    {
        public long? lastSeen { get; set; }
    }

    public class GetDeviceResponseMetaData
    {
        public string name { get; set; }
        public double? version { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public WskAuthAttempt[] wskAuthAttempts { get; set; }
        public int? authCount { get; set; }
        public GetDeviceResponseSys sys { get; set; }
        public string socketId { get; set; }
    }

    public class GetDeviceResult
    {
        public string _id { get; set; }
        public string varName { get; set; }
        public GetDeviceResponseMetaData metaData { get; set; }
        public bool enabled { get; set; }
        public bool deleted { get; set; }
        public DateTimeOffset? createdDate { get; set; }
        public int? activated { get; set; }
        public string[] deviceTypeIds { get; set; }
        public DateTimeOffset? activatedDate { get; set; }
    }

    public class GetDeviceResponse
    {
        public GetDeviceResult[] result { get; set; }
    }

}