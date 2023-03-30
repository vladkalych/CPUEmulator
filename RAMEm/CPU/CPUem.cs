using RAMEm.Convertor;
using RAMEm.CPU.Registries;
using RAMEm.CPU.Registries.Doubleword;
using RAMEm.CPU.Registries.RegistryException;
using RAMEm.CPU.Registries.Word;
using RAMEm.RAM;
using System.Text;

namespace RAMEm.CPU
{
	public class CPUem
	{
		private DoublewordRegistry eax = new DoublewordRegistry(
			new WordRegistry(
				new Registry(), 
				new Registry()
				)
			);

		private DoublewordRegistry ebx = new DoublewordRegistry(
			new WordRegistry(
				new Registry(),
				new Registry()
				)
			);

		private DoublewordRegistry ecx = new DoublewordRegistry(
			new WordRegistry(
				new Registry(),
				new Registry()
				)
			);

		private DoublewordRegistry edx = new DoublewordRegistry(
			new WordRegistry(
				new Registry(),
				new Registry()
				)
			);

		public RAMem ram;


		public CPUem(RAMem ram) 
		{
			this.ram = ram;
		}

		/*
		 * Description   |  Opcode |  Opcode   |  Opcode   |
		 * --------------|---------|-----------|-----------|
		 *		         |  8 bits |  16 bits  |  32 bits  |
		 * --------------|---------|-----------|-----------|
		 * MOV reg reg   |  0x10   |   0x25	   |   0x40	   |
		 * MOV reg mem   |  0x11   |   0x26	   |   0x41	   |
		 * MOV mem reg   |  0x12   |   0x27	   |   0x42	   |
		 * MOV reg imm   |  0x13   |   0x28	   |   0x43	   |
		 * --------------|---------|-----------|-----------|
		 * ADD reg reg   |  0x15   |   0x30	   |   0x45	   |
		 * ADD reg mem	 |  0x16   |   0x31	   |   0x46	   |
		 * ADD reg imm	 |  0x17   |   0x32	   |   0x47	   |
		 * --------------|---------|-----------|-----------|
		 * SUB reg reg	 |  0x20   |   0x35	   |   0x50	   |
		 * SUB reg mem	 |  0x21   |   0x36	   |   0x51	   |
		 * SUB reg imm	 |  0x22   |   0x37	   |   0x52	   |
		 * --------------|---------|-----------|-----------|
		 * 
		 * ------------------------|---------|
		 * Output - read		   |  0xF0   |
		 * text from			   |         |
		 * memory:				   |		 |
		 * EAX - address		   |         |
		 * EBX - length			   |		 |
		 * ------------------------|---------|
		 * Input - write		   |  0xF1   |
		 * text to				   |         |
		 * memory:				   |		 |
		 * EAX - *******		   |         |
		 * EBX - ***			   |		 |
		 * ------------------------|---------|
		 * Convert to ASCII from   |  0xF5   |
		 * memory and write to	   |         |
		 * memory.				   |         |
		 * Read:				   |         |
		 * EAX - address		   |		 |
		 * EBX - length			   |         |
		 * Write:				   |         |
		 * EAX - address		   |		 |
		 * EBX - length			   |         |
		 * ------------------------|---------|
		 * Convert from			   |  0xF6   |
		 * ASCII from			   |         |	 
		 * memory and write to     |         |
		 * memory				   |         |
		 * Read:				   |         |	 
		 * EAX - address		   |         |
		 * EBX - length			   |         |
		 * Write:				   |         |
		 * EAX - address		   |         |
		 * EBX - length			   |         |
		 * ------------------------|---------|
		 * 
		 * 
		 * 
		 * 
		 * 
		 * 
		 *
		 */

		// 2 byte for address

		public void Run(byte[] instructions)
		{
			for (int i = 0; i < instructions.Length; i++)
			{
				switch (instructions[i])
				{
					case 0xF0:
						uint address = Read32Registry((byte)RegistryAddress.EAX);
						uint length = Read32Registry((byte)RegistryAddress.EBX);
						byte[] data = ram.Read(address, length);
						string str = Encoding.ASCII.GetString(data);
						Console.WriteLine(str);
						break;

					case 0xF1:

					
					case 0xF5:
						address = Read32Registry((byte)RegistryAddress.EAX);
						length = Read32Registry((byte)RegistryAddress.EBX);
						data = ram.Read(address, length);

						if (data.Length < 4) 
						{
							Array.Resize(ref data, 4);
						}

						uint startAddress = address;
						uint lengthOfResult = 0; // Длинна результата

						uint temp = BitConverter.ToUInt32(data, 0);
						byte digitCount = Convert.ToByte(temp.ToString().Length); // Узнаем сколько цифр в числе
						lengthOfResult += digitCount;
						byte[] symbols = ASCIIConvertor.ConvertNumberToArraySymbols(temp); // Массив для ASCII числа с учётем + 1 для символа конца строки
						ram.Write(address, symbols);

						Write32Registry((byte)RegistryAddress.EAX, startAddress);
						Write32Registry((byte)RegistryAddress.EBX, lengthOfResult);
						break;

					case 0xF6:
						address = Read32Registry((byte)RegistryAddress.EAX);
						length = Read32Registry((byte)RegistryAddress.EBX);
						data = ram.Read(address, length);

						startAddress = address;
						lengthOfResult = 0; // Длинна результата

						uint startPosition = 0;

						for (uint q = 0; q < data.Length; q++)
						{
							if (data[q] != 0) 
							{
								continue;
							}

							uint sizeOfArray = q - startPosition;
							byte[] asciiValue = new byte[sizeOfArray];
							Array.Copy(data, startPosition, asciiValue, 0, sizeOfArray);
							startPosition = (uint)q + 1; // Делаем начальную позицию нового значение в ASCII после символа EOL
							uint value = ASCIIConvertor.ConvertArraySymbolsToNumber(asciiValue);

							if (value < Byte.MaxValue) 
							{
								ram.Write(address, Convert.ToByte(value));
								break;
							}

							if (value < UInt16.MaxValue) 
							{
								ushort value1 = Convert.ToUInt16(value);
								ram.Write(address, BitConverter.GetBytes(value1));
								break;
							}

							ram.Write(address, BitConverter.GetBytes(value));
							address++; // Передвигаем адрес записи на следующий
							lengthOfResult++;
						}

						Write32Registry((byte)RegistryAddress.EAX, startAddress);
						Write32Registry((byte)RegistryAddress.EBX, lengthOfResult);
						break;

						//			-- 8 bits --

					case 0x10: // MOV reg reg 8 bits
						byte reg1 = instructions[i + 1];
						byte reg2 = instructions[i + 2];
						Write8Registry(reg1, Read8Registry(reg2));
						i += 2;
						break;
					case 0x11: // MOV reg mem 8 bits
						byte reg = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						Write8Registry(reg, ram.Read(address));
						i += 5;
						break;
					case 0x12: // MOV mem reg 8 bits
						address = BitConverter.ToUInt32(instructions, i + 1);
						reg = instructions[i + 5];
						ram.Write(address, Read8Registry(reg));
						i += 5;
						break;
					case 0x13: // MOV reg imm 8 bits
						reg = instructions[i + 1];
						byte byteValue = instructions[i + 2];
						Write8Registry(reg, byteValue);
						i += 2;
						break;
					case 0x15: // ADD reg reg 8 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						byteValue = (byte)(Read8Registry(reg1) + Read8Registry(reg2));
						Write8Registry((byte)RegistryAddress.AL, byteValue);
						i += 2;
						break;
					case 0x16: // ADD reg mem 8 bits
						reg1 = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						byteValue = (byte)(Read8Registry(reg1) + ram.Read(address));
						Write8Registry((byte)RegistryAddress.AL, byteValue);
						i += 5;
						break;
					case 0x17: // ADD reg imm 8 bits
						reg1 = instructions[i + 1];
						byteValue = instructions[i + 2];
						byteValue = (byte)(Read8Registry(reg1) + byteValue);
						Write8Registry((byte)RegistryAddress.AL, byteValue);
						i += 2;
						break;
					case 0x20: // SUB reg reg 8 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						byteValue = (byte)(Read8Registry(reg1) - Read8Registry(reg2));
						Write8Registry((byte)RegistryAddress.AL, byteValue);
						i += 2;
						break;
					case 0x21: // SUB reg mem 8 bits
						reg1 = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						byteValue = (byte)(Read8Registry(reg1) - ram.Read(address));
						Write8Registry((byte)RegistryAddress.AL, byteValue);
						i += 5;
						break;
					case 0x22: // SUB reg imm 8 bits
						reg1 = instructions[i + 1];
						byteValue = instructions[i + 2];
						byteValue = (byte)(Read8Registry(reg1) - byteValue);
						Write8Registry((byte)RegistryAddress.AL, byteValue);
						i += 2;
						break;

						//			-- 16 bits --

					case 0x25: // MOV reg reg 16 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						Write16Registry(reg1, Read16Registry(reg2));
						i += 2;
						break;
					case 0x26: // MOV reg mem 16 bits
						reg = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						ushort ushortValue = BitConverter.ToUInt16(ram.Read(address, 2));
						Write16Registry(reg, ushortValue);
						i += 5;
						break;
					case 0x27: // MOV mem reg 16 bits
						address = BitConverter.ToUInt32(instructions, i + 1);
						reg = instructions[i + 5];
						ushortValue = Read16Registry(reg);
						ram.Write(address, BitConverter.GetBytes(ushortValue)); 
						i += 5;
						break;
					case 0x28: // MOV reg imm 16 bits
						reg = instructions[i + 1];
						ushortValue = BitConverter.ToUInt16(instructions, i + 2);
						Write16Registry(reg, ushortValue);
						i += 3;
						break;
					case 0x30: // ADD reg reg 16 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						ushortValue = (ushort)(Read16Registry(reg1) + Read16Registry(reg2));
						Write16Registry((byte)RegistryAddress.AX, ushortValue);
						i += 2;
						break;
					case 0x31: // ADD reg mem 16 bits
						reg1 = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						ushortValue = (ushort)(Read16Registry(reg1) + ram.Read(address));
						Write16Registry((byte)RegistryAddress.AX, ushortValue);
						i += 5;
						break;
					case 0x32: // ADD reg imm 16 bits
						reg1 = instructions[i + 1];
						ushortValue = BitConverter.ToUInt16(instructions, i + 2);
						ushortValue = (ushort)(Read16Registry(reg1) + ushortValue);
						Write16Registry((byte)RegistryAddress.AX, ushortValue);
						i += 3;
						break;
					case 0x35: // SUB reg reg 16 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						ushortValue = (byte)(Read16Registry(reg1) - Read16Registry(reg2));
						Write16Registry((byte)RegistryAddress.AX, ushortValue);
						i += 2;
						break;
					case 0x36: // SUB reg mem 16 bits
						reg1 = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						ushortValue = (ushort)(Read16Registry(reg1) - ram.Read(address));
						Write16Registry((byte)RegistryAddress.AX, ushortValue);
						i += 5;
						break;
					case 0x37: // SUB reg imm 16 bits
						reg1 = instructions[i + 1];
						ushortValue = BitConverter.ToUInt16(instructions, i + 2);
						ushortValue = (ushort)(Read16Registry(reg1) - ushortValue);
						Write16Registry((byte)RegistryAddress.AX, ushortValue);
						i += 3;
						break;

						//			-- 32 bits --

					case 0x40: // MOV reg reg 32 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						Write32Registry(reg1, Read32Registry(reg2));
						i += 2;
						break;
					case 0x41: // MOV reg mem 32 bits
						reg = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						uint uintValue = BitConverter.ToUInt32(ram.Read(address, 4));
						Write32Registry(reg, uintValue);
						i += 5;
						break;
					case 0x42: // MOV mem reg 32 bits
						address = BitConverter.ToUInt32(instructions, i + 1);
						reg = instructions[i + 5];
						uintValue = Read32Registry(reg);
						ram.Write(address, BitConverter.GetBytes(uintValue));
						i += 5;
						break;
					case 0x43: // MOV reg imm 32 bits
						reg = instructions[i + 1];
						uintValue = BitConverter.ToUInt32(instructions, i + 2);
						Write32Registry(reg, uintValue);
						i += 5;
						break;
					case 0x45: // ADD reg reg 32 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						uintValue = Read32Registry(reg1) + Read32Registry(reg2);
						Write32Registry((byte)RegistryAddress.EAX, uintValue);
						i += 2;
						break;
					case 0x46: // ADD reg mem 32 bits
						reg1 = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						uintValue = (uint)(Read32Registry(reg1) + ram.Read(address));
						Write32Registry((byte)RegistryAddress.EAX, uintValue);
						i += 5;
						break;
					case 0x47: // ADD reg imm 32 bits
						reg1 = instructions[i + 1];
						uintValue = BitConverter.ToUInt32(instructions, i + 2);
						uintValue = (uint)(Read32Registry(reg1) + uintValue);
						Write32Registry((byte)RegistryAddress.EAX, uintValue);
						i += 5;
						break;
					case 0x50: // SUB reg reg 32 bits
						reg1 = instructions[i + 1];
						reg2 = instructions[i + 2];
						uintValue = (byte)(Read32Registry(reg1) - Read32Registry(reg2));
						Write32Registry((byte)RegistryAddress.EAX, uintValue);
						i += 2;
						break;
					case 0x51: // SUB reg mem 32 bits
						reg1 = instructions[i + 1];
						address = BitConverter.ToUInt32(instructions, i + 2);
						uintValue = (uint)(Read32Registry(reg1) - ram.Read(address));
						Write32Registry((byte)RegistryAddress.EAX, uintValue);
						i += 5;
						break;
					case 0x52: // SUB reg imm 32 bits
						reg1 = instructions[i + 1];
						uintValue = BitConverter.ToUInt32(instructions, i + 2);
						uintValue = (ushort)(Read32Registry(reg1) - uintValue);
						Write32Registry((byte)RegistryAddress.EAX, uintValue);
						i += 5;
						break;
					default:
						throw new InvalidOperationException($"Unknown opcode {instructions[i]:X2}");
				}
			}
		}

		private byte Read8Registry(byte registry) 
		{
			switch (registry)
			{
				case (byte) RegistryAddress.AH:
					return eax.ReadH();
				case (byte) RegistryAddress.AL:
					return eax.ReadL();
				case (byte) RegistryAddress.BH:
					return ebx.ReadH();
				case (byte) RegistryAddress.BL:
					return ebx.ReadL();
				case (byte) RegistryAddress.CH:
					return ecx.ReadH();
				case (byte) RegistryAddress.CL:
					return ecx.ReadL();
				case (byte) RegistryAddress.DH:
					return edx.ReadH();
				case (byte) RegistryAddress.DL:
					return edx.ReadL();
				default:
					throw new NotFoundRegistryException();
			}
		}

		private ushort Read16Registry(byte registry)
		{
			switch (registry)
			{
				case (byte) RegistryAddress.AX:
					return eax.ReadWordRegistry();
				case (byte) RegistryAddress.BX:
					return ebx.ReadWordRegistry();
				case (byte) RegistryAddress.CX:
					return ecx.ReadWordRegistry();
				case (byte) RegistryAddress.DX:
					return edx.ReadWordRegistry();
				default:
					throw new NotFoundRegistryException();
			}
		}

		private uint Read32Registry(byte registry)
		{
			switch (registry)
			{
				case (byte) RegistryAddress.EAX:
					return eax.ReadDoubleWordRegistry();
				case (byte) RegistryAddress.EBX:
					return ebx.ReadDoubleWordRegistry();
				case (byte) RegistryAddress.ECX:
					return ecx.ReadDoubleWordRegistry();
				case (byte) RegistryAddress.EDX:
					return edx.ReadDoubleWordRegistry();
				default:
					throw new NotFoundRegistryException();
			}
		}

		private void Write32Registry(byte registry, uint value)
		{
			switch (registry)
			{
				case (byte) RegistryAddress.EAX:
					eax.WriteDoubleWordRegistry(value);
					break;
				case (byte) RegistryAddress.EBX:
					ebx.WriteDoubleWordRegistry(value);
					break;
				case (byte) RegistryAddress.ECX:
					ecx.WriteDoubleWordRegistry(value);
					break;
				case (byte) RegistryAddress.EDX:
					edx.WriteDoubleWordRegistry(value);
					break;
				default:
					throw new NotFoundRegistryException();
			}
		}

		private void Write16Registry(byte registry, ushort value) 
		{
			switch (registry)
			{
				case (byte) RegistryAddress.AX:
					eax.WriteWordRegistry(value);
					break;
				case (byte) RegistryAddress.BX:
					ebx.WriteWordRegistry(value);
					break;
				case (byte) RegistryAddress.CX:
					ecx.WriteWordRegistry(value);
					break;
				case (byte) RegistryAddress.DX:
					edx.WriteWordRegistry(value);
					break;
				default:
					throw new NotFoundRegistryException();
			}
		}

		private void Write8Registry(byte registry, byte value)
		{
			switch (registry)
			{
				case (byte) RegistryAddress.AH:
					eax.WriteH(value);
					break;
				case (byte) RegistryAddress.AL:
					eax.WriteL(value);
					break;
				case (byte) RegistryAddress.BH:
					ebx.WriteH(value);
					break;
				case (byte) RegistryAddress.BL:
					ebx.WriteL(value);
					break;
				case (byte) RegistryAddress.CH:
					ecx.WriteH(value);
					break;
				case (byte) RegistryAddress.CL:
					ecx.WriteL(value);
					break;
				case (byte) RegistryAddress.DH:
					edx.WriteH(value);
					break;
				case (byte) RegistryAddress.DL:
					edx.WriteL(value);
					break;
				default:
					throw new NotFoundRegistryException();
			}
		}

	}
}
