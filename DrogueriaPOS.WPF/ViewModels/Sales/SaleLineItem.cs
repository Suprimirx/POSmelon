using CommunityToolkit.Mvvm.ComponentModel;
using DrogueriaPOS.Domain.Entities;

namespace DrogueriaPOS.WPF.ViewModels.Sales;
// Modelo de presentación para una línea de venta en la UI.
// No es una entidad de dominio, solo existe para la pantalla de ventas.
public partial class SaleLineItem : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubTotal))]
    [NotifyPropertyChangedFor(nameof(Base))]
    [NotifyPropertyChangedFor(nameof(TotalIVA))]
    [NotifyPropertyChangedFor(nameof(Total))]
    private int _amount = 1;

    public int ProductId { get; }
    public string BarCode { get; }
    public string ProductName { get; }
    public decimal UnitPrice { get; }
    public decimal IvaPercentage { get; }
    public int AvailableStock { get; }

    public decimal SubTotal => Amount * UnitPrice;
    public decimal Base => IvaPercentage == 0 ? 0 : Math.Round(Amount * (UnitPrice / (1 + (IvaPercentage / 100m))), 2);
    public decimal TotalIVA => IvaPercentage == 0 ? 0 : SubTotal - Base;
    public decimal Total => SubTotal;

    public SaleLineItem(Product product)
    {
        ProductId = product.Id;
        BarCode = product.BarCode;
        ProductName = product.BrandName;
        UnitPrice = product.SalePrice;
        IvaPercentage = product.IVAPercentage;
        AvailableStock = product.Stock;
    }
}
