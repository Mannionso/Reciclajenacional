namespace ReciclajeNacional.POO
{
    public class RegistroReciclaje
    {
        public int IdRegistro { get; set; }

        public int IdUsuario { get; set; }

        public int IdMaterial { get; set; }

        public int IdCentro { get; set; }

        public decimal CantidadKg { get; set; }

        public DateTime Fecha { get; set; }

        public int PuntosObtenidos { get; set; }
    }
}