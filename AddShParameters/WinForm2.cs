using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;

namespace AddShParameters
{
    public partial class WinForm2 : System.Windows.Forms.Form
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
                        foreach (ListViewItem Item in Program.CatWindow.listView6.CheckedItems)
                        {
                            if (!shParameters.PcategorySet.Contains(Program.doc.Settings.Categories.get_Item(Item.Text)))
                            {
                                shParameters.PcategorySet.Insert(Program.doc.Settings.Categories.get_Item(Item.Text));
                                //shParameters.Pcategories.Remove("Не выбрано");
                                //shParameters.Pcategories.Add(Item.Text);

                            }
                        }
                    }
                }
            }

            foreach (ShParameters shParameters in Program.SelectedParametersList)
            {
                foreach (Category item in shParameters.PcategorySet)
                {
                    MessageBox.Show(item.Name);
                }
            }


            Program.MainWindow.UpdateListSelectedParameters();

            Program.CatWindow.Close();
        }
    }
}
