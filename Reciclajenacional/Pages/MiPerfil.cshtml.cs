using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class MiPerfilModel : PageModel
    {
        private readonly ConexionBD conexionBD;

        public MiPerfilModel(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public Usuario Usuario { get; set; } = new Usuario();

        public string NombreCentro { get; set; } = "";

        public void OnGet()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                Response.Redirect("/Login");
                return;
            }

            CargarUsuario(idUsuario.Value);
        }

        private void CargarUsuario(int idUsuario)
        {
            using (SqlConnection conexion = conexionBD.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT 
                        u.IdUsuario,
                        u.Nombre,
                        u.Correo,
                        u.Provincia,
                        u.Puntos,
                        u.IdCentro,
                        c.Nombre AS NombreCentro
                    FROM Usuario u
                    LEFT JOIN CentroReciclaje c
                        ON u.IdCentro = c.IdCentro
                    WHERE u.IdUsuario = @IdUsuario";

                using (SqlCommand comando =
                       new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue(
                        "@IdUsuario",
                        idUsuario);

                    using (SqlDataReader lector =
                           comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            Usuario.IdUsuario =
                                Convert.ToInt32(lector["IdUsuario"]);

                            Usuario.Nombre =
                                lector["Nombre"].ToString() ?? "";

                            Usuario.Correo =
                                lector["Correo"].ToString() ?? "";

                            Usuario.Provincia =
                                lector["Provincia"].ToString() ?? "";

                            Usuario.Puntos =
                                Convert.ToInt32(lector["Puntos"]);

                            if (lector["IdCentro"] != DBNull.Value)
                            {
                                Usuario.IdCentro =
                                    Convert.ToInt32(lector["IdCentro"]);
                            }

                            if (lector["NombreCentro"] != DBNull.Value)
                            {
                                NombreCentro =
                                    lector["NombreCentro"].ToString() ?? "";
                            }
                            else
                            {
                                NombreCentro = "Sin centro asignado";
                            }
                        }
                    }
                }
            }
        }

        public IActionResult OnPostCerrarSesion()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}