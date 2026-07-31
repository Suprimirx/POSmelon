using DrogueriaPOS.Domain.Enums;

namespace DrogueriaPOS.Domain.Entities;
// Representa un cierre de caja diario
public class CashRegisterSession
{

    private readonly List<Invoice> _invoices = new();

    public int Id { get; private set; }
    public string CashierName { get; private set; }
    public DateTime OpeningDate { get; private set; }
    public DateTime? ClosingDate { get; private set; }
    public decimal InitialCashAmount { get; private set; }
    public decimal TotalSales { get; private set; }
    public decimal TotalCashSales { get; private set; }
    public decimal TotalTransferSales { get; private set; }
    public decimal TotalCash { get; private set; }
    public decimal TotalMoney { get; private set; }
    public int InvoiceCount { get; private set; }
    public CashEstatus State { get; private set; }
    public string Observations { get; private set; }


    public decimal ExpectedCash => TotalCashSales + InitialCashAmount;
    public decimal Difference => TotalCash - ExpectedCash;
    public TimeSpan OpenDuration => (ClosingDate ?? DateTime.Now) - OpeningDate;

    public bool IsOpened => State == CashEstatus.OPENED;
    public bool IsClosed => State == CashEstatus.CLOSED;
    public bool HasShortage => Difference < 0;
    public bool HasSurplus => Difference > 0;

    public IReadOnlyCollection<Invoice> Invoices => _invoices.AsReadOnly();


    public CashRegisterSession(string cashierName, decimal initialCashAmount)
    {

        if (string.IsNullOrWhiteSpace(cashierName))
            throw new ArgumentException("El nombre del cajero es requerido", nameof(cashierName));
        // Validar monto inicial
        if (initialCashAmount < 0)
            throw new ArgumentException("Monto inicial no puede ser negativo", nameof(initialCashAmount));

        CashierName = cashierName.Trim();
        InitialCashAmount = initialCashAmount;
        OpeningDate = DateTime.Now;
        State = CashEstatus.OPENED;
        Observations = string.Empty;
    }

    private CashRegisterSession() { }


    public void Close(decimal actualCash, string observations = "")
    {
        ValidateClose();

        TotalCash = actualCash;
        TotalMoney = TotalCash + TotalTransferSales;
        ClosingDate = DateTime.Now;
        State = CashEstatus.CLOSED;
        Observations = observations ?? string.Empty;

        CalculateTotals();
    }

    public void AddInvoice(Invoice invoice)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));
        if (!IsOpened)
            throw new InvalidOperationException("No se pueden agregar facturas a una caja cerrada");

        _invoices.Add(invoice);
        CalculateTotals();

    }

    private void ValidateClose()
    {
        if (!IsOpened)
            throw new InvalidOperationException("La caja no está abierta");
    }

    private void CalculateTotals()
    {
        // Calcular ventas (facturas completadas)
        var validInvoices = _invoices.Where(f => f.State == InvoiceStatus.GENERATED).ToList();

        TotalSales = validInvoices.Sum(f => f.Total);
        InvoiceCount = validInvoices.Count;

        TotalCashSales = validInvoices
            .Where(f => f.PaymentMethod == PaymentMethod.CASH)
            .Sum(f => f.Total);

        TotalTransferSales = validInvoices
            .Where(f => f.PaymentMethod == PaymentMethod.BANK_TRANSFER)
            .Sum(f => f.Total);
    }

}
