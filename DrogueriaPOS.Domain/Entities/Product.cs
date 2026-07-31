using DrogueriaPOS.Domain.Exceptions;

namespace DrogueriaPOS.Domain.Entities;

public class Product
{
    //private int? _laboratoryId;
    public int Id { get; private set; } // Internal so repository can assign ID.
    public string BarCode { get; private set; }
    public string BrandName { get; private set; }
    public string GenericName { get; private set; }
    public string Concentration { get; private set; }
    public string Presentation { get; private set; }
    public string InvimaRegistration { get; private set; }
    public string Description { get; private set; }
    public decimal SalePrice { get; private set; }
    public int Stock { get; private set; }
    public decimal IVAPercentage { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    //public int? LaboratoryId { get => _laboratoryId; private set => _laboratoryId = value; }


    public Product(
        string barcode,
        string brandName,
        decimal salePrice,
        decimal IvaPercentage,
        int initialStock,
        string description = "",
        string genericName = "",
        string concentration = "",
        string presentation = "",
        string invimaRegistration = "")
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Código de barras es requerido", nameof(barcode));

        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("Nombre del producto es requerido", nameof(brandName));

        if (salePrice <= 0)
            throw new ArgumentException("Precio debe ser mayor que cero", nameof(salePrice));

        if (IvaPercentage < 0 || IvaPercentage > 100)
            throw new ArgumentException("IVA debe ser entre 0 y 100", nameof(IvaPercentage));


        BarCode = barcode.Trim();
        BrandName = brandName.Trim();
        Description = description?.Trim() ?? "";
        GenericName = genericName?.Trim() ?? "";
        Concentration = concentration?.Trim() ?? "";
        Presentation = presentation?.Trim() ?? "";
        InvimaRegistration = invimaRegistration?.Trim() ?? "";
        SalePrice = salePrice;
        IVAPercentage = IvaPercentage;
        Stock = initialStock;
        IsActive = true;
        CreatedAt = DateTime.Now;
        UpdatedAt = null;

    }

    // Constructor para Entity Framework (Sin parámetros)
    private Product() { }

    public void UpdateBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Barcode is required", nameof(barcode));

        BarCode = barcode.Trim();
        UpdatedAt = DateTime.Now;
    }

    public void UpdateName(string brandName, string genericName = null)
    {
        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("Product name is required", nameof(brandName));

        BrandName = brandName.Trim();

        if (genericName != null)
        {
            GenericName = genericName.Trim();
        }

        UpdatedAt = DateTime.Now;
    }
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Precio debe ser mayor que cero", nameof(newPrice));

        SalePrice = newPrice;
        UpdatedAt = DateTime.Now;
    }

    public void UpdateDetails(
            string description = null,
            string concentration = null,
            string presentation = null,
            string invimaRegistration = null)
    {
        if (description != null)
            Description = description.Trim();
        if (concentration != null)
            Concentration = concentration.Trim();
        if (presentation != null)
            Presentation = presentation.Trim();
        if (invimaRegistration != null)
            InvimaRegistration = invimaRegistration.Trim();

        UpdatedAt = DateTime.Now;
    }

    public bool HasAvailableStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        return Stock >= quantity && IsActive;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (!IsActive)
            throw new InvalidOperationException("Cannot decrease stock of an inactive product");

        if (quantity > Stock)
            throw new StockInsuficienteException(BrandName, Stock, quantity);

        Stock -= quantity;
        UpdatedAt = DateTime.Now;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Stock += quantity;
    }

    public decimal CalculateIVA()
    {
        return SalePrice * (IVAPercentage / 100);
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.Now;
    }

    public void Reactivate()
    {
        IsActive = true;
        UpdatedAt = DateTime.Now;
    }
}
