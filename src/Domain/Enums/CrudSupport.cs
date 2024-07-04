namespace Domain;

[Flags]
public enum CrudSupport
{
    None = 0,
    Create = 1,
    Read = 2,
    Update = 4,
    Delete = 8,

    All = 15
}