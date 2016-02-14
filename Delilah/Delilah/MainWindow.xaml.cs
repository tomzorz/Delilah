using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Delilah.Providers;

namespace Delilah
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public MainWindow()
		{
			_currentMenuItems = new ObservableCollection<MenuOption>();
			InitializeComponent();
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			LoadRandomVideo();
			_currentProvider = new MainMenuProvider();
			PopulateMainMenu();
		}

		private IProvider _currentProvider;

		private void PopulateMainMenu()
		{
			_currentMenuItems.Clear();
			var mmr = _currentProvider.Query();
			foreach (var menuOption in mmr)
			{
				_currentMenuItems.Add(menuOption);
			}
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

		private ObservableCollection<MenuOption> _currentMenuItems;

		public ObservableCollection<MenuOption> CurrentMenuItems => _currentMenuItems;

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					if (_currentProvider.GetType() == typeof(MainMenuProvider))
					{
						Application.Current.Shutdown();
					}
					else
					{
						PopulateMainMenu();
					}
					break;
				case Key.Left:
				case Key.NumPad4:
				case Key.A:
				case Key.Back:
					HandleBack();
					break;
				case Key.Right:
				case Key.NumPad5:
				case Key.NumPad6:
				case Key.D:
				case Key.Enter:
					HandleForward();
					break;
				case Key.Up:
				case Key.NumPad8:
				case Key.W:
					HandleUp();
					break;
				case Key.Down:
				case Key.NumPad2:
				case Key.S:
					HandleDown();
					break;
				default:
					//nothing to do
					break;
			}
		}

		private void HandleDown()
		{
			var selected = CurrentMenuItems.FirstOrDefault(x => x.IsSelected);
			if(selected == null) return;
			var index = CurrentMenuItems.IndexOf(selected);
			if(index == CurrentMenuItems.Count - 1) return;
			selected.IsSelected = false;
			CurrentMenuItems.ElementAt(++index).IsSelected = true;
		}

		private void HandleUp()
		{
			var selected = CurrentMenuItems.FirstOrDefault(x => x.IsSelected);
			if (selected == null) return;
			var index = CurrentMenuItems.IndexOf(selected);
			if (index == 0) return;
			selected.IsSelected = false;
			CurrentMenuItems.ElementAt(--index).IsSelected = true;
		}

		private void HandleForward()
		{
			var selected = CurrentMenuItems.FirstOrDefault(x => x.IsSelected);
			if (selected == null) return;
			_currentProvider.Query(selected.Id);
		}

		private void HandleBack()
		{

		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}