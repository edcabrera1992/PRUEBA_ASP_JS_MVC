using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
   // [Authorize]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly Venta _Service = new Venta();

        [HttpGet]
        public ActionResult<IEnumerable<VentaModel>> Get()
        {
            IEnumerable<VentaModel> arrayLista = new VentaModel[] { };
            object respuesta = new object();
            int elementosTotales = 0;

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    
                    
                    arrayLista = _Service.buscarListado();
                    transaction.Complete();
                }

                if (arrayLista.Count() > 0)
                {
                    elementosTotales = arrayLista.Count();
                }

                respuesta = new
                {
                    status = "success",
                    response = arrayLista,
                    total = elementosTotales
                };

                return new OkObjectResult(respuesta);
            }
            catch (System.Exception ex)
            {
                respuesta = new
                {
                    status = "error",
                    response = ex.Message
                };

                return new OkObjectResult(respuesta);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<VentaModel> Get(int id)
        {
            VentaModel datos = new VentaModel();
            object respuesta = new object();

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    datos = _Service.buscarPorNumSec(id);
                    transaction.Complete();
                }

                if (datos == null)
                {
                    respuesta = new
                    {
                        status = "empty",
                        response = datos
                    };
                    return new OkObjectResult(respuesta);
                }
                else
                {
                    respuesta = new
                    {
                        status = "success",
                        response = datos
                    };
                    return new OkObjectResult(respuesta);
                }
            }
            catch (System.Exception ex)
            {
                respuesta = new
                {
                    status = "error",
                    response = ex.Message
                };

                return new OkObjectResult(respuesta);
            }
        }

        [HttpPost]
        public ActionResult<object> Post([FromBody] VentaModel datos)
        {
            object response = new object();
            RespuestaDB respuestaBD = new RespuestaDB();

            try
            {
               

                // Aquí se pueden agregar validaciones específicas si es necesario
                using (TransactionScope transaction = new TransactionScope())
                {
                    // Guarda los datos de la venta
                    respuestaBD = _Service.guardar(datos);

                    // Verifica si el pago está presente y tiene un monto válido
                    foreach (var pago in datos.pagos)
                    {
                        if (pago.MontoPago <= 0)
                        {
                            return BadRequest(new { status = "error", response = "El monto de uno de los pagos no es válido." });
                        }
                        respuestaBD = _Service.guardarPago(respuestaBD.numsec,pago.MontoPago,pago.TipoPago);
                    }


                    transaction.Complete(); // Completa la transacción si todo salió bien
                }

                // Si la respuesta es un error
                if (respuestaBD.status == "error")
                {
                    response = new
                    {
                        status = "error",
                        response = respuestaBD.response
                    };
                    return new OkObjectResult(response);
                }
                else
                {
                    // Si la venta se guarda correctamente
                    response = new
                    {
                        status = "success",
                        response = respuestaBD.response
                    };
                    return new OkObjectResult(response);
                }
            }
            catch (System.Exception ex)
            {
                // Respuesta en caso de error
                response = new
                {
                    status = "error",
                    response = ex.Message
                };
                return new OkObjectResult(response);
            }
        }


        [HttpPut("{id}")]
        public ActionResult<object> Put(int id, [FromBody] VentaModel datos, decimal montoInput)
        {
            object response = new object();
            RespuestaDB respuestaBD = new RespuestaDB();

            try
            {
                datos.id = id;
                using (TransactionScope transaction = new TransactionScope())
                {
                    respuestaBD = _Service.modificar(datos);
                    transaction.Complete();
                }

                if (respuestaBD.status == "error")
                {
                    response = new
                    {
                        status = "error",
                        response = respuestaBD.response
                    };
                    return new OkObjectResult(response);
                }
                else
                {
                    response = new
                    {
                        status = "success",
                        response = respuestaBD.response
                    };
                    return new OkObjectResult(response);
                }
            }
            catch (System.Exception ex)
            {
                response = new
                {
                    status = "error",
                    response = ex.Message
                };

                return new OkObjectResult(response);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<object> Delete(int id)
        {
            object response = new object();
            RespuestaDB respuestaBD = new RespuestaDB();

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    respuestaBD = _Service.eliminar(id);
                    transaction.Complete();
                }

                if (respuestaBD.status == "error")
                {
                    response = new
                    {
                        status = "error",
                        response = respuestaBD.response
                    };
                    return new OkObjectResult(response);
                }
                else
                {
                    response = new
                    {
                        status = "success",
                        response = respuestaBD.response
                    };
                    return new OkObjectResult(response);
                }
            }
            catch (System.Exception ex)
            {
                response = new
                {
                    status = "error",
                    response = ex.Message
                };

                return new OkObjectResult(response);
            }
        }

        [HttpGet("reporte-pdf")]
        public IActionResult ObtenerReportePDF()
        {
            try
            {
                // Ruta del archivo temporal
                string rutaTemporal = Path.Combine(Path.GetTempPath(), "Reporte_Ventas.pdf");

                // Llamar al método estático directamente desde la clase
                ReporteVentas.GenerarReportePDF(rutaTemporal);

                // Leer el archivo generado
                byte[] fileBytes = System.IO.File.ReadAllBytes(rutaTemporal);

                // Eliminar el archivo temporal después de leerlo
                System.IO.File.Delete(rutaTemporal);

                // Retornar el archivo como respuesta HTTP
                return File(fileBytes, "application/pdf", "Reporte_Ventas.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    response = ex.Message
                });
            }
        }

    }
}