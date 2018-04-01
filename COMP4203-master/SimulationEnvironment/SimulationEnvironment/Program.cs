using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class Program
    {
        static void Main(string[] args)
        {
            SimulationEnvironment sTest = new SimulationEnvironment();
            sTest.GetNodes().Add(new MobileNode(0, 0, 100));
            sTest.GetNodes().Add(new MobileNode(100, 110, 100));
            sTest.GetNodes().Add(new MobileNode(110, 100, 100));
            sTest.GetNodes().Add(new MobileNode(198, 198, 100));
            sTest.GetMessages().Add(new Message(sTest.GetNodes()[0], sTest.GetNodes()[3]));
            foreach (MobileNode node in sTest.GetNodes())
            {
                node.Print();
                foreach (MobileNode n in sTest.GetNodes())
                {
                    if (!node.Equals(n))
                    {
                        if (node.IsWithinRangeOf(n))
                        {
                            Console.WriteLine("Node {0} is within range. Distance: {1}", n.GetNodeID(), node.GetDistance(n));
                        }
                        else
                        {
                            Console.WriteLine("Node {0} is not within range. Distance: {1}", n.GetNodeID(), node.GetDistance(n));
                        }
                    }
                }
                Console.WriteLine();
            }
            sTest.GetMessages()[0].Print();

            List<RoutingPacket> packets = sTest.GetNodes()[0].DSRRouteDiscovery(sTest.GetNodes()[3], sTest);
            // Calling SA-DSR 
            RoutingPacket optRoute = sTest.GetNodes()[0].SADSRRouteDiscovery(sTest.GetNodes()[3], sTest);
            // Calling Modified SA-DSR
            RoutingPacket optRoute2 = sTest.GetNodes()[0].MSADSRRouteDiscovery(sTest.GetNodes()[3], sTest);
            Console.WriteLine("{0} Routes Found.", packets.Count);

            foreach (RoutingPacket route in packets)
            {
                Console.WriteLine("Route:");
                Console.WriteLine("==============================");
                foreach (MobileNode node in route.GetNodeRoute())
                {
                    Console.Write("{0} ", node.GetNodeID());
                }
                Console.WriteLine();
                Console.WriteLine("==============================");
            }
            // Testing SA-DSR
            Console.Write("SA-DSR Optimal Route: ");
            foreach (MobileNode node in optRoute.GetNodeRoute())
            {
                Console.Write("{0} ", node.GetNodeID());
            }
            Console.WriteLine();
            Console.WriteLine("==============================");

            // Testing modified SA-DSR
            Console.Write("Modified SA-DSR Optimal Route: ");
            foreach (MobileNode node in optRoute2.GetNodeRoute())
            {
                Console.Write("{0} ", node.GetNodeID());
            }
            Console.WriteLine();
            Console.WriteLine("==============================");

            sTest.GetNodes()[0].DSRSendMessage(sTest.GetMessages()[0], packets[0]);

            Console.ReadKey();

            SimulationEnvironment sim = new SimulationEnvironment();
            sim.GenerateRandomNodes(15);
            sim.GenerateRandomMessages(50);

            Console.WriteLine("==============================================");
            Console.WriteLine("Simulation Nodes");
            Console.WriteLine("==============================================");

            foreach (MobileNode node in sim.GetNodes())
            {
                node.Print();
                foreach (MobileNode n in sim.GetNodes())
                {
                    if (!node.Equals(n))
                    {
                        if (node.IsWithinRangeOf(n))
                        {
                            Console.WriteLine("Node {0} is within range. Distance: {1}", n.GetNodeID(), node.GetDistance(n));
                        }
                        else
                        {
                            Console.WriteLine("Node {0} is not within range. Distance: {1}", n.GetNodeID(), node.GetDistance(n));
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("Simulation Messages");
            Console.WriteLine("==============================================");
            foreach (Message message in sim.GetMessages())
            {
                message.Print();
            }
            foreach (MobileNode node in sTest.GetNodes())
            {
                node.printKnownRoutes();
            }
            // Export data here
            DataExporter d = new DataExporter();
            d.export(sTest);
            Console.ReadKey();
        }
    }
}