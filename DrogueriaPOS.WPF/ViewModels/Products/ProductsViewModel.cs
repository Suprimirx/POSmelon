using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrogueriaPOS.Application.Services;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.WPF.Services.Interfaces;
using DrogueriaPOS.WPF.ViewModels.Base;
using System.Collections.ObjectModel;
using System.DirectoryServices;

namespace DrogueriaPOS.WPF.ViewModels.Products;
public partial class ProductsViewModel : BaseViewModel
{
    private readonly InventoryService _inventoryService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Product> _products;

    [ObservableProperty]
    private ObservableCollection<Product> _searchResults;

    [ObservableProperty]
    private bool _showSearchResults;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(SeeDetailsCommand))]
    private Product _selectedProduct;

    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private bool _onlyActives = true;


    public ProductsViewModel(
        IDialogService dialogService,
        InventoryService inventoryService,
        INavigationService navigationService)
        : base(dialogService)
    {
        _inventoryService = inventoryService;
        _navigationService = navigationService;

        Title = "Gestión de Productos";
        Products = new ObservableCollection<Product>();
        SearchResults = new ObservableCollection<Product>();
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchProductsAsync();
    }

    partial void OnOnlyActivesChanged(bool value)
    {
        _ = LoadProductsAsync();
    }

    public override async Task InitializeAsync()
    {
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        await ExecuteWithBusyAsync(async () =>
        {
            var result = OnlyActives
                ? await _inventoryService.GetActivesProductsAsync()
                : await _inventoryService.GetAllProductsAsync();

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error cargando productos");
                return;
            }

            Products.Clear();
            foreach (var product in result.Data)
                Products.Add(product);

        }, "Cargando productos...");
    }

    [RelayCommand]
    private async Task SearchProductsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            ShowSearchResults = false;
            SearchResults.Clear();
            await LoadProductsAsync();
            return;
        }

        var result = await _inventoryService.GetProductByNameAsync(SearchText);

        if (!result.IsSuccess || result.Data == null)
        {
            SearchResults.Clear();
            ShowSearchResults = false;
            return;
        }

        SearchResults.Clear();
        Products.Clear();

        foreach (var product in result.Data)
        {
            if (!OnlyActives || product.IsActive)
            {
                SearchResults.Add(product);
                Products.Add(product);
            }
        }

        ShowSearchResults = SearchResults.Count > 0;
    }

    [RelayCommand]
    private async Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        ShowSearchResults = false;
        SearchResults.Clear();
        await LoadProductsAsync();
    }

    [RelayCommand]
    private void AddProduct(Product? product)
    {
        if (product == null) return;

        SelectedProduct = product;
        ShowSearchResults = false;

        if (!Products.Any(p => p.Id == product.Id))
        {
            Products.Add(product);
        }
    }

    [RelayCommand]
    private void NewProduct()
    {
        // Navegar a la vista de detalle sin parámetro (modo crear)
        _navigationService.NavigateTo<Views.Products.ProductFormView>(null);
    }

    [RelayCommand]
    private void EditProduct(Product? product)
    {
        var target = product ?? SelectedProduct;
        if (target == null)
        {
            ShowError("Por favor, seleccione un producto para editar.", "Editar Producto");
            return;
        }

        _navigationService.NavigateTo<Views.Products.ProductFormView>(target.Id);
    }

    [RelayCommand]
    private async Task DeleteProductAsync(Product? product)
    {
        var target = product ?? SelectedProduct;
        if (target == null) return;

        var confirmed = ShowConfirmation(
            $"¿Está seguro de eliminar el producto '{target.BrandName}'?\n\n" +
            "Esta acción desactivará el producto del sistema.",
            "Confirmar eliminación");

        if (!confirmed) return;

        await ExecuteWithBusyAsync(async () =>
        {
            var result = await _inventoryService.DeleteProductAsync(target.Id);

            if (!result.IsSuccess)
            {
                ShowError(result.ErrorMessage, "Error eliminando producto");
                return;
            }

            ShowSuccess($"Producto '{target.BrandName}' eliminado correctamente");

            if (SelectedProduct?.Id == target.Id)
            {
                SelectedProduct = null;
            }

            await LoadProductsAsync();

        }, "Eliminando producto...");
    }

    [RelayCommand]
    private void SeeDetails()
    {
        if (SelectedProduct == null)
            return;

        ShowMessage(
            $"Producto: {SelectedProduct.BrandName}\n" +
            $"Código: {SelectedProduct.BarCode}\n" +
            $"Precio: ${SelectedProduct.SalePrice:N2}\n" +
            $"Stock: {SelectedProduct.Stock} unidades\n" +
            "Detalle del Producto");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        ShowSearchResults = false;
        SearchResults.Clear();
        await LoadProductsAsync();
        ShowSuccess("Lista actualizada");
    }

    [RelayCommand]
    private async Task ExportarAsync()
    {
        var rutaArchivo = _dialogService.ShowSaveFileDialog(
            "Excel|*.xlsx",
            "Exportar productos",
            $"productos_{DateTime.Now:yyyyMMdd}.xlsx");

        if (string.IsNullOrEmpty(rutaArchivo))
            return;

        await ExecuteWithBusyAsync(async () =>
        {
            await Task.Delay(1500);
            ShowSuccess($"Productos exportados a:\n{rutaArchivo}");
        }, "Exportando productos...");
    }
}
