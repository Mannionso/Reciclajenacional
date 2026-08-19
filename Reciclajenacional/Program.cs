using Microsoft.Data.SqlClient;
using ReciclajeNacional;

var builder = WebApplication.CreateBuilder(args);

// agregar servicios para razor pages
builder.Services.AddRazorPages();

var app = builder.Build();

// probar conexion con la base de datos
var conexion = new ConexionBD();

using (SqlConnection cn = conexion.ObtenerConexion())
{
    try
    {
        cn.Open();
        Console.WriteLine("CONEXION EXITOSA A LA BASE DE DATOS");
        cn.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR DE CONEXION: " + ex.Message);
    }
}

// configuracion de la aplicacion
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();