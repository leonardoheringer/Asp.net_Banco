using DigiBank.Data;
using DigiBank.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigiBank.Services
{
    public class ClienteService
    {
        private readonly DigiBankContext _context;

        public ClienteService(DigiBankContext context)
        {
            _context = context;
        }

        // Implemente os métodos do serviço aqui
    }
}