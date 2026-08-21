namespace ReciclajeNacional.POO
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; } = "";

        public string Correo { get; set; } = "";

        public string Provincia { get; set; } = "";

        public int Puntos { get; set; }

        public string Contrasena { get; set; } = "";

        public int IdCentro { get; set; }
    }
}