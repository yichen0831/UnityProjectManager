using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnityProjectManager
{
    public partial class Form1 : Form
    {
        const string registryRoot = "Software\\Unity Technologies\\Unity Editor 5.x";
        List<string> recentProjectPaths = new List<string>(); // store recent project paths in the registry
        Dictionary<string, string> subkeyDict = new Dictionary<string, string>(); // map readablePath to subkey

        public Form1()
        {
            InitializeComponent();
            removeButton.Enabled = false;
            LoadUnityProjectList();
        }

        private void LoadUnityProjectList()
        {
            recentProjectPaths.Clear();
            subkeyDict.Clear();

            RegistryKey keyUnity = Registry.CurrentUser.OpenSubKey(registryRoot, false);
            string[] subkeys = keyUnity.GetValueNames();

            foreach (var subkey in subkeys)
            {
                if (subkey.StartsWith("RecentlyUsedProjectPaths"))
                {
                    recentProjectPaths.Add(subkey);
                }
            }

            byte[] path;
            foreach (var subkey in recentProjectPaths)
            {
                path = (byte[])Registry.GetValue("HKEY_CURRENT_USER\\Software\\Unity Technologies\\Unity Editor 5.x", subkey, 0);
                string readablePath = Encoding.Default.GetString(path);
                subkeyDict.Add(readablePath, subkey);
                listBox1.Items.Add(readablePath);
            }

            keyUnity.Close();

            if (listBox1.Items.Count > 0)
            {
                removeButton.Enabled = true;
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
            {
                removeButton.Enabled = false;
                return;
            }

            if (listBox1.SelectedIndex != -1)
            {
                RegistryKey keyUnity = Registry.CurrentUser.OpenSubKey(registryRoot, true);
                string selected = (string)listBox1.SelectedItem;
                string subkey = subkeyDict[selected];
                keyUnity.DeleteValue(subkey);

                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                subkeyDict.Remove(selected);

                keyUnity.Close();
            }

            if (listBox1.Items.Count == 0)
            {
                removeButton.Enabled = false;
            }
        }
    }
}
