using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Repositories;
public interface IProductRepository
{
    // CRUD Básico
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetActivesAsync();
    Task CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id); // Eliminación lógica
    Task<bool> ExistsAsync(int id);

    Task<Product?> GetByBarcodeAsync(string barCode);
    Task<IEnumerable<Product>> SearchByNameAsync(string name);

    Task<bool> BarCodeExistsAsync(string barcode);
}

