    using DrogueriaPOS.Application.Common;
using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.Application.Services;

public class InventoryService
{
    private readonly IProductRepository _productRepository;

    public InventoryService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<Product>> GetProductByIdAsync(int id)
    {

        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return Result<Product>.Failure("Producto no encontrado.");

        return Result<Product>.Success(product);
    }

    public async Task<Result<IEnumerable<Product>>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return Result<IEnumerable<Product>>.Success(products);
    }

    public async Task<Result<IEnumerable<Product>>> GetActivesProductsAsync()
    {
        var products = await _productRepository.GetActivesAsync();

        return Result<IEnumerable<Product>>.Success(products);

    }

    public async Task<Result<Product>> CreateProductAsync(
        string barCode,
        string brandName,
        string genericName,
        string concentration,
        string presentation,
        string invimaRegistration,
        decimal salePrice,
        decimal ivaPercentage,
        int initialStock,
        string description = "")
    {

        if (await _productRepository.BarCodeExistsAsync(barCode))
            return Result<Product>.Failure("El código de barras ya existe");

        // Usar constructor público
        var product = new Product(
            barcode: barCode,
            brandName: brandName,
            salePrice: salePrice,
            IvaPercentage: ivaPercentage,
            initialStock: initialStock,
            description: description,
            genericName: genericName,
            concentration: concentration,
            presentation: presentation,
            invimaRegistration: invimaRegistration
        );

        await _productRepository.CreateAsync(product);
        //product.Id = id;

        return Result<Product>.Success(product);
    }

    public async Task<Result<Product>> UpdateProductAsync(
        int id,
        string barCode,
        string brandName,
        string genericName,
        string concentration,
        string presentation,
        string invimaRegistration,
        decimal salePrice,
        int stock,
        decimal ivaPercentage,
        bool isActive,
        string description = "")
    {
        // Verificar que el producto existe
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return Result<Product>.Failure("Producto no encontrado");

        // Verificar que el nuevo código de barras no lo tenga otro producto
        if (product.BarCode != barCode && await _productRepository.BarCodeExistsAsync(barCode))
            return Result<Product>.Failure("El código de barras ya está en uso por otro producto");

        if (stock > product.Stock)
            product.IncreaseStock(stock - product.Stock);
        else if (stock < product.Stock)
            product.DecreaseStock(product.Stock - stock);

        // La entidad aplica sus propias reglas
        product.UpdateName(brandName, genericName);
        product.UpdatePrice(salePrice);
        product.UpdateDetails(description, concentration, presentation, invimaRegistration);

        if (isActive) 
            product.Reactivate();
        else 
            product.Deactivate();

        await _productRepository.UpdateAsync(product);
        return Result<Product>.Success(product);
    }

    public async Task<Result> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return Result.Failure("Producto no encontrado");

        product.Deactivate();
        await _productRepository.UpdateAsync(product);
        return Result.Success();
    }

    public async Task<Result<Product>> GetProductByBarCodeAsync(string barCode)
    {
        var product = await _productRepository.GetByBarcodeAsync(barCode);

        if (product == null)
            return Result<Product>.Failure("Producto no encontrado");

        return Result<Product>.Success(product);
    }

    public async Task<Result<IEnumerable<Product>>> GetProductByNameAsync(string name)
    {

        var products = await _productRepository.SearchByNameAsync(name);
        return Result<IEnumerable<Product>>.Success(products);

    }
}