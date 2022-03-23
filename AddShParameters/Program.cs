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

            try
            {
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
                        if (externalDefinition.Name.Contains("SW_02"))
                        { Shparameter.PIsInstance = true; }
                        else { Shparameter.PIsInstance = false; }
                        Shparameter.PName = externalDefinition.Name;
                        ParameterList.Add(Shparameter);
                    }
                }
            }

            catch (Autodesk.Revit.Exceptions.InternalException ex)
            {
                MessageBox.Show("Revit не смог прочитать файл общих параметров! Проверьте правильно ли вы указали расположение файла Управление->Файл общих параметров");
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Укажите файл общих параметров во вкладке Управление->Файл общих параметров");
            }

            List<ShParameters> sortParamList = ParameterList.OrderBy(name => name.PName).ToList();

            ParameterList = sortParamList;

            BuildinParam.Clear();
            BuildinParam.Add("Размеры / Dimensions");
            BuildinParam.Add("Прочие / Others");
            BuildinParam.Add("Зависимости / Consraints");
            BuildinParam.Add("Конструкции / Construction");
            BuildinParam.Add("Данные / Data");
            BuildinParam.Add("Идентификация / Identity Data");
            BuildinParam.Add("Материалы и отделка / Materials and Finishes");
            BuildinParam.Add("Механизмы / Mechanical");
            BuildinParam.Add("Механизмы - расход / Mechanical-Airflow");
            BuildinParam.Add("Механизмы - нагрузки / Mechanical-Loads");
            BuildinParam.Add("Сантехника / Plumbing");
            BuildinParam.Add("Несущие конструкции / Structural");
            BuildinParam.Add("Расчет несущих конструкций / Structural Analysis");
            BuildinParam.Add("Текст / Text");
            BuildinParam.Add("Видимость / Visibility");
            BuildinParam.Add("Результаты анализа / Area");
            BuildinParam.Add("Электросети / Electrical");
            BuildinParam.Add("Электросети - Создание цепей / Electrical-Circuiting");
            BuildinParam.Add("Электросети - Освещение / Electrical-Lighting");
            BuildinParam.Add("Электросети - Нагрузки / Electrical-Loads");
            BuildinParam.Add("Силы / Forces");
            BuildinParam.Add("Общие / General");
            BuildinParam.Add("Графика / Graphics");
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

            if (MainWindow.comboBox1.Items.Count>0)
            { MainWindow.comboBox1.SelectedIndex = 0; }

            MainWindow.comboBox2.Items.Clear();

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

                    LvItem.SubItems.Add(shParameters.PDataType.TypeId);
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
                if (doc.FamilyManager.CurrentType != null)
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
