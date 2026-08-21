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

        public List<Canje> Canjes { get; set; } =
            new List<Canje>();

        public void OnGet()
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                Response.Redirect("/Login");
                return;
            }

            CargarCanjes(idUsuario.Value);
        }

        private void CargarCanjes(int idUsuario)
        {
            Canjes.Clear();

            string conexion =
                _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn =
                new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"
                    SELECT
                        IdCanje,
                        IdUsuario,
                        IdRecompensa,
                        Fecha,
                        PuntosUtilizados
                    FROM Canje
                    WHERE IdUsuario = @IdUsuario
                    ORDER BY Fecha DESC";

                using (SqlCommand cmd =
                    new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdUsuario",
                        idUsuario);

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Canjes.Add(
                                new Canje
                                {
                                    IdCanje =
                                        Convert.ToInt32(
                                            reader["IdCanje"]),

                                    IdUsuario =
                                        Convert.ToInt32(
                                            reader["IdUsuario"]),

                                    IdRecompensa =
                                        Convert.ToInt32(
                                            reader["IdRecompensa"]),

                                    Fecha =
                                        Convert.ToDateTime(
                                            reader["Fecha"]),

                                    PuntosUtilizados =
                                        Convert.ToInt32(
                                            reader["PuntosUtilizados"])
                                });
                        }
                    }
                }
            }
        }
    }
}