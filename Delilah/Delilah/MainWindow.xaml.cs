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
using System.Windows.Threading;
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
			MainWindowRoot.MouseMove += MainWindow_OnMouseMove;
			_dt = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.0)
			};
			_dt.Tick += _dt_Tick;
			_dt.Start();
		}

		private int _elapsed = 0;

		private bool Hidden
		{
			get { return _hidden; }
			set
			{
				_hidden = value;
				MainWindowRoot.Cursor = _hidden ? Cursors.None : Cursors.Arrow;
				InteractiveContent.Visibility = _hidden ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		public string Time
		{
			get { return _time; }
			set
			{
				if(value == _time) return;
				_time = value;
				OnPropertyChanged();
			}
		}

		private void _dt_Tick(object sender, EventArgs e)
		{
			Time = DateTime.Now.ToString("T");
			if (Hidden) return;
			_elapsed += 1;
			if (_elapsed <= 30) return;
			Hidden = true;
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
		private DispatcherTimer _dt;
		private string _time;
		private bool _hidden;

		public ObservableCollection<MenuOption> CurrentMenuItems => _currentMenuItems;

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (Hidden)
			{
				_elapsed = 0;
				Hidden = false;
				return;
			}
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

		private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
		{
			_elapsed = 0;
			Hidden = false;
		}
	}
}