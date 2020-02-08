using Rhino;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // TODO logic

                //IEnumerable<int> testSelectedItems = ofd.GetSelectedWindowStyles();

                //foreach (int i in testSelectedItems)
                //{
                //    RhinoApp.WriteLine(i.ToString());
                //}

                List<Guid> selectedWindowStyles = ofd.GetSelectedWindowStyles();

                foreach (Guid id in selectedWindowStyles)
                {
                    RhinoApp.WriteLine(id.ToString());
                }

                RhinoApp.WriteLine("Ok");



                return Result.Success;
            }

            return Result.Cancel;
        }
    }
}
