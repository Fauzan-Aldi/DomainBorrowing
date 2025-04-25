using System;

namespace ClientC2
{
   
    public struct BeaconId
    {
        public int CobaltStrikeId;

        public Guid InternalId;

        public override string ToString()
            => $"{CobaltStrikeId}_{InternalId}";
    }
}
