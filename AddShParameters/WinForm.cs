using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

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

            UpdateListSelectedParameters();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
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

            UpdateListSelectedParameters();
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
                            case "Размеры":
                                shParameters.PDataCategory = BuiltInParameterGroup.PG_GEOMETRY;
                                break;

                            case "Прочие":
                                shParameters.PDataCategory = BuiltInParameterGroup.INVALID;
                                break;

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

            UpdateListSelectedParameters();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (Program.doc.IsFamilyDocument)
            {

                List<FamilyParameter> familyParametersList = new List<FamilyParameter>();

                familyParametersList = (List<FamilyParameter>)Program.doc.FamilyManager.GetParameters();

                List<string> familyParameterName = new List<string>();
                List<string> SkippedNamesList = new List<string>();
                string SkippedNamesString;

                foreach (FamilyParameter item in familyParametersList)
                {
                    familyParameterName.Add(item.Definition.Name);
                }

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

                    else { SkippedNamesList.Add(shParameters.PName); }
                }

                if (SkippedNamesList.Count != 0)
                {
                    SkippedNamesString = string.Join(", ", SkippedNamesList);

                    MessageBox.Show("Выбранные параметры добавлены в семейство, а параметры " + SkippedNamesString + " пропущены, так как уже есть в семействе");
                }

                else { MessageBox.Show("Выбранные параметры добавлены в семейство"); }
            }
            else { MessageBox.Show("Данный документ не является семейством, добавление параметров не возможно!"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //saving set of parameters
            XmlDocument xmldoc = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);

            //create the root element
            XmlElement root = xmldoc.DocumentElement;
            xmldoc.InsertBefore(xmlDeclaration, root);

            //string.Empty makes cleaner code
            XmlElement element = xmldoc.CreateElement(string.Empty, "Параметры", string.Empty);

            xmldoc.AppendChild(element);

            foreach (ShParameters Item in Program.SelectedParametersList)
            {
                XmlElement ParamName = xmldoc.CreateElement(string.Empty, "Имя_параметра", string.Empty);
                XmlText TextParamName = xmldoc.CreateTextNode(Item.PName);
                XmlElement IsInstance = xmldoc.CreateElement(string.Empty, "Параметр_экземпляра", string.Empty);
                XmlText TextIsInstance = xmldoc.CreateTextNode(Item.PIsInstance.ToString());
                XmlElement DataCategory = xmldoc.CreateElement(string.Empty, "Группирование_параметров", string.Empty);
                XmlText TextDataCategory = xmldoc.CreateTextNode(Item.PDataCategory.ToString());
                element.AppendChild(ParamName);
                ParamName.AppendChild(TextParamName);
                ParamName.AppendChild(IsInstance);
                ParamName.AppendChild(TextIsInstance);
                ParamName.AppendChild(DataCategory);
                ParamName.AppendChild(TextDataCategory);

            }

            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.DefaultExt = "xml";

            saveFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                xmldoc.Save(saveFile.FileName);
                MessageBox.Show("Выбранные параметры сохранены");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //loading set of parameters
            XmlDocument xmldoc = new XmlDocument();

            xmldoc.PreserveWhitespace = true;

            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                xmldoc.Load(openFile.FileName);

                MessageBox.Show("Файл " + openFile.FileName + " успешно загружен");

                foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                {
                    foreach (ShParameters Item in Program.ParameterList)
                    {
                        if ((Item.PName == xmlnode.FirstChild.InnerText) & (!Program.SelectedParametersList.Contains(Item)))
                        {
                            Program.SelectedParametersList.Add(Item);
                        }
                    }
                }

                foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                {
                    foreach (ShParameters Item in Program.SelectedParametersList)
                    {
                        if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("True"))))
                        {
                            Item.PIsInstance = true;
                        }
                        else if (xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("False")))
                        {
                            Item.PIsInstance = false;
                        }
                    }
                }

                foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                {
                    foreach (ShParameters Item in Program.SelectedParametersList)
                    {

                        if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_GEOMETRY"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_GEOMETRY;
                        }

                        if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("INVALID"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.INVALID;
                        }

                        if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_TEXT"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_TEXT;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_CONSTRAINTS"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_CONSTRAINTS;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_CONSTRUCTION"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_CONSTRUCTION;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_DATA"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_DATA;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_IDENTITY_DATA"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_IDENTITY_DATA;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_MATERIALS"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_MATERIALS;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_MECHANICAL"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_MECHANICAL;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_MECHANICAL_AIRFLOW"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_MECHANICAL_LOADS"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_MECHANICAL_LOADS;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_PLUMBING"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_PLUMBING;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_STRUCTURAL"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_STRUCTURAL;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_STRUCTURAL_ANALYSIS"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_VISIBILITY"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_VISIBILITY;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_AREA"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_AREA;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_ELECTRICAL"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_ELECTRICAL_CIRCUITING"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_ELECTRICAL_LIGHTING"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_ELECTRICAL_LOADS"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_ELECTRICAL_LOADS;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_FORCES"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_FORCES;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_GENERAL"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_GENERAL;
                        }
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_GRAPHICS"))))
                        {
                            Item.PDataCategory = BuiltInParameterGroup.PG_GRAPHICS;
                        }
                    }
                }
            }

            UpdateListSelectedParameters();
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

            UpdateListSelectedParameters();
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

            UpdateListSelectedParameters();
        }

        public void UpdateListSelectedParameters()
        {
            listView2.Items.Clear();

            foreach (ShParameters ParItem in Program.SelectedParametersList)
            {
                ListViewItem LvItem = new ListViewItem(ParItem.PName);

                if (ParItem.PIsInstance == true)
                {
                    LvItem.SubItems.Add("Экземпляр");
                }
                else
                {
                    LvItem.SubItems.Add("Тип");
                }

                switch (ParItem.PDataCategory)
                {
                    case BuiltInParameterGroup.PG_GEOMETRY:
                        LvItem.SubItems.Add("Размеры");
                        break;

                    case BuiltInParameterGroup.INVALID:
                        LvItem.SubItems.Add("Прочие");
                        break;

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

                IEnumerable enumerable = ParItem.PcategorySet.GetEnumerator();

                enumerable.Reset();

                while (enumerable.MoveNext())
                {
                    Category Item = enumerable.Current as Category;

                    LvItem.SubItems.Add(Item.Name+", ");
                }

                //foreach (Category Item in ParItem.PcategorySet)
                //{
                //    LvItem.SubItems.Add(Item.Name);
                //}

                listView2.Items.Add(LvItem);
            }
        }

        public void UpdateListinFamily()
        {
            listView3.Items.Clear();

            if (Program.doc.IsFamilyDocument)
            {
                foreach (FamilyParameter Item in Program.doc.FamilyManager.GetParameters())
                {
                    if (Item.IsShared)
                    {
                        ListViewItem LvItem = new ListViewItem(Item.Definition.Name);

                        if (!listView3.Items.Contains(LvItem))
                        {
                            listView3.Items.Add(LvItem);
                        }

                        if (Program.famType.HasValue(Item))
                        {
                            if (Item.StorageType == StorageType.Integer)
                            { LvItem.SubItems.Add(Program.famType.AsInteger(Item).ToString()); }
                            if (Item.StorageType == StorageType.String)
                            { LvItem.SubItems.Add(Program.famType.AsString(Item)); }
                            if ((Item.StorageType == StorageType.Double) & (Item.Definition.UnitType != UnitType.UT_Electrical_Potential))
                            { LvItem.SubItems.Add((((double)Program.famType.AsDouble(Item)).ToString("F"))); }
                            if ((Item.StorageType == StorageType.Double) & (Item.Definition.UnitType == UnitType.UT_Electrical_Potential))
                            { LvItem.SubItems.Add((((double)Program.famType.AsDouble(Item)) * Math.Pow(0.3048, 2)).ToString("F")); }
                        }

                        LvItem.SubItems.Add(Item.Formula);
                    }

                }
                listView3.Sort();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FamilyTypeSet familyTypeSet = Program.doc.FamilyManager.Types;

            foreach (FamilyType Item in familyTypeSet)
            {
                if ((string)comboBox3.SelectedItem == Item.Name)

                {
                    Transaction transaction = new Transaction(Program.doc, Item.Name);
                    transaction.Start();
                    Program.doc.FamilyManager.CurrentType = Item;
                    Program.famType = Item;
                    transaction.Commit();
                }
            }

            UpdateListinFamily();

            //FamilyTypeSetIterator familyTypeSetIterator = doc.FamilyManager.Types.ForwardIterator();

            //familyTypeSetIterator.Reset();

            //familyTypeSetIterator.MoveNext();

            //FamilyType famType = familyTypeSetIterator.Current as FamilyType;

            //while (familyTypeSetIterator.MoveNext())
            //{
            //    FamilyType famType = familyTypeSetIterator.Current as FamilyType;

            //    MainWindow.listView3.Items.Add(LvItem);
            //    LvItem.SubItems.Add(famType.AsString(Item));
            //    LvItem.SubItems.Add(Item.Formula);
            //}
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateListSelectedParameters();
            UpdateListinFamily();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Program.CatWindow.listView6.Items.Clear();

            foreach (Category Item in Program.doc.Settings.Categories)
            {
                if ((Item.CategoryType == CategoryType.Model) | (Item.CategoryType == CategoryType.AnalyticalModel))
                {
                    ListViewItem LvItem = new ListViewItem(Item.Name);

                    if (!Program.CatWindow.listView6.Items.Contains(LvItem))
                    {
                        Program.CatWindow.listView6.Items.Add(LvItem);
                    }
                }
            }

            Program.CatWindow.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Добавление параметров в проект

            //foreach (ShParameters Item in Program.SelectedParametersList)
            //{
            //    foreach (string i in Item.Pcategories)
            //    {
            //        if (Program.doc.Settings.Categories.Contains(i))
            //        {
            //            CategorySet categorySet = Program.doc.Application.Create.NewCategorySet();

            //            //Category category = Category.GetCategory(Program.doc, );

            //            //categorySet.Insert(category);
            //        }
            //    }
            //}

        }
    }
}
