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
    public partial class WinForm2 : Form
    {
        public WinForm2()
        {
            InitializeComponent();
        }

        private void listView6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //добавление категорий к параметрам
            foreach (ListViewItem i in Program.MainWindow.listView2.SelectedItems)
            {
                foreach (ShParameters shParameters in Program.SelectedParametersList)
                {
                    if (shParameters.PName == i.Text)
                    {
                        shParameters.Pcategories.Add(Program.CatWindow.listView6.SelectedItems.ToString());
                    }
                }
            }

            Program.MainWindow.UpdateListSelectedParameters();

            Program.CatWindow.Close();
        }
    }
}
