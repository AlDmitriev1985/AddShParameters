using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace AddShParameters
{
    public class ShParameters
    {
        public Guid PGUID { get; set; }
        public string PName { get; set; }
        public ForgeTypeId PDataType { get; set; }
        public DefinitionGroup PGroup { get; set; }
        public string PDescription { get; set; }
        public BuiltInParameterGroup PDataCategory { get; set; }
        public bool PIsInstance { get; set; }
        public ExternalDefinition PexternalDefinition { get; set; }
        public CategorySet PcategorySet { get; set; }


        public ShParameters()
        {
            PcategorySet = Program.doc.Application.Create.NewCategorySet();
        }
    }

}
