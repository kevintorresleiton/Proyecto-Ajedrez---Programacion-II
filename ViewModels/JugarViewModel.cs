using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using System;
using Avalonia.Threading;

namespace JaqueAndo.ViewModels;

public partial class JugarViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private DispatcherTimer _timer;

    [ObservableProperty]
    private Usuario _usuarioActual;

    [ObservableProperty]
    private string _turnoActual = "Blancas";

    [ObservableProperty]
    private string _tiempoBlancas = "05:00";

    [ObservableProperty]
    private string _tiempoNegras = "05:00";

    [ObservableProperty]
    private bool _juegoEnPausa = true;

    private int _segundosBlancas = 300;
    private int _segundosNegras = 300;

    public JugarViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (JuegoEnPausa) return;

        if (TurnoActual == "Blancas")
        {
            if (_segundosBlancas > 0) _segundosBlancas--;
            TiempoBlancas = TimeSpan.FromSeconds(_segundosBlancas).ToString(@"mm\:ss");
        }
        else
        {
            if (_segundosNegras > 0) _segundosNegras--;
            TiempoNegras = TimeSpan.FromSeconds(_segundosNegras).ToString(@"mm\:ss");
        }
    }

    [RelayCommand]
    private void IniciarPausarJuego()
    {
        JuegoEnPausa = !JuegoEnPausa;
        if (JuegoEnPausa) _timer.Stop();
        else _timer.Start();
    }

    [RelayCommand]
    private void CambiarTurno()
    {
        TurnoActual = TurnoActual == "Blancas" ? "Negras" : "Blancas";
    }

    [RelayCommand]
    private void ReiniciarPartida()
    {
        _timer.Stop();
        _segundosBlancas = 300;
        _segundosNegras = 300;
        TiempoBlancas = "05:00";
        TiempoNegras = "05:00";
        TurnoActual = "Blancas";
        JuegoEnPausa = true;
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _timer.Stop();
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}