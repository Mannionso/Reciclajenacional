using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ReciclajeNacional.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ConexionBD conexionBD;

        public LoginModel(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        [BindProperty]
        public string Correo { get; set; } = "";

        [BindProperty]
        public string Contrasena { get; set; } = "";

        public string Mensaje { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            try
            {
                using (SqlConnection conexion = conexionBD.ObtenerConexion())
                {
                    conexion.Open();

                    string consulta = @"
                        SELECT IdUsuario, Nombre
                        FROM Usuario
                        WHERE Correo = @Correo
                        AND Contrasena = @Contrasena";

                    using (SqlCommand comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@Correo", Correo);
                        comando.Parameters.AddWithValue("@Contrasena", Contrasena);

                        using (SqlDataReader lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                // Guardamos los datos del usuario en la sesión
                                HttpContext.Session.SetInt32(
                                    "IdUsuario",
                                    lector.GetInt32(0)
                                );

                                HttpContext.Session.SetString(
                                    "NombreUsuario",
                                    lector.GetString(1)
                                );

                                return RedirectToPage("/Index");
                            }
                            else
                            {
                                Mensaje = "Correo o contraseña incorrectos.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Error al iniciar sesión: " + ex.Message;
            }

            return Page();
        }
    }
}