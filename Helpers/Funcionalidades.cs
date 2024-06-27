using Compras.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;



namespace Compras.Helpers
{
    public class Funcionalidades
    {
        public readonly MarsystemsDemoDbContext _context;

        public Funcionalidades(MarsystemsDemoDbContext context)
        {
            _context = context;
        }

        public bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Iduser == id);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            var inputHash = HashPassword(inputPassword);
            return inputHash == hashedPassword;
        }
    }
}
