using System.Data;

namespace WebApplication1.Interface
{
    public interface Ic_conexion
    {
        public IDbConnection conexionSQL { get; }
    }
}
