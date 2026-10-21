using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JaqueAndo.Models;
using JaqueAndo.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace JaqueAndo.ViewModels;

public partial class RankingViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty] private Usuario _usuarioActual;
    [ObservableProperty] private ObservableCollection<RankingItemDto> _ranking = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HayMensaje))]
    private string _mensaje = string.Empty;

    public bool HayMensaje => !string.IsNullOrEmpty(Mensaje);

    public RankingViewModel(MainViewModel mainViewModel, Usuario usuario)
    {
        _mainViewModel = mainViewModel;
        _usuarioActual = usuario;
        _ = CargarRankingAsync();
    }

    [RelayCommand]
    private async Task CargarRankingAsync()
    {
        Mensaje = "Cargando ranking...";

        var (lista, error) = await ApiService.ObtenerRankingAsync();
        if (lista == null)
        {
            Mensaje = error ?? "No se pudo cargar el ranking.";
            return;
        }

        for (int i = 0; i < lista.Count; i++)
            lista[i].Puesto = i + 1;

        Ranking = new ObservableCollection<RankingItemDto>(lista);
        Mensaje = lista.Count == 0 ? "Todavía no hay partidas de trivia registradas." : string.Empty;
    }

    [RelayCommand]
    private void VolverMenu()
    {
        _mainViewModel.NavigateTo(new MenuPrincipalViewModel(_mainViewModel, UsuarioActual));
    }
}