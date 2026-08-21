using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class BuscarUsuarioModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public BuscarUsuarioModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public string Busqueda { get; set; } = "";

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();

        public void OnGet()
        {
            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                BuscarUsuarios();
            }
        }

        private void BuscarUsuarios()
        {
            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"
                    SELECT Nombre, Correo, Provincia, Puntos
                    FROM Usuario
                    WHERE Nombre LIKE @Busqueda
                       OR Correo LIKE @Busqueda
                    ORDER BY Nombre";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Busqueda", "%" + Busqueda + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario();

                            usuario.Nombre = reader["Nombre"].ToString();
                            usuario.Correo = reader["Correo"].ToString();
                            usuario.Provincia = reader["Provincia"].ToString();
                            usuario.Puntos = Convert.ToInt32(reader["Puntos"]);

                            Usuarios.Add(usuario);
                        }
                    }
                }
            }
        }
    }
}