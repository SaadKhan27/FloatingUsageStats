using DeveloperTools.ViewModels;
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

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
        }

        // MouseDown event handler for both TextBlocks
        private void OnTextBlockMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
            _dragElement = sender as UIElement;
            _dragElement.CaptureMouse();
        }

        // MouseMove event handler for dragging
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
    }
}
