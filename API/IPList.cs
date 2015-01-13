using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PS3Lib.Properties;
namespace PS3Lib
{
    public partial class IPList : Form
    {
        public IPList()
        {
            InitializeComponent();
        }
        public string[] ipmac = { " ", " " };
        
        private void IPList_Load(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.AddRange(ipmac);
            }
            catch
            {

            }
            label1.Text = "PS3Lib " + "Version: " + this.ProductVersion + "\n" + "Modified By: BaSs_HaXoR";
            label1.Left = (label1.Parent.Width - label1.Width) / 2;
        }
        public String connection = "N/a";
        public String MACaddress = "N/a";
        public bool isconnected = false;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            connection = comboBox1.Text;
            MACaddress = comboBox1.Text;
            connection = connection.Split('-')[0];
            MACaddress = MACaddress.Substring(MACaddress.LastIndexOf(@"-") + 1);
            connection = connection.Replace(" ", "");
        }
    }
}
