using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using SharpEpub;
using Ionic;
using Ionic.BZip2;
using Ionic.Crc;
using Ionic.Zip;
using Ionic.Zlib;

namespace RSS2ePub
{
    public partial class Form1 : Form
    {
        XmlReader xmlReader;
        XmlDocument feedXML;
        XmlNode nodeRSS;
        XmlNode nodeChannel;
        XmlNode nodeItem;
        String title;
        LinkedList<XmlNode> itemNodes = new LinkedList<XmlNode>();
        //LinkedList<String> selectedIndexes = new LinkedList<String>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            button1.Enabled = false;
            itemNodes.Clear();
            checkedListBox1.Items.Clear();

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                //Had help from http://stackoverflow.com/questions/5598698/c-sharp-rss-feed-reader-with-thread-feeds-are-written-to-listview-multiple-time

                xmlReader = new XmlTextReader(textBox1.Text);

                feedXML = new XmlDocument();

                feedXML.Load(xmlReader);

                

                //Only child of <xml> is <rss>
                for (int i = 0; i < feedXML.ChildNodes.Count; i++)
                {
                    if (feedXML.ChildNodes[i].Name == "rss")
                    {
                        //The rss tag is found and we know where it is
                        nodeRSS = feedXML.ChildNodes[i];
                        break;
                    }
                }


                //Only child of <rss> is <channel>
                for (int i = 0; i < nodeRSS.ChildNodes.Count; i++)
                {
                    if (nodeRSS.ChildNodes[i].Name == "channel")
                    {
                        //The channel tag is found and we know where it is
                        nodeChannel = nodeRSS.ChildNodes[i];
                        break;
                    }
                }

                title = nodeChannel["title"].InnerText;

                //Every post will be listed under <item> tag, child of <channel>
                for (int i = 0; i < nodeChannel.ChildNodes.Count; i++)
                {
                    if (nodeChannel.ChildNodes[i].Name == "item")
                    {

                        nodeItem = nodeChannel.ChildNodes[i];
                        itemNodes.AddLast(nodeItem);

                        Invoke((MethodInvoker)(() =>
                                    {
                                        checkedListBox1.Items.Add(nodeItem["title"].InnerText);
                                    }));
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
            /*foreach (var item in checkedListBox1.CheckedItems)
            {
                selectedIndexes.AddLast(item.ToString());
            }*/

            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //Epub ebook = new Epub("C:\\epub\\", TocOptions.ByFilename);
            EpubOnFly ebook = new EpubOnFly();
            ebook.Metadata.Creator = "RSS2ePub";
            ebook.Metadata.Language = "tr";
            ebook.Metadata.Title = title;
            ebook.Metadata.Date = DateTime.Now.ToShortDateString();
            
            //ebook.DirectorySearchOption = SharpEpub.sear
            


            foreach (XmlNode item in itemNodes)
            {
                if (checkedListBox1.CheckedItems.Contains(item["title"].InnerText))
                {
                    MessageBox.Show(item["description"].InnerText);
                }
                //MessageBox.Show(selectedIndexes.ToString());
            }

            ebook.Structure.Directories.ImageFolder = "Image";
            ebook.Structure.Directories.CssFolder = "Css";

            ebook.BuildToFile(@"c:\epub\qwe.epub");

        }
    }
}
