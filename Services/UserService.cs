namespace WebApi.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Data;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Repositories;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
}

public class UserService : IUserService
{
    // users hardcoded for simplicity, store in a db with hashed passwords in production applications
    private List<User> _users = new List<User>
    {
        new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
    };

    private readonly AppSettings _appSettings;
    private UserRepository _userRepository;


    public UserService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public IEnumerable<User> GetAll()
    {
        return _users;
    }

    public User GetById(int id)
    {
        return _users.FirstOrDefault(x => x.Id == id);
    }

    // helper methods

    private string generateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        //var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

        var options = new DbContextOptionsBuilder<DataBaseContext>()
              .UseSqlServer("Data Source=192.82.95.103,1456;Initial Catalog=CoreApiDb;User ID=sa;Password=0nTime@123;")
              .Options;

        var context = new DataBaseContext(options);

        var _users = context.Users.ToList();

        var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

        // return null if user not found
        if (user == null) return null;

        // authentication successful so generate jwt token
        var token = generateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }
    public User CreateUser(User model)
    {
        // Create a new User instance from the input model
        var user = new User
        {
            Username = model.Username,
            Password = model.Password,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        // Save the user using the repository
        var createdUser = _userRepository.Create(user);

        return createdUser;
    }
}