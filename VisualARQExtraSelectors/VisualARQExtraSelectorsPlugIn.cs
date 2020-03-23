using Rhino.PlugIns;
using System;

namespace VisualARQExtraSelectors
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class VisualARQExtraSelectorsPlugIn : PlugIn

    {
        public VisualARQExtraSelectorsPlugIn()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the VisualARQ Extra Selector plug-in.</summary>
        public static VisualARQExtraSelectorsPlugIn Instance
        {
            get; private set;
        }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            Guid VisualARQPluginId = Guid.Parse("9f93914a-7d70-4ee5-a134-39caf822389b");

            if (PlugInExists(VisualARQPluginId, out bool loaded, out bool loadProtected))
            {
                if (loadProtected)
                {
                    Rhino.UI.Dialogs.ShowMessage("This plugin requires VisualARQ to be enabled.", Name);
                    return LoadReturnCode.ErrorNoDialog;
                }
                else
                {
                    Rhino.RhinoApp.WriteLine(Name + " version " + Version + " loaded.");
                    return LoadReturnCode.Success;
                }


                // TODO How to use LoadPlugIn to load it at start in case it is not loaded and check if it is successful?

                //bool success;

                //if (loaded)
                //{
                //    success = true;
                //}
                //else
                //{
                //    bool VisualARQLoaded = LoadPlugIn(VisualARQPluginId, true, false);

                //    if (VisualARQLoaded)
                //    {
                //        success = true;
                //    }
                //    else if (loadProtected)
                //    {
                //        Rhino.UI.Dialogs.ShowMessage("VisualARQ couldn't be loaded because it is Load Protected.", Name);
                //        success = false;
                //    }
                //    else
                //    {
                //        Rhino.UI.Dialogs.ShowMessage("VisualARQ couldn't be loaded.", Name);
                //        success = false;
                //    }
                //}

                //if (success)
                //{
                //    Rhino.RhinoApp.WriteLine(Name + " version " + Version + " loaded.");
                //    return LoadReturnCode.Success;
                //}
                //else
                //{
                //    return LoadReturnCode.ErrorNoDialog;
                //}
            }
            else
            {
                Rhino.UI.Dialogs.ShowMessage("VisualARQ should be installed and enabled for this plugin to work.", Name);
                return LoadReturnCode.ErrorNoDialog;
            }
        }
    }
}