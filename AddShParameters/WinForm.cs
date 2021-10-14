﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Build;

namespace AddShParameters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public partial class WinForm : System.Windows.Forms.Form
    {
        public List<string> Listgroup = new List<string>();

        public List<string> CategoriesList = new List<string>();

        public WinForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Выпадающий список выбора групп из ФОПа

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
            //Выбор параметр типа или экземпляра

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
            //Выбор параметр типа или экземпляра

            foreach (ListViewItem i in listView2.SelectedItems)
            {
                foreach (ShParameters shParameters in Program.SelectedParametersList)
                {
                    if (shParameters.PName == i.Text)
                    {
                        shParameters.PIsInstance = false;
                    }
                }
            }

            UpdateListSelectedParameters();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Выбор группы для параметров (видимость, данные и т.д.)

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

            //Добавление параметров из окна выбранных параметров в семейство

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
            //Сохранение набора параметров

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
                XmlElement ParCategorySet = xmldoc.CreateElement(string.Empty, "Перечень_категорий", string.Empty);

                CategoriesList.Clear();

                foreach (Category Itemcategory in Item.PcategorySet)
                {
                    CategoriesList.Add(Itemcategory.Name);
                }
                XmlText TextCategorySet = xmldoc.CreateTextNode(string.Join(", ", CategoriesList));

                element.AppendChild(ParamName);
                ParamName.AppendChild(TextParamName);
                ParamName.AppendChild(IsInstance);
                ParamName.AppendChild(TextIsInstance);
                ParamName.AppendChild(DataCategory);
                ParamName.AppendChild(TextDataCategory);
                ParamName.AppendChild(ParCategorySet);
                ParamName.AppendChild(TextCategorySet);

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
            //Загрузка набора параметров

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
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Equals("PG_DATA"))))
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
                        else if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Equals("PG_MECHANICAL"))))
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
                        else if ((xmlnode.InnerText.Equals(Item.PName) & (xmlnode.InnerText.Equals("PG_STRUCTURAL"))))
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
                        else if ((xmlnode.InnerText.Equals(Item.PName) & (xmlnode.InnerText.Equals("PG_ELECTRICAL"))))
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

                foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                {
                    foreach (ShParameters Item in Program.SelectedParametersList)
                    {
                        foreach (Category catitem in Program.doc.Settings.Categories)
                        {
                            if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains(catitem.Name))))
                            {
                                Item.PcategorySet.Insert(catitem);
                            }
                        }
                    }
                }

                UpdateListSelectedParameters();
            }
        }


        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Кнопка переноса из окна параметров ФОПа в выбранные параметры

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
            //Кнопка переноса из окна выбранных параметров в окно параметров ФОПа

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
            //Метод обновления параметров в окне выбранных параметров

            listView2.Items.Clear();

            foreach (ShParameters ParItem in Program.SelectedParametersList)
            {
                CategoriesList.Clear();

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

                foreach (Category Item in ParItem.PcategorySet)
                {
                    CategoriesList.Add(Item.Name);
                }

                CategoriesList.Sort();

                LvItem.SubItems.Add(string.Join(", ", CategoriesList));

                listView2.Items.Add(LvItem);
            }
        }

        public void UpdateListinFamily()
        {
            //Обновление списка параметров и их значений в семействе

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

                        LvItem.SubItems.Add("Не выбрано действие");
                    }
                }

                listView3.Sort();

                foreach (FamilyParameter Item in Program.doc.FamilyManager.GetParameters())
                {
                    if (Item.IsShared)
                    {
                        if (!comboBox4.Items.Contains(Item.Definition.Name))
                        { comboBox4.Items.Add(Item.Definition.Name); }
                    }
                }
                if (comboBox4.Items.Count != 0)
                { comboBox4.SelectedIndex = 0; }
            }
        }

        public void AddSelectedParameterstoProject()
        {
            //Метод добавления параметров в окне выбранных параметров

            if (!Program.doc.IsFamilyDocument)
            {
                List<string> SkippedList = new List<string>();

                List<string> AddedCategoryList = new List<string>();

                BindingMap bindingMap = Program.doc.ParameterBindings;

                foreach (ShParameters Item in Program.SelectedParametersList)
                {
                    if (Item.PcategorySet.IsEmpty)
                    {
                        SkippedList.Add(Item.PName);
                    }

                    if ((!bindingMap.Contains(Item.PexternalDefinition)) & (!Item.PcategorySet.IsEmpty))
                    {

                        if (Item.PIsInstance == true)
                        {
                            Transaction transaction = new Transaction(Program.doc, Item.PName);
                            transaction.Start();
                            InstanceBinding instanceBinding = Program.doc.Application.Create.NewInstanceBinding(Item.PcategorySet);
                            bindingMap.Insert(Item.PexternalDefinition, instanceBinding, Item.PDataCategory);
                            transaction.Commit();
                        }
                        else
                        {
                            Transaction transaction = new Transaction(Program.doc, Item.PName);
                            transaction.Start();
                            TypeBinding typeBinding = Program.doc.Application.Create.NewTypeBinding(Item.PcategorySet);
                            bindingMap.Insert(Item.PexternalDefinition, typeBinding, Item.PDataCategory);
                            transaction.Commit();
                        }
                    }

                    else if (!Item.PcategorySet.IsEmpty)
                    {
                        ElementBinding elementbinding = Program.doc.ParameterBindings.get_Item(Item.PexternalDefinition) as ElementBinding;

                        foreach (Category cat in Item.PcategorySet)
                        {
                            if (!elementbinding.Categories.Contains(cat))
                            {
                                elementbinding.Categories.Insert(cat);
                            }
                        }

                        Transaction transaction = new Transaction(Program.doc, Item.PName);
                        transaction.Start();
                        bindingMap.ReInsert(Item.PexternalDefinition, elementbinding, Item.PDataCategory);
                        transaction.Commit();

                        AddedCategoryList.Add(Item.PName);
                    }
                }

                if (SkippedList.Count != 0)
                {
                    MessageBox.Show("Параметры " + string.Join(", ", SkippedList) + " пропущены, так как для них не выбрана категория в проекте!");
                }

                if (AddedCategoryList.Count != 0)
                {
                    MessageBox.Show("Для параметров " + string.Join(", ", AddedCategoryList) + " добавлены дополнительные категории в проекте");
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Переключение типоразмеров в семействе

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

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Переключение вкладок основого окна

            UpdateListSelectedParameters();
            UpdateListinFamily();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Кнопка выбора категории в проекте

            Program.CatWindow.listView6.Items.Clear();

            foreach (Category Item in Program.doc.Settings.Categories)
            {
                if ((Item.AllowsBoundParameters) & (Item.IsVisibleInUI))
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
            //Кнопка добавление параметров в проект

            AddSelectedParameterstoProject();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Кнопка проверка проекта на параметры

            button3_Click(this.button3, EventArgs.Empty);

            AddSelectedParameterstoProject();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Добавление действия удаления с выбранными общими параметрами в общем окне параметров

            if (Program.doc.IsFamilyDocument)
            {
                foreach (ListViewItem Item in listView3.SelectedItems)
                {
                    if (Item.SubItems[3].Text == "Не выбрано действие")
                    {
                        Item.SubItems.Insert(3, Item.SubItems.Add("Удалить"));
                    }
                }
                //UpdateListinFamily();
            }

            this.checkBox1.Checked = false;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Добавление действия замены на параметр из выпадающего списка

            if (Program.doc.IsFamilyDocument)
            {
                foreach (ListViewItem Item in listView3.SelectedItems)
                {
                    if (Item.SubItems[3].Text != "Удалить")
                    {
                        Item.SubItems.Insert(3, Item.SubItems.Add("Заменить на " + comboBox4.Text));
                    }
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //Сохранение действий по замене и удалению параметров в .xml файл

            if (Program.doc.IsFamilyDocument)
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

                foreach (ListViewItem Item in listView3.Items)
                {
                    if (Item.SubItems[3].Text != "Не выбрано действие")
                    {
                        XmlElement ParamName = xmldoc.CreateElement(string.Empty, "Имя_параметра", string.Empty);
                        XmlText TextParamName = xmldoc.CreateTextNode(Item.Text);
                        XmlElement ParamNameReplace = xmldoc.CreateElement(string.Empty, "Действие", string.Empty);
                        XmlText TextparamNameReplace = xmldoc.CreateTextNode(Item.SubItems[3].Text);

                        element.AppendChild(ParamName);
                        ParamName.AppendChild(TextParamName);
                        ParamName.AppendChild(ParamNameReplace);
                        ParamName.AppendChild(TextparamNameReplace);
                    }
                }

                SaveFileDialog saveFile = new SaveFileDialog();

                saveFile.DefaultExt = "xml";

                saveFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    xmldoc.Save(saveFile.FileName);
                    MessageBox.Show("Выбранные действия с параметрами сохранены");
                }
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Загрузка действий из .xml файла

            if (Program.doc.IsFamilyDocument)
            {
                //loading set of parameters
                XmlDocument xmldoc = new XmlDocument();

                xmldoc.PreserveWhitespace = true;

                OpenFileDialog openFile = new OpenFileDialog();

                openFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    xmldoc.Load(openFile.FileName);

                    MessageBox.Show("Файл с настройками действий с параметрами" + openFile.FileName + " успешно загружен");

                    foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                    {
                        if ((!listView3.Items.ContainsKey(xmlnode.FirstChild.InnerText)))
                        {
                            listView3.Items.Add(xmlnode.FirstChild.InnerText);

                        }
                    }

                    //foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                    //{
                    //    foreach (ShParameters Item in Program.SelectedParametersList)
                    //    {
                    //        if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("True"))))
                    //        {
                    //            Item.PIsInstance = true;
                    //        }
                    //        else if (xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("False")))
                    //        {
                    //            Item.PIsInstance = false;
                    //        }
                    //    }
                    //}

                    //foreach (XmlNode xmlnode in xmldoc.DocumentElement.GetElementsByTagName("Имя_параметра"))
                    //{
                    //    foreach (ShParameters Item in Program.SelectedParametersList)
                    //    {

                    //        if ((xmlnode.InnerText.Contains(Item.PName) & (xmlnode.InnerText.Contains("PG_GEOMETRY"))))
                    //        {
                    //            Item.PDataCategory = BuiltInParameterGroup.PG_GEOMETRY;
                    //        }
                    //    }
                    //}


                    UpdateListinFamily();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Выполнение действий, предварительно заданных пользователем

            if (Program.doc.IsFamilyDocument)
            {
                foreach (ListViewItem Item in listView3.Items)
                {
                    foreach (FamilyParameter familyParameter in Program.doc.FamilyManager.GetParameters())
                    {
                        if ((Item.SubItems[3].Text.StartsWith("Заменить на ")) & ((Item.SubItems[3].Text.EndsWith(familyParameter.Definition.Name))))
                        {
                            Transaction transaction = new Transaction(Program.doc, Item.Text);
                            transaction.Start();
                            Program.doc.FamilyManager.ReplaceParameter(Program.doc.FamilyManager.get_Parameter(Item.Text),
                                                                        familyParameter.Definition.Name + "_family parameter",
                                                                        Program.doc.FamilyManager.get_Parameter(Item.Text).Definition.ParameterGroup,
                                                                        Program.doc.FamilyManager.get_Parameter(Item.Text).IsInstance);
                            transaction.Commit();

                            string Familyparametername = familyParameter.Definition.Name + "_family parameter";

                            string Sharedhparametername = Familyparametername.Replace("_family parameter", "");

                            Transaction transaction2 = new Transaction(Program.doc, Sharedhparametername);
                            transaction2.Start();
                            Program.doc.FamilyManager.RemoveParameter(familyParameter);
                            Program.doc.FamilyManager.ReplaceParameter(Program.doc.FamilyManager.get_Parameter(Familyparametername),
                                            Program.ParameterList.Find(name => name.PName == Sharedhparametername).PexternalDefinition,
                                            Program.ParameterList.Find(name => name.PName == Sharedhparametername).PDataCategory,
                                            Program.ParameterList.Find(name => name.PName == Sharedhparametername).PIsInstance);
                            transaction2.Commit();
                        }
                    }
                }

                foreach (ListViewItem Item in listView3.Items)
                {
                    foreach (FamilyParameter familyParameter in Program.doc.FamilyManager.GetParameters())
                    {
                        if ((familyParameter.Definition.Name == Item.Text) & (Item.SubItems[3].Text == "Удалить"))
                        {
                            Transaction transaction = new Transaction(Program.doc, familyParameter.Definition.Name);
                            transaction.Start();
                            Program.doc.FamilyManager.RemoveParameter(familyParameter);
                            transaction.Commit();
                        }
                    }
                }

                UpdateListinFamily();
            }
        }
    }
}
