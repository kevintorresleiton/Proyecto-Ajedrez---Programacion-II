using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Data;
using JaqueAndo.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace JaqueAndo.ViewModels;

public partial class BaseDatosViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private Usuario _usuarioActual;

    [ObservableProperty]
    private ObservableCollection<Jugador> _jugadores = new();

    [ObservableProperty]
    private Jugador? _jugadorSeleccionado;

    [ObservableProperty]
    private string _filtroTexto = string.Empty;

    // Campos del Formulario CRUD
    [ObservableProperty] private string _nombreForm = string.Empty;
    [ObservableProperty] private string _apellidoForm = string.Empty;
    [ObservableProperty] private string _idFideForm = "N/A";
    [ObservableProperty] private string _sexoForm = "M";
    [ObservableProperty] private bool _esSocioForm;
    [ObservableProperty] private string _clubForm = "Libre";

    public BaseDatosViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;
        CargarJugadores();
    }

    [RelayCommand]
    private void CargarJugadores()
    {
        using var db = new AppDbContext();
        var lista = db.Jugadores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(FiltroTexto))
        {
            lista = lista.Where(j => j.Nombre.ToLower().Contains(FiltroTexto.ToLower()) || j.Apellido.ToLower().Contains(FiltroTexto.ToLower()));
        }

        Jugadores = new ObservableCollection<Jugador>(lista.ToList());
    }

    [RelayCommand]
    private void GuardarJugador()
    {
        if (string.IsNullOrWhiteSpace(NombreForm) || string.IsNullOrWhiteSpace(ApellidoForm)) return;

        using var db = new AppDbContext();

        if (JugadorSeleccionado == null)
        {
            // Crear Nuevo
            var nuevo = new Jugador
            {
                Nombre = NombreForm,
                Apellido = ApellidoForm,
                IdFide = IdFideForm,
                Sexo = SexoForm,
                EsSocio = EsSocioForm,
                Club = ClubForm,
                FechaNacimiento = DateTime.Now.AddYears(-20)
            };
            db.Jugadores.Add(nuevo);
        }
        else
        {
            // Modificar Existente
            var jug = db.Jugadores.Find(JugadorSeleccionado.Id);
            if (jug != null)
            {
                jug.Nombre = NombreForm;
                jug.Apellido = ApellidoForm;
                jug.IdFide = IdFideForm;
                jug.Sexo = SexoForm;
                jug.EsSocio = EsSocioForm;
                jug.Club = ClubForm;
            }
        }

        db.SaveChanges();
        LimpiarFormulario();
        CargarJugadores();
    }

    [RelayCommand]
    private void EliminarJugador()
    {
        if (JugadorSeleccionado == null) return;

        using var db = new AppDbContext();
        var jug = db.Jugadores.Find(JugadorSeleccionado.Id);
        if (jug != null)
        {
            db.Jugadores.Remove(jug);
            db.SaveChanges();
        }

        LimpiarFormulario();
        CargarJugadores();
    }

    [RelayCommand]
    private void CargarFormularioParaEditar()
    {
        if (JugadorSeleccionado == null) return;

        NombreForm = JugadorSeleccionado.Nombre;
        ApellidoForm = JugadorSeleccionado.Apellido;
        IdFideForm = JugadorSeleccionado.IdFide;
        SexoForm = JugadorSeleccionado.Sexo;
        EsSocioForm = JugadorSeleccionado.EsSocio;
        ClubForm = JugadorSeleccionado.Club;
    }

    [RelayCommand]
    private void LimpiarFormulario()
    {
        JugadorSeleccionado = null;
        NombreForm = string.Empty;
        ApellidoForm = string.Empty;
        IdFideForm = "N/A";
        SexoForm = "M";
        EsSocioForm = false;
        ClubForm = "Libre";
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}