using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationProtocols
{
    class Message
    {
        static int messageCount = 0;
        private int messageID;
        private MobileNode sourceNode, destinationNode;
        private double msgSpeed = 0;
        public Message(MobileNode src, MobileNode dst)
        {
            messageID = ++messageCount;
            sourceNode = src;
            destinationNode = dst;
        }

        public MobileNode GetSourceNode() { return sourceNode; }
        public MobileNode GetDestinstationNode() { return destinationNode; }
        public int GetMessageID() { return messageID; }
        public double GetMsgSpeed() { return msgSpeed; }
        public void calcMsgSpeed(MobileNode src, MobileNode dst)
        {
            Random rand = new Random();
            int time = rand.Next(50, 101);
            msgSpeed = src.GetDistance(dst) / time;
        }

        public void Print() => Console.WriteLine("Message from Node {0} to Node {1}.", sourceNode.GetNodeID(), destinationNode.GetNodeID());
    }
}
