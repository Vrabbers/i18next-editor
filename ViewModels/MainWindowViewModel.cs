using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace I18NextEditor.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    string _path = string.Empty;
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    string _newKey = string.Empty;
    public string NewKey
    {
        get => _newKey;
        set => this.RaiseAndSetIfChanged(ref _newKey, value);
    }

    Entry? _selectedItem;
    public Entry? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public string? SelectedString
    {
        get => SelectedItem?.Key;
        set
        {
            if (SelectedItem is null)
                return;
            var i = Data.IndexOf(SelectedItem);
            Data[i] = new(SelectedItem.Key, value ?? string.Empty);
        }
    }

    public ObservableCollection<Entry> Data { get; } = new();

    public void AddKey()
    {
        if (string.IsNullOrWhiteSpace(NewKey))
            return;

        var newItem = new Entry(NewKey.Trim().Replace(' ', '_'), string.Empty);
        Data.Add(newItem);
        SelectedItem = newItem;
        NewKey = string.Empty;
    }

    public MainWindowViewModel()
    {
        Delete = ReactiveCommand.Create(DeleteMethod, this.WhenAnyValue(me => me.SelectedItem).Select(item => item is not null));
    }

    public ReactiveCommand<Unit, Unit> Delete { get; }

    public void DeleteMethod()
    {
        if (SelectedItem is null)
            return;
        Data.Remove(SelectedItem);
        SelectedItem = null;
    }

}

public sealed class Entry : ReactiveObject
{
    readonly string _firstValue;
    string _key = string.Empty;
    public string Key 
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }
    string _value = string.Empty;
    public string Value 
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    readonly ObservableAsPropertyHelper<bool> _strChanged;
    public bool StrChanged => _strChanged.Value;

    public Entry(string key, string value)
    {
        Key = key;
        Value = value;
        _firstValue = value;
        _strChanged = this
            .WhenAnyValue(me => me.Value)
            .Select(str => str != _firstValue)
            .ToProperty(this, me => me.StrChanged);
    }
}
