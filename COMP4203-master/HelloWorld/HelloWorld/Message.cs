using System;

namespace SimulationProtocols
{
    class Message
    {
        static int messageCount = 0;
        private int messageID;
        private MobileNode sourceNode, destinationNode;

        public Message(MobileNode src, MobileNode dst)
        {
            messageID = ++messageCount;
            sourceNode = src;
            destinationNode = dst;
        }

        public MobileNode GetSourceNode() { return sourceNode; }
        public MobileNode GetDestinstationNode() { return destinationNode; }
        public int GetMessageID() { return messageID; }

        public void Print() => Console.WriteLine("Message from Node {0} to Node {1}.", sourceNode.GetNodeID(), destinationNode.GetNodeID());
    }
}
