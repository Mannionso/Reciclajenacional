using Microsoft.Data.SqlClient;

namespace ReciclajeNacional
{
    public class ConexionBD
    {
        private readonly string cadenaConexion =
            "Server=(localdb)\\MSSQLLocalDB;Database=reciclaje_nacional;Trusted_Connection=True;TrustServerCertificate=True;";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}