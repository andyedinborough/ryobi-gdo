using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using gdo.Models;
using gdo.Ryobi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gdo
{
    public class RyobiClient : IDisposable
    {
        static long _uuid = 1;

        public RyobiClient(string cookie = null)
        {
            Cookie = cookie;
        }

        private HttpClient http = new HttpClient();

        public string Cookie { get; set; }

        public void Dispose()
        {
            http.Dispose();
        }

        public object Door(string deviceId, long moduleId, long portId, bool open)
            => GenerateCommand(deviceId, moduleId, portId, new { doorCommand = open ? 1 : 0 });

        public object Light(string deviceId, long moduleId, long portId, bool lightState)
            => GenerateCommand(deviceId, moduleId, portId, new { lightState });

        public async Task SendCommandAsync(string email, string apiKey, object command)
        {
            using (var websocket = await ConnectAsync(email, apiKey))
            {
                var json = JsonConvert.SerializeObject(Authorize(email, apiKey));
                await SendAsync(websocket, json);
                var response = await ReceiveAsync(websocket);
                Console.WriteLine(response);

                json = JsonConvert.SerializeObject(command);
                await SendAsync(websocket, json);
                response = await ReceiveAsync(websocket);
                response = await ReceiveAsync(websocket);
                Console.WriteLine(response);
            }
        }

        private object Authorize(string email, string apiKey) => new
        {
            jsonrpc = "2.0",
            id = 3,
            method = "srvWebSocketAuth",
            @params = new { varName = email, apiKey }
        };

        public static async Task SendAsync(WebSocket socket, string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task<string> ReceiveAsync(WebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var mem = new MemoryStream())
            {
                WebSocketReceiveResult result = null;
                while (result?.EndOfMessage != true)
                {
                    result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    mem.Write(buffer.Array, buffer.Offset, result.Count);
                }
                var response = Encoding.UTF8.GetString(mem.ToArray());
                return response;
            }
        }

        private async Task<WebSocket> ConnectAsync(string email, string apiKey)
        {
            var mre = new ManualResetEventSlim();
            var websocket = new ClientWebSocket();
            websocket.Options.AddSubProtocol("echo-protocol");
            await websocket.ConnectAsync(new Uri("wss://tti.tiwiconnect.com/api/wsrpc"), CancellationToken.None);
            //var text = await ReceiveAsync(websocket);
            return websocket;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                return await PostJsonToJsonAsync<LoginResponse>("https://tti.tiwiconnect.com/api/login", new
                {
                    password,
                    email
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private object GenerateCommand(string deviceId, long moduleId, long portId, object moduleMsg)
            => new
            {
                jsonrpc = "2.0",
                method = "gdoModuleCommand",
                id = Interlocked.Increment(ref _uuid),
                @params = new
                {
                    msgType = 16,
                    moduleType = moduleId,
                    portId,
                    moduleMsg,
                    topic = deviceId
                }
            };

        private async Task<T> PostJsonToJsonAsync<T>(string url, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            if (!string.IsNullOrEmpty(Cookie))
            {
                request.Headers.TryAddWithoutValidation("Cookie", Cookie);
            }
            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            using (var res = await http.SendAsync(request))
            {
                var json = await res.Content.ReadAsStringAsync();
                if (res.Headers.TryGetValues("Set-Cookie", out var values) && string.IsNullOrEmpty(Cookie))
                {
                    Cookie = values.FirstOrDefault();
                }
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        private async Task<T> GetJsonAsync<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(Cookie))
            {
                request.Headers.TryAddWithoutValidation("Cookie", Cookie.Split(';')[0]);
            }
            using (var res = await http.SendAsync(request))
            {
                var json = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    Error = (x, xe) =>
                    {
                        Console.WriteLine(xe);
                    }
                });
            }
        }

        public async Task<GetDeviceResponse> GetDevicesAsync()
        {
            try
            {
                return await GetJsonAsync<GetDeviceResponse>("https://tti.tiwiconnect.com/api/endnodes");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public AT GetVariableFromDevice(DeviceStatusResult device, string name)
        {
            return device.deviceTypeMap
                .Values
                .Select(x => GetVariable(x, name))
                .FirstOrDefault(x => x != null);
        }

        public AT GetVariable(Module module, string name)
        {
            var variable = module.at.Values.FirstOrDefault(x => x.varName == name);
            if (variable != null)
            {
                //System.Diagnostics.Debugger.Break();
            }
            return variable;
        }

        public T GetVariableValue<T>(Module module, string name)
        {
            var value = GetVariable(module, name)?.value;
            if (value is JObject jobj)
            {
                return jobj.ToObject<T>();
            }
            else if (value is JArray arr)
            {
                return arr.ToObject<T>();
            }
            return (T)value;
        }

        public bool HasModuleProfile(Module module, string profile)
        {
            var profiles = GetVariableValue<string[]>(module, "moduleProfiles");
            if (profiles == null)
            {
                return false;
            }
            return profiles.Any(x => x.StartsWith(profile + "_"));
        }

        public async Task<DeviceStatusResponse> GetDeviceStatusAsync(string id)
        {
            try
            {
                return await GetJsonAsync<DeviceStatusResponse>($"https://tti.tiwiconnect.com/api/devices/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}