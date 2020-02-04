using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VisualARQAdvancedSelector
{
    class MyPoco
    {
        public string Text { get; set; }
        public bool Check { get; set; }
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


            // Styles list TEST
            ObservableCollection<MyPoco> collection = new ObservableCollection<MyPoco>
            {
                new MyPoco { Text = "Style 1", Check = true },
                new MyPoco { Text = "Style 2", Check = false }
            };

            Styles_list.DataStore = collection;
            //GridView Styles_list = new GridView
            //{
            //   DataStore = collection
            //};

            Styles_list.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Property<MyPoco, string>(r => r.Text) },
                HeaderText = "Style"
            });

            Styles_list.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Property<MyPoco, bool?>(r => r.Check) },
                HeaderText = ""
            });


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
            layout.AddRow(Styles_list, Styles_dropdown);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Profiles_label);
            // layout.AddRow(Profiles_list);
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

        GridView Styles_list = new GridView();
        

        // Object type label
        private Label Object_type_label = new Label
        {
            Text = "Object type"
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
            Text = "Styles"
        };

        



        // Styles dropdown test
        private DropDown Styles_dropdown = new DropDown
        {
            DataStore = new string[4] { "Style 1", "Style 2", "Style 3", "Style 4" },
            SelectedIndex = 0
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
        public List<Guid> GetSelectedWindowStyles()
        {
            List<Guid> styleIds = new List<Guid>();

            // TODO...
            styleIds = Styles_list.SelectedItems;

            return styleIds;
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
