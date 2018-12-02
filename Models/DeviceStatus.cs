namespace gdo.Models
{
    public class DeviceStatus
    {
        public bool? DoorIsOpen { get; set; }
        public bool? LightIsOn { get; set; }
        public long? ModuleId { get; set; }
        public long? PortId { get; set; }
    }
}