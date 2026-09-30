using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Data;
using System.Linq;

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
    private void IniciarSesion()
    {
        using var db = new AppDbContext();
        var user = db.Usuarios.FirstOrDefault(u => u.Username.ToLower() == Username.Trim().ToLower() && u.Password == Password);

        if (user != null)
        {
            _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, user));
        }
        else
        {
            ErrorMessage = "Usuario o contraseña incorrectos.";
        }
    }
}