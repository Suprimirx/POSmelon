using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DrogueriaPOS.Infrastructure.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .OrderBy(p => p.BrandName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActivesAsync()
    {
        try
        {
            return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.BrandName)
            .ToListAsync();
        }
        catch (Exception ex)
        {
            // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
            Console.WriteLine($"Error fetching active products: {ex.Message}");
            return new List<Product>(); // Return an empty list in case of error
        }

    }

    public async Task CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<Product> GetByBarcodeAsync(string barCode)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.BarCode == barCode);
    }

    public async Task<bool> BarCodeExistsAsync(string barCode)
    {
        return await _context.Products.AnyAsync(p => p.BarCode == barCode);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return await GetActivesAsync();

        var trimmedName = name.Trim();
        return await _context.Products
            .Where(p => p.IsActive && (
                EF.Functions.Like(p.BrandName, $"%{trimmedName}%") ||
                EF.Functions.Like(p.GenericName, $"%{trimmedName}%")
            ))
            .OrderBy(p => p.BrandName)
            .ToListAsync();
    }
}

