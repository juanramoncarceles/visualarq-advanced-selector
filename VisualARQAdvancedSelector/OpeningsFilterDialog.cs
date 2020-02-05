using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using static VisualARQ.Script;

namespace VisualARQAdvancedSelector
{
    class TextItem
    {
        public string Text { get; set; }
        
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

            // Profile templates thumbs list
            Icon Rect_profile = new Icon(@"EmbeddedResources/Rectangular-24.ico");
            Icon Round_profile = new Icon(@"EmbeddedResources/Round-24.ico");
            Icon Romanic_profile = new Icon(@"EmbeddedResources/Romanic-24.ico");
            Icon Gothic_profile = new Icon(@"EmbeddedResources/Gothic-24.ico");
            Icon Arch90_profile = new Icon(@"EmbeddedResources/90arch-24.ico");
            Icon Custom_profile = new Icon(@"EmbeddedResources/Custom-24.ico");

            // Table layout to add all the controls
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
            // layout.AddRow(Profiles_list);
            // Test profile icons
            layout.AddRow(Rect_profile);
            layout.AddRow(Round_profile);
            layout.AddRow(Romanic_profile);
            layout.AddRow(Gothic_profile);
            layout.AddRow(Arch90_profile);
            layout.AddRow(Custom_profile);

            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Profile_dim_label);
            layout.AddRow(Profile_dim_comparison, Profile_dim_first_input); // TODO: missing the second input
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Add_to_selection_checkbox);
            layout.EndVertical();
            layout.AddSeparateRow(null, DefaultButton, null, AbortButton, null);
            Content = layout;
        }

        // Opening styles grid container.
        GridView Window_styles_list = new GridView();
        GridView Door_styles_list = new GridView();

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

        // Profile templates label
        private Label Profiles_label = new Label
        {
            Text = "Profiles"
        };

        // TODO: multi select list of profile templates.

        // Profile dimensions label
        private Label Profile_dim_label = new Label
        {
            Text = "Profile dimensions"
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
