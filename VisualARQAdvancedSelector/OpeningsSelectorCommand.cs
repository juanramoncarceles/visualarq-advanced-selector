using Rhino;
using Rhino.Commands;
using System;
using System.Linq;
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

        
        private enum ProfileMainDimension
        {
            Width = 0,
            Height = 1
        }


        private Guid GetOpeningProfileTemplate(Guid openingId)
        {
            return GetOpeningStyleProfileTemplate(GetProductStyle(openingId));
        }


        private bool OpeningProfileMatchesDimension(ProfileMainDimension dimension, string comparisonType, double firstValue, double secondValue, Guid openingId)
        {
            Guid profileTemplateId = GetOpeningProfileTemplate(openingId);
            Guid profileId = GetOpeningProfile(openingId);
            double profileDimension = 0.0;
            bool isValidProfileTemplate = false;
            if (IsRectangularProfile(profileTemplateId))
            {
                isValidProfileTemplate = true;
                if (dimension == ProfileMainDimension.Width)
                    profileDimension = GetRectangularProfileSize(profileId).Width;
                else if (dimension == ProfileMainDimension.Height)
                    profileDimension = GetRectangularProfileSize(profileId).Height;
            }
            else if (IsCircularProfile(profileTemplateId))
            {
                isValidProfileTemplate = true;
                profileDimension = GetCircularProfileSize(profileId).Radius * 2;
            }
            else if (IsRomanArchProfile(profileTemplateId))
            {
                isValidProfileTemplate = true;
                if (dimension == ProfileMainDimension.Width)
                    profileDimension = GetRomanArchProfileSize(profileId).Width;
                else if (dimension == ProfileMainDimension.Height)
                    profileDimension = GetRomanArchProfileSize(profileId).Height;
            }
            else if (IsGothicArchProfile(profileTemplateId))
            {
                isValidProfileTemplate = true;
                if (dimension == ProfileMainDimension.Width)
                    profileDimension = GetGothicArchProfileSize(profileId).Width;
                else if (dimension == ProfileMainDimension.Height)
                    profileDimension = GetGothicArchProfileSize(profileId).Height;
            }
            else if (IsQuarterArchProfile(profileTemplateId))
            {
                isValidProfileTemplate = true;
                if (dimension == ProfileMainDimension.Width)
                    profileDimension = GetQuarterArchProfileSize(profileId).Width;
                else if (dimension == ProfileMainDimension.Height)
                    profileDimension = GetQuarterArchProfileSize(profileId).Height;
            }

            if (isValidProfileTemplate)
            {
                if (comparisonType == ComparisonType.isEqualTo && profileDimension == firstValue)
                    return true;
                else if (comparisonType == ComparisonType.isLessThan && profileDimension < firstValue)
                    return true;
                else if (comparisonType == ComparisonType.isGreaterThan && profileDimension > firstValue)
                    return true;
                else if (comparisonType == ComparisonType.isBetween && profileDimension > firstValue && profileDimension < secondValue)
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
                IEnumerable<Rhino.DocObjects.RhinoObject> rhobjs;

                if (ofd.GetFromSelection() == true)
                    rhobjs = doc.Objects.GetSelectedObjects(false, false).ToList();
                else
                    rhobjs = doc.Objects;

                if (ofd.GetAddToSelection() == null || ofd.GetAddToSelection() == false)
                    doc.Objects.UnselectAll();

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
                            bool checkWidth = ofd.CheckWidthDimension();
                            bool checkHeight = ofd.CheckHeightDimension();
                            if (checkWidth && !checkHeight &&
                                OpeningProfileMatchesDimension(ProfileMainDimension.Width, ofd.GetWidthComparisonType(), ofd.GetWidthFirstInputValue(), ofd.GetWidthSecondInputValue(), rhobj.Id))
                            {
                                matched.Add(rhobj);
                            }
                            else if (!checkWidth && checkHeight &&
                                OpeningProfileMatchesDimension(ProfileMainDimension.Height, ofd.GetHeightComparisonType(), ofd.GetHeightFirstInputValue(), ofd.GetHeightSecondInputValue(), rhobj.Id))
                            {
                                matched.Add(rhobj);
                            }
                            else if (checkWidth && checkHeight &&
                                OpeningProfileMatchesDimension(ProfileMainDimension.Width, ofd.GetWidthComparisonType(), ofd.GetWidthFirstInputValue(), ofd.GetWidthSecondInputValue(), rhobj.Id) &&
                                OpeningProfileMatchesDimension(ProfileMainDimension.Height, ofd.GetHeightComparisonType(), ofd.GetHeightFirstInputValue(), ofd.GetHeightSecondInputValue(), rhobj.Id))
                            {
                                matched.Add(rhobj);
                            }
                            else if (!checkWidth && !checkHeight)
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
