using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using System;
using System.Collections.Generic;
using Avalonia.Threading;

namespace JaqueAndo.ViewModels;

public partial class TriviaViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private List<PreguntaTrivia> _preguntas;
    private int _indiceActual = 0;
    private DispatcherTimer _timer;

    [ObservableProperty]
    private Usuario _usuarioActual;

    [ObservableProperty]
    private PreguntaTrivia? _preguntaActual;

    [ObservableProperty]
    private int _puntaje = 0;

    [ObservableProperty]
    private int _tiempoRestante = 15;

    [ObservableProperty]
    private string _mensajeResultado = string.Empty;

    [ObservableProperty]
    private bool _triviaFinalizada = false;

    public TriviaViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        _preguntas = new List<PreguntaTrivia>
        {
            new PreguntaTrivia { Id = 1, Enunciado = "¿Cuál es la única pieza que puede saltar sobre otras?", OpcionA = "Torre", OpcionB = "Caballo", OpcionC = "Alfil", OpcionD = "Dama", OpcionCorrecta = 2, Explicacion = "El Caballo es la única pieza capaz de saltar sobre piezas propias o enemigas en forma de L." },
            new PreguntaTrivia { Id = 2, Enunciado = "¿Cuántas casillas tiene un tablero de ajedrez estándar?", OpcionA = "36", OpcionB = "48", OpcionC = "64", OpcionD = "81", OpcionCorrecta = 3, Explicacion = "Un tablero posee una cuadrícula de 8x8, lo que da un total de 64 casillas." },
            new PreguntaTrivia { Id = 3, Enunciado = "¿Qué movimiento especial permite mover el Rey y la Torre en una misma jugada?", OpcionA = "Enroque", OpcionB = "Peón al paso", OpcionC = "Promoción", OpcionD = "Coronación", OpcionCorrecta = 1, Explicacion = "El enroque es el único movimiento en el que se desplazan dos piezas simultáneamente." },
            new PreguntaTrivia { Id = 4, Enunciado = "¿Quién fue el primer Campeón Mundial oficial de ajedrez?", OpcionA = "Garry Kasparov", OpcionB = "Wilhelm Steinitz", OpcionC = "Bobby Fischer", OpcionD = "José Raúl Capablanca", OpcionCorrecta = 2, Explicacion = "Wilhelm Steinitz se convirtió en el primer campeón mundial oficial en 1886." },
            new PreguntaTrivia { Id = 5, Enunciado = "¿Qué valor numérico aproximado se le asigna a la Dama?", OpcionA = "3 puntos", OpcionB = "5 puntos", OpcionC = "9 puntos", OpcionD = "10 puntos", OpcionCorrecta = 3, Explicacion = "La Dama equivale a 9 puntos por su versatilidad de movimientos." }
        };

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;

        CargarPregunta();
    }

    private void CargarPregunta()
    {
        if (_indiceActual < _preguntas.Count)
        {
            PreguntaActual = _preguntas[_indiceActual];
            TiempoRestante = 15;
            MensajeResultado = string.Empty;
            _timer.Start();
        }
        else
        {
            _timer.Stop();
            TriviaFinalizada = true;
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (TiempoRestante > 0)
        {
            TiempoRestante--;
        }
        else
        {
            Responder(0); // Se agotó el tiempo
        }
    }

    [RelayCommand]
    private void Responder(int opcion)
    {
        _timer.Stop();
        if (PreguntaActual == null) return;

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
            _indiceActual++;
            CargarPregunta();
        }, TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _timer.Stop();
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}