using BackendAPI.Models;

namespace BackendAPI.Services.Contrato
{
    public interface IDepartamentoService
    {
        Task<List<Departamento>> GetList();
    }
}
