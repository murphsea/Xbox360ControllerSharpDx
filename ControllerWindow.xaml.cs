using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Xbox360ControllerSharpDx
{
    /// <summary>
    /// Interaction logic for ControllerWindow.xaml
    /// </summary>
    public partial class ControllerWindow : Window
    {
        private SharpDX.XInput.Controller _xbox360Controller;
        private Thread controllerStateThread;

        public ControllerWindow()
        {
            InitializeComponent();
            lblDpadState.Content = "";
            lblLeftStickX.Content = "";
            lblLeftStickY.Content = "";
            lblLeftTriggerV.Content = "";
            lblRightStickX.Content = "";
            lblRightStickY.Content = "";
            lblRightTriggerV.Content = "";
            lblLeftStickState.Content = "";
            lblRightStickState.Content = "";
            
        }

        private void ConnectToXbox360Controller()
        {
            try
            {
                // Connect to the selected Xbox 360 Controller
                switch((ControllerSelection.SelectedItem as ComboBoxItem).Content.ToString())
                {
                    case "Controller One":
                        _xbox360Controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.One);
                        break;
                    case "Controller Two":
                        _xbox360Controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Two);
                        break;
                    case "Controller Three":
                        _xbox360Controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Three);
                        break;
                    case "Controller Four":
                        _xbox360Controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Four);
                        break;
                }
                
                if(_xbox360Controller.IsConnected)
                {
                    lblConnectMessage.Content = "Connected to Controller " + _xbox360Controller.UserIndex.ToString();
                    btnConnect.IsEnabled = false;
                    ControllerSelection.IsEnabled = false;
                    controllerStateThread = new Thread(new ThreadStart(ReadControllerState));
                    controllerStateThread.Start();
                }
                else if(ControllerSelection.SelectedItem!=null)
                {
                    lblConnectMessage.Content = "Failed to connect to " + ((ControllerSelection.SelectedItem as ComboBoxItem).Content.ToString());
                }
            }
            catch(Exception connectException)
            {
                MessageBox.Show("Error Connecting:" + connectException.Message, "Error Connecting to Xbox 360 Controller");
            }
        }

        private void ReadControllerState()
        {
            SharpDX.XInput.State _currentControllerState =  _xbox360Controller.GetState();

            // Loop until both A and B buttons are pressed at the same time
            while(_currentControllerState.Gamepad.Buttons!=(SharpDX.XInput.GamepadButtonFlags.B | SharpDX.XInput.GamepadButtonFlags.A))
            {
                // Get DPad State
                

                Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        // Dpad direction
                        string sDpadDirection = "";
                        if (_currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadUp)
                            sDpadDirection = "Up";
                        else if (_currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadDown)
                            sDpadDirection = "Down";
                        else if (_currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadLeft)
                            sDpadDirection = "Left";
                        else if (_currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.DPadRight)
                            sDpadDirection = "Right";
               
                        lblDpadState.Content = sDpadDirection;
               
                        // Left Thumb Stick
                        lblLeftStickX.Content = _currentControllerState.Gamepad.LeftThumbX.ToString();
                        lblLeftStickY.Content = _currentControllerState.Gamepad.LeftThumbY.ToString();
                        lblLeftStickState.Content = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.LeftThumb ? "Down" : "Up";

                        // Left Trigger
                        lblLeftTriggerV.Content = _currentControllerState.Gamepad.LeftTrigger.ToString();
                        
                        // Right Thumb Stick
                        lblRightStickX.Content = _currentControllerState.Gamepad.RightThumbX.ToString();
                        lblRightStickY.Content = _currentControllerState.Gamepad.RightThumbY.ToString();
                        lblRightStickState.Content = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.RightThumb ? "Down" : "Up";

                        // Right Trigger
                        lblRightTriggerV.Content = _currentControllerState.Gamepad.RightTrigger.ToString();
                        
                        // Left and Right Shoulder Buttons above the triggers on the Xbox 360 Controller
                        rectLeftButton.Fill = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.LeftShoulder ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);
                        rectRightButton.Fill = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.RightShoulder ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);
                        
                        // A, B, Y and X buttons 
                        btnAYes.Fill = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.A ? new SolidColorBrush(Colors.Purple) : new SolidColorBrush(Colors.White);
                        btnBYes.Fill = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.B ? new SolidColorBrush(Colors.Purple) : new SolidColorBrush(Colors.White);
                        btnYYes.Fill = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.Y ? new SolidColorBrush(Colors.Purple) : new SolidColorBrush(Colors.White);
                        btnXYes.Fill = _currentControllerState.Gamepad.Buttons == SharpDX.XInput.GamepadButtonFlags.X ? new SolidColorBrush(Colors.Purple) : new SolidColorBrush(Colors.White);

                    }));
                _currentControllerState = _xbox360Controller.GetState();
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                    }));

        }




        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

            ConnectToXbox360Controller();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(controllerStateThread!=null)
                controllerStateThread.Abort();
        }
    }
}
