using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using JaqueAndo.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JaqueAndo.ViewModels;

public partial class BaseDatosViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty] private Usuario _usuarioActual;
    [ObservableProperty] private ObservableCollection<JugadorDto> _jugadores = new();
    [ObservableProperty] private JugadorDto? _jugadorSeleccionado;
    [ObservableProperty] private string _filtroTexto = string.Empty;
    [ObservableProperty] private string _mensaje = string.Empty;

    // Lista de clubes para el desplegable
    [ObservableProperty] private ObservableCollection<ClubDto> _clubes = new();
    [ObservableProperty] private ClubDto? _clubSeleccionado;

    // Campos del formulario
    [ObservableProperty] private string _nombreForm = string.Empty;
    [ObservableProperty] private string _apellidoForm = string.Empty;
    [ObservableProperty] private string _idFideForm = "N/A";
    [ObservableProperty] private string _sexoForm = "M";
    [ObservableProperty] private bool _esSocioForm;

    public BaseDatosViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;
        _ = InicializarAsync();
    }

    private async Task InicializarAsync()
    {
        await CargarJugadoresAsync();

        var (clubes, error) = await ApiService.ObtenerClubesAsync();
        if (clubes != null)
            Clubes = new ObservableCollection<ClubDto>(clubes);
        else if (string.IsNullOrEmpty(Mensaje))
            Mensaje = error ?? "No se pudieron cargar los clubes.";
    }

    [RelayCommand]
    private async Task CargarJugadoresAsync()
    {
        Mensaje = string.Empty;

        var (lista, error) = await ApiService.ObtenerJugadoresAsync();
        if (lista == null)
        {
            Mensaje = error ?? "No se pudieron cargar los jugadores.";
            return;
        }

        var filtro = (FiltroTexto ?? string.Empty).Trim().ToLower();
        var filtrados = string.IsNullOrEmpty(filtro)
            ? lista
            : lista.Where(j => j.Nombre.ToLower().Contains(filtro) ||
                               j.Apellido.ToLower().Contains(filtro)).ToList();

        Jugadores = new ObservableCollection<JugadorDto>(filtrados);
    }

    [RelayCommand]
    private async Task GuardarJugadorAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreForm) || string.IsNullOrWhiteSpace(ApellidoForm))
        {
            Mensaje = "Nombre y apellido son obligatorios.";
            return;
        }

        var esNuevo = JugadorSeleccionado == null;

        var dto = new JugadorDto
        {
            Id = JugadorSeleccionado?.Id ?? 0,
            Nombre = NombreForm.Trim(),
            Apellido = ApellidoForm.Trim(),
            IdFide = string.IsNullOrWhiteSpace(IdFideForm) ? "N/A" : IdFideForm.Trim(),
            Sexo = SexoForm,
            EsSocio = EsSocioForm,
            ClubId = ClubSeleccionado?.Id,
            FechaNacimiento = JugadorSeleccionado?.FechaNacimiento ?? DateTime.Now.AddYears(-20),
            EloEstimado = JugadorSeleccionado?.EloEstimado ?? 1200
        };

        var error = esNuevo
            ? await ApiService.CrearJugadorAsync(dto)
            : await ApiService.ActualizarJugadorAsync(dto);

        if (error != null)
        {
            Mensaje = error;
            return;
        }

        LimpiarFormulario();
        await CargarJugadoresAsync();
        Mensaje = esNuevo ? "Jugador creado correctamente." : "Jugador actualizado correctamente.";
    }

    [RelayCommand]
    private async Task EliminarJugadorAsync()
    {
        if (JugadorSeleccionado == null)
        {
            Mensaje = "Seleccioná un jugador de la tabla para eliminarlo.";
            return;
        }

        var error = await ApiService.EliminarJugadorAsync(JugadorSeleccionado.Id);
        if (error != null)
        {
            Mensaje = error;
            return;
        }

        LimpiarFormulario();
        await CargarJugadoresAsync();
        Mensaje = "Jugador eliminado.";
    }

    [RelayCommand]
    private void CargarFormularioParaEditar()
    {
        if (JugadorSeleccionado == null)
        {
            Mensaje = "Seleccioná un jugador de la tabla para editarlo.";
            return;
        }

        NombreForm = JugadorSeleccionado.Nombre;
        ApellidoForm = JugadorSeleccionado.Apellido;
        IdFideForm = JugadorSeleccionado.IdFide;
        SexoForm = JugadorSeleccionado.Sexo;
        EsSocioForm = JugadorSeleccionado.EsSocio;
        ClubSeleccionado = Clubes.FirstOrDefault(c => c.Id == JugadorSeleccionado.ClubId);
        Mensaje = string.Empty;
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
        ClubSeleccionado = null;
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}