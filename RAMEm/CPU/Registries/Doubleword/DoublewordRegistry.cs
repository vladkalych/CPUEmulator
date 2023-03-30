using RAMEm.CPU.Registries.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.CPU.Registries.Doubleword
{
	public class DoublewordRegistry
	{
		private WordRegistry wordRegistry;
		private ushort value = 0x0;

		public DoublewordRegistry(WordRegistry wordRegistry)
		{
			this.wordRegistry = wordRegistry;
		}

		public byte ReadL()
		{
			return this.wordRegistry.L.Value;
		}

		public byte ReadH() 
		{
			return this.wordRegistry.H.Value;
		}

		public ushort ReadWordRegistry() 
		{
			byte[] result = new byte[2];
			result[0] = ReadL();
			result[1] = ReadH();
			return BitConverter.ToUInt16(result, 0);
		}

		public uint ReadDoubleWordRegistry() 
		{
			byte[] bytes = new byte[4];
			bytes[0] = ReadL();
			bytes[1] = ReadH();
			byte[] ushortBytes = BitConverter.GetBytes(value);
			bytes[2] = ushortBytes[0];
			bytes[3] = ushortBytes[1];
			return BitConverter.ToUInt32(bytes, 0);
		}

		public void WriteH(byte value) 
		{
			this.wordRegistry.H.Value = value;
		}

		public void WriteL(byte value) 
		{
			this.wordRegistry.L.Value = value;
		}

		public void ReadH(byte value)
		{
			this.wordRegistry.H.Value = value;
		}

		public void WriteWordRegistry(ushort value)
		{
			byte[] bytes = BitConverter.GetBytes(value); 
			WriteL(bytes[0]); 
			WriteH(bytes[1]); 
		}

		public void WriteDoubleWordRegistry(uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			WriteL(bytes[0]);
			WriteH(bytes[1]);
			this.value = BitConverter.ToUInt16(bytes, 2);
		}
	}
}
