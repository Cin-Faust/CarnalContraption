namespace CarnalContraption.Application.Services.PiShock;

public record Settings
{
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}