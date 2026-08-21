using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class RegistroreciclajeModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public RegistroreciclajeModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public RegistroReciclaje Registro { get; set; } = new RegistroReciclaje();

        public List<Material> Materiales { get; set; } = new List<Material>();

        public void OnGet()
        {
            CargarMateriales();
        }

        public IActionResult OnPost()
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToPage("/Login");
            }

            Registro.IdUsuario = idUsuario.Value;

            if (Registro.IdMaterial <= 0)
            {
                ModelState.AddModelError(
                    "Registro.IdMaterial",
                    "Seleccione un material.");
            }

            if (Registro.CantidadKg <= 0)
            {
                ModelState.AddModelError(
                    "Registro.CantidadKg",
                    "La cantidad debe ser mayor a 0.");
            }

            if (!ModelState.IsValid)
            {
                CargarMateriales();
                return Page();
            }

            string conexion =
                _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                int idCentro = 0;

                string consultaCentro = @"
                    SELECT IdCentro
                    FROM Usuario
                    WHERE IdUsuario = @IdUsuario";

                using (SqlCommand cmd =
                    new SqlCommand(consultaCentro, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdUsuario",
                        Registro.IdUsuario);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        idCentro = Convert.ToInt32(resultado);
                    }
                }

                if (idCentro == 0)
                {
                    ModelState.AddModelError(
                        "",
                        "No se encontró un centro asignado.");

                    CargarMateriales();
                    return Page();
                }

                decimal puntosPorKg = 0;

                string consultaMaterial = @"
                    SELECT PuntosPorKg
                    FROM Material
                    WHERE IdMaterial = @IdMaterial";

                using (SqlCommand cmd =
                    new SqlCommand(consultaMaterial, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdMaterial",
                        Registro.IdMaterial);

                    object resultado = cmd.ExecuteScalar();

                    if (resultado != null)
                    {
                        puntosPorKg =
                            Convert.ToDecimal(resultado);
                    }
                }

                Registro.PuntosObtenidos =
                    Convert.ToInt32(
                        Math.Round(
                            Registro.CantidadKg * puntosPorKg));

                string insertarRegistro = @"
                    INSERT INTO RegistroReciclaje
                    (
                        IdUsuario,
                        IdMaterial,
                        IdCentro,
                        CantidadKg,
                        Fecha,
                        PuntosObtenidos
                    )
                    VALUES
                    (
                        @IdUsuario,
                        @IdMaterial,
                        @IdCentro,
                        @CantidadKg,
                        @Fecha,
                        @PuntosObtenidos
                    )";

                using (SqlCommand cmd =
                    new SqlCommand(insertarRegistro, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdUsuario",
                        Registro.IdUsuario);

                    cmd.Parameters.AddWithValue(
                        "@IdMaterial",
                        Registro.IdMaterial);

                    cmd.Parameters.AddWithValue(
                        "@IdCentro",
                        idCentro);

                    cmd.Parameters.AddWithValue(
                        "@CantidadKg",
                        Registro.CantidadKg);

                    cmd.Parameters.AddWithValue(
                        "@Fecha",
                        DateTime.Now);

                    cmd.Parameters.AddWithValue(
                        "@PuntosObtenidos",
                        Registro.PuntosObtenidos);

                    cmd.ExecuteNonQuery();
                }

                string actualizarUsuario = @"
                    UPDATE Usuario
                    SET Puntos = Puntos + @Puntos
                    WHERE IdUsuario = @IdUsuario";

                using (SqlCommand cmd =
                    new SqlCommand(actualizarUsuario, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@Puntos",
                        Registro.PuntosObtenidos);

                    cmd.Parameters.AddWithValue(
                        "@IdUsuario",
                        Registro.IdUsuario);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        private void CargarMateriales()
        {
            Materiales.Clear();

            string conexion =
                _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string consulta = @"
                    SELECT
                        IdMaterial,
                        Nombre,
                        Descripcion,
                        PuntosPorKg
                    FROM Material";

                using (SqlCommand cmd =
                    new SqlCommand(consulta, cn))
                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Materiales.Add(new Material
                        {
                            IdMaterial =
                                Convert.ToInt32(
                                    reader["IdMaterial"]),

                            Nombre =
                                reader["Nombre"].ToString() ?? "",

                            Descripcion =
                                reader["Descripcion"].ToString() ?? "",

                            PuntosPorKg =
                                Convert.ToDecimal(
                                    reader["PuntosPorKg"])
                        });
                    }
                }
            }
        }
    }
}