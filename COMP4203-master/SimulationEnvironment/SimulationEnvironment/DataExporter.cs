using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimulationProtocols
{
    class DataExporter
    {
        public void export(SimulationEnvironment sim)
        {
            // Append metrics to the end of the csv file 
            StringBuilder data = new StringBuilder();
            string path = "D:\\data.csv";
            data.AppendFormat("{0}, {1}, {2}", AEED(sim), PDR(sim), NRO(sim));
            data.AppendLine();
            File.AppendAllText(path, data.ToString());
        }
        private double AEED(SimulationEnvironment sim)
        {
            // Calculate the average time it takes for a data packet to get delivered
            // Get the speed of each message in the simulate environment and calculate average speed
            // Return calculated result
            double total = 0;
            foreach (Message msg in sim.GetMessages())
            {
                total += msg.GetMsgSpeed();
            }
            return total / sim.GetMessages().Count;
        }
        private double PDR(SimulationEnvironment sim)
        {
            // Get information about the amount of packets received and sent 
            // Look at the packets sent and received and calculate the ratio
            // Return calculated result
            double sent = 0;
            double received = 0;
            foreach (MobileNode node in sim.GetNodes())
            {
                sent += node.getNumberOfSentPackets();
                received += node.getNumberOfReceivedPackets();
            }
            if (received != 0)
            {
                return sent / received;
            }
            return received;
        }
        private double NRO(SimulationEnvironment sim)
        {
            // Calculate total control packets divided by the total packets received in the network
            // Get the number of control and received packets
            // Sum up all the control and received packets and divide them to obtain nro 
            // Return calculated result
            double sent = 0;
            double received = 0;
            foreach (MobileNode node in sim.GetNodes())
            {
                sent += node.GetNumRPackets();
                received += node.getNumberOfReceivedPackets();
            }
            if (received != 0)
            {
                return sent / received;
            }
            return received;
        }
        private void BDD(SimulationEnvironment sim)
        {
            // This will be considered later
        }
    }
}
