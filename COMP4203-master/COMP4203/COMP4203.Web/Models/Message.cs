using COMP4203.Web.Controllers;
using System;

namespace COMP4203.Web.Models
{
	public class Message
    {
        static int messageCount = 0;
        private int messageID;
        private MobileNode sourceNode, destinationNode;

        public Message(MobileNode src, MobileNode dst)
        {
            messageID = ++messageCount;
            sourceNode = src;
            destinationNode = dst;
        }

        public MobileNode GetSourceNode() { return sourceNode; }
        public MobileNode GetDestinstationNode() { return destinationNode; }
        public int GetMessageID() { return messageID; }

        public void Print()
        {
            OutputPaneController outputPaneController = new OutputPaneController();
            outputPaneController.PrintToOutputPane("Message", "Message from Node " + sourceNode.GetNodeID() + " to Node " + destinationNode.GetNodeID() + ".");
        }
    }
}
