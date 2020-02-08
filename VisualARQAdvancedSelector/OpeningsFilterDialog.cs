using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static VisualARQ.Script;

namespace VisualARQAdvancedSelector
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

    public class OpeningsFilterDialog : Dialog<bool>
    {
        public OpeningsFilterDialog()
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
                HeaderText = "Window Styles"
            });

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
                HeaderText = "Door Styles"
            });

            // The executing assembly to retrieve the icons.
            Assembly assembly = Assembly.GetExecutingAssembly();
            string iconsPath = "VisualARQAdvancedSelector.EmbeddedResources";

            // Profile templates thumbs list
            Icon Rect_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Rectangular-24.ico"));
            Icon Round_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Round-24.ico"));
            Icon Romanic_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Romanic-24.ico"));
            Icon Gothic_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Gothic-24.ico"));
            Icon Arch90_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".90arch-24.ico"));
            Icon Custom_profile = new Icon(assembly.GetManifestResourceStream(iconsPath + ".Custom-24.ico"));

            // Profile templates list
            Guid[] profileTemplateIds = GetProfileTemplates();
            List<ImageTextItem> profileTemplateNames = new List<ImageTextItem>();
            foreach (Guid profileTemplateId in profileTemplateIds)
            {
                string profileName = GetProfileName(profileTemplateId);
                if (!Array.Exists(Structural_profiles, p => p == profileName) && profileName != "Romanic" && profileName != "Gothic" && profileName != "90º Arc") // For now those 3 are skiped
                {
                    switch (profileName)
                    {
                        case "Rectangular":
                            profileTemplateNames.Add(new ImageTextItem { Text = profileName, Image = Rect_profile, Id = profileTemplateId });
                            break;
                        case "Circular":
                            profileTemplateNames.Add(new ImageTextItem { Text = profileName, Image = Round_profile, Id = profileTemplateId });
                            break;
                        //case "Romanic":
                        //    profileTemplateNames.Add(new ImageTextItem { Text = profileName, Image = Romanic_profile, Id = profileTemplateId });
                        //    break;
                        //case "Gothic":
                        //    profileTemplateNames.Add(new ImageTextItem { Text = profileName, Image = Gothic_profile, Id = profileTemplateId });
                        //    break;
                        //case "90º Arc":
                        //    profileTemplateNames.Add(new ImageTextItem { Text = profileName, Image = Arch90_profile, Id = profileTemplateId });
                        //    break;
                        default:
                            profileTemplateNames.Add(new ImageTextItem { Text = profileName, Image = Custom_profile, Id = profileTemplateId });
                            break;
                    }
                    Rhino.RhinoApp.WriteLine(GetProfileName(profileTemplateId)); // Temp
                }
            }
            Profile_templates_list.AllowMultipleSelection = true;
            Profile_templates_list.Height = 150;
            Profile_templates_list.ShowHeader = false;
            Profile_templates_list.SelectedRowsChanged += SelectedProfileTemplatesHandler;
            Profile_templates_list.DataStore = profileTemplateNames;
            Profile_templates_list.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell
                {
                    ImageBinding = Binding.Property<ImageTextItem, Image>(r => r.Image),
                    TextBinding = Binding.Property<TextItem, string>(r => r.Text)
                }
            });

            // Add the dropdowns event listeners
            Rect_profile_width_comparison.DropDownClosed += SelectedRectProfileWidthComparisonType;
            // TODO... also rect height and circ radius

            // Table layout to add all the controls.
            DynamicLayout layout = new DynamicLayout
            {
                Spacing = new Size(15, 15),
                Padding = new Padding(10),
            };
            layout.BeginVertical();
            layout.AddRow(Object_type_label);
            layout.AddRow(Add_windows_checkbox, Add_doors_checkbox);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Styles_label);
            layout.AddRow(Window_styles_list, Door_styles_list);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Profiles_label);
            layout.AddRow(Profile_templates_list);
            layout.EndVertical();
            layout.BeginVertical();
            DynamicLayout rectProfileGroup = new DynamicLayout
            {
                Spacing = new Size(8, 8)
            };
            rectProfileGroup.AddRow(Rect_profile_width_label, Rect_profile_width_comparison, Rect_profile_width_first_input, Rect_profile_width_second_input);
            rectProfileGroup.AddRow(Rect_profile_height_label, Rect_profile_height_comparison, Rect_profile_height_first_input, Rect_profile_height_second_input);
            Rectangular_profile_dim_group.Content = rectProfileGroup;
            layout.AddRow(Rectangular_profile_dim_group);
            DynamicLayout circProfileGroup = new DynamicLayout
            {
                Spacing = new Size(8, 8)
            };
            circProfileGroup.AddRow(Circ_profile_radius_label, Circ_profile_radius_comparison, Circ_profile_radius_first_input, Circ_profile_radius_second_input);
            Circular_profile_dim_group.Content = circProfileGroup;
            layout.AddRow(Circular_profile_dim_group);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Add_to_selection_checkbox);
            layout.EndVertical();
            layout.AddSeparateRow(null, DefaultButton, null, AbortButton, null);
            Content = layout;
        }

        private string[] Structural_profiles = new string[6] { "Circular Hollow", "I Shape", "L Shape", "Rectangular Hollow", "T Shape", "U Shape" };

        // Object type label
        private Label Object_type_label = new Label
        {
            Text = "Object type",
            Height = 20,
            ToolTip = "Choose the opening type you would like to include in the search."
        };

        // Add windows input
        private CheckBox Add_windows_checkbox = new CheckBox
        {
            Text = "Windows",
            Checked = true
        };

        // Add doors input
        private CheckBox Add_doors_checkbox = new CheckBox
        {
            Text = "Doors",
            Checked = true
        };

        // Styles label
        private Label Styles_label = new Label
        {
            Text = "Styles",
            Height = 20,
            ToolTip = "Indicate the styles you would like to include in the search. Multiple selection is possible by pressing Ctrl key."
        };

        // Opening styles grid container.
        GridView Window_styles_list = new GridView();
        GridView Door_styles_list = new GridView();

        // Profile templates label
        private Label Profiles_label = new Label
        {
            Text = "Profiles",
            Height = 20,
            ToolTip = "Indicate all the profile types that you would like to include in the search. Press the Ctrl key for multiple select."
        };

        // Profile templates grid container.
        GridView Profile_templates_list = new GridView();


        // RECTANGULAR PROFILE

        // Rectangular profile template dimensions group.
        private GroupBox Rectangular_profile_dim_group = new GroupBox
        {
            Text = "Rectangular profile dimensions",
            Padding = 5
        };

        // Rectangular profile width dimension label
        private Label Rect_profile_width_label = new Label
        {
            Text = "Width",
            VerticalAlignment = VerticalAlignment.Center            
        };

        // Rectangular profile height dimension label
        private Label Rect_profile_height_label = new Label
        {
            Text = "Heigh",
            VerticalAlignment = VerticalAlignment.Center
        };

        // Rectangular profile width dimension comparison
        private DropDown Rect_profile_width_comparison = new DropDown
        {
            DataStore = new string[4] { "is equal to", "is less than", "is greater than", "is between" },
            SelectedIndex = 0
        };

        // Rectangular profile height dimension comparison
        private DropDown Rect_profile_height_comparison = new DropDown
        {
            DataStore = new string[4] { "is equal to", "is less than", "is greater than", "is between" },
            SelectedIndex = 0
        };

        // Rectangular profile width dimension first input
        private TextBox Rect_profile_width_first_input = new TextBox();

        // Rectangular profile width dimension second input
        private TextBox Rect_profile_width_second_input = new TextBox();

        // Rectangular profile height dimension first input
        private TextBox Rect_profile_height_first_input = new TextBox();

        // Rectangular Profile height dimension second input
        private TextBox Rect_profile_height_second_input = new TextBox();

        // CIRCULAR PROFILE

        // Circular profile template dimensions group.
        private GroupBox Circular_profile_dim_group = new GroupBox
        {
            Text = "Circular profile dimensions",
            Padding = 5
        };

        // Rectangular profile radius dimension label
        private Label Circ_profile_radius_label = new Label
        {
            Text = "Radius",
            VerticalAlignment = VerticalAlignment.Center
        };

        // Ciruclar profile radius dimension comparison
        private DropDown Circ_profile_radius_comparison = new DropDown
        {
            DataStore = new string[4] { "is equal to", "is less than", "is greater than", "is between" },
            SelectedIndex = 0
        };

        // Circular profile radius dimension first input
        private TextBox Circ_profile_radius_first_input = new TextBox();

        // Circular profile radius dimension second input
        private TextBox Circ_profile_radius_second_input = new TextBox();

        // ADD TO SELECTION

        // Add to current selection input
        private CheckBox Add_to_selection_checkbox = new CheckBox
        {
            Text = "Add to current selection",
            Checked = true
        };


        // METHODS

        // Check if windows should be included.
        public bool? IncludeWindowType()
        {
            return Add_windows_checkbox.Checked;
        }

        // Check if doors should be included.
        public bool? IncludeDoorType()
        {
            return Add_doors_checkbox.Checked;
        }

        /// <summary>
        /// Returns a list with the ids of all selected window styles
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetSelectedWindowStyles()
        {
            IEnumerable<object> selectedWindowItems = Window_styles_list.SelectedItems; // SelectedRows returns the indexes
            List<Guid> styleIds = new List<Guid>();
            foreach (TextItem s in selectedWindowItems)
                styleIds.Add(s.Id);
            return styleIds;
        }

        /// <summary>
        /// Returns a list with the ids of all selected door styles
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetSelectedDoorStyles()
        {
            IEnumerable<object> selectedDoorItems = Door_styles_list.SelectedItems;
            List<Guid> styleIds = new List<Guid>();
            foreach (TextItem s in selectedDoorItems)
                styleIds.Add(s.Id);
            return styleIds;
        }

        /// <summary>
        /// Returns a list with the ids of all the selected profile templates.
        /// </summary>
        /// <returns></returns>
        public List<Guid> GetSelectedProfileTemplates()
        {
            IEnumerable<object> selectedProfileItems = Profile_templates_list.SelectedItems;
            List<Guid> profileTemplateIds = new List<Guid>();
            foreach (ImageTextItem s in selectedProfileItems)
                profileTemplateIds.Add(s.Id);
            return profileTemplateIds;
        }

        // Gets the rectangular profile width type of dimension comparison.
        public int GetRectProfileWidthComparisonType()
        {
            return Rect_profile_width_comparison.SelectedIndex;
        }

        // Gets the rectangular profile width first input.
        public string GetRectProfileWidthFirstDimension()
        {
            return Rect_profile_width_first_input.Text;
        }

        // Get the rectangular profile width second input.
        public string GetRectProfileWidthSecondDimension()
        {
            return Rect_profile_width_second_input.Text;
        }

        // Gets the circular profile radius type of dimension comparison.
        public int GetCircProfileRadiusComparisonType()
        {
            return Circ_profile_radius_comparison.SelectedIndex;
        }

        // Gets the circular profile radius first input.
        public string GetCircProfileRadiusFirstDimension()
        {
            return Circ_profile_radius_first_input.Text;
        }

        // Gets the circular profile radius second input.
        public string GetCircProfileRadiusSecondDimension()
        {
            return Circ_profile_radius_second_input.Text;
        }

        // Get the add to current selection checkbox
        public bool? GetAddToSelection()
        {
            return Add_to_selection_checkbox.Checked;
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

        /// <summary>
        /// Enables or disables the inputs for the profiles dimension based on the templates selected.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedProfileTemplatesHandler<TEventArgs>(object sender, TEventArgs e)
        {
            IEnumerable<object> selectedItems = Profile_templates_list.SelectedItems;
            
            if (selectedItems.Any(i => ((ImageTextItem)i).Text == "Rectangular"))
                Rectangular_profile_dim_group.Enabled = true;
            else
                Rectangular_profile_dim_group.Enabled = false;

            if (selectedItems.Any(i => ((ImageTextItem)i).Text == "Circular"))
                Circular_profile_dim_group.Enabled = true;
            else
                Circular_profile_dim_group.Enabled = false;
            
        }

        private void SelectedRectProfileWidthComparisonType<TEventArgs>(object sender, TEventArgs e)
        {
            if (GetRectProfileWidthComparisonType() == 3) // 3 must correspond to the "between" option
                Rect_profile_width_second_input.Enabled = true;
            else
                Rect_profile_width_second_input.Enabled = false;
        }

        // private void SelectedRectProfileHeightComparisonType

        // private void SelectedCircProfileRadiusComparisonType
    }
}
