using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Modeller.Parsers.Models;

namespace Modeller.NET.Tool.Commands;

/// <summary>
/// Lee's suggested changes to the enterprise object
/// </summary>
internal sealed class LeesBucket : IDisposable
{
    private readonly List<Builder> _processedBuilders;
    private readonly ObservableCollection<Builder> _builders;
    private Enterprise? _enterprise = null;
    private bool _disposed;
    
    public LeesBucket(IEnumerable<Builder> builders)
    {
        _builders = new(builders);
        _processedBuilders = new ();
        _builders.CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ProcessChanges();
    }

    public void Start()
    {
        if (_builders.Any())
            ProcessChanges();
    }

    public void Add(Builder builder) => _builders.Add(builder);
    
    private bool alreadyProcessing = false;

    public void Handled(Builder builder)
    {
        _processedBuilders.Add(builder);
        _builders.Remove(builder);
    }

    private void ProcessChanges()
    {
        if (alreadyProcessing) return;

        AnsiConsole.WriteLine("processing changes...");
        alreadyProcessing = true;
        
        var enterprise = _enterprise;
        while (_builders.Any())
        {
            var builder = _builders.FirstOrDefault(b => b.WhereAllDependenciesMet(_processedBuilders));
            if (builder is null)
            {
                //throw new InvalidOperationException("Circular dependency detected or missing dependency.");

                AnsiConsole.WriteLine("- No builder exists without dependencies");
                break; // wait for the next change as there are no more builders to process
            }
            
            if (builder is ProjectBuilder pb)
            {
                AnsiConsole.WriteLine("- Creating Enterprise");
                enterprise = pb.Process();

                Handled(builder);
            }
            else 
            {
                Debug.Assert(enterprise is not null);

                AnsiConsole.Write("- Adding {Type} result to Enterprise {Value}  ", builder.GetType().Name, builder.ToString());
                enterprise = builder.Process(enterprise);

                Handled(builder);
            }

            AnsiConsole.WriteLine(".  {Count} remaining", _builders.Count);
        }

        alreadyProcessing = false;
        AnsiConsole.WriteLine("processing completed... for now...");
        _enterprise = enterprise;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) _builders.CollectionChanged -= OnCollectionChanged;

        _disposed = true;
    }
}