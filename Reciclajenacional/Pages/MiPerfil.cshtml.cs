using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReciclajeNacional.Pages
{
    public class MiPerfilModel : PageModel
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Provincia { get; set; }
        public int Puntos { get; set; }

        // Cargar datos
        public void OnGet()
        {
            Nombre = HttpContext.Session.GetString("Nombre") ?? "";
            Correo = HttpContext.Session.GetString("Correo") ?? "";
            Provincia = HttpContext.Session.GetString("Provincia") ?? "";

            if (int.TryParse(HttpContext.Session.GetString("Puntos"), out int puntos))
            {
                Puntos = puntos;
            }
        }

        // Cerrar sesión
        public IActionResult OnPostCerrarSesion()
        {
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}