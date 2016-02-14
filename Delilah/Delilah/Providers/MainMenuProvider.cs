using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Delilah.Providers
{
	public class MainMenuProvider : IProvider
	{
		public IEnumerable<MenuOption> Query(string id = null)
		{
			if(id == null) return new List<MenuOption>
			{
				new MenuOption { Id = "Main_Netflix", IsSelected = true, Name = "Netflix" },
				new MenuOption { Id = "Main_Steam", IsSelected = false, Name = "Steam Link" },
				new MenuOption { Id = "Main_Shows", IsSelected = false, Name = "Shows" },
				new MenuOption { Id = "Main_Movies", IsSelected = false, Name = "Movies" },
				new MenuOption { Id = "Main_Latest", IsSelected = false, Name = "Latest content" },
			};
			switch (id)
			{
				case "Main_Netflix":
					System.Diagnostics.Process.Start("https://netflix.com");
					break;
				case "Main_Steam":
					System.Diagnostics.Process.Start("steam://open/bigpicture");
					break;
				case "Main_Shows":
					System.Diagnostics.Process.Start(@"Q:\");
					break;
				case "Main_Movies":
					System.Diagnostics.Process.Start(@"R:\");
					break;
				case "Main_Latest":
					System.Diagnostics.Process.Start(@"P:\");
					break;
			}
			return null;
		}
	}
}