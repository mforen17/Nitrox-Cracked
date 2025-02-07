﻿using System.Collections.Generic;
using NitroxClient.Debuggers;
using NitroxModel.Packets;

namespace NitroxClient.Communication
{
    // TODO: Spinlocks don't seem to be necessary here, but I don't know for certain.
    public class PacketReceiver
    {
        private readonly INetworkDebugger networkDebugger;
        private readonly Queue<Packet> receivedPackets;

        public PacketReceiver(INetworkDebugger networkDebugger = null)
        {
            receivedPackets = new Queue<Packet>();
            this.networkDebugger = networkDebugger;
        }

        public void PacketReceived(Packet packet, int byteSize)
        {
            lock (receivedPackets)
            {
                networkDebugger?.PacketReceived(packet, byteSize);
                receivedPackets.Enqueue(packet);
            }
        }

        public Queue<Packet> GetReceivedPackets()
        {
            Queue<Packet> packets = new Queue<Packet>();

            lock (receivedPackets)
            {
                while (receivedPackets.Count > 0)
                {
                    packets.Enqueue(receivedPackets.Dequeue());
                }
            }

            return packets;
        }
    }
}
