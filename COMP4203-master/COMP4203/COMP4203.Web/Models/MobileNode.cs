using COMP4203.Web.Controllers;
using COMP4203.Web.Controllers.Hubs;
using System;
using System.Collections.Generic;

namespace COMP4203.Web.Models
{
	public class MobileNode
    {
        static int TRANSMIT_COST = 2;
        static int RECEIVE_PROCESS_COST = 1;

        public string FillColour { get; set; }
        public int BorderWidth { get; set; }
        public string StrokeColour { get; set; }
        public int Radius { get; set; }

        static int nodeCount = 0;
        static int range = 200;
        private int nodeID;
        public int BatteryLevel;
        public int CenterX, CenterY;
        private Dictionary<int, List<RoutingPacket>> knownRoutes;
        public Guid Id;
        public int CanvasIndex;
        private int ac; // Altruistic coefficient
        private bool drop; // Indicate whether or not this node dropped the packet

        public MobileNode()
        {
            nodeID = nodeCount++;
            BatteryLevel = 100;
            CenterX = CenterY = 0;
            knownRoutes = new Dictionary<int, List<RoutingPacket>>();
            Id = Guid.NewGuid();
            CanvasIndex = -1;
            BorderWidth = 2;
            StrokeColour = "#FFFFFF";
            Radius = 10;
            ac = 50;
            drop = false;
        }

        public MobileNode(int x, int y, int bLevel)
        {
            nodeID = ++nodeCount;
            BatteryLevel = 100;
            CenterX = x;
            CenterY = y;
            BatteryLevel = bLevel; knownRoutes = new Dictionary<int, List<RoutingPacket>>();
            Id = Guid.NewGuid();
            CanvasIndex = -1;
            BorderWidth = 2;
            StrokeColour = "#FFFFFF";
            Radius = 10;
            ac = 50;
            drop = false;
        }

        public int GetNodeID()
        {
            return nodeID;
        }

        public int GetBatteryLevel()
        {
            return BatteryLevel;
        }

        public int GetXPosition()
        {
            return CenterX;
        }

        public int GetYPosition()
        {
            return CenterY;
        }
        
        public void Print()
        {
            new OutputPaneController().PrintToOutputPane("Note", "Node #" + nodeID + " - Battery Level: " + BatteryLevel +
                " - Location: " + CenterX + "," + CenterY);
        }

        public void PrintNodesWithinRange(SimulationEnvironment env)
        {
            foreach (MobileNode n in env.GetNodes())
            {
                if (!this.Equals(n))
                {
                    if (IsWithinRangeOf(n))
                    {
                        new OutputPaneController().PrintToOutputPane("Node_Range", "Node " + n.GetNodeID() + " is within range. Distance: " + GetDistance(n));
                    }
                    else
                    {
                        new OutputPaneController().PrintToOutputPane("Node_Range", "Node " + n.GetNodeID() + " is not within range. Distance: " + GetDistance(n));
                    }
                }
            }
        }

        public double GetDistance(MobileNode node)
        {
            return Math.Sqrt((Math.Pow((CenterX - node.CenterX), 2)) + (Math.Pow((CenterY - node.CenterY), 2)));
        }

        // Retrieve node's altruistic coefficient
        public int GetAC()
        {
            return ac;
        }

        // Retrieve drop boolean
        public bool GetDrop()
        {
            return drop;
        }

        public void TransmitPacket()
        {
            BatteryLevel -= TRANSMIT_COST;
        }

        public void ReceiveProcessPacket()
        {
            BatteryLevel -= RECEIVE_PROCESS_COST;
        }

        public bool IsWithinRangeOf(MobileNode node)
        {
            return (GetDistance(node) < 200);
        }

        public List<MobileNode> GetNodesWithinRange(SimulationEnvironment env)
        {
            List<MobileNode> nodes = new List<MobileNode>();
            foreach (MobileNode node in env.GetNodes())
            {
                if (IsWithinRangeOf(node) && !node.Equals(this))
                {
                    nodes.Add(node);
                }
            }
            return nodes;
        }
        // Added SA-DSR here
        public List<RoutingPacket> RouteDiscoverySADSR(MobileNode destNode, SimulationEnvironment env)
        {
            new OutputPaneController().PrintToOutputPane("SADSR", "Performing Route Discovery from Node " + nodeID + " to Node " + destNode.GetNodeID() + ".");
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDiscovery(destNode, env, rPacket);
            routes = CalcAltruisticSADSR(routes);
            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (RoutingPacket r in routes)
                {
                    bool exists = false;
                    foreach (RoutingPacket r2 in knownRoutes[destNode.GetNodeID()])
                    {
                        if (r2.RouteCompare(r))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        knownRoutes[destNode.GetNodeID()].Add(r);
                    }
                }
            }
            else
            {
                knownRoutes.Add(destNode.GetNodeID(), routes);
            }
            return routes;
        }
        // Calculate altruistic coefficient of source node during SA-DSR route discovery 
        private List<RoutingPacket> CalcAltruisticSADSR(List<RoutingPacket> routes)
        {
            foreach (RoutingPacket r in routes)
            {
                foreach (MobileNode node in r.GetNodeRoute())
                {
                    if (node.PacketDrop() == true)
                    {
                        if (r.GetNodeRoute()[0].ac >= 0)
                        {
                            new OutputPaneController().PrintToOutputPane("SADSR", nodeID + " has dropped a packet");
                            node.drop = true;
                            r.GetNodeRoute()[0].ac -= 10;
                        }
                    }
                    else
                    {
                        if (r.GetNodeRoute()[0].ac <= 100)
                        {
                            new OutputPaneController().PrintToOutputPane("SADSR", nodeID + " has successfully transmitted a packet");
                            node.drop = false;
                            r.GetNodeRoute()[0].ac += 10;
                        }
                    }
                }
            }
            return routes;
        }
        // Get the optimal route for SA-DSR
        public RoutingPacket GetOptimalRouteSADSR(MobileNode node)
        {
            List<RoutingPacket> routes = GetRoutesToNode(node);
            RoutingPacket optRoute = new RoutingPacket();
            double sdp = 0;
            if (routes == null) { return null; }
            foreach (RoutingPacket r in routes)
            {
                r.CalcSDP();
            }
            foreach (RoutingPacket r in routes)
            {
                if (sdp < r.getSDP())
                {
                    sdp = r.getSDP();
                    optRoute = r;
                }
            }
            return optRoute;
        }
        // Calculates the probability of a node dropping a packet based on their battery levels
        private bool PacketDrop()
        {
            Random random = new Random();
            // 50% chance of dropping packet if battery level is between 20 to 50 percent
            if (20 <= this.BatteryLevel && this.BatteryLevel < 50)
            {
                int r = random.Next(0, 1);
                if (r == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 10% chance of dropping packet if battery level is between 50 and 80 percent
            else if (50 <= this.BatteryLevel && this.BatteryLevel < 80)
            {
                int r = random.Next(0, 11);
                if (r == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 0% chance of dropping packet if battery level is between 80 to 100 percent
            else if (80 <= this.BatteryLevel && this.BatteryLevel <= 100)
            {
                return false;
            }
            else
            {
                // 100% chance of dropping packet if battery level is less than 20 percent 
                return true;
            }
        }
        // Add modified SA-DSR here
        public List<RoutingPacket> RouteDiscoveryMSADSR(MobileNode destNode, SimulationEnvironment env)
        {
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDiscovery(destNode, env, rPacket);
            routes = TwoAck(routes);
            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (RoutingPacket r in routes)
                {
                    bool exists = false;
                    foreach (RoutingPacket r2 in knownRoutes[destNode.GetNodeID()])
                    {
                        if (r2.RouteCompare(r))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        knownRoutes[destNode.GetNodeID()].Add(r);
                    }
                }
            }
            else
            {
                knownRoutes.Add(destNode.GetNodeID(), routes);
            }
            return routes;
        }
        // Implement TWOACK
        private List<RoutingPacket> TwoAck(List<RoutingPacket> routes)
        {
            foreach (RoutingPacket r in routes)
            {
                for (int i = 0; i < r.GetNodeRoute().Count; i++)
                {
                    if (i >= 2)
                    {
                        // If the middle node drops the acknowledgement during TWOACK then this node misbehaves
                        if (r.GetNodeRoute()[i - 1].PacketDrop() == true)
                        {
                            if (!r.GetMisbehavedNodes().Contains(r.GetNodeRoute()[i - 1]))
                            {
                                r.AddNodeToMisbehaved(r.GetNodeRoute()[i - 1]);
                            }
                        }
                    }
                }
            }
            return routes;
        }
        // Get the route with the least number of selfish nodes
        public RoutingPacket GetLeastSelfishRouteMSADSR(MobileNode node)
        {
            List<RoutingPacket> routes = GetRoutesToNode(node);
            RoutingPacket optRoute = new RoutingPacket();
            int minMisbehavedNodes = 999999;
            if (routes == null) { return null; }
            foreach (RoutingPacket r in routes)
            {
                if (minMisbehavedNodes >= r.GetMisbehavedNodes().Count)
                {
                    minMisbehavedNodes = r.GetMisbehavedNodes().Count;
                    optRoute = r;
                }
            }
            return optRoute;
        }
        // Add DSR here
        public List<RoutingPacket> RouteDiscoveryDSR(MobileNode destNode, SimulationEnvironment env)
        {
            new OutputPaneController().PrintToOutputPane("DSR", "Performing Route Discovery from Node " + nodeID + " to Node " + destNode.GetNodeID() + ".");
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDiscovery(destNode, env, rPacket);
            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (RoutingPacket r in routes)
                {
                    bool exists = false;
                    foreach (RoutingPacket r2 in knownRoutes[destNode.GetNodeID()])
                    {
                        if (r2.RouteCompare(r))
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                    {
                        knownRoutes[destNode.GetNodeID()].Add(r);
                    }
                }
            }
            else
            {
                knownRoutes.Add(destNode.GetNodeID(), routes);
            }
            return routes;
        }

        private List<RoutingPacket> DSRDiscovery(MobileNode destNode, SimulationEnvironment env, RoutingPacket route)
        {
            List<RoutingPacket> routes = new List<RoutingPacket>();

            if (knownRoutes.ContainsKey(destNode.GetNodeID()))
            {
                foreach (RoutingPacket r in knownRoutes[destNode.GetNodeID()])
                {
                    RoutingPacket r2 = route.Copy();
                    r2.AddNodesToRoute(r.GetNodeRoute());
                    routes.Add(r2);
                }
                return routes;
            }

            List<MobileNode> nodesWithinRange = GetNodesWithinRange(env);
            if (nodesWithinRange.Count == 0 && !destNode.Equals(this)) { return null; }

            foreach (MobileNode node in nodesWithinRange)
            {
                // If node isn't in route yet...
                if (!route.IsInRouteAlready(node))
                {
                    // If node is the destination node...
                    if (node.Equals(destNode))
                    {
                        //Obtaining all possible routes
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this); // Adding nodes to route
                        rPacket.AddNodeToRoute(node);
                        routes.Add(rPacket); // Adding all possible routes
                        new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, 500, env.RREQ_COLOUR);
                        new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID));
                        env.TransmitData(node, this, 500, env.RREP_COLOUR);
                    }
                    else
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID()));
                        env.TransmitData(this, node, 500, env.RREQ_COLOUR);
                        routes.AddRange(node.DSRDiscovery(destNode, env, rPacket)); // Recursive call
                    }
                }
            }
            foreach (RoutingPacket r in routes)
            {
                if (r.GetNodeRoute().Contains(destNode))
                {
                    List<MobileNode> rList = r.GetNodeRoute();
                    for (int i = 0; i < rList.Count; i++)
                    {
                        if (rList[i] == this && i != 0)
                        {
                            new OutputPaneController().PrintToOutputPane("DSR", string.Format("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i-1].GetNodeID()));
                            env.TransmitData(this, rList[i - 1], 500, env.RREP_COLOUR);
                        }
                    }
                    
                }
            }
            return routes;
        }

        public List<RoutingPacket> GetRoutesToNode(MobileNode node)
        {
            // If there are no known routes for this destination, return null.
            if (!knownRoutes.ContainsKey(node.GetNodeID())) { return null; }
            // Otherwise, return the list of known routes.
            return knownRoutes[node.GetNodeID()];
        }

        public RoutingPacket GetBestRouteDSR(MobileNode node)
        {
            List<RoutingPacket> routes = GetRoutesToNode(node);
            if (routes == null) { return null; }
            if (routes.Count == 0) { return null; }
            int lowestValue = 99999999;
            int lowestIndex = -1;
            for (int i = 0; i < routes.Count; i++)
            {
                int rLength = routes[i].GetRouteLength();
                if (rLength < lowestValue)
                {
                    lowestValue = rLength;
                    lowestIndex = i;
                }
            }
            return routes[lowestIndex];
        }
    }
}
