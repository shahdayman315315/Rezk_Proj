using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rezk_Proj.Helpers;
using Rezk_Proj.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rezk_Proj.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly JWT _jwt;
        public AuthService(UserManager<IdentityUser> userManager, ApplicationDbContext context,IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _context = context;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel() { Message = "Email already Exists." };

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new AuthModel() { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
            }
            if (model.Role.Equals("Employer", StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(user, "Employer");
                await _context.Employers.AddAsync(new Employer
                {
                    Name = model.UserName,
                    NationalId = model.NationalId,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    LocationString = model.LocationString,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude
                });
                await _context.SaveChangesAsync();
            }
            else { 
                await _userManager.AddToRoleAsync(user, "Applicant");
                await _context.Applicants.AddAsync(new Applicant
                {
                    Name = model.UserName,
                    NationalId = model.NationalId,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    LocationString = model.LocationString,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude
                });
                await _context.SaveChangesAsync();
            }
            var jwtsecuritytoken = await CreateJwtToken(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                UserName = user.UserName,
                Role = model.Role,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtsecuritytoken),
                ExpirationDate = jwtsecuritytoken.ValidTo
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            IList<string> rolelist = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = user.UserName;
            authModel.ExpirationDate = jwtSecurityToken.ValidTo;
            authModel.Role = rolelist.FirstOrDefault();

            return authModel;
        }

    }
}
