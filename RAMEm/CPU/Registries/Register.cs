using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.CPU.Registries
{
    public class Registry
    {
        byte value = 0x0;

        public byte Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
