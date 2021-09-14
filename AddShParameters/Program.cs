using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace AddShParameters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Program : IExternalCommand
    {
        public static List<ShParameters> ParameterList = new List<ShParameters>();
        public static List<ShParameters> SelectedParametersList = new List<ShParameters>();
        public static List<string> BuildinParam = new List<string>();
        public static Document doc;
        public static FamilyType famType;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get the name of the active document
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            //Get access to shared parameter file
            DefinitionFile file = uidoc.Application.Application.OpenSharedParameterFile();

            //Get groups from shared parameter file
            DefinitionGroups myGroups = file.Groups;

            IEnumerator<DefinitionGroup> enumerator = myGroups.GetEnumerator();

            enumerator.Reset();

            //Clearing list of shared parameters
            ParameterList.Clear();

            //Loop for creating clasess
            while (enumerator.MoveNext())
            {
                DefinitionGroup myGroup = enumerator.Current;

                foreach (ExternalDefinition externalDefinition in myGroup.Definitions)
                {
                    ShParameters Shparameter = new ShParameters();

                    Shparameter.PDataCategory = BuiltInParameterGroup.PG_TEXT;
                    Shparameter.PDataType = externalDefinition.ParameterType;
                    Shparameter.PDescription = externalDefinition.Description;
                    Shparameter.PexternalDefinition = externalDefinition;
                    Shparameter.PGroup = externalDefinition.OwnerGroup;
                    Shparameter.PGUID = externalDefinition.GUID;
                    Shparameter.PIsInstance = false;
                    Shparameter.PName = externalDefinition.Name;
                    ParameterList.Add(Shparameter);
                }
            }

            List<ShParameters> sortParamList = ParameterList.OrderBy(name => name.PName).ToList();

            ParameterList = sortParamList;

            BuildinParam.Clear();
            BuildinParam.Add("Размеры");
            BuildinParam.Add("Прочие");
            BuildinParam.Add("Зависимости");
            BuildinParam.Add("Конструкции");
            BuildinParam.Add("Данные");
            BuildinParam.Add("Идентификация");
            BuildinParam.Add("Материалы и отделка");
            BuildinParam.Add("Механизмы");
            BuildinParam.Add("Механизмы - расход");
            BuildinParam.Add("Механизмы - нагрузки");
            BuildinParam.Add("Сантехника");
            BuildinParam.Add("Несущие конструкции");
            BuildinParam.Add("Расчет несущих конструкций");
            BuildinParam.Add("Текст");
            BuildinParam.Add("Видимость");
            BuildinParam.Add("Результаты анализа");
            BuildinParam.Add("Электросети");
            BuildinParam.Add("Электросети - Создание цепей");
            BuildinParam.Add("Электросети - Освещение");
            BuildinParam.Add("Электросети - Нагрузки");
            BuildinParam.Add("Силы");
            BuildinParam.Add("Общие");
            BuildinParam.Add("Графика");
            BuildinParam.Sort();


            WinForm MainWindow = new WinForm();

            List<string> Listgroup = new List<string>();

            foreach (var shParameters in ParameterList)
            {
                if (!Listgroup.Contains(shParameters.PGroup.Name))
                {
                    Listgroup.Add(shParameters.PGroup.Name);
                }
            }

            Listgroup.Sort();

            MainWindow.Listgroup = Listgroup;

            foreach (var Item in Listgroup)
            {
                MainWindow.comboBox1.Items.Add(Item);
                MainWindow.comboBox5.Items.Add(Item);
            }

            MainWindow.comboBox1.SelectedIndex = 0;
            MainWindow.comboBox5.SelectedIndex = 0;

            foreach (var Item in BuildinParam)
            {
                MainWindow.comboBox2.Items.Add(Item);
                MainWindow.comboBox6.Items.Add(Item);
            }

            MainWindow.comboBox2.SelectedIndex = 0;
            MainWindow.comboBox6.SelectedIndex = 0;

            MainWindow.listView1.Items.Clear();
            MainWindow.listView5.Items.Clear();

            foreach (ShParameters shParameters in ParameterList)
            {
                if (shParameters.PGroup.Name == MainWindow.comboBox1.SelectedItem.ToString())
                {
                    ListViewItem LvItem1 = new ListViewItem(shParameters.PName);
                    ListViewItem LvItem2 = new ListViewItem(shParameters.PName);

                    LvItem1.SubItems.Add(shParameters.PDataType.ToString());
                    LvItem1.SubItems.Add(shParameters.PDescription);
                    MainWindow.listView1.Items.Add(LvItem1);
                    
                    LvItem2.SubItems.Add(shParameters.PDataType.ToString());
                    LvItem2.SubItems.Add(shParameters.PDescription);
                    MainWindow.listView5.Items.Add(LvItem2);
                }
            }

            MainWindow.listView2.Items.Clear();

            List<string> ListFamType = new List<string>();

            if (doc.IsFamilyDocument)
            {
                foreach (FamilyType Item in doc.FamilyManager.Types)
                {
                    ListFamType.Add(Item.Name);
                }
            }

            ListFamType.Sort();

            foreach (var Item in ListFamType)
            {
                MainWindow.comboBox3.Items.Add(Item);
            }

            SelectedParametersList.Clear();

            //doc.FamilyManager.Set();

            if (doc.IsFamilyDocument)
            {
                famType = doc.FamilyManager.CurrentType;
                MainWindow.comboBox3.SelectedItem = famType.Name;
            }

            MainWindow.updatelistview2();

            if (doc.IsFamilyDocument)
            {
                foreach (FamilyParameter Item in doc.FamilyManager.GetParameters())
                {
                    if (Item.IsShared)
                    {
                        MainWindow.comboBox4.Items.Add(Item.Definition.Name);
                    }
                }
            }

            foreach (Category Item in doc.Settings.Categories)
            {
                if ((Item.CategoryType==CategoryType.Model)|(Item.CategoryType == CategoryType.AnalyticalModel))
                {
                    ListViewItem LvItem = new ListViewItem(Item.Name);
                    MainWindow.listView6.Items.Add(LvItem);
                }
            }

            MainWindow.ShowDialog();

            return Result.Succeeded;
        }

    }

}
