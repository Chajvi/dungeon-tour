namespace DungeonTour.Models;

public sealed class EncounterOption
{
    public EncounterType Type { get; }
    public string Description { get; }

    public string? SpecialComboId { get; }

    public EncounterOption(EncounterType type, string description, string? specialComboId = null)
    {
        Type = type;
        Description = description;
        SpecialComboId = specialComboId;
    }
}
