using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Repositories
{
    public class UserRepository
    {
        private readonly DataBaseContext _context;

        public UserRepository(DataBaseContext context)
        {
            _context = context;
        }

        public User Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }
    }
    public interface IUserRepository
    {
        User Create(User user);
        User GetById(int id);
        IEnumerable<User> GetAll();
        // Other CRUD methods...
    }
}
