using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace VisualARQAdvancedSelector
{
    public class VisualARQAdvancedSelectorCommand : Command
    {
        public VisualARQAdvancedSelectorCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static VisualARQAdvancedSelectorCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "VisualARQAdvancedSelectorCommand"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO Here the main logic...

            SelectionDialog sd = new SelectionDialog();

            bool rc = sd.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);

            if (rc)
            {
                // TODO Filter also by type of obj?
                // rhobjs = filter( lambda x: va.IsColumn(x.Id) , Rhino.RhinoDoc.ActiveDoc.Objects)

                Rhino.DocObjects.Tables.ObjectTable rhobjs = RhinoDoc.ActiveDoc.Objects;

                string paramName = sd.GetParamName();

                if (paramName != "")
                {
                    // List to store all the objects that match.
                    List<Rhino.DocObjects.RhinoObject> matched = new List<Rhino.DocObjects.RhinoObject>();

                    string paramValue = sd.GetParamValue();

                    if (paramValue != "") // Search by name and value.
                    {
                        // TODO...

                        RhinoApp.WriteLine("Search by name and value.");

                    }
                    else // Search only by name.
                    {
                        // TODO...

                        RhinoApp.WriteLine("Search only by name.");

                    }

                    // Select all the ones that matched.
                    if (matched.Count > 0)
                    {
                        // TODO...
                        RhinoApp.WriteLine("Selcting...");
                    }
                }
            }




            // Example code
            //Point3d pt0;
            //using (GetPoint getPointAction = new GetPoint())
            //{
            //    getPointAction.SetCommandPrompt("Please select the start point");
            //    if (getPointAction.Get() != GetResult.Point)
            //    {
            //        RhinoApp.WriteLine("No start point was selected.");
            //        return getPointAction.CommandResult();
            //    }
            //    pt0 = getPointAction.Point();
            //}

            return Result.Success;
        }
    }
}
