using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReciclajeNacional.Pages
{
    public class EditarPerfilModel : PageModel
    {
        private readonly ConexionBD conexionBD;

        public EditarPerfilModel(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        [BindProperty]
        public string Nombre { get; set; } = "";

        [BindProperty]
        public string Correo { get; set; } = "";

        [BindProperty]
        public string Provincia { get; set; } = "";

        public string Mensaje { get; set; } = "";

        public IActionResult OnGet()
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToPage("/Login");
            }

            CargarDatos(idUsuario.Value);

            return Page();
        }

        private void CargarDatos(int idUsuario)
        {
            using (SqlConnection conexion =
                   conexionBD.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT Nombre, Correo, Provincia
                    FROM Usuario
                    WHERE IdUsuario = @IdUsuario";

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
                            Nombre =
                                lector["Nombre"].ToString() ?? "";

                            Correo =
                                lector["Correo"].ToString() ?? "";

                            Provincia =
                                lector["Provincia"].ToString() ?? "";
                        }
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToPage("/Login");
            }

            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Provincia))
            {
                Mensaje = "Todos los campos son obligatorios.";
                return Page();
            }

            try
            {
                using (SqlConnection conexion =
                       conexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        UPDATE Usuario
                        SET Nombre = @Nombre,
                            Correo = @Correo,
                            Provincia = @Provincia
                        WHERE IdUsuario = @IdUsuario";

                    using (SqlCommand comando =
                           new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue(
                            "@Nombre",
                            Nombre);

                        comando.Parameters.AddWithValue(
                            "@Correo",
                            Correo);

                        comando.Parameters.AddWithValue(
                            "@Provincia",
                            Provincia);

                        comando.Parameters.AddWithValue(
                            "@IdUsuario",
                            idUsuario.Value);

                        comando.ExecuteNonQuery();
                    }
                }

                return RedirectToPage("/MiPerfil");
            }
            catch (Exception ex)
            {
                Mensaje =
                    "Error al actualizar el perfil: " +
                    ex.Message;

                return Page();
            }
        }
    }
}