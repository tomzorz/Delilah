using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Delilah
{
	public class MenuOption : INotifyPropertyChanged
	{
		private bool _isSelected;
		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				if(value == _name) return;
				_name = value;
				OnPropertyChanged();
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if(value == _isSelected) return;
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string Id { get; set; }
	}
}
