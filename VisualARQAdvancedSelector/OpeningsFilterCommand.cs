using Rhino;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using static VisualARQ.Script;

namespace VisualARQAdvancedSelector
{
    public class OpeningsFilterCommand : Command
    {
        public OpeningsFilterCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static OpeningsFilterCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "vaexOpeningsFilter"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            OpeningsFilterDialog ofd = new OpeningsFilterDialog();

            bool rc = ofd.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);

            if (rc)
            {
                Rhino.DocObjects.Tables.ObjectTable rhobjs = RhinoDoc.ActiveDoc.Objects;
                
                // List to store all the objects that match.
                List<Rhino.DocObjects.RhinoObject> matched = new List<Rhino.DocObjects.RhinoObject>();

                bool? includeWindows = ofd.IncludeWindowType();
                bool? includeDoors = ofd.IncludeDoorType();

                List<Guid> selectedWindowStyles;
                if (includeWindows == true)
                {
                    RhinoApp.WriteLine("Windows included");
                    selectedWindowStyles = ofd.GetSelectedWindowStyles();
                    if (selectedWindowStyles.Count > 0)
                    {
                        RhinoApp.WriteLine("Selected window styles");
                        foreach (var s in selectedWindowStyles)
                        {
                            RhinoApp.WriteLine(s.ToString());
                        }
                        matched.AddRange(rhobjs.Where(rhobj => IsWindow(rhobj.Id) && selectedWindowStyles.Contains(GetProductStyle(rhobj.Id))));
                    }
                }

                List<Guid> selectedDoorStyles;
                if (includeDoors == true)
                {
                    selectedDoorStyles = ofd.GetSelectedDoorStyles();
                    if (selectedDoorStyles.Count > 0)
                    {
                        matched.AddRange(rhobjs.Where(rhobj => IsDoor(rhobj.Id) && selectedDoorStyles.Contains(GetProductStyle(rhobj.Id))));
                    }
                }

                // TODO profile template and dimensions...
                // List<Guid> selectedProfileTemplates = dialog.GetSelectedProfileTemplates();

                //dialog.GetRectProfileWidthComparisonType();

                // profile dimensions
                //VisualARQ.Script.GetOpeningProfile()
                //VisualARQ.Script.GetOpeningStyleProfileTemplate()


                
                // Select all the ones that matched.
                if (matched.Count > 0)
                {
                    if (ofd.GetAddToSelection() == null || ofd.GetAddToSelection() == false)
                    {
                        rhobjs.UnselectAll();
                    }
                    foreach (Rhino.DocObjects.RhinoObject o in matched)
                    {
                        o.Select(true);
                    }
                    if (matched.Count == 1)
                    {
                        RhinoApp.WriteLine("1 object was selected.");
                    }
                    else
                    {
                        RhinoApp.WriteLine("{0} objects were selected.", matched.Count);
                    }
                }
                else
                {
                    RhinoApp.WriteLine("No objects were found.");
                }

                return Result.Success;
            }
            return Result.Cancel;
        }
    }
}
