using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public abstract class ValueOfCollection
{
    protected ValueOfCollection(string value)
    {
        Value = value;
    }

    // TODO Remove setters from all Id - move them to constructors
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    public int Index { get; set; }

    [Required]
    public string Value { get; set; }
}
