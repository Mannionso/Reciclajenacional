using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReciclajeNacional.Pages
{
    public class EstadisticasModel : PageModel
    {
        public decimal TotalKg { get; set; }
        public int TotalPuntos { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalUsuarios { get; set; }

        public void OnGet()
        {
            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consultaKg =
                    "SELECT ISNULL(SUM(CantidadKg), 0) FROM RegistroReciclaje";

                using (SqlCommand cmd = new SqlCommand(consultaKg, cn))
                {
                    TotalKg = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                string consultaPuntos =
                    "SELECT ISNULL(SUM(PuntosObtenidos), 0) FROM RegistroReciclaje";

                using (SqlCommand cmd = new SqlCommand(consultaPuntos, cn))
                {
                    TotalPuntos = Convert.ToInt32(cmd.ExecuteScalar());
                }

                string consultaRegistros =
                    "SELECT COUNT(*) FROM RegistroReciclaje";

                using (SqlCommand cmd = new SqlCommand(consultaRegistros, cn))
                {
                    TotalRegistros = Convert.ToInt32(cmd.ExecuteScalar());
                }

                string consultaUsuarios =
                    "SELECT COUNT(*) FROM Usuario";

                using (SqlCommand cmd = new SqlCommand(consultaUsuarios, cn))
                {
                    TotalUsuarios = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}