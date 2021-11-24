using Application.Interfaces;
using Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackendAPI.Security;

public class JwtGenerator : IJwtGenerator
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtGenerator(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
		_issuer = config["JWT:Issuer"];
		_audience = config["JWT:Audience"];
	}

    public string CreateToken(User user)
    {
		var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.NameId, user.UserName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email)
			};

		var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddMinutes(15).AddDays(100), // TODO Remove days
			SigningCredentials = credentials,
			Issuer = _issuer,
			Audience = _audience,
		};

		//var token = new JwtSecurityToken(issuer: _issuer, audience: _audience, claims: claims, expires: DateTime.Now.AddMinutes(15), notBefore: DateTime.Now, signingCredentials: credentials);

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

    public RefreshToken GenerateRefreshToken()
    {
		var randomNumber = new byte[32];
		using var randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(randomNumber);
		return new RefreshToken
		{
			Token = Convert.ToBase64String(randomNumber)
		};
	}
}