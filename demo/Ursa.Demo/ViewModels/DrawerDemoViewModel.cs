using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Common;
using Ursa.Controls;
using Ursa.Controls.Options;

namespace Ursa.Demo.ViewModels;

public partial class DrawerDemoViewModel: ObservableObject
{
    public ICommand OpenDrawerCommand { get; set; }
    
    [ObservableProperty] private Position _selectedPosition;
    

    public DrawerDemoViewModel()
    {
        OpenDrawerCommand = new AsyncRelayCommand(OpenDrawer);
    }

    private async Task OpenDrawer()
    {
        await Drawer.ShowCustom<Calendar, string, bool>("Hello World", new CustomDrawerOptions() { Position = SelectedPosition, MinWidth = 400 });
    }
}