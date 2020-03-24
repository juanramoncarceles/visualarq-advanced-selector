using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using static VisualARQ.Script;

namespace VisualARQExtraSelectors
{
    public class ParametersSelectorCommand : Command
    {
        public ParametersSelectorCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static ParametersSelectorCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "vaxSelectObjectsByParameter"; }
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Identifies if the VisualARQ parameter type is a numerical type that can be interpreted without conversion.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsDirectNumericalType(ParameterType t)
        {
            if (t == ParameterType.Number ||
                t == ParameterType.Integer ||
                t == ParameterType.Length ||
                t == ParameterType.Area || // Remove from here and convert to meters before comparing?
                t == ParameterType.Volume || // Remove from here and convert to meters before comparing?
                t == ParameterType.Scale ||
                t == ParameterType.Ratio ||
                t == ParameterType.Currency)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void FilterByParameter(RhinoDoc doc, ParametersSelectorForm form)
        {
            Rhino.DocObjects.Tables.ObjectTable rhobjs = doc.Objects;

            string paramName = form.GetParamName();

            if (paramName != "")
            {
                // List to store all the objects that match.
                List<Rhino.DocObjects.RhinoObject> matched = new List<Rhino.DocObjects.RhinoObject>();

                string paramValue = form.GetParamValue();

                // TODO Filter also by type of obj?
                // rhobjs = filter( lambda x: va.IsColumn(x.Id) , Rhino.RhinoDoc.ActiveDoc.Objects)

                if (paramValue != "") // Search by name and value.
                {
                    // If the input param value can be converted to num then compare as num and as text.
                    if (Double.TryParse(paramValue, out double numValue))
                    {
                        int comparison = form.GetComparisonType();
                        if (comparison == 0) // Equality comparison.
                        {
                            foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                            {
                                Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                if (paramId != Guid.Empty && GetParameterValue(paramId, o.Id) != null)
                                {
                                    ParameterType t = GetParameterType(paramId);
                                    // First type number comparison.
                                    if (IsDirectNumericalType(t) && numValue == Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // Type angle comparison.
                                    else if (t == ParameterType.Angle && DegreeToRadian(numValue) == Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // Type percentage comparison.
                                    else if (t == ParameterType.Percentage && (numValue / 100.0) == Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // If it is a number but as a string.
                                    else if (GetParameterValue(paramId, o.Id) != null && paramValue == GetParameterValue(paramId, o.Id).ToString())
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                        }
                        else if (comparison == 1) // Less than comparison.
                        {
                            foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                            {
                                Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                if (paramId != Guid.Empty && GetParameterValue(paramId, o.Id) != null)
                                {
                                    ParameterType t = GetParameterType(paramId);
                                    // First type number comparison.
                                    if (IsDirectNumericalType(t) && numValue > Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // Type angle comparison.
                                    else if (t == ParameterType.Angle && DegreeToRadian(numValue) > Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // Type percentage comparison.
                                    else if (t == ParameterType.Percentage && (numValue / 100.0) > Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                        }
                        else if (comparison == 2) // Greater than comparison.
                        {
                            foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                            {
                                Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                if (paramId != Guid.Empty && GetParameterValue(paramId, o.Id) != null)
                                {
                                    ParameterType t = GetParameterType(paramId);
                                    // First type number comparison.
                                    if (IsDirectNumericalType(t) && numValue < Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // Type angle comparison.
                                    else if (t == ParameterType.Angle && DegreeToRadian(numValue) < Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                    // Type percentage comparison.
                                    else if (t == ParameterType.Percentage && (numValue / 100.0) < Convert.ToDouble(GetParameterValue(paramId, o.Id)))
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                        }
                    }
                    else // If it cannot be converted to num then only compare as bool and text.
                    {
                        int comparison = form.GetComparisonType();
                        if (comparison == 0) // Equality comparison.
                        {
                            foreach (Rhino.DocObjects.RhinoObject o in rhobjs)
                            {
                                Guid paramId = GetObjectParameterId(paramName, o.Id, true);
                                if (paramId != Guid.Empty && GetParameterValue(paramId, o.Id) != null)
                                {
                                    ParameterType t = GetParameterType(paramId);
                                    if (t == ParameterType.Boolean)
                                    {
                                        if ((string.Equals(paramValue, "yes", StringComparison.OrdinalIgnoreCase) ||
                                            string.Equals(paramValue, "true", StringComparison.OrdinalIgnoreCase)) &&
                                            "True" == GetParameterValue(paramId, o.Id).ToString())
                                        {
                                            matched.Add(o);
                                        }
                                        else if ((string.Equals(paramValue, "no", StringComparison.OrdinalIgnoreCase) ||
                                            string.Equals(paramValue, "false", StringComparison.OrdinalIgnoreCase)) &&
                                            "False" == GetParameterValue(paramId, o.Id).ToString())
                                        {
                                            matched.Add(o);
                                        }
                                    }
                                    else if (paramValue == GetParameterValue(paramId, o.Id).ToString())
                                    {
                                        matched.Add(o);
                                    }
                                }
                            }
                        }
                    }
                }
                else // Search only by parameter name.
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
                    if (form.GetAddToSelection() == null || form.GetAddToSelection() == false)
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
            }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            ParametersSelectorForm psf = new ParametersSelectorForm();

            psf.Show();

            return Result.Success;
        }
    }
}
