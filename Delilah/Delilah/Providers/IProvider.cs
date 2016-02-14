using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delilah.Providers
{
	public interface IProvider
	{
		IEnumerable<MenuOption> Query(string id = null);
	}
}