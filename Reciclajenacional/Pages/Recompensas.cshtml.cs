using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class RecompensasModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RecompensasModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Recompensa> Recompensas { get; set; } =
            new List<Recompensa>();

        public int PuntosUsuario { get; set; }

        public string Mensaje { get; set; } = "";

        public void OnGet()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                Response.Redirect("/Login");
                return;
            }

            CargarPuntos(idUsuario.Value);
            CargarRecompensas();
        }

        public IActionResult OnPostCanjear(int idRecompensa)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                string conexion =
                    _configuration.GetConnectionString("Conexion");

                using (SqlConnection cn =
                    new SqlConnection(conexion))
                {
                    cn.Open();

                    string consultaRecompensa = @"
                        SELECT Nombre, PuntosNecesarios
                        FROM Recompensa
                        WHERE IdRecompensa = @IdRecompensa";

                    string nombreRecompensa = "";
                    int puntosNecesarios = 0;

                    using (SqlCommand cmd =
                        new SqlCommand(consultaRecompensa, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdRecompensa",
                            idRecompensa);

                        using (SqlDataReader reader =
                            cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                TempData["MensajeCanje"] =
                                    "La recompensa no existe.";

                                return RedirectToPage();
                            }

                            nombreRecompensa =
                                reader["Nombre"].ToString() ?? "";

                            puntosNecesarios =
                                Convert.ToInt32(
                                    reader["PuntosNecesarios"]);
                        }
                    }

                    string actualizarPuntos = @"
                        UPDATE Usuario
                        SET Puntos = Puntos - @Puntos
                        WHERE IdUsuario = @IdUsuario
                        AND Puntos >= @Puntos";

                    int filasActualizadas;

                    using (SqlCommand cmd =
                        new SqlCommand(actualizarPuntos, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@Puntos",
                            puntosNecesarios);

                        cmd.Parameters.AddWithValue(
                            "@IdUsuario",
                            idUsuario.Value);

                        filasActualizadas =
                            cmd.ExecuteNonQuery();
                    }

                    if (filasActualizadas == 0)
                    {
                        TempData["MensajeCanje"] =
                            "No tienes suficientes puntos para canjear esta recompensa.";

                        return RedirectToPage();
                    }

                    string insertarCanje = @"
                        INSERT INTO Canje
                        (
                            IdUsuario,
                            IdRecompensa,
                            Fecha,
                            PuntosUtilizados
                        )
                        VALUES
                        (
                            @IdUsuario,
                            @IdRecompensa,
                            @Fecha,
                            @PuntosUtilizados
                        )";

                    using (SqlCommand cmd =
                        new SqlCommand(insertarCanje, cn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@IdUsuario",
                            idUsuario.Value);

                        cmd.Parameters.AddWithValue(
                            "@IdRecompensa",
                            idRecompensa);

                        cmd.Parameters.AddWithValue(
                            "@Fecha",
                            DateTime.Now);

                        cmd.Parameters.AddWithValue(
                            "@PuntosUtilizados",
                            puntosNecesarios);

                        cmd.ExecuteNonQuery();
                    }

                    TempData["MensajeCanje"] =
                        "¡Felicidades! Canjeaste " +
                        nombreRecompensa +
                        " por " +
                        puntosNecesarios +
                        " puntos.";
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeCanje"] =
                    "Error al realizar el canje: " +
                    ex.Message;
            }

            return RedirectToPage();
        }

        private void CargarPuntos(int idUsuario)
        {
            string conexion =
                _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn =
                new SqlConnection(conexion))
            {
                cn.Open();

                string consulta = @"
                    SELECT Puntos
                    FROM Usuario
                    WHERE IdUsuario = @IdUsuario";

                using (SqlCommand cmd =
                    new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdUsuario",
                        idUsuario);

                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado != null &&
                        resultado != DBNull.Value)
                    {
                        PuntosUsuario =
                            Convert.ToInt32(resultado);
                    }
                }
            }
        }

        private void CargarRecompensas()
        {
            Recompensas.Clear();

            string conexion =
                _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn =
                new SqlConnection(conexion))
            {
                cn.Open();

                string consulta = @"
                    SELECT
                        IdRecompensa,
                        Nombre,
                        Descripcion,
                        PuntosNecesarios
                    FROM Recompensa
                    ORDER BY PuntosNecesarios";

                using (SqlCommand cmd =
                    new SqlCommand(consulta, cn))
                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Recompensas.Add(
                            new Recompensa
                            {
                                IdRecompensa =
                                    Convert.ToInt32(
                                        reader["IdRecompensa"]),

                                Nombre =
                                    reader["Nombre"]
                                    .ToString() ?? "",

                                Descripcion =
                                    reader["Descripcion"]
                                    .ToString() ?? "",

                                PuntosNecesarios =
                                    Convert.ToInt32(
                                        reader["PuntosNecesarios"])
                            });
                    }
                }
            }
        }
    }
}