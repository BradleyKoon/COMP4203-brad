using System;
using System.Collections.Generic;
using System.Web.Http;
using COMP4203.Web.Controllers.Extend;
using COMP4203.Web.Controllers.Hubs;
using COMP4203.Web.Models;
using Newtonsoft.Json;

namespace COMP4203.Web.Controllers
{
    public class MainController : ApiControllerWithHub<MainHub>
    {
	    public MainController()
	    {

        }
        // PLEASE MOVE SA-DSR TO ANOTHER SESSION 
        [HttpGet, Route("api/main/demo/{tabIndex}")]
        public void RunDemo(int tabIndex)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            // Add Test Nodes
            sim.AddNode(new MobileNode(100, 100, 20));
            sim.AddNode(new MobileNode(200, 210, 20));
            sim.AddNode(new MobileNode(210, 200, 100));
            sim.AddNode(new MobileNode(298, 298, 100));
            // Add Test Message
            sim.AddMessage(new Message(sim.GetNodes()[0], sim.GetNodes()[3]));
            // Print Simulation Nodes
            new OutputPaneController().PrintToOutputPane("Note", "Simulation Nodes:");
            foreach (MobileNode node in sim.GetNodes())
            {
                node.Print();
                node.PrintNodesWithinRange(sim);
            }
            // Testing DSR
            /*new OutputPaneController().PopulateNodesDSR(sim.GetNodes(), tabIndex);
            sim.SendMessageDSR(sim.GetMessages()[0]);*/
            // Testing SADSR 
            /*new OutputPaneController().PopulateNodesSADSR(sim.GetNodes(), tabIndex);
            sim.SendMessageSADSR(sim.GetMessages()[0]);*/
            // Testing modified SADSR
            new OutputPaneController().PopulateNodesMSADSR(sim.GetNodes(), tabIndex);
            sim.SendMessageMSADSR(sim.GetMessages()[0]);
        }

        [HttpGet, Route("api/main/run/{nodeNumber}/{messageNumber}/{simSpeedNumber}/{tabIndex}")]
        public void RunTest(
	        int nodeNumber,
	        int messageNumber,
	        int simSpeedNumber,
	        int tabIndex)
        {
            SimulationEnvironment sim = new SimulationEnvironment();
            sim.GenerateRandomNodes(nodeNumber);
            sim.GenerateRandomMessages(messageNumber);
            new OutputPaneController().PopulateNodesDSR(sim.GetNodes(), tabIndex);
            sim.RunSimulation();
            
        }
    }
}