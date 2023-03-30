using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.CPU.Registries.RegistryException
{
	[Serializable]
	public class NotFoundRegistryException : Exception
	{
		public NotFoundRegistryException()
		{
		}

		public NotFoundRegistryException(string message)
			: base(message)
		{
		}

		public NotFoundRegistryException(string message, Exception inner)
			: base(message, inner)
		{
		}
		protected NotFoundRegistryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
