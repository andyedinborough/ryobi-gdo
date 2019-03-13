using Microsoft.Extensions.Caching.Memory;

namespace gdo
{
    public class RpcService
    {
        private readonly IMemoryCache _cache;

        public RpcService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GetCommand(string machine)
        {
            var key = GetCacheKey(machine);
            var cmd = _cache.Get<string>(key);
            _cache.Remove(key);
            return cmd;
        }

        public string GetPicFilePath(string machine)
        {
            var dir = System.IO.Path.GetTempPath();
            var filepath = System.IO.Path.Combine(dir, $"{machine.ToLower()}-pic.jpg");
            return filepath;
        }

        public void SetCommand(string machine, string cmd)
        {
            var key = GetCacheKey(machine);
            _cache.Set<string>(key, cmd);
        }

        private string GetCacheKey(string machine)
            => $"{nameof(RpcService)}:{machine}";
    }

}