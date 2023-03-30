using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAMEm.CPU.Registries.Word
{
    public class WordRegistry
	{
		Registry h;
		Registry l;

		public WordRegistry(Registry h, Registry l)
		{
			this.h = h;
			this.l = l;
		}

		public Registry H 
		{
			get { return h; }
		}

		public Registry L
		{
			get { return l; }
		}
	}
}
