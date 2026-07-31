
namespace DrogueriaPOS.Domain.Exceptions;
public class FacturaYaAnuladaException : Exception
{
    public FacturaYaAnuladaException(string numeroFactura)
        : base($"La factura {numeroFactura} ya está anulada")
    {
    }
}
