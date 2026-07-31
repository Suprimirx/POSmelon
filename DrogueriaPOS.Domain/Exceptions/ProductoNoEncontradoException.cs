
namespace DrogueriaPOS.Domain.Exceptions
{
    public class ProductoNoEncontradoException : Exception
    {
        public ProductoNoEncontradoException(string codigoBarras)
            : base($"No se encontró producto con código de barras: {codigoBarras}")
        {

        }

        public ProductoNoEncontradoException(int productoId)
            : base($"No se encontró producto con ID: {productoId}")
        {

        }
    }
}
