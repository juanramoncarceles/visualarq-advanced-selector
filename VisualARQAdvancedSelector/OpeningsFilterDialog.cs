using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Reflection;
using static VisualARQ.Script;

namespace VisualARQAdvancedSelector
{
    class TextItem
    {
        public string Text { get; set; }
    }

    class ImageTextItem
    {
        public string Text { get; set; }
        public Icon Image { get; set; }
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
            List<TextItem> windowStyleNames = new List<TextItem>();
            foreach (Guid wStyleId in windowStyleIds)
            {
                windowStyleNames.Add(new TextItem { Text = GetStyleName(wStyleId) });
            }
            Window_styles_list.AllowMultipleSelection = true;
            Window_styles_list.Height = 150;
            Window_styles_list.DataStore = windowStyleNames;
            Window_styles_list.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<TextItem, string>(r => r.Text) },
                HeaderText = "Window Styles"
            });

            // Door styles list
            Guid[] doorStyleIds = GetAllDoorStyleIds();
            List<TextItem> doorStyleNames = new List<TextItem>();
            foreach (Guid dStyleId in doorStyleIds)
            {
                doorStyleNames.Add(new TextItem { Text = GetStyleName(dStyleId) });
            }
            Door_styles_list.AllowMultipleSelection = true;
            Door_styles_list.Height = 150;
            Door_styles_list.DataStore = doorStyleNames;
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
            foreach (Guid pTemplId in profileTemplateIds)
            {
                string pn = GetProfileName(pTemplId);
                switch (pn)
                {
                    case "Rectangular":
                        profileTemplateNames.Add(new ImageTextItem { Text = pn, Image = Rect_profile });
                        break;
                    case "Circular":
                        profileTemplateNames.Add(new ImageTextItem { Text = pn, Image = Round_profile });
                        break;
                    case "Romanic":
                        profileTemplateNames.Add(new ImageTextItem { Text = pn, Image = Romanic_profile });
                        break;
                    case "Gothic":
                        profileTemplateNames.Add(new ImageTextItem { Text = pn, Image = Gothic_profile });
                        break;
                    case "90º Arc":
                        profileTemplateNames.Add(new ImageTextItem { Text = pn, Image = Arch90_profile });
                        break;
                    default:
                        profileTemplateNames.Add(new ImageTextItem { Text = pn, Image = Custom_profile });
                        break;
                }
                if (IsOpeningProfile(pTemplId)) // TEMP
                {
                    // Only 90ªArc, Romanic and Gothic are considered opening profiles.
                    Rhino.RhinoApp.WriteLine(GetProfileName(pTemplId)); // Temp
                }
            }
            Profile_templates_list.AllowMultipleSelection = true;
            Profile_templates_list.Height = 150;
            Profile_templates_list.ShowHeader = false;
            Profile_templates_list.DataStore = profileTemplateNames;
            Profile_templates_list.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell
                {
                    ImageBinding = Binding.Property<ImageTextItem, Image>(r => r.Image),
                    TextBinding = Binding.Property<TextItem, string>(r => r.Text)
                }
            });

            // Table layout to add all the controls.
            DynamicLayout layout = new DynamicLayout
            {
                Spacing = new Size(15, 15),
                Padding = new Padding(10)
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
            layout.AddRow(Profile_dim_label);
            layout.AddRow(Profile_width_label, Profile_dim_comparison, Profile_dim_first_input, "and", Profile_dim_second_input); // TODO: missing the second input
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Add_to_selection_checkbox);
            layout.EndVertical();
            layout.AddSeparateRow(null, DefaultButton, null, AbortButton, null);
            Content = layout;
        }

        // Object type label
        private Label Object_type_label = new Label
        {
            Text = "Object type",
            Height = 22,
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
            Height = 22,
            ToolTip = "Indicate the styles you would like to include in the search. Multiple selection is possible by pressing Ctrl key."
        };

        // Opening styles grid container.
        GridView Window_styles_list = new GridView();
        GridView Door_styles_list = new GridView();

        // Profile templates label
        private Label Profiles_label = new Label
        {
            Text = "Profiles"
        };

        // Profile templates grid container.
        GridView Profile_templates_list = new GridView();

        // Profile dimensions label
        private Label Profile_dim_label = new Label
        {
            Text = "Profile dimensions"
        };

        // Profile width dimensions label
        private Label Profile_width_label = new Label
        {
            Text = "Width",
            VerticalAlignment = VerticalAlignment.Bottom, // Is this doing anything?
            Width = 25 // Is this doing anything?
        };

        // Profile height dimensions label
        private Label Profile_height_label = new Label
        {
            Text = "Heigh"
        };

        // Profile dimensions comparison
        private DropDown Profile_dim_comparison = new DropDown
        {
            DataStore = new string[4] { "is equal to", "is less than", "is greater than", "is between" },
            SelectedIndex = 0
        };

        // Profile dimension primary input
        private TextBox Profile_dim_first_input = new TextBox();

        // Profile dimension secondary input
        private TextBox Profile_dim_second_input = new TextBox();

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

        // Get all the selected window styles.
        // TEMP set as List<Guid>
        public IEnumerable<int> GetSelectedWindowStyles()
        {
            //List<int> styleIds = new List<int>();
            
            // TODO... return guids of styles...
            IEnumerable<int> selectedRows = Window_styles_list.SelectedRows;

            return selectedRows;
        }

        // Get all the selected door styles.
        public List<Guid> GetSelectedDoorStyles()
        {
            List<Guid> styleIds = new List<Guid>();

            // TODO...

            return styleIds;
        }

        // Get all the selected profile templates.
        public List<Guid> GetSelectedProfileTemplates()
        {
            List<Guid> profileTemplateIds = new List<Guid>();

            // TODO...

            return profileTemplateIds;
        }

        // Get the type of profile dimension comparison.
        public int GetProfileDimComparisonType()
        {
            return Profile_dim_comparison.SelectedIndex;
        }

        // Get the first profile dimension input.
        public string GetFirstProfileDimension()
        {
            return Profile_dim_first_input.Text;
        }

        // Get the second profile dimension input.
        public string GetSecondProfileDimension()
        {
            return Profile_dim_second_input.Text;
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
    }
}
