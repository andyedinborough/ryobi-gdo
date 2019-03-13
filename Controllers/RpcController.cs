using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace gdo.Controllers
{
    public class RpcController : Controller
    {
        private readonly RpcService _rpcService;

        public RpcController(RpcService rpcService)
        {
            _rpcService = rpcService;
        }

        [Route("rpc/device/{machine}")]
        public string GetNextCommand(string machine)
            => _rpcService.GetCommand(machine) ?? "noop";

        [Route("rpc/device/{machine}/pic")]
        public async Task ReceivePic(string machine)
        {
            var file = _rpcService.GetPicFilePath(machine);
            using (var rdr = new StreamReader(Request.Body))
            {
                var b64 = await rdr.ReadToEndAsync();
                var data = Convert.FromBase64String(b64);
                System.IO.File.WriteAllBytes(file, data);
            }
        }

        [Route("rpc/device/{machine}/pic-path")]
        public IActionResult PicPath(string machine)
        {
            var file = _rpcService.GetPicFilePath(machine);
            return Content(file, "text/plain");
        }

        [Route("rpc/device/{machine}/pic.jpg")]
        public async Task<IActionResult> ViewPic(string machine)
        {
            try
            {
                var file = _rpcService.GetPicFilePath(machine);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
                _rpcService.SetCommand(machine, "pic");
                var start = DateTime.Now;
                while (!System.IO.File.Exists(file))
                {
                    if ((DateTime.Now - start).TotalSeconds > 60)
                    {
                        return NotFound();
                    }
                    await Task.Delay(1000);
                }
                var data = System.IO.File.ReadAllBytes(file);
                return File(data, "image/jpg");
            }
            catch (Exception ex)
            {
                return Content(ex.ToString(), "text/plain");
            }
        }
    }
}