﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace scfeu {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {

		/// <summary>
		/// Flag to control `IsEnabled` of almost all UI
		/// </summary>
		private bool isUiInteractive = true;
		public bool IsUiInteractive {
			get { return isUiInteractive; }
			set {
				if (isUiInteractive != value) {
					isUiInteractive = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUiInteractive)));
				}
			}
		}

		public MainWindow() {
			InitializeComponent();

			progressBar.Value = 10.0;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void ScanDirectoryButton_Click(object sender, RoutedEventArgs e) {
			IsUiInteractive = !IsUiInteractive;
		}

		private void window_DragOver(object sender, DragEventArgs e) {
			e.Effects = DragDropEffects.None;
			try {
				if (e.Data.GetDataPresent("FileNameW")) {
					string path = ((string[])e.Data.GetData("FileNameW"))[0];
					if (System.IO.Directory.Exists(path) && directoryTextBox.IsEnabled) {
						e.Effects = DragDropEffects.Copy;
					}
				}
			} catch {
			}
			e.Handled = true;
		}

		private void window_Drop(object sender, DragEventArgs e) {
			try {
				if (e.Data.GetDataPresent("FileNameW")) {
					string path = ((string[])e.Data.GetData("FileNameW"))[0];
					if (System.IO.Directory.Exists(path) && directoryTextBox.IsEnabled) {
						directoryTextBox.Text = path;
						e.Handled = true;
						ScanDirectoryButton_Click(this, null);
					}
				}
			} catch {
			}
		}
	}
}
