using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
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

        [BindProperty]
        public Usuario Usuario { get; set; } = new Usuario();

        public string Mensaje { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Usuario.Contrasena) ||
                Usuario.Contrasena.Length < 4 ||
                Usuario.Contrasena.Length > 8)
            {
                Mensaje = "La contraseña debe tener entre 4 y 8 caracteres.";
                return Page();
            }

            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string verificarCorreo = @"
                    SELECT COUNT(*)
                    FROM Usuario
                    WHERE Correo = @Correo";

                using (SqlCommand cmdCorreo = new SqlCommand(verificarCorreo, cn))
                {
                    cmdCorreo.Parameters.AddWithValue("@Correo", Usuario.Correo);

                    int existe = Convert.ToInt32(cmdCorreo.ExecuteScalar());

                    if (existe > 0)
                    {
                        Mensaje = "El correo ya está registrado.";
                        return Page();
                    }
                }

                string buscarCentro = @"
                    SELECT TOP 1 IdCentro
                    FROM CentroReciclaje
                    WHERE Provincia = @Provincia";

                int idCentro;

                using (SqlCommand cmdCentro = new SqlCommand(buscarCentro, cn))
                {
                    cmdCentro.Parameters.AddWithValue("@Provincia", Usuario.Provincia);

                    object resultado = cmdCentro.ExecuteScalar();

                    if (resultado == null)
                    {
                        Mensaje = "No existe un centro de reciclaje para esa provincia.";
                        return Page();
                    }

                    idCentro = Convert.ToInt32(resultado);
                }

                string sql = @"
                    INSERT INTO Usuario
                    (Nombre, Correo, Provincia, Puntos, Contrasena, IdCentro)
                    VALUES
                    (@Nombre, @Correo, @Provincia, 0, @Contrasena, @IdCentro)";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", Usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Correo", Usuario.Correo);
                    cmd.Parameters.AddWithValue("@Provincia", Usuario.Provincia);
                    cmd.Parameters.AddWithValue("@Contrasena", Usuario.Contrasena);
                    cmd.Parameters.AddWithValue("@IdCentro", idCentro);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Login");
        }
    }
}