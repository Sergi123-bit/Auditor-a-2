using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AppInsegura.Datos;
using AppInsegura.Modelos;

namespace AppInsegura.Servicios
{
    public class AuthService
    {
        private readonly BaseDatosUsuarios baseDatos;

        public AuthService(BaseDatosUsuarios baseDatos)
        {
            this.baseDatos = baseDatos;
        }

        public Usuario Registrar(string nombre, string contrasena, string rol = "jugador")
        {
            // CORRECCIÓN 1: Validacion de entradas (evita valores nulos, vacíos o cadenas largas)
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 20)
            {
                throw new ArgumentException("Nombre de usuario no v�lido.");
            }

            if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 8)
            {
                throw new ArgumentException("La contrase�a debe tener al menos 8 caracteres.");
            }

            // CORRECCIÓN 1 (continuación): Lista blanca para evitar escalada de privilegios en el rol
             if (rol != "jugador" && rol != "admin")
             {
                 rol = "jugador";
            }

            var nuevo = new Usuario
            {
                Nombre = nombre,
                ContrasenaHash = CalcularHash(contrasena),
                Rol = rol,
                TokenSesion = ""
            };

            baseDatos.Agregar(nuevo);
            return nuevo;
        }

        public Usuario? IniciarSesion(string nombre, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contrasena))
            {
                return null;
            }

            Usuario? usuario = baseDatos.BuscarExacto(nombre);
            if (usuario == null)
            {
                return null;
            }

            string hashIntento = CalcularHash(contrasena);
            if (usuario.ContrasenaHash != hashIntento)
            {
                return null;
            }

            usuario.TokenSesion = GenerarTokenSesion();

            // CORRECCIÓN 4: Eliminación del token de sesión del log de consola
            // Prevención de fugas de información: no se registran datos sensibles
            // (como el token de sesión) en la consola o los registros
            Console.WriteLine($"[LOG] Login correcto -> usuario: {usuario.Nombre}");

            GuardarSesionEnDisco(usuario);

            return usuario;
        }

        private string CalcularHash(string contrasena)
        {
            // CORRECCIÓN 2: Reemplazo de MD5 por SHA-256 (algoritmo seguro de hashing)
            // Criptografía segura: se reemplaza MD5 por SHA-256 para evitar colisiones y el uso de algoritmos obsoletos
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(contrasena));
            return Convert.ToHexString(bytes);
        }

        private string GenerarTokenSesion()
        {
            // CORRECCIÓN 3: Reemplazo de System.Random por un generador de números aleatorios criptográficamente seguro
            byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToHexString(randomBytes);
        }

        private void GuardarSesionEnDisco(Usuario usuario)
        {
            // CORRECCIÓN 5: No guardar tokens ni datos sensibles en texto plano en el fichero local
            // Protección de datos persistidos: se elimina la escritura de tokens de sesión en archivos de texto plano locales
            File.WriteAllText("sesion.txt", $"{usuario.Nombre}");
        }
    }
}
