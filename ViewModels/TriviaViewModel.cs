using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using System;
using System.Collections.Generic;
using Avalonia.Threading;

namespace JaqueAndo.ViewModels;

public partial class TriviaViewModel : ViewModelBase
{
    private static int _grupoActualIndex = 0; // Mantiene el índice global del grupo
    private readonly MainViewModel _mainViewModel;
    private List<PreguntaTrivia> _preguntasActivas = new();
    private int _indicePreguntaActual = 0;
    private DispatcherTimer _timer;

    [ObservableProperty] private Usuario _usuarioActual;
    [ObservableProperty] private PreguntaTrivia _preguntaActual = new();
    [ObservableProperty] private int _puntaje = 0;
    [ObservableProperty] private int _tiempoRestante = 15;
    [ObservableProperty] private string _mensajeResultado = string.Empty;
    [ObservableProperty] private string _tituloEncuesta = string.Empty;
    [ObservableProperty] private bool _mostrarPregunta = true;
    [ObservableProperty] private bool _mostrarFinal = false;

    public TriviaViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;

        CargarGrupoEncuesta();
    }

    private void CargarGrupoEncuesta()
    {
        // Definición de los 3 grupos de encuestas
        var grupos = new List<(string Titulo, List<PreguntaTrivia> Preguntas)>
        {
            ("Encuesta 1: Reglas Básico-Intermedias", new List<PreguntaTrivia>
            {
                new PreguntaTrivia { Id = 1, Enunciado = "¿Cuál es la única pieza que puede saltar sobre otras?", OpcionA = "1. Torre", OpcionB = "2. Caballo", OpcionC = "3. Alfil", OpcionD = "4. Dama", OpcionCorrecta = 2, Explicacion = "El Caballo salta en forma de L." },
                new PreguntaTrivia { Id = 2, Enunciado = "¿Cuántas casillas tiene un tablero estándar?", OpcionA = "1. 36", OpcionB = "2. 48", OpcionC = "3. 64", OpcionD = "4. 81", OpcionCorrecta = 3, Explicacion = "Es una cuadrícula de 8x8 = 64." },
                new PreguntaTrivia { Id = 3, Enunciado = "¿Qué movimiento permite mover el Rey y la Torre juntos?", OpcionA = "1. Enroque", OpcionB = "2. Peón al paso", OpcionC = "3. Promoción", OpcionD = "4. Coronación", OpcionCorrecta = 1, Explicacion = "El enroque mueve ambas piezas simultáneamente." },
                new PreguntaTrivia { Id = 4, Enunciado = "¿En qué casilla inicial se coloca la Dama Blanca?", OpcionA = "1. d1 (Blanca)", OpcionB = "2. e1 (Negra)", OpcionC = "3. c1", OpcionD = "4. a1", OpcionCorrecta = 1, Explicacion = "La Dama Blanca va siempre en su color (d1)." },
                new PreguntaTrivia { Id = 5, Enunciado = "¿Qué ocurre si un Peón llega a la última fila?", OpcionA = "1. Vuelve a inicio", OpcionB = "2. Se corona/promociona", OpcionC = "3. Se elimina", OpcionD = "4. Gana 2 puntos", OpcionCorrecta = 2, Explicacion = "Se transforma en Dama, Torre, Alfil o Caballo." }
            }),

            ("Encuesta 2: Historia y Grandes Maestros", new List<PreguntaTrivia>
            {
                new PreguntaTrivia { Id = 6, Enunciado = "¿Quién fue el primer Campeón Mundial oficial de ajedrez?", OpcionA = "1. Garry Kasparov", OpcionB = "2. Wilhelm Steinitz", OpcionC = "3. Bobby Fischer", OpcionD = "4. Capablanca", OpcionCorrecta = 2, Explicacion = "Wilhelm Steinitz se coronó en 1886." },
                new PreguntaTrivia { Id = 7, Enunciado = "¿Qué Gran Maestro argentino disputó el título mundial contra Alekhine?", OpcionA = "1. Miguel Najdorf", OpcionB = "2. Roberto Grau", OpcionC = "3. Erich Eliskases", OpcionD = "4. Oscar Panno", OpcionCorrecta = 1, Explicacion = "Miguel Najdorf fue una leyenda del ajedrez argentino." },
                new PreguntaTrivia { Id = 8, Enunciado = "¿Contra qué computadora perdió Garry Kasparov en 1997?", OpcionA = "1. Stockfish", OpcionB = "2. AlphaZero", OpcionC = "3. Deep Blue", OpcionD = "4. Fritz", OpcionCorrecta = 3, Explicacion = "Deep Blue de IBM superó a Kasparov en el histórico duelo." },
                new PreguntaTrivia { Id = 9, Enunciado = "¿De qué nacionalidad es el excampeón Magnus Carlsen?", OpcionA = "1. Rusa", OpcionB = "2. Noruega", OpcionC = "3. Alemana", OpcionD = "4. Sueca", OpcionCorrecta = 2, Explicacion = "Magnus Carlsen es oriundo de Noruega." },
                new PreguntaTrivia { Id = 10, Enunciado = "¿En qué año se fundó la FIDE (Federación Internacional)?", OpcionA = "1. 1924", OpcionB = "2. 1900", OpcionC = "3. 1950", OpcionD = "4. 1980", OpcionCorrecta = 1, Explicacion = "La FIDE fue fundada en París en 1924." }
            }),

            ("Encuesta 3: Táctica y Valor de las Piezas", new List<PreguntaTrivia>
            {
                new PreguntaTrivia { Id = 11, Enunciado = "¿Qué valor relativo aproximado tiene la Dama?", OpcionA = "1. 3 puntos", OpcionB = "2. 5 puntos", OpcionC = "3. 9 puntos", OpcionD = "4. 10 puntos", OpcionCorrecta = 3, Explicacion = "La Dama equivale aproximadamente a 9 peones." },
                new PreguntaTrivia { Id = 12, Enunciado = "¿Qué piezas valen aproximadamente 3 puntos (pieza menor)?", OpcionA = "1. Torre y Peón", OpcionB = "2. Alfil y Caballo", OpcionC = "3. Dama y Rey", OpcionD = "4. Torre y Alfil", OpcionCorrecta = 2, Explicacion = "El Alfil y el Caballo son considerados piezas menores." },
                new PreguntaTrivia { Id = 13, Enunciado = "¿Cómo se denomina la jugada que ataca dos piezas a la vez?", OpcionA = "1. Clavada", OpcionB = "2. Doblete o Horquilla", OpcionC = "3. Enfilada", OpcionD = "4. Tablas", OpcionCorrecta = 2, Explicacion = "El doblete ataca dos objetivos en forma simultánea." },
                new PreguntaTrivia { Id = 14, Enunciado = "¿Qué ocurre cuando el Rey no está en jaque pero no tiene jugadas legales?", OpcionA = "1. Gana el rival", OpcionB = "2. Tablas por Ahogado", OpcionC = "3. Pierde 5 puntos", OpcionD = "4. Reinicia el turno", OpcionCorrecta = 2, Explicacion = "Es ahogado (pat) y resulta en empate (tablas)." },
                new PreguntaTrivia { Id = 15, Enunciado = "¿Cuánto vale una Torre en el sistema numérico estándar?", OpcionA = "1. 3 puntos", OpcionB = "2. 5 puntos", OpcionC = "3. 7 puntos", OpcionD = "4. 9 puntos", OpcionCorrecta = 2, Explicacion = "La Torre es una pieza mayor con valor de 5 puntos." }
            })
        };

        // Asignar grupo actual
        var grupo = grupos[_grupoActualIndex % grupos.Count];
        TituloEncuesta = grupo.Titulo;
        _preguntasActivas = grupo.Preguntas;

        // Avanzar el índice rotativo para la próxima visita
        _grupoActualIndex = (_grupoActualIndex + 1) % grupos.Count;

        _indicePreguntaActual = 0;
        CargarSiguientePregunta();
    }

    private void CargarSiguientePregunta()
    {
        if (_indicePreguntaActual < _preguntasActivas.Count)
        {
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