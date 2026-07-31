
namespace DrogueriaPOS.Domain.Entities;
/// <summary>
/// Representa una línea de detalle dentro de una factura
/// </summary>
public class InvoiceLine
{
    public int Id { get; private set; }
    public int InvoiceId { get; private set; }
    public int ProductId { get; private set; }
    // Datos duplicados del producto (para historial inmutable)
    public string BarCode { get; private set; }
    public string ProductName { get; private set; }
    public int Amount { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal IvaPercentage { get; private set; }
    // Subtotal sin IVA
    public decimal SubTotal { get; private set; }
    public decimal Base { get; private set; }
    // Monto del IVA
    public decimal TotalIVA { get; private set; }
   

    // Factura a la que pertenece este detalle
    public Invoice Invoice { get; private set; }

    // Producto vendido
    public Product Product { get; private set; }


    public InvoiceLine(Product product, int amount)
    {
        // Validar producto
        if (product == null)
            throw new ArgumentNullException(nameof(product), "Producto es requerido");

        // Validar cantidad
        if (amount <= 0)
            throw new ArgumentException("Cantidad debe ser mayor que cero", nameof(amount));

        // Validar que el producto tenga stock
        if (!product.HasAvailableStock(amount))
            throw new InvalidOperationException(
                $"Stock insuficiente para {product.BrandName}");

        ProductId = product.Id;
        BarCode = product.BarCode;
        ProductName = product.BrandName;
        Amount = amount;
        UnitPrice = product.SalePrice;
        IvaPercentage = product.IVAPercentage;

        CalculateTotals();
    }

    // Constructor privado para Entity Framework
    private InvoiceLine() { }


    private void CalculateTotals()
    {
        SubTotal= Amount * UnitPrice;
        if (IvaPercentage == 0)
        {
            Base = 0;
            TotalIVA = 0;
            return;
        }

        Base = Math.Round(Amount * (UnitPrice / (1 + IvaPercentage / 100m)), 2);
        TotalIVA = SubTotal - Base;
    }
}


