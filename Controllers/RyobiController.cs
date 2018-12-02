using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdo.Models;
using Microsoft.AspNetCore.Mvc;

namespace gdo.Controllers
{
    [Route("api/[controller]")]
    public class RyobiController : Controller
    {
        private RyobiClient _ryobi = new RyobiClient();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ryobi?.Dispose();
            }
        }

        [HttpPost("[action]")]
        public async Task<LoginResult> Login(string email, string password)
        {
            var response = await _ryobi.LoginAsync(email, password);
            return new LoginResult
            {
                ApiKey = response?.result?.auth?.apiKey,
                Cookie = _ryobi.Cookie,
            };
        }

        [HttpPost("[action]")]
        public async Task<IEnumerable<Device>> GetDevices(string cookie)
        {
            _ryobi.Cookie = cookie;
            var response = await _ryobi.GetDevicesAsync();
            var returned = response?.result?
                .Where(x => x.deleted != true
                    && x.enabled == true
                    && x.activated > 0
                    && !x.metaData.description.Contains("Hub"))
                .Select(Map);
            return returned;
        }

        [HttpPost("[action]")]
        public async Task<DeviceStatus> DeviceStatus(string cookie, string id)
        {
            _ryobi.Cookie = cookie;
            var response = await _ryobi.GetDeviceStatusAsync(id);

            var garageDoorModule = response.result[0].deviceTypeMap.Values
                .FirstOrDefault(x => _ryobi.HasModuleProfile(x, "garageDoor"));

            var status = new DeviceStatus
            {
                PortId = (long?)_ryobi.GetVariable(garageDoorModule, "portId")?.value,
                ModuleId = (long?)_ryobi.GetVariable(garageDoorModule, "moduleId")?.value,
                DoorIsOpen = (long?)_ryobi.GetVariableFromDevice(response.result[0], "doorState")?.value > 0,
                LightIsOn = (bool?)_ryobi.GetVariableFromDevice(response.result[0], "lightState")?.value,
            };
            return status;
        }


        [HttpPost("[action]")]
        public async Task<bool> Operate(string email, string apiKey, string deviceId, long moduleId, long portId, bool? door = null, bool? light = null)
        {
            var cmd = door != null
                ? _ryobi.Door(deviceId, moduleId, portId, door.Value)
                : _ryobi.Light(deviceId, moduleId, portId, light.Value);
            await _ryobi.SendCommandAsync(email, apiKey, cmd);
            return true;
        }

        [Route("[action]")]
        public async Task<bool> Act(string email, string password, string deviceName, string part, bool value)
        {
            var login = await Login(email, password);
            var devices = await GetDevices(_ryobi.Cookie);
            var device = devices.FirstOrDefault(x => x.Name == deviceName);
            var status = await DeviceStatus(_ryobi.Cookie, device.Id);

            switch (part)
            {
                case "door":
                    await Operate(email, login.ApiKey, device.Id, status.ModuleId.Value, status.PortId.Value, door: value);
                    break;

                case "light":
                    await Operate(email, login.ApiKey, device.Id, status.ModuleId.Value, status.PortId.Value, light: value);
                    break;
            }

            return true;
        }

        private static Device Map(gdo.Ryobi.GetDeviceResult dev) => new Device
        {
            Name = dev.metaData.name,
            Id = dev.varName,
        };
    }
}
