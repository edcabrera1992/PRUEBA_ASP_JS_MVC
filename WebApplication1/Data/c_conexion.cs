using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Interface;

namespace WebApplication1.Data
{
    public class c_conexion : Ic_conexion
    {

        private IConfiguration appSettingsInstance;

        public c_conexion()
        {
            appSettingsInstance = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json").Build();
        }


        public IDbConnection conexionSQL
        {
            get
            {
                 return new SqlConnection(appSettingsInstance.GetConnectionString("CadenaConexionSQL"));
            }
        }


    }



}
