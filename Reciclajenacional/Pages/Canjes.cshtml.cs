using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class CanjesModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CanjesModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Canje> Canjes { get; set; } = new List<Canje>();

        public void OnGet()
        {
            CargarCanjes();
        }

        private void CargarCanjes()
        {
            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"
                    SELECT IdCanje, IdUsuario, IdRecompensa, Fecha, PuntosUtilizados
                    FROM Canje
                    ORDER BY Fecha DESC";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Canjes.Add(new Canje
                            {
                                IdCanje = Convert.ToInt32(reader["IdCanje"]),
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                IdRecompensa = Convert.ToInt32(reader["IdRecompensa"]),
                                Fecha = Convert.ToDateTime(reader["Fecha"]),
                                PuntosUtilizados = Convert.ToInt32(reader["PuntosUtilizados"])
                            });
                        }
                    }
                }
            }
        }
    }
}