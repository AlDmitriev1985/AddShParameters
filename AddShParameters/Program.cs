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
        public static WinForm2 CatWindow = new WinForm2();
        public static WinForm MainWindow = new WinForm();

        public DefinitionGroup myGroup;

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
                myGroup = enumerator.Current;

                foreach (ExternalDefinition externalDefinition in myGroup.Definitions)
                {
                    ShParameters Shparameter = new ShParameters();

                    Shparameter.PDataCategory = BuiltInParameterGroup.PG_TEXT;
                    Shparameter.PDataType = externalDefinition.GetDataType();
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


            List<string> Listgroup = new List<string>();

            foreach (var shParameters in ParameterList)
            {
                if (!Listgroup.Contains(shParameters.PGroup.Name))
                {
                    Listgroup.Add(shParameters.PGroup.Name);
                }
            }

            Listgroup.Sort();

            MainWindow.comboBox1.Items.Clear();

            MainWindow.Listgroup = Listgroup;

            foreach (var Item in Listgroup)
            {
                MainWindow.comboBox1.Items.Add(Item);
            }

            MainWindow.comboBox1.SelectedIndex = 0;


            foreach (var Item in BuildinParam)
            {
                MainWindow.comboBox2.Items.Add(Item);
            }

            MainWindow.comboBox2.SelectedIndex = 0;

            MainWindow.listView1.Items.Clear();

            foreach (ShParameters shParameters in ParameterList)
            {
                if (shParameters.PGroup.Name == MainWindow.comboBox1.SelectedItem.ToString())
                {
                    ListViewItem LvItem = new ListViewItem(shParameters.PName);

                    LvItem.SubItems.Add(shParameters.PDataType.ToString());
                    LvItem.SubItems.Add(shParameters.PDescription);
                    MainWindow.listView1.Items.Add(LvItem);
                }
            }

            MainWindow.listView2.Items.Clear();

            List<string> ListFamType = new List<string>();

            if (doc.IsFamilyDocument)
            {
                foreach (FamilyType Item in doc.FamilyManager.Types)
                {
                    if (!ListFamType.Contains(Item.Name))
                    { ListFamType.Add(Item.Name); }
                }
            }

            ListFamType.Sort();

            MainWindow.comboBox3.Items.Clear();

            foreach (var Item in ListFamType)
            {
                if (!MainWindow.comboBox3.Items.Contains(Item))
                { MainWindow.comboBox3.Items.Add(Item); }
            }

            SelectedParametersList.Clear();


            if (doc.IsFamilyDocument)
            {
                if (famType != null)
                {
                    famType = doc.FamilyManager.CurrentType;
                    MainWindow.comboBox3.SelectedItem = famType.Name;
                }
                else
                {
                    Transaction transaction = new Transaction(doc, "New family type creation");
                    transaction.Start();
                    famType = doc.FamilyManager.NewType(doc.Title);
                    transaction.Commit();

                    MainWindow.comboBox3.SelectedItem = famType.Name;
                }
            }

            MainWindow.comboBox4.Items.Clear();

            MainWindow.UpdateListinFamily();

            MainWindow.ShowDialog();

            return Result.Succeeded;
        }
    }
}
