using Rhino;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using static VisualARQ.Script;

namespace VisualARQExtraSelectors
{
    public class OpeningsSelectorCommand : Command
    {
        public OpeningsSelectorCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        
        ///<summary>The only instance of this command.</summary>
        public static OpeningsSelectorCommand Instance
        {
            get; private set;
        }

        
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "vaxSelectOpenings"; }
        }


        // TODO Crear aqui el struct de los perfiles templates o en el dialog?


        private Guid GetOpeningProfileTemplate(Guid openingId)
        {
            return GetOpeningStyleProfileTemplate(GetProductStyle(openingId));
        }


        private bool OpeningProfileMatchesDimension(string comparisonType, double firstValue, double secondValue, Guid openingId)
        {
            Guid profileTemplateId = GetOpeningProfileTemplate(openingId);
            Guid profileId = GetOpeningProfile(openingId);
            double profileWidth = 0.0;
            bool isValidProfileTemplate = false;
            if (IsRectangularProfile(profileTemplateId))
            {
                profileWidth = GetRectangularProfileSize(profileId).Width;
                isValidProfileTemplate = true;
            }
            else if (IsCircularProfile(profileTemplateId))
            {
                profileWidth = GetCircularProfileSize(profileId).Radius * 2;
                isValidProfileTemplate = true;
            }
            else if (IsRomanArchProfile(profileTemplateId))
            {
                profileWidth = GetRomanArchProfileSize(profileId).Width;
                isValidProfileTemplate = true;
            }
            else if (IsGothicArchProfile(profileTemplateId))
            {
                profileWidth = GetGothicArchProfileSize(profileId).Width;
                isValidProfileTemplate = true;
            }
            else if (IsQuarterArchProfile(profileTemplateId))
            {
                profileWidth = GetQuarterArchProfileSize(profileId).Width;
                isValidProfileTemplate = true;
            }

            if (isValidProfileTemplate)
            {
                if (comparisonType == ComparisonType.isEqualTo && profileWidth == firstValue)
                    return true;
                else if (comparisonType == ComparisonType.isLessThan && profileWidth < firstValue)
                    return true;
                else if (comparisonType == ComparisonType.isGreaterThan && profileWidth > firstValue)
                    return true;
                else if (comparisonType == ComparisonType.isBetween && profileWidth > firstValue && profileWidth < secondValue)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }


        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            OpeningsSelectorDialog ofd = new OpeningsSelectorDialog();

            bool rc = ofd.ShowModal(Rhino.UI.RhinoEtoApp.MainWindow);

            if (rc)
            {
                if (ofd.GetAddToSelection() == null || ofd.GetAddToSelection() == false)
                    RhinoDoc.ActiveDoc.Objects.UnselectAll();

                IEnumerable<Rhino.DocObjects.RhinoObject> rhobjs = ofd.GetFromSelection() == true ? RhinoDoc.ActiveDoc.Objects.GetSelectedObjects(false, false) : RhinoDoc.ActiveDoc.Objects;
                
                // List to store all the objects that match.
                List<Rhino.DocObjects.RhinoObject> matched = new List<Rhino.DocObjects.RhinoObject>();

                List<Guid> selectedProfileTemplates = ofd.GetSelectedProfileTemplates();
                List<Guid> selectedWindowStyles = ofd.GetSelectedWindowStyles();
                List<Guid> selectedDoorStyles = ofd.GetSelectedDoorStyles();

                bool includeWindows = selectedWindowStyles.Count > 0;
                bool includeDoors = selectedDoorStyles.Count > 0;

                foreach (Rhino.DocObjects.RhinoObject rhobj in rhobjs)
                {
                    if ((includeWindows && IsWindow(rhobj.Id) && selectedWindowStyles.Contains(GetProductStyle(rhobj.Id))) ||
                        (includeDoors && IsDoor(rhobj.Id) && selectedDoorStyles.Contains(GetProductStyle(rhobj.Id))))
                    {
                        if (selectedProfileTemplates.Contains(GetOpeningProfileTemplate(rhobj.Id)))
                        {
                            if (ofd.CheckWidthDimension() || ofd.CheckHeightDimension())
                            {
                                if (ofd.CheckWidthDimension() && OpeningProfileMatchesDimension(ofd.GetWidthComparisonType(), ofd.GetWidthFirstInputValue(), ofd.GetWidthSecondInputValue(), rhobj.Id))
                                    matched.Add(rhobj);
                                else if (ofd.CheckHeightDimension() && OpeningProfileMatchesDimension(ofd.GetHeightComparisonType(), ofd.GetHeightFirstInputValue(), ofd.GetHeightSecondInputValue(), rhobj.Id))
                                    matched.Add(rhobj);
                            }
                            else
                            {
                                matched.Add(rhobj);
                            }
                        }
                    }
                }
                                
                // Set as selected all the ones that matched.
                if (matched.Count > 0)
                {
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
