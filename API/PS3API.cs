using PS3Lib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
namespace PS3Lib
{
	public class PS3API
	{
		private class Common
		{
			public static CCAPI CcApi;
			public static TMAPI TmApi;
		}
		public class ConsoleList
		{
			private PS3API Api;
			private List<CCAPI.ConsoleInfo> data;
			public ConsoleList(PS3API f)
			{
				this.Api = f;
				this.data = this.Api.CCAPI.GetConsoleList();
			}
			private Lang getSysLanguage()
			{
				Lang result;
				if (PS3API.SetLang.defaultLang != Lang.Null)
				{
					result = PS3API.SetLang.defaultLang;
				}
				else
				{
					if (CultureInfo.CurrentCulture.ThreeLetterWindowsLanguageName.StartsWith("FRA"))
					{
						result = Lang.French;
					}
					else
					{
						result = Lang.English;
					}
				}
				return result;
			}
			public bool Show()
			{
				bool Result = false;
				int tNum = -1;
				Label lblInfo = new Label();
				Button btnConnect = new Button();
				Button button = new Button();
				ListViewGroup listViewGroup = new ListViewGroup("Consoles", HorizontalAlignment.Left);
				ListView listView = new ListView();
				Form formList = new Form();
				btnConnect.Location = new Point(12, 254);
				btnConnect.Name = "btnConnect";
				btnConnect.Size = new Size(198, 23);
				btnConnect.TabIndex = 1;
				btnConnect.Text = this.strTraduction("btnConnect");
				btnConnect.UseVisualStyleBackColor = true;
				btnConnect.Enabled = false;
				btnConnect.Click += delegate(object sender, EventArgs e)
				{
					if (tNum > -1)
					{
						if (this.Api.ConnectTarget(this.data[tNum].Ip))
						{
							this.Api.setTargetName(this.data[tNum].Name);
							Result = true;
						}
						else
						{
							Result = false;
						}
						formList.Close();
					}
					else
					{
						MessageBox.Show(this.strTraduction("errorSelect"), this.strTraduction("errorSelectTitle"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				};
				button.Location = new Point(216, 254);
				button.Name = "btnRefresh";
				button.Size = new Size(86, 23);
				button.TabIndex = 1;
				button.Text = this.strTraduction("btnRefresh");
				button.UseVisualStyleBackColor = true;
				button.Click += delegate(object sender, EventArgs e)
				{
					tNum = -1;
					listView.Clear();
					lblInfo.Text = this.strTraduction("selectGrid");
					btnConnect.Enabled = false;
					this.data = this.Api.CCAPI.GetConsoleList();
					int num2 = this.data.Count<CCAPI.ConsoleInfo>();
					for (int j = 0; j < num2; j++)
					{
						ListViewItem value2 = new ListViewItem(" " + this.data[j].Name + " - " + this.data[j].Ip)
						{
							ImageIndex = 0
						};
						listView.Items.Add(value2);
					}
				};
				listView.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
				listViewGroup.Header = "Consoles";
				listViewGroup.Name = "consoleGroup";
				listView.Groups.AddRange(new ListViewGroup[]
				{
					listViewGroup
				});
				listView.HideSelection = false;
				listView.Location = new Point(12, 12);
				listView.MultiSelect = false;
				listView.Name = "ConsoleList";
				listView.ShowGroups = false;
				listView.Size = new Size(290, 215);
				listView.TabIndex = 0;
				listView.UseCompatibleStateImageBehavior = false;
				listView.View = View.List;
				listView.ItemSelectionChanged += delegate(object sender, ListViewItemSelectionChangedEventArgs e)
				{
					tNum = e.ItemIndex;
					btnConnect.Enabled = true;
					string text;
					if (this.data[tNum].Name.Length > 18)
					{
						text = this.data[tNum].Name.Substring(0, 17) + "...";
					}
					else
					{
						text = this.data[tNum].Name;
					}
					string text2;
					if (this.data[tNum].Ip.Length > 16)
					{
						text2 = this.data[tNum].Name.Substring(0, 16) + "...";
					}
					else
					{
						text2 = this.data[tNum].Ip;
					}
					lblInfo.Text = string.Concat(new string[]
					{
						this.strTraduction("selectedLbl"),
						" ",
						text,
						" / ",
						text2
					});
				};
				lblInfo.AutoSize = true;
				lblInfo.Location = new Point(12, 234);
				lblInfo.Name = "lblInfo";
				lblInfo.Size = new Size(158, 13);
				lblInfo.TabIndex = 3;
				lblInfo.Text = this.strTraduction("selectGrid");
				formList.Icon = Resources.fenetre;
				formList.MinimizeBox = false;
				formList.MaximizeBox = false;
				formList.ClientSize = new Size(314, 285);
				formList.AutoScaleDimensions = new SizeF(6f, 13f);
				formList.AutoScaleMode = AutoScaleMode.Font;
				formList.FormBorderStyle = FormBorderStyle.FixedSingle;
				formList.StartPosition = FormStartPosition.CenterScreen;
				formList.Text = this.strTraduction("formTitle");
				formList.Controls.Add(listView);
				formList.Controls.Add(lblInfo);
				formList.Controls.Add(btnConnect);
				formList.Controls.Add(button);
				ImageList imageList = new ImageList();
				imageList.Images.Add(Resources.ps3);
				listView.SmallImageList = imageList;
				int num = this.data.Count<CCAPI.ConsoleInfo>();
				for (int i = 0; i < num; i++)
				{
					ListViewItem value = new ListViewItem(" " + this.data[i].Name + " - " + this.data[i].Ip)
					{
						ImageIndex = 0
					};
					listView.Items.Add(value);
				}
				if (num > 0)
				{
					formList.ShowDialog();
				}
				else
				{
					Result = false;
					formList.Close();
					MessageBox.Show(this.strTraduction("noConsole"), this.strTraduction("noConsoleTitle"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				return Result;
			}
			private string strTraduction(string keyword)
			{
				string text;
				string result;
				if (this.getSysLanguage() == Lang.French)
				{
					switch (keyword)
					{
					case "btnConnect":
						text = "Connexion";
						result = text;
						return result;
					case "btnRefresh":
						text = "Rafraîchir";
						result = text;
						return result;
					case "errorSelect":
						text = "Vous devez d'abord sélectionner une console.";
						result = text;
						return result;
					case "errorSelectTitle":
						text = "Sélectionnez une console.";
						result = text;
						return result;
					case "selectGrid":
						text = "Sélectionnez une console dans la grille.";
						result = text;
						return result;
					case "selectedLbl":
						text = "Sélection :";
						result = text;
						return result;
					case "formTitle":
						text = "Choisissez une console...";
						result = text;
						return result;
					case "noConsole":
						text = "Aucune console disponible, démarrez CCAPI Manager (v2.5) et ajoutez une nouvelle console.";
						result = text;
						return result;
					case "noConsoleTitle":
						text = "Aucune console disponible.";
						result = text;
						return result;
					}
				}
				else
				{
					switch (keyword)
					{
					case "btnConnect":
						text = "Connection";
						result = text;
						return result;
					case "btnRefresh":
						text = "Refresh";
						result = text;
						return result;
					case "errorSelect":
						text = "You need to select a console first.";
						result = text;
						return result;
					case "errorSelectTitle":
						text = "Select a console.";
						result = text;
						return result;
					case "selectGrid":
						text = "Select a console within this grid.";
						result = text;
						return result;
					case "selectedLbl":
						text = "Selected :";
						result = text;
						return result;
					case "formTitle":
						text = "Select a console...";
						result = text;
						return result;
					case "noConsole":
						text = "None consoles available, run CCAPI Manager (v2.5) and add a new console.";
						result = text;
						return result;
					case "noConsoleTitle":
						text = "None console available.";
						result = text;
						return result;
					}
				}
				text = "?";
				result = text;
				return result;
			}
		}
		private class SetAPI
		{
			public static SelectAPI API;
		}
		private class SetLang
		{
			public static Lang defaultLang;
		}
		private static string targetIp = string.Empty;
		private static string targetName = string.Empty;
		public CCAPI CCAPI
		{
			get
			{
				return new CCAPI();
			}
		}
		public Extension Extension
		{
			get
			{
				return new Extension(PS3API.SetAPI.API);
			}
		}
		public TMAPI TMAPI
		{
			get
			{
				return new TMAPI();
			}
		}
		public PS3API(SelectAPI API = SelectAPI.ControlConsole)
		{
			PS3API.SetAPI.API = API;
			this.MakeInstanceAPI(API);
		}
		public bool AttachProcess()
		{
			this.MakeInstanceAPI(this.GetCurrentAPI());
			bool flag = false;
			bool result;
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				result = PS3API.Common.TmApi.AttachProcess();
			}
			else
			{
				if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
				{
					flag = PS3API.Common.CcApi.SUCCESS(PS3API.Common.CcApi.AttachProcess());
				}
				result = flag;
			}
			return result;
		}
		public void ChangeAPI(SelectAPI API)
		{
			PS3API.SetAPI.API = API;
			this.MakeInstanceAPI(this.GetCurrentAPI());
		}
		public bool ConnectTarget(int target = 0)
		{
			this.MakeInstanceAPI(this.GetCurrentAPI());
			bool result;
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				result = PS3API.Common.TmApi.ConnectTarget(target);
			}
			else
			{
				result = new PS3API.ConsoleList(this).Show();
			}
			return result;
		}
		public bool ConnectTarget(string ip)
		{
			this.MakeInstanceAPI(this.GetCurrentAPI());
			bool result;
			if (PS3API.Common.CcApi.SUCCESS(PS3API.Common.CcApi.ConnectTarget(ip)))
			{
				PS3API.targetIp = ip;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public void DisconnectTarget()
		{
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				PS3API.Common.TmApi.DisconnectTarget();
			}
			else
			{
				PS3API.Common.CcApi.DisconnectTarget();
			}
		}
		public byte[] GetBytes(uint offset, int length)
		{
			byte[] array = new byte[length];
			byte[] result;
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				PS3API.Common.TmApi.GetMemory(offset, array);
				result = array;
			}
			else
			{
				if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
				{
					PS3API.Common.CcApi.GetMemory(offset, array);
				}
				result = array;
			}
			return result;
		}
		public string GetConsoleName()
		{
			string name;
			string result;
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				name = PS3API.Common.TmApi.SCE.GetTargetName();
			}
			else
			{
				if (PS3API.targetName != string.Empty)
				{
					name = PS3API.targetName;
				}
				else
				{
					if (PS3API.targetIp != string.Empty)
					{
						List<CCAPI.ConsoleInfo> list = new List<CCAPI.ConsoleInfo>();
						list = PS3API.Common.CcApi.GetConsoleList();
						if (list.Count > 0)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Ip == PS3API.targetIp)
								{
									name = list[i].Name;
									result = name;
									return result;
								}
							}
						}
					}
					name = PS3API.targetIp;
				}
			}
			result = name;
			return result;
		}
		public SelectAPI GetCurrentAPI()
		{
			return PS3API.SetAPI.API;
		}
		public string GetCurrentAPIName()
		{
			string result;
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				result = Enum.GetName(typeof(SelectAPI), SelectAPI.TargetManager).Replace("Manager", " Manager");
			}
			else
			{
				result = Enum.GetName(typeof(SelectAPI), SelectAPI.ControlConsole).Replace("Console", " Console");
			}
			return result;
		}
		public void GetMemory(uint offset, byte[] buffer)
		{
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				PS3API.Common.TmApi.GetMemory(offset, buffer);
			}
			else
			{
				if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
				{
					PS3API.Common.CcApi.GetMemory(offset, buffer);
				}
			}
		}
		public void InitTarget()
		{
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				PS3API.Common.TmApi.InitComms();
			}
		}
		private void MakeInstanceAPI(SelectAPI API)
		{
			if (API == SelectAPI.TargetManager && PS3API.Common.TmApi == null)
			{
				PS3API.Common.TmApi = new TMAPI();
			}
			if (API == SelectAPI.ControlConsole && PS3API.Common.CcApi == null)
			{
				PS3API.Common.CcApi = new CCAPI();
			}
		}
		public Assembly PS3TMAPI_NET()
		{
			return PS3API.Common.TmApi.PS3TMAPI_NET();
		}
		public void SetFormLang(Lang Language)
		{
			PS3API.SetLang.defaultLang = Language;
		}
		public void SetMemory(uint offset, byte[] buffer)
		{
			if (PS3API.SetAPI.API == SelectAPI.TargetManager)
			{
				PS3API.Common.TmApi.SetMemory(offset, buffer);
			}
			else
			{
				if (PS3API.SetAPI.API == SelectAPI.ControlConsole)
				{
					PS3API.Common.CcApi.SetMemory(offset, buffer);
				}
			}
		}
		public void setTargetName(string value)
		{
			PS3API.targetName = value;
		}
	}
}
