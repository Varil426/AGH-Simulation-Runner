using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class SimulationRunAttempt
{
    public SimulationRunAttempt()
    {
        ResultValues = new Collection<ResultValue>();
        ParamValues = new Collection<ParamValue>();
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public Guid Id { get; set; }

    [Required]
    public virtual Simulation Simulation { get; set; } = null!;

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public int AttemptNumer { get; set; }

    public virtual ICollection<ResultValue> ResultValues { get; set; }

    public virtual ICollection<ParamValue> ParamValues { get; set; }
}