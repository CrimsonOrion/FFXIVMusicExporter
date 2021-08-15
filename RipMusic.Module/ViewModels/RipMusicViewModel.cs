using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using FFXIVMusicExporter.Core.Events;
using FFXIVMusicExporter.Core.Music;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace RipMusic.Module.ViewModels;

public class RipMusicViewModel : BindableBase
{

    private ObservableCollection<string> _updateText = new();
    private readonly IRipBGMService _ripBGMService;
    private readonly IOggToWavService _oggToWavService;
    private readonly IWavToMP3Service _wavToMP3Service;
    private readonly IEventAggregator _eventAggregator;

    public ObservableCollection<string> UpdateText
    {
        get => _updateText;
        set => SetProperty(ref _updateText, value);
    }

    public DelegateCommand RipMusicCommand => new(RipMusic);
    public DelegateCommand OggToWavCommand => new(ConvertOggToWav);
    public DelegateCommand WavToMP3Command => new(ConvertWavToMP3);

    public RipMusicViewModel(IEventAggregator eventAggregator, IRipBGMService ripBGMService, IOggToWavService oggToWavService, IWavToMP3Service wavToMP3Service)
    {
        _eventAggregator = eventAggregator;
        _eventAggregator.GetEvent<RipBGMEvent>().Subscribe(PublishMessage);

        _ripBGMService = ripBGMService;
        _oggToWavService = oggToWavService;
        _wavToMP3Service = wavToMP3Service;
    }

    private async void RipMusic() => await _ripBGMService.GetFilesAsync(new System.Threading.CancellationToken());
    private async void ConvertOggToWav() => await _oggToWavService.ConvertToWavAsync(Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "OGG")), new System.Threading.CancellationToken());
    private async void ConvertWavToMP3() => await _wavToMP3Service.ConvertToMP3Async(Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "WAV")), new System.Threading.CancellationToken());

    private void PublishMessage(string message)
    {
        UpdateText.Add(message);
        UpdateText.Move(UpdateText.Count - 1, 0);
    }
}