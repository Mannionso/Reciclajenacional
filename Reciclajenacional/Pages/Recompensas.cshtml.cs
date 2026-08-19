using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class RecompensasModel : PageModel
    {
        [BindProperty]
        public Recompensa Recompensa { get; set; } = new Recompensa();

        public List<Recompensa> Recompensas { get; set; } = new List<Recompensa>();

        public void OnGet()
        {
            CargarRecompensas();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                CargarRecompensas();
                return Page();
            }

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta = @"
                    INSERT INTO Recompensa
                    (Nombre, Descripcion, PuntosNecesarios)
                    VALUES
                    (@Nombre, @Descripcion, @PuntosNecesarios)";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", Recompensa.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", Recompensa.Descripcion);
                    cmd.Parameters.AddWithValue("@PuntosNecesarios",
                        Recompensa.PuntosNecesarios);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        private void CargarRecompensas()
        {
            Recompensas.Clear();

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta = @"
                    SELECT IdRecompensa, Nombre, Descripcion, PuntosNecesarios
                    FROM Recompensa";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Recompensas.Add(new Recompensa
                        {
                            IdRecompensa = Convert.ToInt32(reader["IdRecompensa"]),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"].ToString(),
                            PuntosNecesarios =
                                Convert.ToInt32(reader["PuntosNecesarios"])
                        });
                    }
                }
            }
        }
    }
}