using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace COMP4203.Web.Models
{
    public class SimulationEnvironment
    {
        private int height, width;
        private List<MobileNode> mobileNodes;
        private List<Message> messages;
        private List<SessionData> sessions;

        public string RREQ_COLOUR = "#9bc146";
        public string RREP_COLOUR = "#ffe338";
        public string DATA_COLOUR = "#52a0d0";
        public string ACK_COLOUR = "#df1313";

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
            new OutputPaneController().PrintToOutputPane("DSR", "Finished Transmitting Messages.");
            new OutputPaneController().PrintToOutputPane("DSR", messages.Count + " messages transmitted.");
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

        // Add send message SADSR here
        public bool SendMessageSADSR(Message message)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            RoutingPacket route = sourceNode.GetOptimalRouteSADSR(destinationNode); // Getting the route with the highest SDP

            // If no known route found
            if (route == null)
            {
                new OutputPaneController().PrintToOutputPane("SADSR", "No Known Route to Destination.");
                sourceNode.RouteDiscoverySADSR(destinationNode, this);
                route = sourceNode.GetOptimalRouteSADSR(destinationNode);
                if (route == null)
                {
                    new OutputPaneController().PrintToOutputPane("DSR", "No Route to Destination.");
                    return false;
                }
            }

            new OutputPaneController().PrintToOutputPane("SADSR", "Sending Message:");
            new OutputPaneController().PrintToOutputPane("SADSR", "Source Node: " + sourceNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("SADSR", "Destination Node: " + destinationNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("SADSR", "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();
            new OutputPaneController().PrintToOutputPane("SADSR", "Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                new OutputPaneController().PrintToOutputPane("SADSR", "Sending Message from " + nodes[i - 1].GetNodeID() + " to " + nodes[i].GetNodeID() + ".");
                TransmitData(nodes[i - 1], nodes[i], 2000, DATA_COLOUR);
            }
            new OutputPaneController().PrintToOutputPane("SADSR", "Received Message at Destination Node " + destinationNode.GetNodeID());

            new OutputPaneController().PrintToOutputPane("SADSR", "Beginning ACK Transmission from Destination Node " + destinationNode.GetNodeID());
            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                new OutputPaneController().PrintToOutputPane("SADSR", "Sending ACK from " + nodes[i + 1].GetNodeID() + " to " + nodes[i].GetNodeID());
                TransmitData(nodes[i + 1], nodes[i], 500, ACK_COLOUR);
            }
            new OutputPaneController().PrintToOutputPane("SADSR", "Received ACK at Source Node " + sourceNode.GetNodeID());
            return true;
        }
        // Add send message modified SADSR here 
        public bool SendMessageMSADSR(Message message)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            RoutingPacket route = sourceNode.GetLeastSelfishRouteMSADSR(destinationNode); // Getting the route with the least number of selfish nodes

            // If no known route found
            if (route == null)
            {
                new OutputPaneController().PrintToOutputPane("MSADSR", "No Known Route to Destination.");
                sourceNode.RouteDiscoveryMSADSR(destinationNode, this);
                route = sourceNode.GetLeastSelfishRouteMSADSR(destinationNode);
                if (route == null)
                {
                    new OutputPaneController().PrintToOutputPane("MSADSR", "No Route to Destination.");
                    return false;
                }
            }

            new OutputPaneController().PrintToOutputPane("MSADSR", "Sending Message:");
            new OutputPaneController().PrintToOutputPane("MSADSR", "Source Node: " + sourceNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("MSADSR", "Destination Node: " + destinationNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("MSADSR", "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();
            new OutputPaneController().PrintToOutputPane("MSADSR", "Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                new OutputPaneController().PrintToOutputPane("MSADSR", "Sending Message from " + nodes[i - 1].GetNodeID() + " to " + nodes[i].GetNodeID() + ".");
                TransmitData(nodes[i - 1], nodes[i], 2000, DATA_COLOUR);
            }
            new OutputPaneController().PrintToOutputPane("MSADSR", "Received Message at Destination Node " + destinationNode.GetNodeID());

            new OutputPaneController().PrintToOutputPane("MSADSR", "Beginning ACK Transmission from Destination Node " + destinationNode.GetNodeID());
            for (int i = nodes.Count - 2; i >= 0; i--)
            {
                new OutputPaneController().PrintToOutputPane("MSADSR", "Sending ACK from " + nodes[i + 1].GetNodeID() + " to " + nodes[i].GetNodeID());
                TransmitData(nodes[i + 1], nodes[i], 500, ACK_COLOUR);
            }
            new OutputPaneController().PrintToOutputPane("MSADSR", "Received ACK at Source Node " + sourceNode.GetNodeID());
            return true;
        }
        // Add send message DSR here 
        public bool SendMessageDSR(Message message)
        {
            MobileNode sourceNode = message.GetSourceNode();
            MobileNode destinationNode = message.GetDestinstationNode();
            RoutingPacket route = sourceNode.GetBestRouteDSR(destinationNode);

            // If no known route found
            if (route == null)
            {
                new OutputPaneController().PrintToOutputPane("DSR", "No Known Route to Destination.");
                sourceNode.RouteDiscoveryDSR(destinationNode, this); // Perform Route Discovery
                route = sourceNode.GetBestRouteDSR(destinationNode); // Attempt to assign newly found best route
                if (route == null)
                {
                    new OutputPaneController().PrintToOutputPane("DSR", "No Route to Destination.");
                    return false;
                }
            }

            new OutputPaneController().PrintToOutputPane("DSR", "Sending Message:");
            new OutputPaneController().PrintToOutputPane("DSR", "Source Node: " + sourceNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("DSR", "Destination Node: " + destinationNode.GetNodeID());
            new OutputPaneController().PrintToOutputPane("DSR", "Route Chosen: " + route.GetRouteAsString());

            List<MobileNode> nodes = route.GetNodeRoute();
            new OutputPaneController().PrintToOutputPane("DSR", "Beginning Message Transmission from Source Node " + sourceNode.GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                new OutputPaneController().PrintToOutputPane("DSR", "Sending Message from " + nodes[i - 1].GetNodeID() + " to " + nodes[i].GetNodeID() + ".");
                TransmitData(nodes[i - 1], nodes[i], 2000, DATA_COLOUR);
            }
            new OutputPaneController().PrintToOutputPane("DSR", "Received Message at Destination Node " + destinationNode.GetNodeID());

            new OutputPaneController().PrintToOutputPane("DSR", "Beginning ACK Transmission from Destination Node " + destinationNode.GetNodeID());
            for (int i = nodes.Count-2; i >=0; i--)
            {
                new OutputPaneController().PrintToOutputPane("DSR", "Sending ACK from " + nodes[i + 1].GetNodeID() + " to " + nodes[i].GetNodeID());
                TransmitData(nodes[i + 1], nodes[i], 500, ACK_COLOUR);
            }
            new OutputPaneController().PrintToOutputPane("DSR", "Received ACK at Source Node " + sourceNode.GetNodeID());
            return true;
        }

        public void TransmitData(MobileNode srcNode, MobileNode dstNode, int wait, string colour)
        {
            new OutputPaneController().PrintArrow(srcNode, dstNode, colour);
            Thread.Sleep(wait);
            srcNode.TransmitPacket();
            dstNode.ReceiveProcessPacket();
            new OutputPaneController().UpdateBatteryLevel(srcNode);
            new OutputPaneController().UpdateBatteryLevel(dstNode);
        }
    }
}
