using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Delilah
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			LoadRandomVideo();
		}

		private void BackgroundVideoHost_OnMediaEnded(object sender, RoutedEventArgs e)
		{
			LoadRandomVideo();
		}

		private void LoadRandomVideo()
		{
			if (!Directory.Exists(Config.BackgroundVideoFolder)) return;
			var videos = Directory.EnumerateFiles(Config.BackgroundVideoFolder).Select(x => x.Split('\\').Last());
			var chooseFrom = videos.Where(x => x.StartsWith(DateTime.Now.Hour < 7 || DateTime.Now.Hour > 19 ? "night" : "day")).ToList();
			var prevVideo = (BackgroundVideoHost.Source ?? new Uri("nonlegit\\path.video", UriKind.Relative)).OriginalString.Split('\\').Last();
			var rnd = new Random();
			var newVideo = chooseFrom.ElementAt(rnd.Next(0, chooseFrom.Count()));
			while (newVideo == prevVideo)
			{
				newVideo = chooseFrom.ElementAt(rnd.Next(0, chooseFrom.Count()));
			}
			BackgroundVideoHost.Source = new Uri(Config.BackgroundVideoFolder + "\\" + newVideo, UriKind.Relative);
		}
	}
}