using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace AddShParameters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public partial class WinForm : System.Windows.Forms.Form
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
            //List<ShParameters> Selectedlist = new List<ShParameters>();

            foreach (ListViewItem i in listView2.SelectedItems)
            {
                foreach (ShParameters shParameters in Program.SelectedParametersList)
                {
                    if (shParameters.PName == i.Text)
                    {
                        //Selectedlist.Add(shParameters);
                        shParameters.PIsInstance = true;
                    }
                }
            }

            updatelistview();

            //foreach (ListViewItem i in listView2.Items)
            //{
            //    foreach (ShParameters shParameters in Selectedlist)
            //    {
            //        if (shParameters.PName == i.Text)
            //        {
            //            i.Checked = true;
            //        }
            //    }
            //}

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //List<ShParameters> Selectedlist = new List<ShParameters>();

            foreach (ListViewItem i in listView2.SelectedItems)
            {
                foreach (ShParameters shParameters in Program.SelectedParametersList)
                {
                    if (shParameters.PName == i.Text)
                    {
                        //Selectedlist.Add(shParameters);
                        shParameters.PIsInstance = false;
                    }
                }
            }

            updatelistview();

            //foreach (ListViewItem i in listView2.Items)
            //{
            //    foreach (ShParameters shParameters in Selectedlist)
            //    {
            //        if (shParameters.PName == i.Text)
            //        {
            //            i.Checked = true;
            //        }
            //    }
            //}
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem i in listView2.SelectedItems)
            {
                foreach (ShParameters shParameters in Program.SelectedParametersList)
                {
                    if (i.Text == shParameters.PName)
                    {
                        switch (comboBox2.SelectedItem)
                        {
                            case "Текст":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_TEXT;
                                break;

                            case "Зависимости":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_CONSTRAINTS;
                                break;

                            case "Конструкции":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_CONSTRUCTION;
                                break;

                            case "Данные":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_DATA;
                                break;

                            case "Идентификация":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_IDENTITY_DATA;
                                break;

                            case "Материалы и отделка":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_MATERIALS;
                                break;

                            case "Механизмы":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_MECHANICAL;
                                break;

                            case "Механизмы - расход":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW;
                                break;

                            case "Механизмы - нагрузки":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_MECHANICAL_LOADS;
                                break;

                            case "Сантехника":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_PLUMBING;
                                break;

                            case "Несущие конструкции":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_STRUCTURAL;
                                break;

                            case "Расчет несущих конструкций":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS;
                                break;

                            case "Видимость":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_VISIBILITY;
                                break;

                            case "Результаты анализа":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_AREA;
                                break;

                            case "Электросети":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL;
                                break;

                            case "Электросети - Создание цепей":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING;
                                break;

                            case "Электросети - Освещение":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING;
                                break;

                            case "Электросети - Нагрузки":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL_LOADS;
                                break;

                            case "Силы":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_FORCES;
                                break;

                            case "Общие":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_GENERAL;
                                break;

                            case "Графика":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_GRAPHICS;
                                break;
                        }
                    }
                }
            }

            updatelistview();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<FamilyParameter> familyParametersList = new List<FamilyParameter>();

            familyParametersList = (List<FamilyParameter>)Program.doc.FamilyManager.GetParameters();

            List<string> familyParameterName = new List<string>();
            List<string> SkippedName = new List<string>();
            string stringSkippedNames;

            foreach (FamilyParameter item in familyParametersList)
            {
                familyParameterName.Add(item.Definition.Name);
            }
            //Добавление выбранных параметров в семейство

            if (Program.doc.IsFamilyDocument)
            {
                foreach (ShParameters shParameters in Program.SelectedParametersList)
                {
                    if (!familyParameterName.Contains(shParameters.PName))
                    {
                        Transaction transaction = new Transaction(Program.doc, shParameters.PName);
                        transaction.Start();
                        Program.doc.FamilyManager.AddParameter(shParameters.PexternalDefinition, shParameters.PDataCategory, shParameters.PIsInstance);
                        transaction.Commit();
                        familyParameterName.Add(shParameters.PName);
                    }

                    else  {SkippedName.Add(shParameters.PName);}
                }
                stringSkippedNames = string.Join(" ", SkippedName);
                MessageBox.Show("Выбранные параметры добавлены в семейство, а параметры " + stringSkippedNames + " пропущены");
            }
            else { MessageBox.Show("Данный документ не является семейством, добавление параметров не возможно!"); }
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
                for (int counter = 0; counter < Program.SelectedParametersList.Count; counter++)
                {
                    if (i.Text == Program.SelectedParametersList[counter].PName)
                    {
                        Program.SelectedParametersList.Remove(Program.SelectedParametersList[counter]);
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

                switch (ParItem.PDataCategory)
                {
                    case BuiltInParameterGroup.PG_TEXT:
                        LvItem.SubItems.Add("Текст");
                        break;

                    case BuiltInParameterGroup.PG_CONSTRAINTS:
                        LvItem.SubItems.Add("Зависимости");
                        break;

                    case BuiltInParameterGroup.PG_CONSTRUCTION:
                        LvItem.SubItems.Add("Конструкции");
                        break;

                    case BuiltInParameterGroup.PG_DATA:
                        LvItem.SubItems.Add("Данные");
                        break;

                    case BuiltInParameterGroup.PG_IDENTITY_DATA:
                        LvItem.SubItems.Add("Идентификация");
                        break;

                    case BuiltInParameterGroup.PG_MATERIALS:
                        LvItem.SubItems.Add("Материалы и отделка");
                        break;

                    case BuiltInParameterGroup.PG_MECHANICAL:
                        LvItem.SubItems.Add("Механизмы");
                        break;

                    case BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW:
                        LvItem.SubItems.Add("Механизмы - расход");
                        break;

                    case BuiltInParameterGroup.PG_MECHANICAL_LOADS:
                        LvItem.SubItems.Add("Механизмы - нагрузки");
                        break;

                    case BuiltInParameterGroup.PG_PLUMBING:
                        LvItem.SubItems.Add("Сантехника");
                        break;

                    case BuiltInParameterGroup.PG_STRUCTURAL:
                        LvItem.SubItems.Add("Несущие конструкции");
                        break;

                    case BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS:
                        LvItem.SubItems.Add("Расчет несущих конструкций");
                        break;

                    case BuiltInParameterGroup.PG_VISIBILITY:
                        LvItem.SubItems.Add("Видимость");
                        break;

                    case BuiltInParameterGroup.PG_AREA:
                        LvItem.SubItems.Add("Результаты анализа");
                        break;

                    case BuiltInParameterGroup.PG_ELECTRICAL:
                        LvItem.SubItems.Add("Электросети");
                        break;

                    case BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING:
                        LvItem.SubItems.Add("Электросети - Создание цепей");
                        break;

                    case BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING:
                        LvItem.SubItems.Add("Электросети - Освещение");
                        break;

                    case BuiltInParameterGroup.PG_ELECTRICAL_LOADS:
                        LvItem.SubItems.Add("Электросети - Нагрузки");
                        break;

                    case BuiltInParameterGroup.PG_FORCES:
                        LvItem.SubItems.Add("Силы");
                        break;

                    case BuiltInParameterGroup.PG_GENERAL:
                        LvItem.SubItems.Add("Общие");
                        break;

                    case BuiltInParameterGroup.PG_GRAPHICS:
                        LvItem.SubItems.Add("Графика");
                        break;
                }

                listView2.Items.Add(LvItem);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

        }

    }
}
