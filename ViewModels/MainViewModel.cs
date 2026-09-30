using CommunityToolkit.Mvvm.ComponentModel;

namespace JaqueAndo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentViewModel;

    public MainViewModel()
    {
        // La pantalla inicial será el Login
        _currentViewModel = new LoginViewModel(this);
    }

    public void NavigateTo(ViewModelBase newViewModel)
    {
        CurrentViewModel = newViewModel;
    }
}