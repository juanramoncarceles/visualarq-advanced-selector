using Eto.Drawing;
using Eto.Forms;

namespace VisualARQExtraSelectors
{
    public class ParametersSelectorForm : Form
    {
        public ParametersSelectorForm()
        {
            // Dialog box initialization
            Title = "Select objects by parameter";
            Resizable = false;
            Topmost = true;

            // Buttons handlers binding
            Select_button.Click += OnSelectButtonClick;
            Reset_button.Click += OnResetButtonClick;

            // Table layout to add all the controls
            DynamicLayout layout = new DynamicLayout
            {
                Spacing = new Size(15, 15),
                Padding = new Padding(10)
            };
            layout.BeginVertical();
            layout.AddRow(Param_name_label);
            layout.AddRow(Param_name_textbox);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(param_value_label);
            layout.AddRow(Comparison_value, Param_value_textbox);
            layout.EndVertical();
            layout.BeginVertical();
            layout.AddRow(Add_to_selection_checkbox);
            layout.EndVertical();
            layout.AddSeparateRow(null, Select_button, null, Reset_button, null);
            Content = layout;
        }

        // Select button
        private readonly Button Select_button = new Button { Text = "Select" };

        // Reset button
        private readonly Button Reset_button = new Button { Text = "Reset" };

        // Parameter name input
        private Label Param_name_label = new Label
        {
            Text = "Parameter name:"
        };
        private TextBox Param_name_textbox = new TextBox(); // O poner Text=null

        // Comparison option dropdown
        private DropDown Comparison_value = new DropDown
        {
            DataStore = new string[3] { "is equal to", "is less than", "is greater than" },
            SelectedIndex = 0
        };

        // Parameter value input
        private Label param_value_label = new Label
        {
            Text = "Parameter value:"
        };
        private TextBox Param_value_textbox = new TextBox(); // O poner Text=null

        // Add to current selection input
        private CheckBox Add_to_selection_checkbox = new CheckBox
        {
            Text = "Add to current selection",
            Checked = true
        };


        // METHODS

        // Get the parameter name
        public string GetParamName()
        {
            return Param_name_textbox.Text;
        }

        // Get the type of comparison
        public int GetComparisonType()
        {
            return Comparison_value.SelectedIndex;
        }

        // Get the value of the parameter
        public string GetParamValue()
        {
            return Param_value_textbox.Text;
        }

        // Get the add to current selection checkbox
        public bool? GetAddToSelection()
        {
            return Add_to_selection_checkbox.Checked;
        }
                
        // Select button click handler
        private void OnSelectButtonClick<TEventArgs>(object sender, TEventArgs e)
        {
            ParametersSelectorCommand.Instance.FilterByParameter(Rhino.RhinoDoc.ActiveDoc, this);
        }

        private void OnResetButtonClick<TEventArgs>(object sender, TEventArgs e)
        {
            Param_name_textbox.Text = "";
            Param_value_textbox.Text = "";
            Comparison_value.SelectedIndex = 0;
        }
    }
}
