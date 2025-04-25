using System;
using System.Collections.Concurrent;
using System.Linq;
using ClientC2;

namespace ServerC2
{
    public class ChannelManager<T> where T : class, IDisposable
    {
        private readonly ConcurrentDictionary<BeaconId, T> _channels =
            new ConcurrentDictionary<BeaconId, T>();

        public ConcurrentDictionary<BeaconId, T> GetAll()
        {
            return _channels;
        }

        public T GetChannelById(BeaconId id)
        {
            return _channels.FirstOrDefault(p => p.Key.ToString() == id.ToString()).Value;
        }

        public BeaconId GetId(T channel)
        {
            return _channels.FirstOrDefault(p => p.Value == channel).Key;
        }
        public BeaconId AddChannel(BeaconId id, T channel)
        {
            _channels.TryAdd(id, channel);
            return GetId(channel);
        }
        public void RemoveChannel(BeaconId id)
        {
            _channels.TryRemove(id, out var socket);
            socket.Dispose();
        }
    }
}