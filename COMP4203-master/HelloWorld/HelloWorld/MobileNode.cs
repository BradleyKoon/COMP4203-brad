
using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class MobileNode
    {
        static int TRANSMIT_COST = 2;
        static int RECEIVE_PROCESS_COST = 1;

        static int nodeCount = 0;
        static int range = 200;
        private int nodeID;
        private int batteryLevel;
        private int xPosition, yPosition;
        private int ac;
        private Dictionary<int, List<RoutingPacket>> knownRoutes;
        private Dictionary<int, RoutingPacket> optimalRoute;

        public MobileNode()
        {
            nodeID = nodeCount++;
            batteryLevel = 100;
            xPosition = yPosition = 0;
            ac = 50; // Added altruism coefficient here
            knownRoutes = new Dictionary<int, List<RoutingPacket>>();
            optimalRoute = new Dictionary<int, RoutingPacket>(); // Optimal route to be used by SA-DSR
        }

        public MobileNode(int x, int y, int bLevel)
        {
            nodeID = ++nodeCount;
            xPosition = x;
            yPosition = y;
            ac = 50;
            batteryLevel = bLevel; knownRoutes = new Dictionary<int, List<RoutingPacket>>();
            optimalRoute = new Dictionary<int, RoutingPacket>();
        }
        // Get ac here
        public int GetAC()
        {
            return ac;
        }
        public int GetNodeID()
        {
            return nodeID;
        }

        public int GetBatteryLevel()
        {
            return batteryLevel;
        }

        public int GetXPosition()
        {
            return xPosition;
        }

        public int GetYPosition()
        {
            return yPosition;
        }

        public void Print()
        {
            Console.WriteLine("Node " + nodeID + ":");
            Console.WriteLine("Battery: " + batteryLevel + "%");
            Console.WriteLine("Location: " + xPosition + ", " + yPosition);
        }

        public void PrintNodesWithinRange(SimulationEnvironment env)
        {
            foreach (MobileNode n in env.GetNodes())
            {
                if (!this.Equals(n))
                {
                    if (IsWithinRangeOf(n))
                    {
                        Console.WriteLine("Node {0} is within range. Distance: {1}", n.GetNodeID(), GetDistance(n));
                    }
                    else
                    {
                        Console.WriteLine("Node {0} is not within range. Distance: {1}", n.GetNodeID(), GetDistance(n));
                    }
                }
            }
        }

        public double GetDistance(MobileNode node)
        {
            return Math.Sqrt((Math.Pow((xPosition - node.xPosition), 2)) + (Math.Pow((yPosition - node.yPosition), 2)));
        }

        public void TransmitPacket()
        {
            batteryLevel -= TRANSMIT_COST;
        }

        public void ReceiveProcessPacket()
        {
            batteryLevel -= RECEIVE_PROCESS_COST;
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
        // Adding SA-DSR here
        /*public RoutingPacket SADSRRouteDiscovery(MobileNode destNode, SimulationEnvironment env)
        {
            // In this protocol, we want to get the route that has the best SDP
            RoutingPacket rPacket = new RoutingPacket();
            RoutingPacket optRoute = new RoutingPacket();
            List<RoutingPacket> routes = DSRDicovery(destNode, env, rPacket);
            double sdp = 0;
            // Calculate the altruism coefficient from source to destination node
            foreach (RoutingPacket r in routes)
            {
                foreach (MobileNode node in r.GetNodeRoute())
                {
                    if (node.PacketDrop() == true)
                    {
                        if (r.GetNodeRoute()[0].ac >= 0)
                        {
                            r.GetNodeRoute()[0].ac -= 10;
                        }
                    }
                    else
                    {
                        if (r.GetNodeRoute()[0].ac <= 100)
                        {
                            r.GetNodeRoute()[0].ac += 10;
                        }
                    }
                }
            }
            // Calculate the SDP of all routes to destination
            foreach (RoutingPacket r in routes)
            {
                r.CalcSDP();
            }
            // Get the optimal route 
            foreach (RoutingPacket r in routes)
            {
                if (sdp < r.getSDP())
                {
                    sdp = r.getSDP();
                    optRoute = r;
                }
            }
            // Print SDP of optimal route
            foreach (RoutingPacket r in routes)
            {
                Console.WriteLine("SDP of route: {0}", r.getSDP());
            }
            if (optimalRoute.ContainsKey(destNode.GetNodeID()))
            {
                optimalRoute[destNode.GetNodeID()] = optRoute;
            }
            else
            {
                optimalRoute.Add(destNode.GetNodeID(), optRoute);
            }
            return optRoute;
        }*/
        // Added SA-DSR here
        public List<RoutingPacket> RouteDiscoverySADSR(MobileNode destNode, SimulationEnvironment env)
        {
            Console.WriteLine("Performing Route Discovery from Node {0} to Node {1}.", nodeID, destNode.GetNodeID());
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDicovery(destNode, env, rPacket);
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
        public List<RoutingPacket> CalcAltruisticSADSR(List<RoutingPacket> routes)
        {
            foreach (RoutingPacket r in routes)
            {
                foreach (MobileNode node in r.GetNodeRoute())
                {
                    if (node.PacketDrop() == true)
                    {
                        if (r.GetNodeRoute()[0].ac >= 0)
                        {
                            r.GetNodeRoute()[0].ac -= 10;
                        }
                    }
                    else
                    {
                        if (r.GetNodeRoute()[0].ac <= 100)
                        {
                            r.GetNodeRoute()[0].ac += 10;
                        }
                    }
                }
            }
            return routes;
        }
        // Get the optimal route for SA-DSR
        public RoutingPacket optimalRouteSADSR(MobileNode node)
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
        public List<RoutingPacket> RouteDiscoveryDSR(MobileNode destNode, SimulationEnvironment env)
        {
            Console.WriteLine("Performing Route Discovery from Node {0} to Node {1}.", nodeID, destNode.GetNodeID());
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDicovery(destNode, env, rPacket);
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

        private List<RoutingPacket> DSRDicovery(MobileNode destNode, SimulationEnvironment env, RoutingPacket route)
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
                        Console.WriteLine("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID());
                        TransmitPacket();
                        node.ReceiveProcessPacket();
                        Console.WriteLine("Sending RREP from Node {0} to Node {1}.", node.GetNodeID(), nodeID);
                        node.TransmitPacket();
                        ReceiveProcessPacket();
                    }
                    else
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        Console.WriteLine("Sending RREQ from Node {0} to Node {1}.", nodeID, node.GetNodeID());
                        TransmitPacket();
                        node.ReceiveProcessPacket();
                        routes.AddRange(node.DSRDicovery(destNode, env, rPacket)); // Recursive call
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
                            Console.WriteLine("Sending RREP from Node {0} to Node {1}.", nodeID, rList[i-1].GetNodeID());
                            TransmitPacket();
                            rList[i - 1].GetNodeID();
                        }
                    }
                    
                }
            }
            return routes;
        }

        public bool dSendMessage(Message message, RoutingPacket route)
        {

            Console.WriteLine("Routing Packet Selected: {0}", route.GetRouteAsString());
            List<MobileNode> nodes = route.GetNodeRoute();
            Console.WriteLine("Beginning Message Transmission from Source Node " + nodes[0].GetNodeID());
            for (int i = 1; i < nodes.Count; i++)
            {
                Console.WriteLine("Sending Message from {0} to {1}.", nodes[i - 1].GetNodeID(), nodes[i].GetNodeID());
            }
            Console.WriteLine("Received Message at Destination Node " + nodes[nodes.Count - 1].GetNodeID());
            return true;
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
            int lowestValue = 999999;
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

        // Calculates the probability of a node dropping a packet based on their battery levels
        private bool PacketDrop()
        {
            Random random = new Random();
            // 50% chance of dropping packet if battery level is between 20 to 50 percent
            if (20 <= this.batteryLevel && this.batteryLevel < 50)
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
            else if (50 <= this.batteryLevel && this.batteryLevel < 80)
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
            else if (80 <= this.batteryLevel && this.batteryLevel <= 100)
            {
                return false;
            }
            else
            {
                // 100% chance of dropping packet if battery level is less than 20 percent 
                return true;
            }
        }
    }
}
