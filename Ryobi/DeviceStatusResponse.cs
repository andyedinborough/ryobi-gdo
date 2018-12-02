using System;
using System.Collections.Generic;

namespace gdo.Ryobi
{
    public class AT
    {
        public object value { get; set; }
        public string _id { get; set; }
        public MetaData metaData { get; set; }
        public object defv { get; set; }
        public string dataType { get; set; }
        public string varType { get; set; }
        public string varName { get; set; }
        // public string[] flags { get; set; }
        public string[] @enum { get; set; }
        public object min { get; set; }
        public object max { get; set; }
    }
    public class Module
    {
        public MetaData metaData { get; set; }
        public object ac { get; set; }
        public IDictionary<string, AT> at { get; set; }
    }

    public class DeviceStatusResponseMetaData
    {
        public string name { get; set; }
        public string icon { get; set; }
    }

    public class DeviceStatusResult
    {
        public string _id { get; set; }
        public string varName { get; set; }
        public DeviceStatusResponseMetaData metaData { get; set; }
        public bool? enabled { get; set; }
        public bool? deleted { get; set; }
        public DateTimeOffset? createdDate { get; set; }
        public int? activated { get; set; }
        public string[] deviceTypeIds { get; set; }
        public IDictionary<string, Module> deviceTypeMap { get; set; }
        public DateTimeOffset? activatedDate { get; set; }
    }

    public class DeviceStatusResponse
    {
        public DeviceStatusResult[] result { get; set; }
    }

}