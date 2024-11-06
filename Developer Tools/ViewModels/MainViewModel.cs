using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DeveloperTools.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _cpuUsage;
        private string _cpuUsagePercentage;
        private double _ramUsage;
        private string _ramUsagePercentage;
        private string _appName;
        private Visibility _textBoxVisibility;
        private Visibility _buttonVisibility;
        private Brush _cpuUsageColor;
        private Brush _ramUsageColor;
        private DispatcherTimer _timer;

        // Property for the process name entered in the TextBox
        public string AppName
        {
            get => _appName;
            set
            {
                _appName = value;
                OnPropertyChanged(nameof(AppName));
            }
        }

        // Property for TextBox visibility
        public Visibility TextBoxVisibility
        {
            get => _textBoxVisibility;
            set
            {
                _textBoxVisibility = value;
                OnPropertyChanged(nameof(TextBoxVisibility));
            }
        }

        // Property for Button visibility
        public Visibility ButtonVisibility
        {
            get => _buttonVisibility;
            set
            {
                _buttonVisibility = value;
                OnPropertyChanged(nameof(ButtonVisibility));
            }
        }

        // Property for CPU usage
        public double CpuUsage
        {
            get => _cpuUsage;
            set
            {
                _cpuUsage = value;
                OnPropertyChanged(nameof(CpuUsage));
            }
        }

        // Property for CPU usage percentage as a string
        public string CpuUsagePercentage
        {
            get => _cpuUsagePercentage;
            set
            {
                _cpuUsagePercentage = value;
                OnPropertyChanged(nameof(CpuUsagePercentage));
            }
        }

        // Property for RAM usage
        public double RamUsage
        {
            get => _ramUsage;
            set
            {
                _ramUsage = value;
                OnPropertyChanged(nameof(RamUsage));
            }
        }

        // Property for RAM usage percentage as a string
        public string RamUsagePercentage
        {
            get => _ramUsagePercentage;
            set
            {
                _ramUsagePercentage = value;
                OnPropertyChanged(nameof(RamUsagePercentage));
            }
        }

        // Property for CPU usage color
        public Brush CpuUsageColor
        {
            get => _cpuUsageColor;
            set
            {
                _cpuUsageColor = value;
                OnPropertyChanged(nameof(CpuUsageColor));
            }
        }

        // Property for RAM usage color
        public Brush RamUsageColor
        {
            get => _ramUsageColor;
            set
            {
                _ramUsageColor = value;
                OnPropertyChanged(nameof(RamUsageColor));
            }
        }

        // Constructor for the ViewModel
        public MainViewModel()
        {
            // Initially show the TextBox and Button
            TextBoxVisibility = Visibility.Visible;
            ButtonVisibility = Visibility.Visible;
            CpuUsageColor = Brushes.Black;
            RamUsageColor = Brushes.Black;

            // Initialize the timer but don't start it yet
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Update every second
            };
            _timer.Tick += (sender, e) => UpdateResources();
        }

        // Method to update resources (CPU/RAM) for the given app
        public void UpdateResources()
        {
            if (string.IsNullOrEmpty(AppName))
            {
                return;
            }

            // Get the selected process by the name entered in the TextBox
            var selectedProcess = Process.GetProcessesByName(AppName).FirstOrDefault();

            if (selectedProcess != null)
            {
                // Get CPU usage (simplified method)
                CpuUsage = GetCpuUsage(selectedProcess);
                CpuUsagePercentage = $"{CpuUsage:F2}%";

                // Get RAM usage
                RamUsage = selectedProcess.PrivateMemorySize64 / (1024.0 * 1024.0); // MB
                RamUsagePercentage = $"{RamUsage:F2} MB";

                // Change color based on thresholds
                CpuUsageColor = CpuUsage > 10 ? Brushes.Red : Brushes.Green;
                RamUsageColor = RamUsage > 500 ? Brushes.Red : Brushes.Green;

                // Hide TextBox and Button after usage is fetched
                TextBoxVisibility = Visibility.Collapsed;
                ButtonVisibility = Visibility.Collapsed;
                StartUpdatingResources();
            }
            else
            {
                CpuUsagePercentage = "App not found.";
                RamUsagePercentage = "N/A";
            }
        }

        // Start the timer (call when button is clicked)
        public void StartUpdatingResources()
        {
            _timer.Start();
        }

        // Simplified method to calculate CPU usage
        private double GetCpuUsage(Process process)
        {
            // Simplified calculation
            return process.TotalProcessorTime.TotalMilliseconds / (Environment.ProcessorCount * 1000);
        }

        // PropertyChanged event for data binding
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to trigger the PropertyChanged event
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
