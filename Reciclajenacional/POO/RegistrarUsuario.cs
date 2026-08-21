using Microsoft.Data.SqlClient;

namespace ReciclajeNacional.POO
{
    public class RegistrarUsuario
    {
        private readonly string _conexion;

        public RegistrarUsuario(string conexion)
        {
            _conexion = conexion;
        }

        public string Registrar(Usuario usuario)
        {
            using (SqlConnection cn = new SqlConnection(_conexion))
            {
                cn.Open();

                // Verificar si el correo ya existe
                string verificarCorreo = @"
                    SELECT COUNT(*)
                    FROM Usuario
                    WHERE Correo = @Correo";

                using (SqlCommand cmd = new SqlCommand(verificarCorreo, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@Correo",
                        usuario.Correo);

                    int existe =
                        Convert.ToInt32(cmd.ExecuteScalar());

                    if (existe > 0)
                    {
                        return "El correo ya está registrado.";
                    }
                }

                // Buscar el centro correspondiente a la provincia
                string buscarCentro = @"
                    SELECT TOP 1 IdCentro
                    FROM CentroReciclaje
                    WHERE Provincia = @Provincia";

                using (SqlCommand cmd = new SqlCommand(buscarCentro, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@Provincia",
                        usuario.Provincia);

                    object resultado =
                        cmd.ExecuteScalar();

                    if (resultado == null)
                    {
                        return "No existe un centro de reciclaje para esa provincia.";
                    }

                    usuario.IdCentro =
                        Convert.ToInt32(resultado);
                }

                // Registrar usuario
                string insertarUsuario = @"
                    INSERT INTO Usuario
                    (
                        Nombre,
                        Correo,
                        Provincia,
                        Puntos,
                        Contrasena,
                        IdCentro
                    )
                    VALUES
                    (
                        @Nombre,
                        @Correo,
                        @Provincia,
                        0,
                        @Contrasena,
                        @IdCentro
                    )";

                using (SqlCommand cmd =
                       new SqlCommand(insertarUsuario, cn))
                {
                    cmd.Parameters.AddWithValue(
                        "@Nombre",
                        usuario.Nombre);

                    cmd.Parameters.AddWithValue(
                        "@Correo",
                        usuario.Correo);

                    cmd.Parameters.AddWithValue(
                        "@Provincia",
                        usuario.Provincia);

                    cmd.Parameters.AddWithValue(
                        "@Contrasena",
                        usuario.Contrasena);

                    cmd.Parameters.AddWithValue(
                        "@IdCentro",
                        usuario.IdCentro);

                    cmd.ExecuteNonQuery();
                }
            }

            return "";
        }
    }
}