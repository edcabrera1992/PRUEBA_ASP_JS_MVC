using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    [ApiController]
    public class ClienteController : Controller
    {
        private readonly Cliente _Service = new Cliente();

        [HttpGet]
        public ActionResult<IEnumerable<ClienteModel>> Get()
        {
            IEnumerable<ClienteModel> arrayLista = new ClienteModel[] { };
            object respuesta = new object();
            int elementosTotales = 0;

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {


                    arrayLista = _Service.buscarListadoCliente();
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
    }
}
