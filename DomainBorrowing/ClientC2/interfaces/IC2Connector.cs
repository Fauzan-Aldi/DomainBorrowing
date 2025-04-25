using System;

namespace ClientC2.interfaces
{
    public interface IC2Connector
    {
        
        bool Started { get; }

        IC2Channel BeaconChannel { get; }

        IC2Channel ServerChannel { get; }

        int Sleep { get; }

        Func<bool> Initialize { get; }

        void Go();

        void Stop();
    }
}
