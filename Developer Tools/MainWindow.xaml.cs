using DeveloperTools.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DeveloperTools
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _dragElement;

        // Import user32.dll to make this window always stay on top
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;

            // Ensure the window stays on top
            this.Topmost = true;
            this.Deactivated += MainWindow_Deactivated;
            // Set up the ComboBox text changed event for filtering
            AppNameComboBox.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnAppNameTextChanged));

        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            // Keep the window on top and set it as the foreground window
            this.Topmost = true;
            SetForegroundWindow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }



        // Filter the ComboBox items based on the text entered
        private void OnAppNameTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && viewModel != null)
            {
                // Update the filtered list in the ViewModel based on the ComboBox text
                viewModel.FilterAppNames(comboBox.Text);
            }
        }


        // MouseDown event handler for TextBlocks
        private void OnTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
            _dragElement = sender as UIElement;
            _dragElement.CaptureMouse();
        }

        // MouseMove event handler for dragging TextBlocks
        private void OnTextBlockMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _dragElement != null)
            {
                Point currentPosition = e.GetPosition(this);

                double offsetX = currentPosition.X - _startPoint.X;
                double offsetY = currentPosition.Y - _startPoint.Y;

                // Update the position of the TextBlock
                double newLeft = Canvas.GetLeft(_dragElement) + offsetX;
                double newTop = Canvas.GetTop(_dragElement) + offsetY;

                Canvas.SetLeft(_dragElement, newLeft);
                Canvas.SetTop(_dragElement, newTop);

                _startPoint = currentPosition; // Reset the starting point
            }
        }

        // MouseUp event handler to release the drag
        private void OnTextBlockMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            if (_dragElement != null)
            {
                _dragElement.ReleaseMouseCapture();
                _dragElement = null;
            }
        }

        private void OnGetAppUsageClicked(object sender, RoutedEventArgs e)
        {
            viewModel.UpdateResources(); // Call the method to update CPU and RAM usage
            viewModel.StartUpdatingResources(); // Start the timer to keep updating
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
