using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppInsegura.Servicios
{
    // Servicio encargado de gestionar el envío de datos a la API remota
    public class RedService
    {
        // Se obtiene la clave de API desde las variables de entorno del sistema operativo
        // Esto evita hardcodear credenciales secretas dentro del código compilado
        private readonly string apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "";

        // Dirección endpoint de la API protegida con el protocolo HTTPS para garantizar el cifrado TLS
        private const string UrlServidor = "https://api.miapp-insegura.local/puntuaciones";

        // Método público y síncrono que sirve como punto de entrada para el resto de la aplicación
        public void EnviarPuntuacion(string nombreUsuario, int puntuacion)
        {
            try
            {
                // Invoca la versión asíncrona de forma bloqueante para completar la tarea antes de continuar
                EnviarPuntuacionAsync(nombreUsuario, puntuacion).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Captura cualquier fallo de red o validación y muestra una alerta amigable en la consola
                // Se ocultan detalles internos del stack trace para no exponer información sensible
                Console.WriteLine("No se ha podido conectar con el servidor (es normal si no tienes conexión real):");
                Console.WriteLine(ex.Message);
            }
        }

        // Método privado asíncrono que realiza la construcción de la petición y la llamada HTTP
        private async Task EnviarPuntuacionAsync(string nombreUsuario, int puntuacion)
        {
            // Saneamiento de datos: se comprueba que el nombre tenga contenido y no supere el límite permitido
            if (string.IsNullOrWhiteSpace(nombreUsuario) || nombreUsuario.Length > 20)
            {
                throw new ArgumentException("Nombre de usuario no válido.");
            }

            // Control de rango de negocio: impide procesar puntuaciones negativas
            if (puntuacion < 0)
            {
                throw new ArgumentException("La puntuación no puede ser negativa.");
            }

            // Instancia del cliente HTTP; la instrucción 'using' garantiza la liberación del socket al terminar
            using var cliente = new HttpClient();

            // Objeto anónimo con los datos que se enviarán en el cuerpo de la petición
            var datos = new
            {
                usuario = nombreUsuario,
                puntos = puntuacion
            };

            // Conversión del objeto C# a formato JSON y preparación del contenido con codificación UTF-8
            string json = JsonSerializer.Serialize(datos);
            using var contenido = new StringContent(json, Encoding.UTF8, "application/json");

            // Inyección del token de autenticación en la cabecera HTTP (Header) en vez de exponerlo en la URL
            cliente.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            // Registro de consola informativo sin incluir URLs con parámetros ni tokens sensibles
            Console.WriteLine($"Enviando puntuación de {nombreUsuario} al servidor...");

            // Envío de la petición HTTP POST de manera asíncrona al servidor web
            HttpResponseMessage respuesta = await cliente.PostAsync(UrlServidor, contenido);

            // Impresión del código de estado HTTP recibido (ej. 200 OK, 401 Unauthorized, etc.)
            Console.WriteLine($"Respuesta del servidor: {(int)respuesta.StatusCode}");
        }
    }
}