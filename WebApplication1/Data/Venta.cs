
using System.Data;
using Dapper;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class Venta
    {
        c_conexion _c_conexion = new c_conexion();

        public IEnumerable<VentaModel> buscarListado()
        {
            try
            {
                IEnumerable<VentaModel> arrayDatos = new List<VentaModel>();
                string nombreFuncion = "ObtenerListaVenta";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    if (cnx == null)
                        throw new Exception("La conexión a la base de datos no está inicializada.");

                    if (string.IsNullOrEmpty(cnx.ConnectionString))
                        throw new Exception("La cadena de conexión no está configurada.");

                    cnx.Open(); // Asegurarse de abrir la conexión

                    arrayDatos = cnx.Query<VentaModel>(
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

        public VentaModel buscarPorNumSec(int id)
        {
            try
            {
                VentaModel datos = new VentaModel();
                string nombreFuncion = "ObtenerListaVentaId";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    datos = cnx.QuerySingleOrDefault<VentaModel>(
                        sql: nombreFuncion,
                        commandType: CommandType.StoredProcedure,
                        param: new
                        {
                            @id = id
                        }
                    );
                }

                return datos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RespuestaDB guardar(VentaModel datos)
        {
            try
            {
                RespuestaDB respuesta = new RespuestaDB();
                string nombreFuncion = "sp_abm_venta";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    respuesta = cnx.QuerySingleOrDefault<RespuestaDB>(
                        sql: nombreFuncion,
                        commandType: CommandType.StoredProcedure,
                        param: new
                        {
                            @accion = 1,
                            @id = datos.id,
                            @FechaVenta = datos.FechaVenta,
                            @MontoTotal = datos.MontoTotal,
                            @ClienteId = datos.ClienteId
                        }
                    );
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public RespuestaDB modificar(VentaModel datos)
        {
            try
            {
                RespuestaDB respuesta = new RespuestaDB();
                string nombreFuncion = "sp_abm_venta";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    respuesta = cnx.QuerySingleOrDefault<RespuestaDB>(
                        sql: nombreFuncion,
                        commandType: CommandType.StoredProcedure,
                        param: new
                        {
                            @accion = 2,
                            @id = datos.id,
                            @FechaVenta = datos.FechaVenta,
                            @MontoTotal = datos.MontoTotal,
                            @ClienteId = datos.ClienteId
                        }
                    );
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RespuestaDB eliminar(int id)
        {
            try
            {
                RespuestaDB respuesta = null;
                string nombreFuncion = "sp_abm_venta";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    respuesta = cnx.QuerySingleOrDefault<RespuestaDB>(
                        sql: nombreFuncion,
                        commandType: CommandType.StoredProcedure,
                        param: new
                        {
                            @accion = 3,
                            @id = id,
                        }
                    );
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RespuestaDB guardarPago(string idVenta , decimal monto, string tipoPago )
        {
            try
            {
                RespuestaDB respuesta = new RespuestaDB();
                string nombreFuncion = "sp_abm_pago";

                using (IDbConnection cnx = _c_conexion.conexionSQL)
                {
                    respuesta = cnx.QuerySingleOrDefault<RespuestaDB>(
                        sql: nombreFuncion,
                        commandType: CommandType.StoredProcedure,
                        param: new
                        {
                            @VentaId =int.Parse( idVenta),
                            @MontoPago = monto,
                            @TipoPago = tipoPago
                        }
                    );
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
