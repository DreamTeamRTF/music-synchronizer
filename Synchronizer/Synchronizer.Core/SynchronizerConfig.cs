namespace Synchronizer.Core;

public class SynchronizerConfig
{
    public string DbConnection { get; init; }
    public string MigrationsAssemly { get; init; }
}