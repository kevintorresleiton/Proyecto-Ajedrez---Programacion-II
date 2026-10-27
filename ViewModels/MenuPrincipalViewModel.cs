using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;

namespace JaqueAndo.ViewModels;

public partial class MenuPrincipalViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private Usuario _usuarioActual;

    public MenuPrincipalViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;
    }
        // Abre la pantalla del tutorial
    [RelayCommand]
    private void IrATutorial() => _mainViewModel.NavigateTo(new TutorialViewModel(_mainViewModel, UsuarioActual));

    [RelayCommand]
    private void IrAJugar() => _mainViewModel.NavigateTo(new JugarViewModel(_mainViewModel, UsuarioActual));

    [RelayCommand]
    private void IrATrivia() => _mainViewModel.NavigateTo(new TriviaViewModel(_mainViewModel, UsuarioActual));

    [RelayCommand]
    private void IrABaseDatos() => _mainViewModel.NavigateTo(new BaseDatosViewModel(_mainViewModel, UsuarioActual));

    [RelayCommand]
    private void IrAEventos() => _mainViewModel.NavigateTo(new EventosApiViewModel(_mainViewModel, UsuarioActual));

    [RelayCommand]
    private void IrARanking() => _mainViewModel.NavigateTo(new RankingViewModel(_mainViewModel, UsuarioActual));
    [RelayCommand]
    private void CerrarSesion() => _mainViewModel.NavigateTo(new LoginViewModel(_mainViewModel));
}