using System;

namespace ClientC2.interfaces
{
    public interface IC2Channel : IDisposable
    {
        bool Connected { get; }

        bool Connect();

        void Close();
        byte[] ReadFrame();

        void SendFrame(byte[] buffer);

        bool ReadAndSendTo(IC2Channel c2);


        byte[] GetStager(string pipeName, bool is64Bit, int taskWaitTime = 100);
    }
}
