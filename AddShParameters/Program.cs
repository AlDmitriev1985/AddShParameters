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

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get the name of the active document
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

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
            BuildinParam.Add(BuiltInParameterGroup.PG_CONSTRAINTS.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_CONSTRUCTION.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_DATA.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_IDENTITY_DATA.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_LENGTH.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_MATERIALS.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_MECHANICAL.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_MECHANICAL_LOADS.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_PLUMBING.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_STRUCTURAL.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_TEXT.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_VISIBILITY.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_AREA.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_ELECTRICAL.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_ELECTRICAL_LOADS.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_FORCES.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_GENERAL.ToString());
            BuildinParam.Add(BuiltInParameterGroup.PG_GRAPHICS.ToString());
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

            SelectedParametersList.Clear();

            MainWindow.ShowDialog();

            return Result.Succeeded;
        }
    }

}
