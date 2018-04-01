using System;
using System.Collections.Generic;

namespace SimulationProtocols
{
    class Program
    {
        static void Main(string[] args)
        {
            SimulationEnvironment sTest = new SimulationEnvironment();
            // Add Test Nodes
            sTest.AddNode(new MobileNode(0, 0, 100));
            sTest.AddNode(new MobileNode(100, 110, 100));
            sTest.AddNode(new MobileNode(110, 100, 100));
            sTest.AddNode(new MobileNode(198, 198, 100));
            // Add Test Message
            sTest.AddMessage(new Message(sTest.GetNodes()[0], sTest.GetNodes()[3]));

            // Print Simulation Nodes
            foreach (MobileNode node in sTest.GetNodes())
            {
                node.Print();
                node.PrintNodesWithinRange(sTest);
                Console.WriteLine();
            }

            // Send 1st message
            sTest.SendMessageDSR(sTest.GetMessages()[0]);

            // Print Nodes After Message
            foreach (MobileNode node in sTest.GetNodes()) { node.Print(); }

            Console.ReadKey();
        }
    }
}
