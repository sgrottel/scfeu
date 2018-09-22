﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scfeu
{
	public class JobSetup : INotifyPropertyChanged
	{

		public const string DefaultIncludePattern = "";
		public const string DefaultExcludePattern = "*.sln;.git;.vs";

		private string dir;
		public string Directory {
			get { return dir; }
			set {
				if (dir != value) {
					dir = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Directory)));
					evalCanScanDir();
				}
			}
		}

		private string incPat = DefaultIncludePattern;
		public string IncludePattern {
			get { return incPat; }
			set {
				if (incPat != value) {
					incPat = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IncludePattern)));
				}
			}
		}

		private string exPat = DefaultExcludePattern;
		public string ExcludePattern {
			get { return exPat; }
			set {
				if (exPat != value) {
					exPat = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExcludePattern)));
				}
			}
		}

		private LineBreak lb = LineBreak.Unix;
		public LineBreak LineBreak {
			get { return lb; }
			set {
				if (lb != value) {
					lb = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineBreak)));
				}
			}
		}

		private Encoding enc = Encoding.UTF8;
		public Encoding Encoding {
			get { return enc; }
			set {
				if (enc != value) {
					enc = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Encoding)));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private bool enabled = true;
		public bool IsEnabled {
			get { return enabled; }
			set {
				if (enabled != value) {
					enabled = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
					evalCanScanDir();
					evalCanFixFiles();
				}
			}
		}

		private bool canScanDir = false;
		public bool CanScanDir {
			get { return canScanDir; }
			set {
				if (canScanDir != value) {
					canScanDir = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanScanDir)));
				}
			}
		}

		private void evalCanScanDir() {
			CanScanDir = enabled
				&& System.IO.Directory.Exists(dir);
		}

		private bool canFixFiles = false;
		public bool CanFixFiles {
			get { return canFixFiles; }
			set {
				if (canFixFiles != value) {
					canFixFiles = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanFixFiles)));
				}
			}
		}

		private void evalCanFixFiles() {
			CanFixFiles = enabled
				&& false; // TODO: Implement
		}
	}
}
