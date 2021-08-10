using System.Threading.Tasks;

using FFXIVMusicExporter.Core;

using Prism.Commands;
using Prism.Mvvm;

namespace RipMusic.Module.ViewModels
{
    public class RipMusicViewModel : BindableBase
    {
        private readonly IRealm _realm;

        private string _updateText = string.Empty;
        public string UpdateText
        {
            get => _updateText;
            set => SetProperty(ref _updateText, value);
        }

        public DelegateCommand RipMusicCommand => new(RipMusic);

        public RipMusicViewModel(IRealm realm) => _realm = realm;

        private async void RipMusic() => UpdateText = await Task.Run(() => "Ripping Music...");
    }
}