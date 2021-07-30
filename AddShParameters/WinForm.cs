using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddShParameters
{
    public partial class WinForm : Form
    {
        public List<string> Listgroup = new List<string>();

        public WinForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            foreach (ShParameters shParameters in Program.ParameterList)
            {
                if (shParameters.PGroup.Name == comboBox1.SelectedItem.ToString())
                {
                    ListViewItem LvItem = new ListViewItem(shParameters.PName);

                    LvItem.SubItems.Add(shParameters.PDataType.ToString());
                    LvItem.SubItems.Add(shParameters.PDescription);
                    listView1.Items.Add(LvItem);
                }
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //foreach (ShParameters shParameters in Program.SelectedParametersList)
            //{
            //    shParameters.PIsInstance = true;
            //}
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //foreach (ShParameters shParameters in Program.SelectedParametersList)
            //{
            //    shParameters.PIsInstance = false;
            //}
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //saving set of parameters
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //loading set of parameters
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView1.SelectedItems)
            {
                foreach (ShParameters ParItem in Program.ParameterList)
                {
                    if (ParItem.PName == i.Text)
                    {
                        if (!Program.SelectedParametersList.Contains(ParItem))
                        {
                            Program.SelectedParametersList.Add(ParItem);
                        }
                    }
                }
            }
            updatelistview();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView2.SelectedItems)
            {
                foreach (ShParameters ParItem in Program.SelectedParametersList)
                {
                    if (i.Text == ParItem.PName)
                    {
                        Program.SelectedParametersList.Remove(ParItem);
                    }
                }
            }

            updatelistview();
        }

        private void updatelistview()
        {
            listView2.Items.Clear();

            foreach (ShParameters ParItem in Program.SelectedParametersList)
            {
                ListViewItem LvItem = new ListViewItem(ParItem.PName);

                if (ParItem.PIsInstance == true)
                { LvItem.SubItems.Add("Экземпляр"); }
                else
                { LvItem.SubItems.Add("Тип"); }

                listView2.Items.Add(LvItem);
            }
        }
    }
}
