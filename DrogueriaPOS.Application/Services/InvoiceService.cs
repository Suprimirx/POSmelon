using DrogueriaPOS.Application.Common;
using DrogueriaPOS.Application.Repositories;
using DrogueriaPOS.Domain.Entities;
using DrogueriaPOS.Domain.Enums;

namespace DrogueriaPOS.Application.Services;
public class InvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICashRegisterSessionRepository _sessionRepository;
    private readonly AppSettingService _appSettingService;

    public InvoiceService(
        IInvoiceRepository invoiceRepository, 
        IProductRepository productRepository, 
        ICashRegisterSessionRepository sessionRepository, 
        AppSettingService appSettingService)
    {
        _invoiceRepository = invoiceRepository;
        _productRepository = productRepository;
        _sessionRepository = sessionRepository;
        _appSettingService = appSettingService;
    }

    public async Task<Result<Invoice>> ProcessSaleAsync(List<(int productId, int amount)> items, PaymentMethod paymentMethod, decimal cashReceived)
    {

        if (items == null || !items.Any())
            return Result<Invoice>.Failure("La factura debe tener al menos un producto");

        var activeSession = await _sessionRepository.GetActiveSessionAsync();
        if (activeSession == null)
            return Result<Invoice>.Failure("No hay ninguna caja abierta para procesar la venta");

        // Obtener configuración
        var cashierNameResult = await _appSettingService.GetAsync("CashierName");
        if (!cashierNameResult.IsSuccess)
            return Result<Invoice>.Failure("El nombre del cajero no está configurado");

        var cashRegisterNumberResult = await _appSettingService.GetAsync("CashRegisterNumber");
        if (!cashRegisterNumberResult.IsSuccess)
            return Result<Invoice>.Failure("El número de caja no está configurado");

        var invoiceNumber = await _invoiceRepository.GenerateNextInvoiceNumberAsync();
        var invoice = new Invoice(invoiceNumber, cashierNameResult.Data, cashRegisterNumberResult.Data, paymentMethod);

        var productsToUpdate = new List<Product>();

        foreach (var (productId, amount) in items)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                return Result<Invoice>.Failure($"Producto con ID {productId} no encontrado");

            if (!product.HasAvailableStock(amount))
                return Result<Invoice>.Failure($"Stock insuficiente para {product.BrandName}");

            var line = new InvoiceLine(product, amount);
            invoice.AddLine(line);
            product.DecreaseStock(amount);
            productsToUpdate.Add(product);

        }

        invoice.CalculateChange(cashReceived);
        // Asociar la factura a la sesión activa
        activeSession.AddInvoice(invoice);

        await _invoiceRepository.ProcessSaleTransactionAsync(invoice, productsToUpdate, activeSession);
        return Result<Invoice>.Success(invoice);

    }


    public async Task<Result> AnnulInvoiceAsync(int invoiceId)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

        if (invoice == null)
            return Result.Failure("Factura no encontrada");

        if (invoice.IsAnnuled)
            return Result.Failure("La factura ya está anulada");

        invoice.Annul();

        var productsToRestore = new List<Product>();

        foreach (var line in invoice.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId);

            if (product == null)
                return Result.Failure($"Producto con ID {line.ProductId} no encontrado");

            product.IncreaseStock(line.Amount);
            productsToRestore.Add(product);
        }

        await _invoiceRepository.AnnulTransactionAsync(invoice, productsToRestore);

        return Result.Success();
    }

    public async Task<Result<Invoice>> GetByNumberAsync(string invoiceNumber)
    {
        var invoice = await _invoiceRepository.GetByNumberAsync(invoiceNumber);

        if (invoice == null)
            return Result<Invoice>.Failure("Factura no encontrada");

        return Result<Invoice>.Success(invoice);
    }

    public async Task<Result<IEnumerable<Invoice>>> GetByDateAsync(DateTime date)
    {
        var invoices = await _invoiceRepository.GetByDateAsync(date);
        return Result<IEnumerable<Invoice>>.Success(invoices);
    }
}