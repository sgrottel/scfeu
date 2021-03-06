﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scfeu.Scanned
{
	class File : Element
	{
		private bool enabled = false;
		private bool selected = false;
		private Encoding encoding = null;

		private int cntWinNL = 0;
		private int cntUnixNL = 0;
		private int cntMacNL = 0;

		private bool trailingWS = false;

		private int indentTabs = 0;
		private int indentSpaces = 0;

		public bool Enabled {
			get { return enabled; }
			set {
				if (enabled != value) {
					enabled = value;
					FirePropertyChanged(nameof(Enabled));
				}
			}
		}

		public bool Selected {
			get { return selected; }
			set {
				if (selected != value) {
					selected = value;
					FirePropertyChanged(nameof(Selected));
				}
			}
		}

		public Encoding Encoding {
			get { return encoding; }
			set {
				if (encoding != value) {
					encoding = value;
					FirePropertyChanged(nameof(Encoding));
					FirePropertyChanged(nameof(EncodingName));
				}
			}
		}

		private bool hasBom = false;
		public bool WithBom {
			get { return hasBom; }
			set {
				if (hasBom != value) {
					hasBom = value;
					FirePropertyChanged(nameof(WithBom));
					FirePropertyChanged(nameof(EncodingName));
				}
			}
		}

		public string EncodingName {
			get { return encoding == null ? "Unknown / Binary" : (encoding.WebName + (hasBom ? " (BOM)" : "")); }
		}

		public string LineBreakInfo {
			get {
				if (cntWinNL == 0 && cntUnixNL == 0 && cntMacNL == 0) return "?";
				List<string> frag = new List<string>();
				if (cntWinNL > 0) {
					if (cntUnixNL == 0 && cntMacNL == 0) return "Win";
					frag.Add("Win " + cntWinNL.ToString());
				}
				if (cntUnixNL > 0) {
					if (cntWinNL == 0 && cntMacNL == 0) return "Nix";
					frag.Add("Nix " + cntUnixNL.ToString());
				}
				if (cntMacNL > 0) {
					if (cntWinNL == 0 && cntUnixNL == 0) return "Mac";
					frag.Add("Mac " + cntMacNL.ToString());
				}
				return string.Join("; ", frag);
			}
		}

		public string LeadingSpaceInfo {
			get {
				if (indentSpaces == 0 && indentTabs == 0) return "?";
				if (indentSpaces == 0) return "Tab";
				if (indentTabs == 0) return "Space";
				if (indentSpaces > indentTabs) return "Space " + indentSpaces.ToString() + "; Tab " + indentTabs.ToString();
				return "Tab " + indentTabs.ToString() + "; Space " + indentSpaces.ToString();
			}
		}

		public string TrailingSpaceInfo {
			get {
				if (encoding == null) return "?";
				return trailingWS ? "Yes" : "No";
			}
		}

		internal static Encoding GuessEncoding(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				{
					Ude.CharsetDetector cdet = new Ude.CharsetDetector();
					cdet.Feed(fs);
					cdet.DataEnd();
					if (cdet.Charset != null) {
						if (cdet.Confidence > 0.85) {
							try {
								Encoding e = Encoding.GetEncoding(cdet.Charset);
								// if e is us-Ascii but never uses 8 bit, change to utf-8 without bom
								if (e == Encoding.ASCII) {
									bool allSmall = true;
									fs.Seek(0, SeekOrigin.Begin);
									byte[] data = new byte[fs.Length];
									int bl = fs.Read(data, 0, data.Length);
									for (int i = 0; i < bl; ++i) {
										if (data[i] > 127) allSmall = false;
									}
									if (allSmall) {
										e = Encoding.UTF8;
									}
								}
								return e;
							} catch {
								throw;
							}
						}
					}
				}
			}
			return null;
		}

		internal void Analyse(string path) {
			Encoding = GuessEncoding(path);

			if (Encoding != null) {
				WithBom = false;
				byte[] bom = Encoding.GetPreamble();
				if (bom != null && bom.Length > 0) {
					byte[] fbom = new byte[bom.Length];
					using (var f = System.IO.File.OpenRead(path)) {
						f.Read(fbom, 0, bom.Length);
						f.Close();
					}
					if (fbom.SequenceEqual(bom)) {
						WithBom = true;
					}
				}

				string content = System.IO.File.ReadAllText(path, Encoding);

				int status = 0;
				cntUnixNL = 0;
				cntMacNL = 0;
				cntWinNL = 0;
				foreach (char c in content) {
					switch (status) {
						case 0:
							if (c == '\n') cntUnixNL++;
							if (c == '\r') status = 1;
							break;
						case 1:
							if (c == '\n') { cntWinNL++; status = 0; }
							else if (c == '\r') cntMacNL++;
							else { cntMacNL++; status = 0; }
							break;
					}
				}
				FirePropertyChanged(nameof(LineBreakInfo));

				string[] lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				indentSpaces = 0;
				indentTabs = 0;
				foreach (string l in lines) {
					if (string.IsNullOrEmpty(l)) continue;
					if (char.IsWhiteSpace(l.Last())) {
						trailingWS = true;
					}
					string r = l.TrimEnd(new char[] { ' ', '\t' });
					if (string.IsNullOrEmpty(r)) continue;

					switch (r.First()) {
						case ' ': indentSpaces++; break;
						case '\t': indentTabs++; break;
					}
				}
				FirePropertyChanged(nameof(LeadingSpaceInfo));
				FirePropertyChanged(nameof(TrailingSpaceInfo));
			}

			Enabled = true;
		}

	}
}
