using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.CPU.Registries
{
	public enum RegistryAddress
	{
		EAX = 0x20,
		EBX = 0x21,
		ECX = 0x22,
		EDX = 0x23,
		
		AX = 0x10,
		BX = 0x11,
		CX = 0x12,
		DX = 0x13,
		     
		AL = 0x01,
		CL = 0x02,
		DL = 0x03,
		BL = 0x04,
		AH = 0x05,
		CH = 0x06,
		DH = 0x07,
		BH = 0x08,		
	}
}
