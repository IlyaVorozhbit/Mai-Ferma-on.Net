using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferma_2018.Logic.Ferma
{

    public class FermaKernelGridTable
    {
        public FermaKernelGridTable(short id, short start_node, short end_node, float length)
        {
            this.id = id;
            this.start_node = start_node;
            this.end_node = end_node;
            this.length = length;
        }

        public short id { get; set; }
        public short start_node { get; set; }
        public short end_node { get; set; }
        public float length { get; set; }
    }

    public class FermaKernelGrid
    {

    }
}
