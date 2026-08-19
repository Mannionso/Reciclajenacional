using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using ReciclajeNacional.POO;

namespace ReciclajeNacional.Pages
{
    public class MaterialesModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public MaterialesModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Material> Materiales { get; set; } = new List<Material>();

        [BindProperty]
        public Material Material { get; set; } = new Material();

        public void OnGet()
        {
            CargarMateriales();
        }

        public IActionResult OnPost()
        {
            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"INSERT INTO Material
                               (Nombre, Descripcion, PuntosPorKg)
                               VALUES
                               (@Nombre, @Descripcion, @PuntosPorKg)";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", Material.Nombre);
                    cmd.Parameters.AddWithValue("@Descripcion", Material.Descripcion);
                    cmd.Parameters.AddWithValue("@PuntosPorKg", Material.PuntosPorKg);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        private void CargarMateriales()
        {
            string conexion = _configuration.GetConnectionString("Conexion");

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = "SELECT IdMaterial, Nombre, Descripcion, PuntosPorKg FROM Material";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
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
        }
    }
}