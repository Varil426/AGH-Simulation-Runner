using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public abstract class ValueOfCollection
{
    protected ValueOfCollection(string value, int index)
    {
        Value = value;
        Index = index;
    }

    // TODO Remove setters from all Id - move them to constructors
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    [Required]
    public int Index { get; set; }

    [Required]
    public string Value { get; set; }
}
