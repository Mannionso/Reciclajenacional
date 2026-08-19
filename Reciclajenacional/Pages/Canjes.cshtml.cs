using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Reciclajenacional.POO;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class CanjesModel : PageModel
    {
        [BindProperty]
        public Canje Canje { get; set; } = new Canje();

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public List<Recompensa> Recompensas { get; set; } = new List<Recompensa>();

        public string Mensaje { get; set; } = "";

        public void OnGet()
        {
            CargarUsuarios();
            CargarRecompensas();
        }

        public IActionResult OnPost()
        {
            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consultaUsuario =
                    "SELECT Puntos FROM Usuario WHERE IdUsuario = @IdUsuario";

                int puntosUsuario = 0;

                using (SqlCommand cmd = new SqlCommand(consultaUsuario, cn))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", Canje.IdUsuario);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado == null)
                    {
                        Mensaje = "El usuario no existe.";
                        CargarUsuarios();
                        CargarRecompensas();
                        return Page();
                    }

                    puntosUsuario = Convert.ToInt32(resultado);
                }

                string consultaRecompensa =
                    "SELECT PuntosNecesarios FROM Recompensa WHERE IdRecompensa = @IdRecompensa";

                int puntosNecesarios = 0;

                using (SqlCommand cmd = new SqlCommand(consultaRecompensa, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdRecompensa",
                        Canje.IdRecompensa
                    );

                    object resultado = cmd.ExecuteScalar();

                    if (resultado == null)
                    {
                        Mensaje = "La recompensa no existe.";
                        CargarUsuarios();
                        CargarRecompensas();
                        return Page();
                    }

                    puntosNecesarios = Convert.ToInt32(resultado);
                }

                if (puntosUsuario < puntosNecesarios)
                {
                    Mensaje = "El usuario no tiene suficientes puntos.";
                    CargarUsuarios();
                    CargarRecompensas();
                    return Page();
                }

                string insertarCanje = @"
                    INSERT INTO Canje
                    (IdUsuario, IdRecompensa, Fecha, PuntosUsados)
                    VALUES
                    (@IdUsuario, @IdRecompensa, @Fecha, @PuntosUsados)";

                using (SqlCommand cmd = new SqlCommand(insertarCanje, cn))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", Canje.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdRecompensa", Canje.IdRecompensa);
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@PuntosUsados", puntosNecesarios);

                    cmd.ExecuteNonQuery();
                }

                string actualizarPuntos = @"
                    UPDATE Usuario
                    SET Puntos = Puntos - @Puntos
                    WHERE IdUsuario = @IdUsuario";

                using (SqlCommand cmd = new SqlCommand(actualizarPuntos, cn))
                {
                    cmd.Parameters.AddWithValue("@Puntos", puntosNecesarios);
                    cmd.Parameters.AddWithValue("@IdUsuario", Canje.IdUsuario);

                    cmd.ExecuteNonQuery();
                }

                Mensaje = "Canje realizado correctamente.";
            }

            CargarUsuarios();
            CargarRecompensas();

            return Page();
        }

        private void CargarUsuarios()
        {
            Usuarios.Clear();

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta =
                    "SELECT IdUsuario, Nombre, Correo, Provincia, Puntos FROM Usuario";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
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

        private void CargarRecompensas()
        {
            Recompensas.Clear();

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta =
                    "SELECT IdRecompensa, Nombre, Descripcion, PuntosNecesarios FROM Recompensa";

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