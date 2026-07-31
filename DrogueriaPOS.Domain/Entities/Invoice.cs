using DrogueriaPOS.Domain.Enums;
using DrogueriaPOS.Domain.Exceptions;

namespace DrogueriaPOS.Domain.Entities;
public class Invoice
{
    public const string DefaultCustomerName = "Consumidor Final";
    public const string DefaultCustomerDocument = "222222222222";
   

    private readonly List<InvoiceLine> _lines = new();

    public int Id { get; private set; }
    public string InvoiceNumber { get; private set; }
    public DateTime Date { get; private set; }
    public string CashierName { get; private set; }
    public string CashRegisterNumber { get; private set; }
    public decimal CashReceived { get; private set; }
    public decimal Base { get; private set; }
    public decimal TotalIVA { get; private set; }
    public decimal Total { get; private set; }
    public InvoiceStatus State { get; private set; }
    public DateTime? AnnulmentDate { get; private set; }
    public int? CashRegisterSessionId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }


    public string CustomerName => DefaultCustomerName;
    public string CustomerDocument => DefaultCustomerDocument;
    public decimal Discount => 0;


    // Detalles de la factura (productos vendidos)
    public IReadOnlyCollection<InvoiceLine> Lines => _lines.AsReadOnly();

    // Cierre de caja al que pertenece esta factura
    public CashRegisterSession CashSession { get; set; }

    public bool IsGenerated => State == InvoiceStatus.GENERATED;
    public bool IsAnnuled => State == InvoiceStatus.VOIDED;
    public int TotalItems => Lines.Sum(l => l.Amount);


    public Invoice(string invoiceNumber, string cashierName, string cashRegisterNumber, PaymentMethod paymentMethod)
    {

        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("Número de factura es requerido", nameof(invoiceNumber));

        if (string.IsNullOrWhiteSpace(cashierName))
            throw new ArgumentException("Nombre del cajero es requerido", nameof(cashierName));

        if (string.IsNullOrWhiteSpace(cashRegisterNumber))
            throw new ArgumentException("Número de caja es requerido", nameof(cashRegisterNumber));

        InvoiceNumber = invoiceNumber.Trim();
        Date = DateTime.Now;
        State = InvoiceStatus.GENERATED;
        CashierName = cashierName;
        CashRegisterNumber = cashRegisterNumber;
        PaymentMethod = paymentMethod;
        TotalIVA = 0;
        Base = 0;
        Total = 0;

    }

    private Invoice() { }

    // Agrega un detalle a la Factura
    public void AddLine(InvoiceLine line)
    {
        if (line == null)
            throw new ArgumentNullException(nameof(line), "Detalle es requerido");

        if (State != InvoiceStatus.GENERATED)
            throw new InvalidOperationException(
                "No se pueden agregar detalles a una factura ANULADA");

        // Agregar a la colección
        _lines.Add(line);

        // Recalcular totales
        CalculateTotals();
    }

    private void CalculateTotals()
    {
        if (!_lines.Any())
        {
            Total = 0;
            TotalIVA = 0;
            Base = 0;
            return;
        }

        Total = _lines.Sum(d => d.SubTotal);
        Base  = _lines.Sum(d => d.Base);
        TotalIVA = _lines.Sum(d => d.TotalIVA);
    }

    // Anula la factura completamente
    public void Annul()
    {
        if (State == InvoiceStatus.VOIDED)
            throw new FacturaYaAnuladaException(InvoiceNumber);

        State = InvoiceStatus.VOIDED;
        AnnulmentDate = DateTime.Now;
    }

    // Calcula el cambio a devolver al cliente
    public decimal CalculateChange(decimal cashReceived)
    {
        if (cashReceived < Total)
            throw new ArgumentException("Efectivo recibido no puede ser menor que el total", nameof(cashReceived));

        CashReceived = cashReceived;
        return cashReceived - Total;
    }

}
