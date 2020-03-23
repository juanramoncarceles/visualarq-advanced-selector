using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static VisualARQ.Script;

namespace VisualARQExtraSelectors
{
    class TextItem
    {
        public string Text { get; set; }
        public Guid Id { get; set; }
    }

    class ImageTextItem
    {
        public string Text { get; set; }
        public Icon Image { get; set; }
        public Guid Id { get; set; }
    }

    class DropDownEntry
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    static class ComparisonType
    {
        public static string none = "all";
        public static string isEqualTo = "isEqualTo";
        public static string isLessThan = "isLessThan";
        public static string isGreaterThan = "isGreaterThan";
        public static string isBetween = "isBetween";
    }

    public class OpeningsSelectorDialog : Dialog<bool>
    {
        public OpeningsSelectorDialog()
        {
            // Dialog box initialization
            Title = "Openings filter";
            Resizable = false;

            // Default button
            DefaultButton = new Button { Text = "Select" };
            DefaultButton.Click += OnSelectButtonClick;

            // Abort button
            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += OnCloseButtonClick;

            // Window styles list
            Guid[] windowStyleIds = GetAllWindowStyleIds();
            List<TextItem> windowStyles = new List<TextItem>();
            foreach (Guid wStyleId in windowStyleIds)
            {
                windowStyles.Add(new TextItem { Text = GetStyleName(wStyleId), Id = wStyleId });
            }
            Window_styles_list.AllowMultipleSelection = true;
            Window_styles_list.Height = 150;
            Window_styles_list.DataStore = windowStyles;
            Window_styles_list.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<TextItem, string>(r => r.Text) },
                HeaderText = "Window Styles",
                Width = 174
            });
            Window_styles_list.SelectAll();

            // Door styles list
            Guid[] doorStyleIds = GetAllDoorStyleIds();
            List<TextItem> doorStyles = new List<TextItem>();
            foreach (Guid dStyleId in doorStyleIds)
            {
                doorStyles.Add(new TextItem { Text = GetStyleName(dStyleId), Id = dStyleId });
            }
            Door_styles_list.AllowMultipleSelection = true;
            Door_styles_list.Height = 150;
            Door_styles_list.DataStore = doorStyles;
            Door_styles_list.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<TextItem, string>(r => r.Text) },
                HeaderText = "Door Styles",
                Width = 174
            });
            Door_styles_list.SelectAll();

            // The executing assembly.
            Assembly assembly = Assembly.GetExecutingAssembly();

            string iconsPath = "VisualARQAdvancedSelector.EmbeddedResources"; // TODO change to VisualARQExtraSelectors

            // Profile templates thumbs list
            Icon Rect_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Rectangular-24.ico"));
            Icon Round_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Round-24.ico"));
            Icon Romanic_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Romanic-24.ico"));
            Icon Gothic_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Gothic-24.ico"));
            Icon Arch90_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".90arch-24.ico"));
            Icon Custom_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Custom-24.ico"));

            // Profile templates list
            Guid[] profileTemplateIds = GetProfileTemplates();
            List<ImageTextItem> profileTemplateItems = new List<ImageTextItem>();
            foreach (Guid profileTemplateId in profileTemplateIds)
            {
                if (!IsStructuralProfileTemplate(profileTemplateId))
                {
                    string profileName = GetProfileName(profileTemplateId);
                    if (IsRectangularProfile(profileTemplateId))
                        profileTemplateItems.Add(new ImageTextItem { Text = profileName, Image = Rect_profile, Id = profileTemplateId });
                    else if (IsCircularProfile(profileTemplateId))
                        profileTemplateItems.Add(new ImageTextItem { Text = profileName, Image = Round_profile, Id = profileTemplateId });
                    else if (IsRomanArchProfile(profileTemplateId))
                        profileTemplateItems.Add(new ImageTextItem { Text = profileName, Image = Romanic_profile, Id = profileTemplateId });
                    else if (IsGothicArchProfile(profileTemplateId))
                        profileTemplateItems.Add(new ImageTextItem { Text = profileName, Image = Gothic_profile, Id = profileTemplateId });
                    else if (IsQuarterArchProfile(profileTemplateId))
                        profileTemplateItems.Add(new ImageTextItem { Text = profileName, Image = Arch90_profile, Id = profileTemplateId });
                    // TODO Temporarily skip custom profiles until those are added to the dimension check
                    //else
                    //    profileTemplateItems.Add(new ImageTextItem { Text = profileName, Image = Custom_profile, Id = profileTemplateId });
                }
            }
            Profile_templates_list.AllowMultipleSelection = true;
            Profile_templates_list.Height = 150;
            Profile_templates_list.ShowHeader = false;
            Profile_templates_list.DataStore = profileTemplateItems;
            Profile_templates_list.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell
                {
                    ImageBinding = Binding.Property<ImageTextItem, Image>(r => r.Image),
                    TextBinding = Binding.Property<TextItem, string>(r => r.Text)
                },
                Width = 174
            });
            Profile_templates_list.SelectAll();

            // Add the dropdowns event listeners
            Profile_width_comparison.DropDownClosed += ProfileWidthComparisonType;
            Profile_height_comparison.DropDownClosed += ProfileHeightComparisonType;

            // Add checkbox include styles event listeners
            Include_all_window_styles.CheckedChanged += OnIncludeAllWindowStylesHandler;
            Include_all_door_styles.CheckedChanged += OnIncludeAllDoorStylesHandler;
            Include_none_window_styles.CheckedChanged += OnIncludeNoneWindowStylesHandler;
            Include_none_door_styles.CheckedChanged += OnIncludeNoneDoorStylesHandler;

            // Styles selection changes handler
            Window_styles_list.SelectedRowsChanged += OnWindowStylesSelectionChanged;
            Door_styles_list.SelectedRowsChanged += OnDoorStylesSelectionChanged;

            // Selection options handler
            Add_to_selection_checkbox.CheckedChanged += OnAddToSelectionHandler;
            From_selection_checkbox.CheckedChanged += OnFromSelectionHandler;

            // Table layout to add all the controls.
            DynamicLayout layout = new DynamicLayout
            {
                Spacing = new Size(15, 15),
                Padding = new Padding(10),
            };
            layout.BeginVertical();
            layout.AddRow(Styles_label);
            layout.AddRow(Include_all_window_styles, Include_all_door_styles);
            layout.AddRow(Include_none_window_styles, Include_none_door_styles);
            layout.AddRow(Window_styles_list, Door_styles_list);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Profiles_label);
            layout.AddRow(Profile_templates_list);
            layout.EndVertical();
            layout.BeginVertical();
            DynamicLayout profileDimensionsInputsGroup = new DynamicLayout
            {
                Spacing = new Size(8, 8)
            };
            profileDimensionsInputsGroup.AddRow(Profile_width_label, Profile_width_comparison, Profile_width_first_input, Profile_width_second_input);
            profileDimensionsInputsGroup.AddRow(Profile_height_label, Profile_height_comparison, Profile_height_first_input, Profile_height_second_input);
            Profile_dim_inputs_group.Content = profileDimensionsInputsGroup;
            layout.AddRow(Profile_dim_inputs_group);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Add_to_selection_checkbox);
            layout.AddRow(From_selection_checkbox);
            layout.EndVertical();
            layout.AddSeparateRow(null, DefaultButton, null, AbortButton, null);
            Content = layout;
        }

        private bool IsStructuralProfileTemplate(Guid profileId)
        {
            if (IsCircularHollowProfile(profileId) ||
                IsRectangularHollowProfile(profileId) ||
                IsIShapeProfile(profileId) ||
                IsLShapeProfile(profileId) ||
                IsTShapeProfile(profileId) ||
                IsUShapeProfile(profileId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static readonly DropDownEntry[] Numerical_comparison_options = new DropDownEntry[5]
        {
            new DropDownEntry { Text = "all", Value = ComparisonType.none },
            new DropDownEntry { Text = "is equal to", Value = ComparisonType.isEqualTo },
            new DropDownEntry { Text = "is less than", Value = ComparisonType.isLessThan },
            new DropDownEntry { Text = "is greater than", Value = ComparisonType.isGreaterThan },
            new DropDownEntry { Text = "is between", Value = ComparisonType.isBetween }
        };


        // Include all window styles input
        private readonly CheckBox Include_all_window_styles = new CheckBox
        {
            Text = "Select all",
            Checked = true
        };

        // Include none windows styles input
        private readonly CheckBox Include_none_window_styles = new CheckBox
        {
            Text = "Unselect all",
            Checked = false
        };

        // Include all door styles input
        private readonly CheckBox Include_all_door_styles = new CheckBox
        {
            Text = "Select all",
            Checked = true
        };

        // Include none window styles input
        private readonly CheckBox Include_none_door_styles = new CheckBox
        {
            Text = "Unselect all",
            Checked = false
        };

        // Styles label
        private readonly Label Styles_label = new Label
        {
            Text = "Styles",
            Height = 20,
            Font = new Font(SystemFont.Bold),
            ToolTip = "Select all the styles you would like to include in the search."
        };

        // Opening styles grid container.
        readonly GridView Window_styles_list = new GridView();
        readonly GridView Door_styles_list = new GridView();

        // Profile templates label
        private readonly Label Profiles_label = new Label
        {
            Text = "Profiles",
            Height = 20,
            Font = new Font(SystemFont.Bold),
            ToolTip = "Select all the profile types that you would like to include in the search."
        };

        // Profile templates grid container.
        private readonly GridView Profile_templates_list = new GridView();

        // Profile template dimensions group.
        private readonly GroupBox Profile_dim_inputs_group = new GroupBox
        {
            Text = "Profile dimensions",
            Font = new Font(SystemFont.Bold),
            ToolTip = "Specify the profile main dimensions.",
            Padding = 5
        };

        // Profile width dimension label
        private readonly Label Profile_width_label = new Label
        {
            Text = "Width",
            VerticalAlignment = VerticalAlignment.Center            
        };

        // Profile height dimension label
        private readonly Label Profile_height_label = new Label
        {
            Text = "Heigh",
            VerticalAlignment = VerticalAlignment.Center
        };

        // Profile width dimension comparison
        private readonly DropDown Profile_width_comparison = new DropDown
        {
            DataStore = Numerical_comparison_options,
            ItemTextBinding = Binding.Property<DropDownEntry, string>(r => r.Text),
            ItemKeyBinding = Binding.Property<DropDownEntry, string>(r => r.Value),
            SelectedIndex = 0
        };

        // Profile height dimension comparison
        private readonly DropDown Profile_height_comparison = new DropDown
        {
            DataStore = Numerical_comparison_options,
            ItemTextBinding = Binding.Property<DropDownEntry, string>(r => r.Text),
            ItemKeyBinding = Binding.Property<DropDownEntry, string>(r => r.Value),
            SelectedIndex = 0
        };

        // Profile width dimension first input
        private readonly NumericStepper Profile_width_first_input = new NumericStepper
        {
            Enabled = false,
            DecimalPlaces = 2,
            MaximumDecimalPlaces = 4
        };

        // Profile width dimension second input
        private readonly NumericStepper Profile_width_second_input = new NumericStepper
        {
            Enabled = false,
            DecimalPlaces = 2,
            MaximumDecimalPlaces = 4
        };

        // Profile height dimension first input
        private readonly NumericStepper Profile_height_first_input = new NumericStepper
        {
            Enabled = false,
            DecimalPlaces = 2,
            MaximumDecimalPlaces = 4
        };

        // Profile height dimension second input
        private readonly NumericStepper Profile_height_second_input = new NumericStepper
        {
            Enabled = false,
            DecimalPlaces = 2,
            MaximumDecimalPlaces = 4
        };


        // ADD TO SELECTION

        // Add to current selection input
        private readonly CheckBox Add_to_selection_checkbox = new CheckBox
        {
            Text = "Add to current selection",
            Checked = true
        };

        // FROM CURRENT SELECTION

        private readonly CheckBox From_selection_checkbox = new CheckBox
        {
            Text = "From current selection"
        };

        // METHODS

        /// <summary>
        /// From all the window styles gets the selected ones.
        /// </summary>
        /// <returns>List with the ids of all selected window styles.</returns>
        public List<Guid> GetSelectedWindowStyles()
        {
            IEnumerable<object> selectedWindowItems = Window_styles_list.SelectedItems; // SelectedRows returns the indexes
            List<Guid> styleIds = new List<Guid>();
            foreach (TextItem s in selectedWindowItems)
                styleIds.Add(s.Id);
            return styleIds;
        }

        /// <summary>
        /// From all the window styles gets the selected ones.
        /// </summary>
        /// <returns>List with the ids of all selected door styles.</returns>
        public List<Guid> GetSelectedDoorStyles()
        {
            IEnumerable<object> selectedDoorItems = Door_styles_list.SelectedItems;
            List<Guid> styleIds = new List<Guid>();
            foreach (TextItem s in selectedDoorItems)
                styleIds.Add(s.Id);
            return styleIds;
        }

        /// <summary>
        /// From all the profile templates gets the selected ones. 
        /// </summary>
        /// <returns>List with the ids of all the selected profile templates.</returns>
        public List<Guid> GetSelectedProfileTemplates()
        {
            IEnumerable<object> selectedProfileItems = Profile_templates_list.SelectedItems;
            List<Guid> profileTemplateIds = new List<Guid>();
            foreach (ImageTextItem s in selectedProfileItems)
                profileTemplateIds.Add(s.Id);
            return profileTemplateIds;
        }

        // Gets the profile width type of dimension comparison.
        public string GetWidthComparisonType()
        {
            return Profile_width_comparison.SelectedKey;
        }

        // Gets the profile width first input.
        public double GetWidthFirstInputValue()
        {
            return Profile_width_first_input.Value;
        }

        // Get the profile width second input.
        public double GetWidthSecondInputValue()
        {
            return Profile_width_second_input.Value;
        }

        // Gets the profile height type of dimension comparison.
        public string GetHeightComparisonType()
        {
            return Profile_height_comparison.SelectedKey;
        }

        // Gets the profile height first input.
        public double GetHeightFirstInputValue()
        {
            return Profile_height_first_input.Value;
        }

        // Gets the profile height second input.
        public double GetHeightSecondInputValue()
        {
            return Profile_height_second_input.Value;
        }

        public bool CheckWidthDimension()
        {
            if (GetWidthComparisonType() != ComparisonType.none && 
                ((GetWidthComparisonType() != ComparisonType.isBetween && Profile_width_first_input.Value > 0) ||
                (GetWidthComparisonType() == ComparisonType.isBetween && (Profile_width_first_input.Value > 0 && Profile_width_second_input.Value > 0))))
                return true;
            else
                return false;
        }

        public bool CheckHeightDimension()
        {
            if (GetHeightComparisonType() != ComparisonType.none &&
                ((GetHeightComparisonType() != ComparisonType.isBetween && Profile_height_first_input.Value > 0) ||
                (GetHeightComparisonType() == ComparisonType.isBetween && (Profile_height_first_input.Value > 0 && Profile_height_second_input.Value > 0))))
                return true;
            else
                return false;
        }

        // Get the add to current selection checkbox
        public bool? GetAddToSelection()
        {
            return Add_to_selection_checkbox.Checked;
        }

        // Get the from current selection checkbox
        public bool? GetFromSelection()
        {
            return From_selection_checkbox.Checked;
        }

        // Close button click handler
        private void OnCloseButtonClick<TEventArgs>(object sender, TEventArgs e)
        {
            // TODO Clean all the inputs?
            Close(false);
        }

        // Select button click handler
        private void OnSelectButtonClick<TEventArgs>(object sender, TEventArgs e)
        {
            //if (false) // TODO: any condition to skip the process?
            //{
            //    Close(false);
            //}
            //else
            //{
                Close(true);
            //}
        }

        // ONCHANGE EVENT LISTENERS

        private void OnIncludeAllWindowStylesHandler<TEventArgs>(object sender, TEventArgs e)
        {
            if (Include_all_window_styles.Checked == true)
            {
                Window_styles_list.SelectAll();
                Include_none_window_styles.Checked = false;
            }
        }

        private void OnIncludeAllDoorStylesHandler<TEventArgs>(object sender, TEventArgs e)
        {
            if (Include_all_door_styles.Checked == true)
            {
                Door_styles_list.SelectAll();
                Include_none_door_styles.Checked = false;
            }
        }

        private void OnIncludeNoneWindowStylesHandler<TEventArgs>(object sender, TEventArgs e)
        {
            if (Include_none_window_styles.Checked == true)
            {
                Window_styles_list.UnselectAll();
                Include_all_window_styles.Checked = false;
            }
        }

        private void OnIncludeNoneDoorStylesHandler<TEventArgs>(object sender, TEventArgs e)
        {
            if (Include_none_door_styles.Checked == true)
            {
                Door_styles_list.UnselectAll();
                Include_all_door_styles.Checked = false;
            }
        }

        private void OnWindowStylesSelectionChanged<TEventArgs>(object sender, TEventArgs e)
        {
            int amountItems = Window_styles_list.DataStore.Count();
            int amountItemsSelected = Window_styles_list.SelectedRows.Count();

            if (amountItems == amountItemsSelected)
            {
                Include_all_window_styles.Checked = true;
                Include_none_window_styles.Checked = false;
            }
            else if (amountItemsSelected == 0)
            {
                Include_all_window_styles.Checked = false;
                Include_none_window_styles.Checked = true;
            }
            else
            {
                Include_all_window_styles.Checked = false;
                Include_none_window_styles.Checked = false;
            }
        }

        private void OnDoorStylesSelectionChanged<TEventArgs>(object sender, TEventArgs e)
        {
            int amountItems = Door_styles_list.DataStore.Count();
            int amountItemsSelected = Door_styles_list.SelectedRows.Count();

            if (amountItems == amountItemsSelected)
            {
                Include_all_door_styles.Checked = true;
                Include_none_door_styles.Checked = false;
            }
            else if (amountItemsSelected == 0)
            {
                Include_all_door_styles.Checked = false;
                Include_none_door_styles.Checked = true;
            }
            else
            {
                Include_all_door_styles.Checked = false;
                Include_none_door_styles.Checked = false;
            }
        }

        private void ProfileWidthComparisonType<TEventArgs>(object sender, TEventArgs e)
        {
            if (GetWidthComparisonType() == ComparisonType.none)
            {
                Profile_width_first_input.Enabled = false;
                Profile_width_second_input.Enabled = false;
            }
            else if (GetWidthComparisonType() == ComparisonType.isBetween)
            {
                Profile_width_first_input.Enabled = true;
                Profile_width_second_input.Enabled = true;
            }
            else
            {
                Profile_width_first_input.Enabled = true;
                Profile_width_second_input.Enabled = false;
            }
        }

        private void ProfileHeightComparisonType<TEventArgs>(object sender, TEventArgs e)
        {
            if (GetHeightComparisonType() == ComparisonType.none)
            {
                Profile_height_first_input.Enabled = false;
                Profile_height_second_input.Enabled = false;
            }
            else if (GetHeightComparisonType() == ComparisonType.isBetween)
            {
                Profile_height_first_input.Enabled = true;
                Profile_height_second_input.Enabled = true;
            }
            else
            {
                Profile_height_first_input.Enabled = true;
                Profile_height_second_input.Enabled = false;
            }
        }


        private void OnAddToSelectionHandler<TEventArgs>(object sender, TEventArgs e)
        {
            if (Add_to_selection_checkbox.Checked == true)
                From_selection_checkbox.Checked = false;
        }

        private void OnFromSelectionHandler<TEventArgs>(object sender, TEventArgs e)
        {
            if (From_selection_checkbox.Checked == true)
                Add_to_selection_checkbox.Checked = false;
        }
    }
}
