using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReciclajeNacional.Pages
{
    public class CentrosModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Provincia { get; set; } = "Todas";

        [BindProperty(SupportsGet = true)]
        public string Busqueda { get; set; } = "";

        public List<CentroReciclaje> Centros { get; set; } = new List<CentroReciclaje>();

        public CentroReciclaje? CentroSeleccionado { get; set; }

        public void OnGet(int? id)
        {
            // Cargamos todos los centros
            CargarCentros();

            // Si se seleccionó un centro con "Más información"
            if (id.HasValue)
            {
                CentroSeleccionado = Centros
                    .FirstOrDefault(c => c.Id == id.Value);

                // Si existe el centro, mostramos solamente ese centro
                if (CentroSeleccionado != null)
                {
                    Centros = new List<CentroReciclaje>
                    {
                        CentroSeleccionado
                    };
                }

                return;
            }

            // Aplicar búsqueda
            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                Centros = Centros
                    .Where(c =>
                        c.Nombre.Contains(
                            Busqueda,
                            StringComparison.OrdinalIgnoreCase) ||

                        c.Ubicacion.Contains(
                            Busqueda,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Aplicar filtro por provincia
            if (!string.IsNullOrWhiteSpace(Provincia) &&
                Provincia != "Todas")
            {
                Centros = Centros
                    .Where(c => c.Provincia == Provincia)
                    .ToList();
            }
        }

        private void CargarCentros()
        {
            Centros = new List<CentroReciclaje>
            {
                new CentroReciclaje
                {
                    Id = 1,
                    Nombre = "Centro de Reciclaje Municipal de San José",
                    Provincia = "San José",
                    Ubicacion = "Hatillo 2, Avenida Central, San José",
                    Horario = "Lunes a domingo, 6:00 a.m. - 6:00 p.m.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros residuos valorizables."
                },

                new CentroReciclaje
                {
                    Id = 2,
                    Nombre = "Centro de Recuperación de Alajuela",
                    Provincia = "Alajuela",
                    Ubicacion = "Muelle de San Carlos, Alajuela",
                    Horario = "Lunes a viernes, 7:00 a.m. - 4:00 p.m.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros materiales valorizables."
                },

                new CentroReciclaje
                {
                    Id = 3,
                    Nombre = "Centro de Recuperación de Cartago",
                    Provincia = "Cartago",
                    Ubicacion = "Pacayas de Alvarado, Cartago",
                    Horario = "Lunes a viernes, 7:00 a.m. - 4:00 p.m.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros materiales valorizables."
                },

                new CentroReciclaje
                {
                    Id = 4,
                    Nombre = "Centro de Acopio y Reciclaje San Rafael",
                    Provincia = "Heredia",
                    Ubicacion = "San Rafael de Heredia, Heredia",
                    Horario = "Lunes a viernes, 6:00 a.m. - 12:00 m.d.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros materiales valorizables."
                },

                new CentroReciclaje
                {
                    Id = 5,
                    Nombre = "Centro de Recuperación de Nandayure",
                    Provincia = "Guanacaste",
                    Ubicacion = "Carmona de Nandayure, 300 metros oeste de la Cámara de Ganaderos",
                    Horario = "Lunes a viernes, 7:00 a.m. - 4:00 p.m.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros materiales valorizables."
                },

                new CentroReciclaje
                {
                    Id = 6,
                    Nombre = "Centro de Recuperación de Los Diamantes",
                    Provincia = "Limón",
                    Ubicacion = "La Emilia de Guápiles, Estación Experimental Los Diamantes",
                    Horario = "Lunes a viernes, 7:00 a.m. - 4:00 p.m.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros materiales valorizables."
                },

                new CentroReciclaje
                {
                    Id = 7,
                    Nombre = "Centro de Recolección de Garabito",
                    Provincia = "Puntarenas",
                    Ubicacion = "Jacó, Ruta 34, contiguo al puente del Río La Mona",
                    Horario = "Lunes a viernes, 8:00 a.m. - 3:00 p.m.",
                    Materiales = "Papel, cartón, plástico, vidrio, aluminio y otros materiales valorizables."
                }
            };
        }

        public class CentroReciclaje
        {
            public int Id { get; set; }

            public string Nombre { get; set; } = "";

            public string Provincia { get; set; } = "";

            public string Ubicacion { get; set; } = "";

            public string Horario { get; set; } = "";

            public string Materiales { get; set; } = "";
        }
    }
}