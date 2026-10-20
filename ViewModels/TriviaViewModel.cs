using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using JaqueAndo.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace JaqueAndo.ViewModels;

public partial class TriviaViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private List<PreguntaTrivia> _preguntasActivas = new();
    private int _indicePreguntaActual = 0;
    private DispatcherTimer _timer;
    private bool _respuestaProcesada = false;
    private bool _resultadoGuardado = false;

    [ObservableProperty] private Usuario _usuarioActual;
    [ObservableProperty] private PreguntaTrivia _preguntaActual = new();
    [ObservableProperty] private int _puntaje = 0;
    [ObservableProperty] private int _tiempoRestante = 15;
    [ObservableProperty] private string _mensajeResultado = string.Empty;
    [ObservableProperty] private string _tituloEncuesta = "Trivia de Ajedrez";
    [ObservableProperty] private bool _mostrarPregunta = true;
    [ObservableProperty] private bool _mostrarFinal = false;

    public TriviaViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;

        _ = CargarPreguntasAsync();
    }

    private async Task CargarPreguntasAsync()
    {
        try
        {
            MensajeResultado = "Cargando preguntas...";

            var (preguntas, error) = await ApiService.ObtenerPreguntasAsync(5);

            if (preguntas == null || preguntas.Count == 0)
            {
                MensajeResultado = error ?? "No hay preguntas disponibles.";
                return;
            }

            _preguntasActivas = preguntas;
            _indicePreguntaActual = 0;
            CargarSiguientePregunta();
        }
        catch (Exception ex)
        {
            MensajeResultado = "Error al cargar la trivia: " + ex.Message;
        }
    }

    private void CargarSiguientePregunta()
    {
        if (_indicePreguntaActual < _preguntasActivas.Count)
        {
            _respuestaProcesada = false;
            PreguntaActual = _preguntasActivas[_indicePreguntaActual];
            TiempoRestante = 15;
            MensajeResultado = string.Empty;
            MostrarPregunta = true;
            MostrarFinal = false;
            _timer.Start();
        }
        else
        {
            _timer.Stop();
            MostrarPregunta = false;
            MostrarFinal = true;
            _ = GuardarResultadoAsync();
        }
    }

    private async Task GuardarResultadoAsync()
    {
        if (_resultadoGuardado) return;
        _resultadoGuardado = true;

        var error = await ApiService.GuardarResultadoTriviaAsync(UsuarioActual.Id, Puntaje);
        MensajeResultado = error ?? $"Puntaje guardado: {Puntaje}";
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (TiempoRestante > 0)
        {
            TiempoRestante--;
        }
        else
        {
            ProcesarRespuesta(0);
        }
    }

    [RelayCommand]
    private void Responder(string opcionStr)
    {
        if (int.TryParse(opcionStr, out int opcion))
        {
            ProcesarRespuesta(opcion);
        }
    }

    private void ProcesarRespuesta(int opcion)
    {
        if (_respuestaProcesada || _preguntasActivas.Count == 0)
            return;

        _respuestaProcesada = true;
        _timer.Stop();

        if (opcion == PreguntaActual.OpcionCorrecta)
        {
            Puntaje += 100 + (TiempoRestante * 10);
            MensajeResultado = $"¡Correcto! 🎉 {PreguntaActual.Explicacion}";
        }
        else
        {
            MensajeResultado = $"Incorrecto ❌. {PreguntaActual.Explicacion}";
        }

        DispatcherTimer.RunOnce(() =>
        {
            _indicePreguntaActual++;
            CargarSiguientePregunta();
        }, TimeSpan.FromSeconds(2));
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _timer.Stop();
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}