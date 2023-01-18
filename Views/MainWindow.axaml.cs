using Avalonia.Controls;
using Avalonia.Interactivity;
using I18NextEditor.ViewModels;
using System.Text.Json;

namespace I18NextEditor.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public async void OpenFileButtonPressed(object sender, RoutedEventArgs eventArgs)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        var storageFiles = await StorageProvider.OpenFilePickerAsync(new());
        if (!storageFiles.Any())
            return;
        storageFiles[0].TryGetUri(out var uri);
        if (uri is not null)
            viewModel.Path = uri.AbsolutePath;
        await LoadTheFile();
    }

    async Task LoadTheFile()
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        viewModel.Data.Clear();
        using var stream = File.OpenRead(viewModel.Path);
        var des = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream)!;
        foreach (var kvp in des!.OrderBy(x => x.Key, StringComparer.InvariantCulture))
            viewModel.Data.Add(new Entry(kvp.Key, kvp.Value));
    }

    public async void SaveButtonPressed(object sender, RoutedEventArgs eventArgs)
    {
        var viewModel = (MainWindowViewModel)DataContext!;
        using (var stream = File.Open(viewModel.Path, FileMode.Create))
        {
            var dict = viewModel.Data.OrderBy(entry => entry.Key, StringComparer.InvariantCulture).ToDictionary(x => x.Key, x => x.Value);
            await JsonSerializer.SerializeAsync(stream, dict, new JsonSerializerOptions() { WriteIndented = true });
        }
        await LoadTheFile();
    }
}