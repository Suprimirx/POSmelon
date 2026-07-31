
namespace DrogueriaPOS.Domain.Exceptions
{
    public class StockInsuficienteException : Exception
    {
        public string NombreProducto { get; }
        public int StockDisponible { get; }
        public int CantidadSolicitada { get; }

        public StockInsuficienteException(string nombreProducto, int stockDisponible, int cantidadSolicitada) 
        : base($"Stock insuficiente para {nombreProducto}. Disponible {stockDisponible}, Solicitado: {cantidadSolicitada}")
        { 
            NombreProducto = nombreProducto;
            StockDisponible = stockDisponible;
            CantidadSolicitada = cantidadSolicitada;
        }
    }
}
