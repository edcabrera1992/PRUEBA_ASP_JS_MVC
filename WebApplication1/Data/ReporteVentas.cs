using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Reflection.Metadata;

using WebApplication1.Models;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Transactions;
using System.Globalization;

//iTextSharp -Version 5.5.13.3

namespace WebApplication1.Data
{
    public class ReporteVentas
    {

        c_conexion _c_conexion = new c_conexion();

        public static void GenerarReportePDF(string rutaArchivo = "c")
        {
            // Conexión a la base de datos SQL Server
            IEnumerable<VentaModel> arrayLista = new VentaModel[] { };

            Venta _Service = new Venta();
            using (TransactionScope transaction = new TransactionScope())
            {
                arrayLista = _Service.buscarListado();
                transaction.Complete();
            }

            // Crear el documento PDF
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(rutaArchivo, FileMode.Create));
            doc.Open();

            // Título del Reporte
            iTextSharp.text.Font tituloFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            Paragraph titulo = new Paragraph("Reporte de Ventas", tituloFont)
            {
                Alignment = Element.ALIGN_CENTER
            };
            doc.Add(titulo);
            doc.Add(new Paragraph("\n"));

            // Crear la tabla en el PDF
            PdfPTable table = new PdfPTable(4); // 4 columnas
            table.WidthPercentage = 100;

            // Encabezados
            string[] encabezados = { "ID Venta", "Fecha Venta", "Monto Total", "Cliente" };
            foreach (string encabezado in encabezados)
            {
                PdfPCell cell = new PdfPCell(new Phrase(encabezado, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                {
                    BackgroundColor = new BaseColor(200, 200, 200), // Color gris claro
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }

            // Llenar la tabla con datos de la base
            foreach (var venta in arrayLista)
            {
                table.AddCell(venta.id.ToString());
                table.AddCell(venta.FechaVenta.ToString("yyyy-MM-dd"));
                string montoFormateado = venta.MontoTotal.ToString("N2", new CultureInfo("es-BO"));  // N2 para dos decimales
                montoFormateado = "Bs " + montoFormateado;  // Agregar el símbolo "Bs"


                table.AddCell(montoFormateado); // Formato de moneda
                table.AddCell(venta.nombre);
            }

            doc.Add(table);
            doc.Close();

            Console.WriteLine($"Reporte generado en: {rutaArchivo}");
        }
    }
}
