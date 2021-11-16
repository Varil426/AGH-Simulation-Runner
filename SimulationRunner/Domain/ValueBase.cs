using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public abstract class ValueBase
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    public string? Value { get; set; }
}