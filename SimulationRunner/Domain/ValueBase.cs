using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public abstract class ValueBase<TCollectionType> where TCollectionType :  ValueOfCollection
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    public string? Value { get; set; }

    /// <summary>
    /// If value is a collection, then this list contains its values.
    /// </summary>
    public virtual ICollection<TCollectionType> Values { get; set; } = new Collection<TCollectionType>();

    public abstract bool IsCollection { get; }
}