using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO; // Required for File operations
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AorusM2
{
    public partial class AorusM2Config : Form
    {

        // --- Member Variables ---

        // Store configuration for each button (Key: CurrentKeyIndex, Value: Config)
        private Dictionary<int, ButtonConfig> _buttonConfigs = new Dictionary<int, ButtonConfig>();
        // Store which button is currently being configured
        private int _selectedButtonIndex = -1; // -1 means no button selected
        private Button _selectedButtonControl = null; // Reference to the selected button for visual feedback

        // Mappings from UI text/selection to configuration values
        private Dictionary<string, int> _fnTypeMap = new Dictionary<string, int>();
        private Dictionary<string, int> _mouseButtonMap = new Dictionary<string, int>();
        private Dictionary<string, int> _mouseWheelMap = new Dictionary<string, int>();
        private Dictionary<string, int> _keyboardModifierMap = new Dictionary<string, int>();
        private Dictionary<string, int> _keyboardKeyMap = new Dictionary<string, int>();
        private Dictionary<string, int> _mediaKeyMap = new Dictionary<string, int>();

        // Template for the export file
        private string _configFileTemplate = @"[{""CurrentKeyIndex"":0,
""fnKey"":{
""nProfile"":0,
""fnType"":0,
""value"":{
""mouse"":{""msbutton"":0,""wheelcount"":0,""tiltcount"":0},
""keyboard"":{""keyModifier"":0,""keyCode"":0},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":0},
""media"":{""key"":0}},
""nIndex"":0},
""IsChecked"":false},

{""CurrentKeyIndex"":1,
""fnKey"":{
""nProfile"":0,
""fnType"":BA1,
""value"":{
""mouse"":{""msbutton"":AA1,""wheelcount"":BB1,""tiltcount"":0},
""keyboard"":{""keyModifier"":AAA1,""keyCode"":BBB1},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":AAAA1},
""media"":{""key"":BBBB1}},
""nIndex"":1},
""IsChecked"":false},

{""CurrentKeyIndex"":2,
""fnKey"":{
""nProfile"":0,
""fnType"":BA2,
""value"":{
""mouse"":{""msbutton"":AA2,""wheelcount"":BB2,""tiltcount"":0},
""keyboard"":{""keyModifier"":AAA2,""keyCode"":BBB2},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":AAAA2},
""media"":{""key"":BBBB2}},
""nIndex"":2},
""IsChecked"":false},

{""CurrentKeyIndex"":3,
""fnKey"":{
""nProfile"":0,
""fnType"":BA3,
""value"":{
""mouse"":{""msbutton"":AA3,""wheelcount"":BB3,""tiltcount"":0},
""keyboard"":{""keyModifier"":AAA3,""keyCode"":BBB3},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":AAAA3},
""media"":{""key"":BBBB3}},
""nIndex"":3},
""IsChecked"":true},

{""CurrentKeyIndex"":4,
""fnKey"":{
""nProfile"":0,
""fnType"":BA4,
""value"":{
""mouse"":{""msbutton"":AA4,""wheelcount"":BB4,""tiltcount"":0},
""keyboard"":{""keyModifier"":AAA4,""keyCode"":BBB4},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":AAAA4},
""media"":{""key"":BBBB4}},
""nIndex"":4},
""IsChecked"":false},

{""CurrentKeyIndex"":5,
""fnKey"":{
""nProfile"":0,
""fnType"":BA5,
""value"":{
""mouse"":{""msbutton"":AA5,""wheelcount"":BB5,""tiltcount"":0},
""keyboard"":{""keyModifier"":AAA5,""keyCode"":BBB5},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":AAAA5},
""media"":{""key"":BBBB5}},
""nIndex"":5},
""IsChecked"":false},

{""CurrentKeyIndex"":6,
""fnKey"":{
""nProfile"":0,
""fnType"":BA6,
""value"":{
""mouse"":{""msbutton"":AA6,""wheelcount"":BB6,""tiltcount"":0},
""keyboard"":{""keyModifier"":AAA6,""keyCode"":BBB6},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":AAAA6},
""media"":{""key"":BBBB6}},
""nIndex"":6},
""IsChecked"":true},

{""CurrentKeyIndex"":8,
""fnKey"":{
""nProfile"":0,
""fnType"":0,
""value"":{
""mouse"":{""msbutton"":0,""wheelcount"":0,""tiltcount"":0},
""keyboard"":{""keyModifier"":0,""keyCode"":0},
""mouseloop"":{""msbutton"":0,""times"":0,""delay"":0},
""scroll"":{""direction"":0,""times"":0},
""macros"":{""macrosIndex"":0},
""media"":{""key"":0}},
""nIndex"":8},
""IsChecked"":false}]
";

        // --- Constructor and Load ---

        public AorusM2Config()
        {
            //Show notification dialog when opening software
            MessageBox.Show("Disclaimer:\r\nUsing third party software to tamper with the mouse configuration is not recommended and may damage the mouse for which I will not be held responsible!", "🛑 Warning!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            InitializeComponent();
            InitializeData();
            InitializeUI();
        }

        private void InitializeData()
        {
            // Initialize configurations for buttons 1-6
            for (int i = 1; i <= 6; i++)
            {
                _buttonConfigs[i] = new ButtonConfig();
            }

            // --- Populate Mappings (based on Setup.txt) ---

            // BA: fnType
            _fnTypeMap.Add("Default", 0);
            _fnTypeMap.Add("Disable", 1);
            _fnTypeMap.Add("Mouse", 2);
            _fnTypeMap.Add("Keyboard", 3);
            _fnTypeMap.Add("Macro", 6);
            _fnTypeMap.Add("DPI+", 9);
            _fnTypeMap.Add("DPI-", 10);
            _fnTypeMap.Add("DPI Loop", 11);
            _fnTypeMap.Add("Media", 13);

            // AA: mouse.msbutton
            _mouseButtonMap.Add(radioButtonLeftButton.Name, 1);
            _mouseButtonMap.Add(radioButtonRightButton.Name, 2); // NOTE: Setup.txt says Right=2, Middle=4. UI names might be swapped? Assuming Setup.txt is correct for values.
            _mouseButtonMap.Add(radioButtonMiddleButton.Name, 4);
            _mouseButtonMap.Add(radioButtonBackward.Name, 8);
            _mouseButtonMap.Add(radioButtonForward.Name, 16);

            // BB: mouse.wheelcount
            _mouseWheelMap.Add(radioButtonForwardScroll.Name, 1);
            _mouseWheelMap.Add(radioButtonBackwardScroll.Name, 255);

            // AAA: keyboard.keyModifier
            _keyboardModifierMap.Add("None", 0);
            _keyboardModifierMap.Add("Alt", 1);
            _keyboardModifierMap.Add("Ctrl", 2);
            _keyboardModifierMap.Add("Shift", 4);
            _keyboardModifierMap.Add("Win", 8);
            _keyboardModifierMap.Add("Alt + Ctrl", 3);
            _keyboardModifierMap.Add("Alt + Shift", 5);
            _keyboardModifierMap.Add("Alt + Win", 9);
            _keyboardModifierMap.Add("Ctrl + Shift", 6);
            _keyboardModifierMap.Add("Ctrl + Win", 10);
            _keyboardModifierMap.Add("Shift + Win", 12);

            // BBB: keyboard.keyCode (Add more as needed from Setup.txt)
            _keyboardKeyMap.Add("Backspace", 8);
            _keyboardKeyMap.Add("Tab", 9);
            _keyboardKeyMap.Add("Enter", 13);
            _keyboardKeyMap.Add("Shift", 16);
            _keyboardKeyMap.Add("Ctrl", 17);
            _keyboardKeyMap.Add("Alt", 18);
            _keyboardKeyMap.Add("Pause/Break", 19);
            _keyboardKeyMap.Add("Caps Lock", 20);
            _keyboardKeyMap.Add("Esc", 27);
            _keyboardKeyMap.Add("Page Up", 33);
            _keyboardKeyMap.Add("Page Down", 34);
            _keyboardKeyMap.Add("End", 35);
            _keyboardKeyMap.Add("Home", 36);
            _keyboardKeyMap.Add("Arrow Left", 37);
            _keyboardKeyMap.Add("Arrow Up", 38);
            _keyboardKeyMap.Add("Arrow Right", 39);
            _keyboardKeyMap.Add("Arrow Down", 40);
            _keyboardKeyMap.Add("Insert", 45);
            _keyboardKeyMap.Add("Delete", 46);
            _keyboardKeyMap.Add(";", 59);
            _keyboardKeyMap.Add("=", 61);
            _keyboardKeyMap.Add("A", 65); _keyboardKeyMap.Add("B", 66); _keyboardKeyMap.Add("C", 67);
            _keyboardKeyMap.Add("D", 68); _keyboardKeyMap.Add("E", 69); _keyboardKeyMap.Add("F", 70);
            _keyboardKeyMap.Add("G", 71); _keyboardKeyMap.Add("H", 72); _keyboardKeyMap.Add("I", 73);
            _keyboardKeyMap.Add("J", 74); _keyboardKeyMap.Add("K", 75); _keyboardKeyMap.Add("L", 76);
            _keyboardKeyMap.Add("M", 77); _keyboardKeyMap.Add("N", 78); _keyboardKeyMap.Add("O", 79);
            _keyboardKeyMap.Add("P", 80); _keyboardKeyMap.Add("Q", 81); _keyboardKeyMap.Add("R", 82);
            _keyboardKeyMap.Add("S", 83); _keyboardKeyMap.Add("T", 84); _keyboardKeyMap.Add("U", 85);
            _keyboardKeyMap.Add("V", 86); _keyboardKeyMap.Add("W", 87); _keyboardKeyMap.Add("X", 88);
            _keyboardKeyMap.Add("Y", 89); _keyboardKeyMap.Add("Z", 90);
            _keyboardKeyMap.Add("0 (Num Lock)", 96); _keyboardKeyMap.Add("1 (Num Lock)", 97); _keyboardKeyMap.Add("2 (Num Lock)", 98);
            _keyboardKeyMap.Add("3 (Num Lock)", 99); _keyboardKeyMap.Add("4 (Num Lock)", 100); _keyboardKeyMap.Add("5 (Num Lock)", 101);
            _keyboardKeyMap.Add("6 (Num Lock)", 102); _keyboardKeyMap.Add("7 (Num Lock)", 103); _keyboardKeyMap.Add("8 (Num Lock)", 104);
            _keyboardKeyMap.Add("9 (Num Lock)", 105); _keyboardKeyMap.Add("* (Num Lock)", 106); _keyboardKeyMap.Add("+ (Num Lock)", 107);
            _keyboardKeyMap.Add("- (Num Lock)", 109); _keyboardKeyMap.Add(". (Num Lock)", 110); _keyboardKeyMap.Add("/ (Num Lock)", 111);
            _keyboardKeyMap.Add("F1", 112); _keyboardKeyMap.Add("F2", 113); _keyboardKeyMap.Add("F3", 114);
            _keyboardKeyMap.Add("F4", 115); _keyboardKeyMap.Add("F5", 116); _keyboardKeyMap.Add("F6", 117);
            _keyboardKeyMap.Add("F7", 118); _keyboardKeyMap.Add("F8", 119); _keyboardKeyMap.Add("F9", 120);
            _keyboardKeyMap.Add("F10", 121); _keyboardKeyMap.Add("F11", 122); _keyboardKeyMap.Add("F12", 123);
            _keyboardKeyMap.Add("Num Lock", 144); _keyboardKeyMap.Add("Scroll Lock", 145);
            _keyboardKeyMap.Add("My Computer", 182); _keyboardKeyMap.Add("My Calculator", 183);
            _keyboardKeyMap.Add(",", 188); _keyboardKeyMap.Add(".", 190); _keyboardKeyMap.Add("/", 191);
            _keyboardKeyMap.Add("`", 192); _keyboardKeyMap.Add("[", 219); _keyboardKeyMap.Add("\\", 220);
            _keyboardKeyMap.Add("]", 221); _keyboardKeyMap.Add("'", 222);
            // ... add all others from Setup.txt

            // BBBB: media.key
            _mediaKeyMap.Add(radioButtonWebBrowser.Name, 0);
            _mediaKeyMap.Add(radioButtonVolumeUp.Name, 1); // Note: Setup.txt maps Volume Up to radioButtonMiddleButton? This seems wrong. Assuming radioButtonVolumeUp.
            _mediaKeyMap.Add(radioButtonVolumeDown.Name, 2);
            _mediaKeyMap.Add(radioButtonCalculator.Name, 3);
            _mediaKeyMap.Add(radioButtonPlayPause.Name, 4);
            _mediaKeyMap.Add(radioButtonStop.Name, 5);
            _mediaKeyMap.Add(radioButtonNext.Name, 6);
            _mediaKeyMap.Add(radioButtonPrevious.Name, 7);
            _mediaKeyMap.Add(radioButtonEmail.Name, 8);
            _mediaKeyMap.Add(radioButtonComputer.Name, 9);
            _mediaKeyMap.Add(radioButtonMediaPlayer.Name, 10);
            _mediaKeyMap.Add(radioButtonMuteOnOff.Name, 11);
        }

        private void InitializeUI()
        {
            // Populate ComboBoxes
            comboBoxSelectFunction.Items.Clear(); // <-- Xóa các mục cũ trước khi thêm
            comboBoxSelectFunction.Items.AddRange(_fnTypeMap.Keys.ToArray());

            comboBoxShortcuts.Items.Clear(); // <-- Xóa các mục cũ trước khi thêm
            comboBoxShortcuts.Items.AddRange(_keyboardModifierMap.Keys.ToArray());

            comboBoxKeyboard.Items.Clear(); // <-- Xóa các mục cũ trước khi thêm
            comboBoxKeyboard.Items.AddRange(_keyboardKeyMap.Keys.ToArray());

            // Set initial state
            HideAllGroupBoxes();
            DisableConfigurationControls();
            ClearSelectionHighlight();
        }

        // --- Helper Methods ---

        private void HideAllGroupBoxes()
        {
            groupBoxMouse.Visible = false;
            groupBoxKeyboard.Visible = false;
            groupBoxMacro.Visible = false;
            groupBoxMedia.Visible = false;
        }

        private void DisableConfigurationControls()
        {
            comboBoxSelectFunction.Enabled = false;
            groupBoxMouse.Enabled = false;
            groupBoxKeyboard.Enabled = false;
            groupBoxMacro.Enabled = false;
            groupBoxMedia.Enabled = false;
            buttonSave.Enabled = false;
            // Keep Export enabled maybe? Or disable until first save? For now, keep enabled.
        }

        private void EnableConfigurationControls()
        {
            comboBoxSelectFunction.Enabled = true;
            groupBoxMouse.Enabled = true; // Visibility is handled separately
            groupBoxKeyboard.Enabled = true;
            groupBoxMacro.Enabled = true;
            groupBoxMedia.Enabled = true;
            buttonSave.Enabled = true;
        }

        private void ClearSelectionHighlight()
        {
            Color defaultButtonColor = Color.Black;

            // Reset background color of all configurable buttons to the new default
            bntRight_Button.BackColor = defaultButtonColor;
            bntMiddle_Button.BackColor = defaultButtonColor;
            btnLeft_Backward.BackColor = defaultButtonColor;
            btnLeft_Forward.BackColor = defaultButtonColor;
            btnRight_Backward.BackColor = defaultButtonColor;
            btnRigh_Forward.BackColor = defaultButtonColor;
        }

        private void HighlightSelectedButton(Button button)
        {
            ClearSelectionHighlight();
            if (button != null)
            {
                button.BackColor = Color.LightSlateGray; // Or another highlight color
                _selectedButtonControl = button;
            }
        }

        private void SelectButton(int index, Button buttonControl)
        {
            _selectedButtonIndex = index;
            HighlightSelectedButton(buttonControl);
            LoadConfigToUI(index);
            EnableConfigurationControls();
            // Update visibility based on loaded FnType
            UpdateGroupBoxVisibility();
            labelSelectedButton.Text = $"Configuring: {buttonControl.Text}"; // Assuming you add a Label named labelSelectedButton
        }

        // Load saved config data into the UI controls
        private void LoadConfigToUI(int index)
        {
            if (_buttonConfigs.TryGetValue(index, out ButtonConfig config))
            {
                // Set FnType ComboBox
                string fnTypeName = _fnTypeMap.FirstOrDefault(kvp => kvp.Value == config.FnType).Key ?? "Default";
                comboBoxSelectFunction.SelectedItem = fnTypeName;

                // Set Mouse GroupBox
                string mouseButtonName = _mouseButtonMap.FirstOrDefault(kvp => kvp.Value == config.MouseButton).Key;
                if (mouseButtonName != null) ((RadioButton)groupBoxMouse.Controls[mouseButtonName]).Checked = true;
                else UncheckRadioButtons(groupBoxMouse); // Uncheck if no match

                string mouseWheelName = _mouseWheelMap.FirstOrDefault(kvp => kvp.Value == config.MouseWheelCount).Key;
                if (mouseWheelName != null) ((RadioButton)groupBoxMouse.Controls[mouseWheelName]).Checked = true;
                // Note: Mouse Button and Wheel are independent, one might be set, the other not.
                // If FnType is not Mouse, these might still load but the groupbox will be hidden.

                // Set Keyboard GroupBox
                string keyModName = _keyboardModifierMap.FirstOrDefault(kvp => kvp.Value == config.KeyboardModifier).Key ?? "None";
                comboBoxShortcuts.SelectedItem = keyModName;
                string keyName = _keyboardKeyMap.FirstOrDefault(kvp => kvp.Value == config.KeyboardKeyCode).Key;
                comboBoxKeyboard.SelectedItem = keyName; // Will be null if key code is 0 or not found

                // Set Macro GroupBox
                textBoxMacro.Text = config.MacrosIndex.ToString(); // Assuming MacrosIndex is stored directly

                // Set Media GroupBox
                string mediaKeyName = _mediaKeyMap.FirstOrDefault(kvp => kvp.Value == config.MediaKey).Key;
                if (mediaKeyName != null) ((RadioButton)groupBoxMedia.Controls[mediaKeyName]).Checked = true;
                else UncheckRadioButtons(groupBoxMedia); // Uncheck if no match
            }
            else
            {
                // Should not happen if initialized correctly, but handle defensively
                ResetUIControls();
            }
        }

        // Helper to uncheck all radio buttons in a container
        private void UncheckRadioButtons(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is RadioButton rb)
                {
                    rb.Checked = false;
                }
            }
        }


        // Reset UI to default state when no button is selected or config is cleared
        private void ResetUIControls()
        {
            comboBoxSelectFunction.SelectedItem = "Default"; // Or set index to 0
            UncheckRadioButtons(groupBoxMouse);
            comboBoxShortcuts.SelectedItem = "None";
            comboBoxKeyboard.SelectedItem = null;
            textBoxMacro.Text = "0";
            UncheckRadioButtons(groupBoxMedia);
            HideAllGroupBoxes();
        }


        // Update GroupBox visibility based on comboBoxSelectFunction
        private void UpdateGroupBoxVisibility()
        {
            HideAllGroupBoxes();
            if (comboBoxSelectFunction.SelectedItem == null) return;

            string selectedFunction = comboBoxSelectFunction.SelectedItem.ToString();

            switch (selectedFunction)
            {
                case "Mouse":
                    groupBoxMouse.Visible = true;
                    break;
                case "Keyboard":
                    groupBoxKeyboard.Visible = true;
                    break;
                case "Macro":
                    groupBoxMacro.Visible = true;
                    break;
                case "Media":
                    groupBoxMedia.Visible = true;
                    break;
                    // Default, Disable, DPI+, DPI-, DPI Loop: No groupbox shown
            }
        }

        // --- Event Handlers ---

        // Button Clicks to Select Configuration Target
        private void bntRight_Button_Click(object sender, EventArgs e) => SelectButton(1, (Button)sender);
        private void bntMiddle_Button_Click(object sender, EventArgs e) => SelectButton(2, (Button)sender);
        private void btnLeft_Backward_Click(object sender, EventArgs e) => SelectButton(3, (Button)sender);
        private void btnLeft_Forward_Click(object sender, EventArgs e) => SelectButton(4, (Button)sender);
        private void btnRight_Backward_Click(object sender, EventArgs e) => SelectButton(5, (Button)sender);
        private void btnRigh_Forward_Click(object sender, EventArgs e) => SelectButton(6, (Button)sender);


        // Main Function Selection
        private void comboBoxSelectFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGroupBoxVisibility();
            // No need to save immediately here, save happens on buttonSave_Click
        }

        // Save Button
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (_selectedButtonIndex < 1 || _selectedButtonIndex > 6)
            {
                MessageBox.Show("Please select a button to configure first.", "No Button Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_buttonConfigs.ContainsKey(_selectedButtonIndex))
            {
                // This shouldn't happen with proper initialization
                _buttonConfigs[_selectedButtonIndex] = new ButtonConfig();
            }

            ButtonConfig currentConfig = _buttonConfigs[_selectedButtonIndex];
            currentConfig.Reset(); // Start fresh for this save

            // 1. Get FnType (BA)
            string selectedFn = comboBoxSelectFunction.SelectedItem?.ToString() ?? "Default";
            currentConfig.FnType = _fnTypeMap.TryGetValue(selectedFn, out int fnTypeVal) ? fnTypeVal : 0;

            // 2. Get values based on FnType
            switch (selectedFn)
            {
                case "Mouse": // FnType = 2
                    // Get AA (msbutton)
                    foreach (RadioButton rb in groupBoxMouse.Controls.OfType<RadioButton>())
                    {
                        if (rb.Checked && _mouseButtonMap.ContainsKey(rb.Name))
                        {
                            currentConfig.MouseButton = _mouseButtonMap[rb.Name];
                            break; // Found the button type
                        }
                    }
                    // Get BB (wheelcount)
                    foreach (RadioButton rb in groupBoxMouse.Controls.OfType<RadioButton>())
                    {
                        if (rb.Checked && _mouseWheelMap.ContainsKey(rb.Name))
                        {
                            currentConfig.MouseWheelCount = _mouseWheelMap[rb.Name];
                            break; // Found the wheel type
                        }
                    }
                    break;

                case "Keyboard": // FnType = 3
                    // Get AAA (keyModifier)
                    string selectedMod = comboBoxShortcuts.SelectedItem?.ToString() ?? "None";
                    currentConfig.KeyboardModifier = _keyboardModifierMap.TryGetValue(selectedMod, out int modVal) ? modVal : 0;
                    // Get BBB (keyCode)
                    string selectedKey = comboBoxKeyboard.SelectedItem?.ToString();
                    currentConfig.KeyboardKeyCode = (selectedKey != null && _keyboardKeyMap.TryGetValue(selectedKey, out int keyVal)) ? keyVal : 0;
                    break;

                case "Macro": // FnType = 6
                    // Get AAAA (macrosIndex)
                    if (int.TryParse(textBoxMacro.Text, out int macroIndex))
                    {
                        currentConfig.MacrosIndex = macroIndex;
                    }
                    else
                    {
                        // Handle error - maybe show message and default to 0?
                        MessageBox.Show("Invalid Macro Index. Please enter a number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        currentConfig.MacrosIndex = 0; // Default to 0 on error
                        textBoxMacro.Text = "0"; // Update UI
                    }
                    break;

                case "Media": // FnType = 13
                              // Get BBBB (key)
                    foreach (RadioButton rb in groupBoxMedia.Controls.OfType<RadioButton>())
                    {
                        if (rb.Checked && _mediaKeyMap.ContainsKey(rb.Name))
                        {
                            currentConfig.MediaKey = _mediaKeyMap[rb.Name];
                            break;
                        }
                    }
                    break;

                    // For Default, Disable, DPI+, DPI-, DPI Loop, all values remain 0
            }

            // Optional: Provide feedback
            // statusStripLabel.Text = $"Configuration saved for button {_selectedButtonIndex}."; // Assuming a StatusStrip label
            MessageBox.Show($"Configuration saved for {_selectedButtonControl?.Text ?? "Button " + _selectedButtonIndex}.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        // Export Button
        private void buttonExport_Click(object sender, EventArgs e)
        {
            string finalConfig = _configFileTemplate; // Đảm bảo biến này dùng template đúng ở trên

            for (int i = 1; i <= 6; i++)
            {
                // Lấy config đã lưu hoặc tạo config mặc định (0) nếu chưa có
                ButtonConfig config = _buttonConfigs.TryGetValue(i, out ButtonConfig savedConfig)
                                      ? savedConfig
                                      : new ButtonConfig();

                // Thực hiện thay thế chính xác các placeholder
                finalConfig = finalConfig.Replace($"BBBB{i}", config.MediaKey.ToString());
                finalConfig = finalConfig.Replace($"AAAA{i}", config.MacrosIndex.ToString());
                finalConfig = finalConfig.Replace($"BBB{i}", config.KeyboardKeyCode.ToString());
                finalConfig = finalConfig.Replace($"AAA{i}", config.KeyboardModifier.ToString());
                finalConfig = finalConfig.Replace($"BA{i}", config.FnType.ToString());
                finalConfig = finalConfig.Replace($"AA{i}", config.MouseButton.ToString());
                finalConfig = finalConfig.Replace($"BB{i}", config.MouseWheelCount.ToString());
                
            }

            // --- Lưu file ---
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Aorus KeyMap File|*.mKeyMap.dat";
                saveFileDialog.Title = "Save KeyMap Configuration";
                saveFileDialog.FileName = "config.mKeyMap.dat"; // Default filename

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, finalConfig);
                        MessageBox.Show($"Configuration exported successfully to:\n{saveFileDialog.FileName}\n\nDisclaimer:\r\nUsing third party software to tamper with the mouse configuration is not recommended and may damage the mouse for which I will not be held responsible.", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open the link in the default web browser
            System.Diagnostics.Process.Start("https://github.com/NightOwlEyes/AorusM2Config");
        }
    } // End Class AorusM2Config
} // End Namespace