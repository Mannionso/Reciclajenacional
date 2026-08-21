using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReciclajeNacional.Pages
{
    public class IndexModel : PageModel
    {
        public string NombreUsuario { get; set; } = "";

        public void OnGet()
        {
            NombreUsuario = HttpContext.Session.GetString("NombreUsuario") ?? "";
        }
    }
}