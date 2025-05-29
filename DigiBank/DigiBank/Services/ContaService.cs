using DigiBank.Data;
using DigiBank.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigiBank.Services
{
    public class ContaService
    {
        private readonly DigiBankContext _context;

        public ContaService(DigiBankContext context)
        {
            _context = context;
        }

        // Implemente os métodos do serviço aqui
    }
}