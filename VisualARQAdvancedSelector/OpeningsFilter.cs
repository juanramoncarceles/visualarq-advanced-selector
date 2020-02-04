using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VisualARQ.Script;

namespace VisualARQAdvancedSelector
{
    public class OpeningsFilter
    {
        private List<Rhino.DocObjects.RhinoObject> Filter(Rhino.DocObjects.RhinoObject[] rhobjs)
        {
            // List to store all the objects that match.
            List<Rhino.DocObjects.RhinoObject> matched = new List<Rhino.DocObjects.RhinoObject>();

            foreach (Rhino.DocObjects.RhinoObject rhobj in rhobjs)
            {
                // Based on the checkboxes
                if (VisualARQ.Script.IsOpening(rhobj.Id))
                {

                }

            }
            // Window, door or both
            //VisualARQ.Script.IsOpening();
            //VisualARQ.Script.IsWindow();
            //VisualARQ.Script.IsDoor();


            // style
            // show all the styles available
            VisualARQ.Script.GetAllWindowStyleIds();
            VisualARQ.Script.GetAllDoorStyleIds();

            // profile type
            // show all the profiles available
            //VisualARQ.Script.IsOpeningProfile();
            //VisualARQ.Script.GetProfileTemplates();

            // profile dimensions
            //VisualARQ.Script.GetOpeningProfile()

            //VisualARQ.Script.GetProductStyle()

            return matched;
        }
    }
}
