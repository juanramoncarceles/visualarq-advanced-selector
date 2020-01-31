using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using static VisualARQ.Script;

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
            get { return "vaAdvancedSelector"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
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
                        if (Single.TryParse(paramValue, out float numValue)) // If the input param value can be converted to num then compare as num and as text.
                        {
                            int comparison = sd.GetComparisonType();
                            if (comparison == 0) // Equality comparison.
                            {
                                foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                                {
                                    Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                    ParameterType t = GetParameterType(paramId);
                                    // First type number comparison.
                                    if ((t == ParameterType.Number || t == ParameterType.Integer || t == ParameterType.Length || t == ParameterType.Ratio) // TODO missing num types
                                        && numValue == float.Parse(GetParameterValue(paramId, o.Id).ToString()))
                                    {
                                        matched.Add(o);
                                    }
                                    else if (GetParameterValue(paramId, o.Id) != null && paramValue == GetParameterValue(paramId, o.Id).ToString()) // If it is a number but as a string.
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                            else if (comparison == 1) // Less than comparison.
                            {
                                foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                                {
                                    Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                    ParameterType t = GetParameterType(paramId);
                                    if ((t == ParameterType.Number || t == ParameterType.Integer || t == ParameterType.Length || t == ParameterType.Ratio) // TODO missing num types
                                        && numValue > float.Parse(GetParameterValue(paramId, o.Id).ToString()))
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                            else if (comparison == 2) // Greater than comparison.
                            {
                                foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                                {
                                    Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                    ParameterType t = GetParameterType(paramId);
                                    if ((t == ParameterType.Number || t == ParameterType.Integer || t == ParameterType.Length || t == ParameterType.Ratio) // TODO missing num types
                                        && numValue < float.Parse(GetParameterValue(paramId, o.Id).ToString()))
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                        }
                        else // If it cannot be converted to num then only compare as string.
                        {
                            int comparison = sd.GetComparisonType();
                            if (comparison == 0) // Equality comparison.
                            {
                                foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                                {
                                    Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                    if (paramValue == GetParameterValue(paramId, o.Id).ToString())
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                        }
                    }
                    else // Search only by name.
                    {
                        foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                        {
                            Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                            if (paramId != Guid.Empty)
                            {
                                matched.Add(o);
                            }
                        }
                    }

                    // Select all the ones that matched.
                    if (matched.Count > 0)
                    {
                        if (sd.GetAddToSelection() == null || sd.GetAddToSelection() == false)
                        {
                            rhobjs.UnselectAll();
                        }
                        foreach (Rhino.DocObjects.RhinoObject o in matched)
                        {
                            o.Select(true);
                        }
                        // TODO set different variations of the phrase.
                        RhinoApp.WriteLine("{0} objects were selected.", matched.Count);
                    }
                    else
                    {
                        RhinoApp.WriteLine("No objects were found.");
                    }
                }
            }

            return Result.Success;
        }
    }
}
