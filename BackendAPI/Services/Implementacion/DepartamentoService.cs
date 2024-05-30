using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;
using BackendAPI.Services.Contrato;

namespace BackendAPI.Services.Implementacion
{
    public class DepartamentoService : IDepartamentoService
    {
        private DbempresaContext _dbContext;

        public DepartamentoService(DbempresaContext dbContext)
        {
            _dbContext = dbContext;
            
        }

        public async Task<List<Departamento>> GetList()
        {
            try
            {
                List<Departamento> lista = new List<Departamento>();
                lista = await _dbContext.Departamentos.ToListAsync();

                return lista;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
