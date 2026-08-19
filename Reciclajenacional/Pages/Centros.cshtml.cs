using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class CentrosModel : PageModel
    {
        [BindProperty]
        public CentroReciclaje Centro { get; set; } = new CentroReciclaje();

        public List<CentroReciclaje> Centros { get; set; } = new List<CentroReciclaje>();

        public void OnGet()
        {
            CargarCentros();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                CargarCentros();
                return Page();
            }

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta = @"
                    INSERT INTO CentroReciclaje
                    (Nombre, Direccion, Horario)
                    VALUES
                    (@Nombre, @Direccion, @Horario)";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", Centro.Nombre);
                    cmd.Parameters.AddWithValue("@Direccion", Centro.Direccion);
                    cmd.Parameters.AddWithValue("@Horario", Centro.Horario);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        private void CargarCentros()
        {
            Centros.Clear();

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta =
                    "SELECT IdCentro, Nombre, Direccion, Horario FROM CentroReciclaje";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Centros.Add(new CentroReciclaje
                        {
                            IdCentro = Convert.ToInt32(reader["IdCentro"]),
                            Nombre = reader["Nombre"].ToString(),
                            Direccion = reader["Direccion"].ToString(),
                            Horario = reader["Horario"].ToString()
                        });
                    }
                }
            }
        }
    }
}