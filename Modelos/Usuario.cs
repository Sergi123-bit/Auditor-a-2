namespace AppInsegura.Modelos
// Define el espacio de nombres que agrupa los modelos de datos de la aplicación.
// Representa la entidad de usuario en el sistema.
// 'Nombre': Nombre del usuario en la aplicación.
// 'ContrasenaHash': Contraseña almacenada en formato cifrado/hash.
// 'Rol': Nivel de permisos asignado al usuario (por defecto "jugador").
// 'TokenSesion': Identificador para validar la sesión activa del usuario.

{
    public class Usuario
    {
        public string Nombre { get; set; } = "";
        public string ContrasenaHash { get; set; } = "";
        public string Rol { get; set; } = "jugador";
        public string TokenSesion { get; set; } = "";
    }
}
