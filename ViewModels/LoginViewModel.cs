using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Services;
using System.Threading.Tasks;

namespace JaqueAndo.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private string _username = "admin";

    [ObservableProperty]
    private string _password = "123";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public LoginViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        ErrorMessage = string.Empty;

        var (usuario, error) = await ApiService.LoginAsync(Username.Trim(), Password);

        if (usuario != null)
            _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, usuario));
        else
            ErrorMessage = error ?? "No se pudo iniciar sesión.";
    }
}