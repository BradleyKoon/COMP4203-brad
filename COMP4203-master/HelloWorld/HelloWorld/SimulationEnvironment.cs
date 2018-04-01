
using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class SimulationEnvironment
    {
        private int height, width;
        private List<MobileNode> mobileNodes;
        private List<Message> messages;
        private List<SessionData> sessions;

        public SimulationEnvironment()
        {
            height = 500;
            width = 500;
            mobileNodes = new List<MobileNode>();
            messages = new List<Message>();
            sessions = new List<SessionData>();
        }

        public int GetHeight() { return height; }
        public int GetWidth() { return width; }
        public List<MobileNode> GetNodes() { return mobileNodes; }
        public List<Message> GetMessages() { return messages; }

        public void RunSimulation()
        {
            SessionData sessionData = new SessionData();

            foreach (Message message in messages)
            {
                SendMessageDSR(message);
            }

            sessions.Add(sessionData);
        }

        public void AddNode(MobileNode node)
        {
            mobileNodes.Add(node);
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);
        }

        public void GenerateRandomNodes(int numNodes)
        {
            Random random = new Random();
            for (int i = 0; i < numNodes; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                int bLevel = random.Next(100);
                mobileNodes.Add(new MobileNode(x, y, bLevel));
            }
        }

        public void GenerateRandomMessages(int numMessages)
        {
            Random random = new Random();
            for (int i = 0; i < numMessages; i++)
            {
                int srcIndex = random.Next(mobileNodes.Count);
                int dstIndex = srcIndex;
                while (dstIndex == srcIndex)
                {
                    dstIndex = random.Next(mobileNodes.Count);
                }
                messages.Add(new Message(mobileNodes[srcIndex], mobileNodes[dstIndex]));
            }
        }

        // Adding SendMessageSADSR here

        public bool SendMessageDSR(Message message)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            RoutingPacket route = sourceNode.GetBestRouteDSR(destinationNode);

            // If no known route found
            if (route == null)
            {
                Console.WriteLine("No Known Route to Destination.");
                sourceNode.RouteDiscoveryDSR(destinationNode, this); // Perform Route Discovery
                route = sourceNode.GetBestRouteDSR(destinationNode); // Attempt to assign newly found best route
                if (route == null)
                {
                    Console.WriteLine("No Route to Destination.");
                    return false;
                }
            }

            Console.WriteLine("Sending Message:");
            Console.WriteLine("Source Node: " + sourceNode.GetNodeID());
            Console.WriteLine("Destination Node: " + destinationNode.GetNodeID());
            Console.WriteLine("Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();
            Console.WriteLine("Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                Console.WriteLine("Sending Message from {0} to {1}.", nodes[i - 1].GetNodeID(), nodes[i].GetNodeID());
                nodes[i - 1].TransmitPacket();
                nodes[i].ReceiveProcessPacket();
            }
            Console.WriteLine("Received Message at Destination Node " + destinationNode.GetNodeID());
            return true;
        }
    }
}
