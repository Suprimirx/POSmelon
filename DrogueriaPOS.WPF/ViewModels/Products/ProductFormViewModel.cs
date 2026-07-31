using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.WPF.ViewModels.Base;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.Application.Services;

namespace DrogueriaPOS.WPF.ViewModels.Products;
public partial class ProductFormViewModel : BaseViewModel
{
    private readonly InventoryService _inventoryService;
    private readonly INavigationService _navigationService;

    private int? _productId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _barCode;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _brandName;

    [ObservableProperty]
    private string _genericName;

    [ObservableProperty]
    private string _concentration;

    [ObservableProperty]
    private string _presentation;

    [ObservableProperty]
    private string _invimaRegistration;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private decimal _salePrice;

    [ObservableProperty]
    private int _stock;

    [ObservableProperty]
    private decimal _ivaPercentage;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private bool _isEditMode;

    public ProductFormViewModel(
        IDialogService dialogService,
        InventoryService inventoryService,
        INavigationService navigationService)
        : base(dialogService)
    {
        _inventoryService = inventoryService;
        _navigationService = navigationService;

        Title = "Nuevo Producto";
        IsActive = true;
        IvaPercentage = 0; // 0% default for medications
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is int productId)
        {
            _productId = productId;
            IsEditMode = true;
            Title = "Editar Producto";
            await LoadProductAsync(productId);
        }
        else
        {
            IsEditMode = false;
            Title = "Nuevo Producto";
        }
    }

    private async Task LoadProductAsync(int id)
    {
        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _inventoryService.GetProductByIdAsync(id);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error loading product");
                Cancel();
                return;
            }

            var product = result.Data;

            BarCode = product.BarCode;
            BrandName = product.BrandName;
            GenericName = product.GenericName;
            Concentration = product.Concentration;
            Presentation = product.Presentation;
            InvimaRegistration = product.InvimaRegistration;
            Description = product.Description;
            SalePrice = product.SalePrice;
            Stock = product.Stock;
            IvaPercentage = product.IVAPercentage;
            IsActive = product.IsActive;

        }, "Cargando producto...");
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(BarCode) &&
               !string.IsNullOrWhiteSpace(BrandName) &&
               SalePrice > 0;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            if (IsEditMode)
                await UpdateProductAsync();
            else
                await CreateProductAsync();

        }, IsEditMode ? "Actualizando producto..." : "Creando producto...");
    }

    private async Task CreateProductAsync()
    {
        var result = await _inventoryService.CreateProductAsync(
            barCode: BarCode.Trim(),
            brandName: BrandName.Trim(),
            genericName: GenericName?.Trim(),
            concentration: Concentration?.Trim(),
            presentation: Presentation?.Trim(),
            invimaRegistration: InvimaRegistration?.Trim(),
            salePrice: SalePrice,
            ivaPercentage: IvaPercentage,
            initialStock: Stock,
            description: Description?.Trim()
        );

        if (!result.IsSuccess)
        {
            ShowError(result.ErrorMessage, "Error creando producto");
            return;
        }

        ShowSuccess($"Producto '{BrandName}' creado exitosamente");
        _navigationService.GoBack();
    }

    private async Task UpdateProductAsync()
    {
        var result = await _inventoryService.UpdateProductAsync(
            id: _productId.Value,
            barCode: BarCode.Trim(),
            brandName: BrandName.Trim(),
            genericName: GenericName?.Trim(),
            concentration: Concentration?.Trim(),
            presentation: Presentation?.Trim(),
            invimaRegistration: InvimaRegistration?.Trim(),
            salePrice: SalePrice,
            stock: Stock,
            ivaPercentage: IvaPercentage,
            isActive: IsActive,
            description: Description?.Trim()
        );

        if (!result.IsSuccess)
        {
            ShowError(result.ErrorMessage, "Error actualizando producto");
            return;
        }

        ShowSuccess($"Producto '{BrandName}' actualizado exitosamente");
        _navigationService.GoBack();
    }

    [RelayCommand]
    private void Cancel()
    {
        var confirmed = ShowConfirmation(
            "¿Está seguro de cancelar? Los cambios no guardados se perderán.",
            "Confirmar cancelación");

        if (confirmed)
            _navigationService.GoBack();
    }
}
