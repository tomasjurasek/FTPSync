using ServiceTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;


namespace ConfigurationApp
{
    public partial class Form1 : Form
    {
        //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        string path = @"C:\Configuration.xml";
        ModulWatcher.FTP ftp = new ModulWatcher.FTP();


         Assembly guiApp;
         ResourceManager rm;

       
        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("cs-CZ");
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
                if (!CheckEmptyTextBoxes())
                {
                    Shared.Configuration configuration = new Shared.Configuration();
                    configuration.Item = new Shared.Item[7];

                    configuration.Item[0] = new Shared.Item { Key = "FTPServer", Value = textBox1.Text };
                    configuration.Item[1] = new Shared.Item { Key = "Location", Value = textBox2.Text };
                    configuration.Item[2] = new Shared.Item { Key = "Modul", Value = Path.GetDirectoryName(Application.ExecutablePath) };
                    configuration.Item[3] = new Shared.Item { Key = "Login", Value = textBox3.Text };
                    configuration.Item[4] = new Shared.Item { Key = "Password", Value = textBox4.Text };
                    configuration.Item[5] = new Shared.Item { Key = "Gmail", Value = textBox5.Text };
                    configuration.Item[6] = new Shared.Item { Key = "PasswordGMail", Value = textBox6.Text };
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        var ser = new XmlSerializer(typeof(Shared.Configuration));

                        ser.Serialize(fileStream, configuration);
                        //configuration = (Shared.Configuration)ser.Deserialize(fileStream);
                    }

                }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            guiApp = Assembly.Load("ConfigurationApp");
            rm = new ResourceManager("ConfigurationApp.Languages.Resource", guiApp);

            if (File.Exists(path))
            {
                textBox1.Text = Shared.Settings.FTPServer;
                textBox2.Text = Shared.Settings.Location;
                textBox3.Text = Shared.Settings.Login;
                textBox4.Text = Shared.Settings.Password;
                textBox5.Text = Shared.Settings.Gmail;
                textBox6.Text = Shared.Settings.GmailPassword;
            }

            ChangeLanguage();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("cs-CZ");
                ChangeLanguage();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton2.Checked)
            {
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-Us");
                    ChangeLanguage();
            }
        }

        private void ChangeLanguage()
        {
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            label1.Text = rm.GetString("ftp", ci);
            label2.Text = rm.GetString("path", ci);
            label3.Text = rm.GetString("login", ci);
            label4.Text = rm.GetString("pass", ci);
            button2.Text = rm.GetString("choose", ci);
            button1.Text = rm.GetString("save", ci);
            label5.Text = rm.GetString("gmail", ci);
            label6.Text = rm.GetString("gmailpassword", ci);
        }

        private bool CheckEmptyTextBoxes()
        {
            foreach (Control item in this.Controls)
            {
                if (item is TextBox)
                {
                    if (String.IsNullOrEmpty(item.Text) && String.IsNullOrWhiteSpace(item.Text))
                        return true;
                }
            }
            return false;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
