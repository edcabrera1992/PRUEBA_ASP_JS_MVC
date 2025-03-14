using System.Data;
using Dapper;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class Cliente
    {
        c_conexion _c_conexion = new c_conexion();
        public IEnumerable<ClienteModel> buscarListadoCliente()
        {
            try
            {
                IEnumerable<ClienteModel> arrayDatos = new List<ClienteModel>();
                string nombreFuncion = "ObtenerListaCliente";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    if (cnx == null)
                        throw new Exception("La conexión a la base de datos no está inicializada.");

                    if (string.IsNullOrEmpty(cnx.ConnectionString))
                        throw new Exception("La cadena de conexión no está configurada.");

                    cnx.Open(); // Asegurarse de abrir la conexión

                    arrayDatos = cnx.Query<ClienteModel>(
                        sql: nombreFuncion,
                        commandType: CommandType.StoredProcedure
                    ).ToList();
                }

                return arrayDatos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en buscarListado: " + ex.Message, ex);
            }
        }

    }
}
