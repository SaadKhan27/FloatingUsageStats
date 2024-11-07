using System;
using System.Collections.ObjectModel;
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
        private Visibility _textBoxVisibilityCpuUsage;
        private Visibility _textBoxVisibilityMemoryUsage;
        private Visibility _buttonVisibility;
        private Brush _cpuUsageColor;
        private Brush _ramUsageColor;
        private DispatcherTimer _timer;
        private ObservableCollection<string> _filteredAppNames;
        private string _selectedAppName;
        private ObservableCollection<string> _allAppNames; // Keep original list

        public MainViewModel()
        {
            // Initially show the ComboBox and Button
            TextBoxVisibility = Visibility.Visible;
            ButtonVisibility = Visibility.Visible;
            CpuUsageColor = Brushes.Black;
            RamUsageColor = Brushes.Black;
            _allAppNames = new ObservableCollection<string>();

            // Initialize the filtered app names and load running processes
            FilteredAppNames = new ObservableCollection<string>();
            LoadRunningProcesses();

            // Initialize the timer but don't start it yet
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Update every second
            };
            _timer.Tick += (sender, e) => UpdateResources();
        }

        // Collection of filtered app names
        public ObservableCollection<string> FilteredAppNames
        {
            get => _filteredAppNames;
            set
            {
                _filteredAppNames = value;
                OnPropertyChanged(nameof(FilteredAppNames));
            }
        }

        // Property for the selected app name in the ComboBox
        public string SelectedAppName
        {
            get => _selectedAppName;
            set
            {
                _selectedAppName = value;
                AppName = value; // Set AppName to the selected app
                OnPropertyChanged(nameof(SelectedAppName));
            }
        }

        // Property for the process name entered in the ComboBox
        public string AppName
        {
            get => _appName;
            set
            {
                _appName = value;
                OnPropertyChanged(nameof(AppName));
            }
        }

        // Property for ComboBox visibility
        public Visibility TextBoxVisibility
        {
            get => _textBoxVisibility;
            set
            {
                _textBoxVisibility = value;
                OnPropertyChanged(nameof(TextBoxVisibility));
            }
        }


        // Property for CpuUsageTextBox visibility
        public Visibility TextBoxVisibilityCpuUsage
        {
            get => _textBoxVisibilityCpuUsage;
            set
            {
                _textBoxVisibilityCpuUsage = value;
                OnPropertyChanged(nameof(TextBoxVisibilityCpuUsage));
            }
        }


        // Property for MemoryUsageTextBox visibility
        public Visibility TextBoxVisibilityMemoryUsage
        {
            get => _textBoxVisibilityMemoryUsage;
            set
            {
                _textBoxVisibilityMemoryUsage = value;
                OnPropertyChanged(nameof(TextBoxVisibilityMemoryUsage));
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

        // Method to update resources (CPU/RAM) for the selected app
       
        public void UpdateResources()
        {
            if (string.IsNullOrEmpty(AppName))
            {
                return;
            }

            var selectedProcess = Process.GetProcessesByName(AppName).FirstOrDefault();

            if (selectedProcess != null)
            {
                CpuUsage = GetCpuUsage(selectedProcess);
                CpuUsagePercentage = $"{CpuUsage:F2}%";

                RamUsage = selectedProcess.PrivateMemorySize64 / (1024.0 * 1024.0); // MB
                RamUsagePercentage = $"{RamUsage:F2} MB";

                CpuUsageColor = CpuUsage > 10 ? Brushes.Red : Brushes.Green;
                RamUsageColor = RamUsage > 500 ? Brushes.Red : Brushes.Green;

                // Hide the ComboBox and Button as the app is running
                TextBoxVisibility = Visibility.Collapsed;
                ButtonVisibility = Visibility.Collapsed;
                StartUpdatingResources();
            }
            else
            {
                // If the app is not found, show the ComboBox and Button
                CpuUsagePercentage = "App not found.";
                RamUsagePercentage = "N/A";

                // Reset visibility to show ComboBox and Button again
                TextBoxVisibility = Visibility.Visible;
                ButtonVisibility = Visibility.Visible;
                TextBoxVisibilityCpuUsage = Visibility.Hidden;
                TextBoxVisibilityMemoryUsage = Visibility.Hidden;

            }
        }




        public void FilterAppNames(string filter)
        {
            FilteredAppNames.Clear(); // Clear the existing items

            var processNames = string.IsNullOrEmpty(filter)
                ? _allAppNames // Show all app names if filter is empty
                : _allAppNames.Where(name => name.StartsWith(filter, StringComparison.OrdinalIgnoreCase));

            foreach (var name in processNames)
            {
                FilteredAppNames.Add(name);
            }
        }


        private void LoadRunningProcesses()
        {
            var processNames = Process.GetProcesses()
                .Select(p => p.ProcessName)
                .Distinct()
                .OrderBy(name => name);

            foreach (var name in processNames)
            {
                _allAppNames.Add(name); // Populate the original list
                FilteredAppNames.Add(name); // Populate the display list
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
