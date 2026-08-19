using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Reciclajenacional.POO;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class UsuariosModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UsuariosModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();

        [BindProperty]
        public Usuario Usuario { get; set; } = new Usuario();

        public void OnGet()
        {
            CargarUsuarios();
        }

        public IActionResult OnPost()
        {
            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"INSERT INTO Usuario
                               (Nombre, Correo, Provincia, Puntos)
                               VALUES
                               (@Nombre, @Correo, @Provincia, @Puntos)";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", Usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", Usuario.Correo);
                    cmd.Parameters.AddWithValue("@Provincia", Usuario.Provincia);
                    cmd.Parameters.AddWithValue("@Puntos", Usuario.Puntos);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        private void CargarUsuarios()
        {
            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"SELECT IdUsuario, Nombre, Correo, Provincia, Puntos
                               FROM Usuario";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuarios.Add(new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Provincia = reader["Provincia"].ToString(),
                                Puntos = Convert.ToInt32(reader["Puntos"])
                            });
                        }
                    }
                }
            }
        }
    }
}