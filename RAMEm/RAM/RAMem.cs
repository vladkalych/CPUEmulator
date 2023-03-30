using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.RAM
{
    public class RAMem
    {
        byte[] memory;

        public RAMem(uint size)
        {
            memory = new byte[size];
        }

        public void Write(uint address, byte data)
        {
            memory[address] = data;
        }

        public void Write(uint address, byte[] data)
        {
            uint counterData = 0;
            for (uint i = address; i < data.Length; i++)
            {
                memory[i] = data[counterData];
                counterData++;
            }
        }

        public byte Read(uint address)
        {
            return memory[address];
        }

        public byte[] Read(uint address, uint length)
        {
            byte[] data = new byte[length];
            uint startPosition = address;

            for (uint i = 0; i < length; i++)
            {
                data[i] = memory[startPosition];
                startPosition++;
            }

            return data;
        }
    }
}
