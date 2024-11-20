namespace Mod7Exer1.View;
using Mod7Exer1.ViewModel;
using Mod7Exer1.Services;

public partial class ViewEmployee : ContentPage
{
	public ViewEmployee()
	{
		InitializeComponent();

        var employeeViewModel = new EmployeeViewModel();
        BindingContext = employeeViewModel;
    }
}