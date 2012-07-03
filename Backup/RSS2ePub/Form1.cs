using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using eBdb.EpubReader;

namespace RSS2ePub
{
    public partial class Form1 : Form
    {
        XmlReader xmlReader;
        LinkedList<String> selectedIndexes = new LinkedList<String>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            button1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                xmlReader = new XmlTextReader(textBox1.Text);

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        switch (xmlReader.Name)
                        {
                            case "title":
                                {
                                    Invoke((MethodInvoker)(() =>
                                    {
                                        checkedListBox1.Items.Add(xmlReader.ReadString());
                                    }));
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
            progressBar1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var item in checkedListBox1.CheckedItems)
            {
                selectedIndexes.AddLast(item.ToString());
            }

            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var item in selectedIndexes)
            {
                MessageBox.Show(selectedIndexes.ToString());
            }
            
        }
    }
}
