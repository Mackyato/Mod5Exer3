using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Mod7Exer1.Model;
using Mod7Exer1.Services;
using Org.BouncyCastle.Crypto;
using System.Linq;


namespace Mod7Exer1.ViewModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;
        public ObservableCollection<Employee> EmployeeList { get; set; }
        public ObservableCollection<Employee> FilteredEmployeeList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }


        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != value)
                {
                    NewEmployeeName = _selectedEmployee.Name;
                    NewEmployeeAddress = _selectedEmployee.Address;
                    NewEmployeeemail = _selectedEmployee.email;
                    NewEmployeeContactNo = _selectedEmployee.ContactNo;
                    IsEmployeeSelected = true;
                }
                else
                {
                    IsEmployeeSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isEmployeeSelected;
        public bool IsEmployeeSelected
        {
            get => _isEmployeeSelected;
            set
            {
                _isEmployeeSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isEmployeeSelectedAdd;
        public bool IsEmployeeSelectedAdd
        {
            get => _isEmployeeSelectedAdd;
            set
            {
                _isEmployeeSelectedAdd = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterEmployeeList();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeName;
        public string NewEmployeeName
        {
            get => _newEmployeeName;
            set
            {
                _newEmployeeName = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeAddress;
        public string NewEmployeeAddress
        {
            get => _newEmployeeAddress;
            set
            {
                _newEmployeeAddress = value;
                OnPropertyChanged();
            }
        }
        private string _newEmployeeemail;
        public string NewEmployeeemail
        {
            get => _newEmployeeemail;
            set
            {
                _newEmployeeemail= value;
                OnPropertyChanged();
            }
        }
        private string _newEmployeeContactNo;
        public string NewEmployeeContactNo
        {
            get => _newEmployeeContactNo;
            set
            {
                _newEmployeeContactNo = value;
                OnPropertyChanged();
            }
        }


        public ICommand AddEmployeeCommand { get; }
        public ICommand LoadDataCommand { get; }
        public ICommand SelectedEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }
        public ICommand UpdateEmployeeCommand { get; }
        //public ICommand EditCommand { get; }
        //public ICommand DeleteCommand { get; }

        public EmployeeViewModel()
        {
            _employeeService = new EmployeeService();
            EmployeeList = new ObservableCollection<Employee>();

            FilteredEmployeeList = new ObservableCollection<Employee>();

            LoadDataCommand = new Command(async () => await LoadData());
            AddEmployeeCommand = new Command(async () => await AddEmployee());
            UpdateEmployeeCommand = new Command(async () => await UpdateEmployee());
            SelectedEmployeeCommand = new Command<Employee>(employee => SelectedEmployee = employee);
            DeleteEmployeeCommand = new Command(async () => await DeleteEmployee(),()=>SelectedEmployee != null);
            
            //EditCommand = new Command<Employee>(OnEditEmployee);
            //DeleteCommand = new Command<Employee>(OnDeleteEmployee);

            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;

            IsBusy = true;
            StatusMessage = "Loading employee data...";
            try
            {
                var employees = await _employeeService.GetAllPersonalsAsync();
                EmployeeList.Clear();
                foreach (var employee in employees)
                {
                    EmployeeList.Add(employee);
                }
                StatusMessage = "Employee data loaded successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AddEmployee()
        {
            if (IsBusy ||
                string.IsNullOrEmpty(NewEmployeeName) ||
                string.IsNullOrEmpty(NewEmployeeAddress) ||
                string.IsNullOrEmpty(NewEmployeeemail) ||
                string.IsNullOrEmpty(NewEmployeeContactNo))
            {
                StatusMessage = "Please fill in all fields before adding.";
                return;
            }

            IsBusy = true;
            StatusMessage = "Adding new employee...";
            try
            {
                var newEmployee = new Employee
                {
                    Name = NewEmployeeName,
                    Address = NewEmployeeAddress,
                    email = NewEmployeeemail,
                    ContactNo = NewEmployeeContactNo
                };

                var isSuccess = await _employeeService.AddEmployeeAsync(newEmployee);
                if (isSuccess)
                {
                    // Clear fields after adding
                    NewEmployeeName = string.Empty;
                    NewEmployeeAddress = string.Empty;
                    NewEmployeeemail = string.Empty;
                    NewEmployeeContactNo = string.Empty;
                    StatusMessage = "New employee added successfully!";
                }
                else
                {
                    StatusMessage = "Failed to add the new employee.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData(); // Load data again after adding a new employee
            }
        }


        //private void OnEditEmployee(Employee employee)
        //{
        //    if (employee != null)
        //    {
        //        // Here, navigate to an EditEmployee page or open a dialog
        //        StatusMessage = $"Editing {employee.Name}";
        //    }
        //}

        private async Task DeleteEmployee()
        {
            if (SelectedEmployee == null)
                return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {SelectedEmployee.Name}?", "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting employee...";
            try
            {
                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID);
                StatusMessage = success ? "Person deleted successfully!" : "Failed to delete employee";

                if (success)
                {
                    EmployeeList.Remove(SelectedEmployee);
                    SelectedEmployee = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error delete employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        //if (employee != null && EmployeeList.Contains(employee))
        //{
        //    EmployeeList.Remove(employee);
        //   StatusMessage = $"{employee.Name} has been deleted.";
        //}

        private async Task UpdateEmployee()
        {
            if(IsBusy || SelectedEmployee == null)
            {
                StatusMessage = "Select a person to update.";
                return;
            }
            IsBusy = true;
            try
            {
                SelectedEmployee.Name = NewEmployeeName;
                SelectedEmployee.Address = NewEmployeeAddress;
                SelectedEmployee.email = NewEmployeeemail;
                SelectedEmployee.ContactNo = NewEmployeeContactNo;

                var success = await _employeeService.UpdateEmployeeAsync(SelectedEmployee);
                
                NewEmployeeName = string.Empty;
                NewEmployeeAddress = string.Empty;
                NewEmployeeemail = string.Empty;
                NewEmployeeContactNo = string.Empty;
                StatusMessage = success ? "Person updated successfully!" : "Failed to update person";

                await LoadData();
                //ClearFields();

            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterEmployeeList()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Clear the filtered list and repopulate it with all employees
                FilteredEmployeeList.Clear();
                foreach (var employee in EmployeeList)
                {
                    FilteredEmployeeList.Add(employee);
                }
            }
            else
            {
                // Filter based on the search text
                var filtered = EmployeeList.Where(p =>
                    p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.ContactNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Update the filtered list with the search results
                FilteredEmployeeList.Clear();
                foreach (var employee in filtered)
                {
                    FilteredEmployeeList.Add(employee);
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
