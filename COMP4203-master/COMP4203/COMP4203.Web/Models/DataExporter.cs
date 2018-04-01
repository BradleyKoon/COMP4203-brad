using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace COMP4203.Web.Models
{
    class DataExporter
    {
        public void export(SimulationEnvironment sim)
        {
            // Append metrics to the end of the csv file 
            StringBuilder data = new StringBuilder();
            string path = "D:\\data.csv";
            data.AppendFormat("{0}, {1}, {2}", 0, 0, 0);
            data.AppendLine();
            File.AppendAllText(path, data.ToString());
        }
        private void AEED(SimulationEnvironment sim)
        {
            // Calculate the average time it takes for a data packet to get delivered
            // Get the speed of each message in the simulate environment and calculate average speed
            // Return calculated result
        }
        private void PDR(SimulationEnvironment sim)
        {
            // Get information about the amount of packets received and sent 
            // Look at the packets sent and received and calculate the ratio
            // Return calculated result
        }
        private void NRO(SimulationEnvironment sim)
        {
            // Calculate total control packets divided by the total packets received in the network
            // Get the number of control and received packets
            // Sum up all the control and received packets and divide them to obtain nro 
            // Return calculated result
        }
        private void BDD(SimulationEnvironment sim)
        {
            // This will be considered later
        }
    }
}