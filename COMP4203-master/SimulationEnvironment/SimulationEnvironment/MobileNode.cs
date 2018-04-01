using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class MobileNode
    {
        static int nodeCount = 0;
        static int range = 200;
        private int nodeID;
        private int batteryLevel;
        private int xPosition, yPosition;
        private double sent, received;
        private double numRPackets;
        private int ac;
        private Dictionary<int, List<RoutingPacket>> knownRoutes;

        public MobileNode()
        {
            nodeID = nodeCount++;
            batteryLevel = 100;
            xPosition = yPosition = 0;
            sent = received = 0;
            numRPackets = 0;
            ac = 50;
            knownRoutes = new Dictionary<int, List<RoutingPacket>>();
        }

        public MobileNode(int x, int y, int bLevel)
        {
            nodeID = ++nodeCount;
            xPosition = x;
            yPosition = y;
            sent = received = 0;
            numRPackets = 0;
            ac = 50;
            batteryLevel = bLevel; knownRoutes = new Dictionary<int, List<RoutingPacket>>();
        }

        public double GetNumRPackets()
        {
            foreach (var k in knownRoutes.Keys)
            {
                numRPackets += knownRoutes[k].Count;
            }
            return numRPackets;
        }

        public void printKnownRoutes()
        {
            foreach (var k in knownRoutes.Keys)
            {
                foreach (RoutingPacket route in knownRoutes[k])
                {
                    Console.Write("Route SDP: {0} ", route.getSDP());
                    foreach (MobileNode node in route.GetNodeRoute())
                    {
                        Console.Write("{0}", node.GetNodeID());
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

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

        public double getNumberOfSentPackets()
        {
            return sent;
        }

        public double getNumberOfReceivedPackets()
        {
            return received;
        }

        public void Print()
        {
            Console.WriteLine("Node " + nodeID + ":");
            Console.WriteLine("Battery: " + batteryLevel + "%");
            Console.WriteLine("Location: " + xPosition + ", " + yPosition);
        }

        public double GetDistance(MobileNode node)
        {
            return Math.Sqrt((Math.Pow((xPosition - node.xPosition), 2)) + (Math.Pow((yPosition - node.yPosition), 2)));
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
        // Implement modified SA-DSR
        public RoutingPacket MSADSRRouteDiscovery(MobileNode destNode, SimulationEnvironment env)
        {
            RoutingPacket optRoute = new RoutingPacket();
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDiscovery(destNode, env, rPacket);
            int minMisbehavedNodes = 999999;
            routes = TwoAck(routes);
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
        // Implement TWOACK
        public List<RoutingPacket> TwoAck(List<RoutingPacket> routes)
        {
            foreach (RoutingPacket r in routes)
            {
               for (int i = 0; i < r.GetNodeRoute().Count; i++)
                {
                    if (i >= 2)
                    {
                        // If the middle node drops the acknowledgement during TWOACK then this node misbehaves
                        if (r.GetNodeRoute()[i-1].PacketDrop() == true)
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
        // Implement SADSR
        public RoutingPacket SADSRRouteDiscovery(MobileNode destNode, SimulationEnvironment env)
        {
            // In this protocol, we want to get the route that has the best SDP
            RoutingPacket rPacket = new RoutingPacket();
            List<RoutingPacket> routes = DSRDiscovery(destNode, env, rPacket);
            routes = CalcAltruisticSADSR(routes);
            return GetOptimalRouteSADSR(routes);
        }
        // Calculate the altruism coefficient from source to destination node
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
        // Calculate optimal route 
        public RoutingPacket GetOptimalRouteSADSR(List<RoutingPacket> routes)
        {
            RoutingPacket optRoute = new RoutingPacket();
            double sdp = 0;
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
            return optRoute;
        }
        // Assigning a route to each destination node 
        public List<RoutingPacket> DSRRouteDiscovery(MobileNode destNode, SimulationEnvironment env)
        {
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
        // Adding nodes to routes
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
                if (!route.IsInRouteAlready(node))
                {
                    if (node.Equals(destNode))
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        rPacket.AddNodeToRoute(node);
                        routes.Add(rPacket);
                    }
                    else
                    {
                        RoutingPacket rPacket = route.Copy();
                        rPacket.AddNodeToRoute(this);
                        routes.AddRange(node.DSRDiscovery(destNode, env, rPacket));
                    }
                }
            }
            return routes;
        }

        public bool DSRSendMessage(Message message, RoutingPacket route)
        {
            this.sent++;
            return DSRSMessage(message, route, 0);
        }

        public bool DSRSMessage(Message message, RoutingPacket route, int step)
        {
            if (message.GetDestinstationNode().Equals(this))
            {
                // Record the message received
                this.received++;
                message.calcMsgSpeed(message.GetSourceNode(), message.GetDestinstationNode());
                Console.WriteLine("Message {0} received at Node {1}.", message.GetMessageID(), nodeID);
                return true;
            }
            else
            {
                // Here randomly get a node to drop the message
                if (this.PacketDrop() == true) {
                    Console.WriteLine("Message {0} dropped by Node {1}.", message.GetMessageID(), nodeID);
                    return false;
                } 
                Console.WriteLine("Relaying Message {0} through Node {1}.", message.GetMessageID(), nodeID);
                return route.GetNodeRoute()[++step].DSRSMessage(message, route, ++step);
            }
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
                } else
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
                } else
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