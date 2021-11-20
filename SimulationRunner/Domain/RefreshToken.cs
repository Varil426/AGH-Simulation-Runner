namespace Domain;

public class RefreshToken
{
	[Required]
	public Guid Id { get; set; }

	[Required]
	public virtual User User { get; set; } = null!;

	[Required]
	public string Token { get; set; } = null!;

	public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7);

	public bool IsExpired => DateTime.UtcNow >= Expires;

	public DateTime? Revoked { get; set; }

	public bool IsActive => Revoked == null && !IsExpired;
}