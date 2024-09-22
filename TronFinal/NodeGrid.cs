using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TronFinal
{
    internal class NodeGrid
    {
        public NodeGrid Next { get; set; }
        public NodeGrid Previous { get; set; }
        public NodeGrid Up { get; set; }
        public NodeGrid Down { get; set; }
        public int NodeNum { get; set; }

        public NodeGrid(int Nodenum)
        {
            this.NodeNum = Nodenum;
        }
    }
}
