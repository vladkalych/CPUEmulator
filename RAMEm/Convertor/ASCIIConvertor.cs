using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.Convertor
{
	public class ASCIIConvertor
	{
		public static uint ConvertArraySymbolsToNumber(byte[] arraySymbols)
		{
			string result = String.Empty;

			foreach (byte temp in arraySymbols)
			{
				result += Convert.ToChar(temp);
			}

			return UInt32.Parse(result);
		}

		public static byte[] ConvertNumberToArraySymbols(uint number)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(number.ToString());
			byte[] result = new byte[bytes.Length + 1];
			Array.Copy(bytes, result, bytes.Length);
			return result; // символы в byte
		}
	}
}
