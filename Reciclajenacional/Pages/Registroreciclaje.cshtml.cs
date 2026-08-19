using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Reciclajenacional.POO;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class RegistroreciclajeModel : PageModel
    {
        [BindProperty]
        public RegistroReciclaje Registro { get; set; } = new RegistroReciclaje();

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public List<Material> Materiales { get; set; } = new List<Material>();
        public List<CentroReciclaje> Centros { get; set; } = new List<CentroReciclaje>();
        public List<RegistroReciclaje> Registros { get; set; } = new List<RegistroReciclaje>();

        public void OnGet()
        {
            CargarUsuarios();
            CargarMateriales();
            CargarCentros();
            CargarRegistros();
        }

        public IActionResult OnPost()
        {
            if (Registro.CantidadKg <= 0)
            {
                ModelState.AddModelError("", "La cantidad debe ser mayor a 0.");
            }

            if (!ModelState.IsValid)
            {
                CargarUsuarios();
                CargarMateriales();
                CargarCentros();
                CargarRegistros();

                return Page();
            }

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                decimal puntosPorKg = 0;

                string consultaMaterial =
                    "SELECT PuntosPorKg FROM Material WHERE IdMaterial = @IdMaterial";

                using (SqlCommand cmd = new SqlCommand(consultaMaterial, cn))
                {
                    cmd.Parameters.AddWithValue("@IdMaterial", Registro.IdMaterial);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        puntosPorKg = Convert.ToDecimal(resultado);
                    }
                }

                Registro.PuntosObtenidos =
                    Convert.ToInt32(Math.Round(Registro.CantidadKg * puntosPorKg));

                string consultaRegistro = @"
                    INSERT INTO RegistroReciclaje
                    (IdUsuario, IdMaterial, IdCentro, CantidadKg, Fecha, PuntosObtenidos)
                    VALUES
                    (@IdUsuario, @IdMaterial, @IdCentro, @CantidadKg, @Fecha, @PuntosObtenidos)";

                using (SqlCommand cmd = new SqlCommand(consultaRegistro, cn))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", Registro.IdUsuario);
                    cmd.Parameters.AddWithValue("@IdMaterial", Registro.IdMaterial);
                    cmd.Parameters.AddWithValue("@IdCentro", Registro.IdCentro);
                    cmd.Parameters.AddWithValue("@CantidadKg", Registro.CantidadKg);
                    cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@PuntosObtenidos", Registro.PuntosObtenidos);

                    cmd.ExecuteNonQuery();
                }

                string actualizarUsuario = @"
                    UPDATE Usuario
                    SET Puntos = Puntos + @Puntos
                    WHERE IdUsuario = @IdUsuario";

                using (SqlCommand cmd = new SqlCommand(actualizarUsuario, cn))
                {
                    cmd.Parameters.AddWithValue("@Puntos", Registro.PuntosObtenidos);
                    cmd.Parameters.AddWithValue("@IdUsuario", Registro.IdUsuario);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
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

        private void CargarMateriales()
        {
            Materiales.Clear();

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta =
                    "SELECT IdMaterial, Nombre, Descripcion, PuntosPorKg FROM Material";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Materiales.Add(new Material
                        {
                            IdMaterial = Convert.ToInt32(reader["IdMaterial"]),
                            Nombre = reader["Nombre"].ToString(),
                            Descripcion = reader["Descripcion"].ToString(),
                            PuntosPorKg = Convert.ToDecimal(reader["PuntosPorKg"])
                        });
                    }
                }
            }
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

        private void CargarRegistros()
        {
            Registros.Clear();

            ConexionBD conexion = new ConexionBD();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string consulta = @"
                    SELECT 
                        r.IdRegistro,
                        u.Nombre AS Usuario,
                        m.Nombre AS Material,
                        c.Nombre AS Centro,
                        r.CantidadKg,
                        r.Fecha,
                        r.PuntosObtenidos
                    FROM RegistroReciclaje r
                    INNER JOIN Usuario u ON r.IdUsuario = u.IdUsuario
                    INNER JOIN Material m ON r.IdMaterial = m.IdMaterial
                    INNER JOIN CentroReciclaje c ON r.IdCentro = c.IdCentro
                    ORDER BY r.Fecha DESC";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Registros.Add(new RegistroReciclaje
                        {
                            IdRegistro = Convert.ToInt32(reader["IdRegistro"]),
                            CantidadKg = Convert.ToDecimal(reader["CantidadKg"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            PuntosObtenidos = Convert.ToInt32(reader["PuntosObtenidos"])
                        });
                    }
                }
            }
        }
    }
}