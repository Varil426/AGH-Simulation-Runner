using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public abstract class ValueOfCollection
{
    protected ValueOfCollection(string value, int index)
    {
        Value = value;
        Index = index;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    [Required]
    public int Index { get; set; }

    [Required]
    public string Value { get; set; }
}
