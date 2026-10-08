using System;
using System.Collections.Generic;
using System.Linq;
using AppInsegura.Modelos;

namespace AppInsegura.Datos
{
    // Simula una tabla de base de datos (equivalente a una tabla SQLite/sqflite).
    // No usa un motor real para que el proyecto compile sin dependencias externas.
    public class BaseDatosUsuarios
    {
        private readonly List<Usuario> usuarios = new List<Usuario>();

        public void Agregar(Usuario usuario)
        {
            usuarios.Add(usuario);
        }

        public List<Usuario> ListarTodos()
        {
            return usuarios;
        }

        public Usuario? BuscarExacto(string nombre)


        // Añado validación de entrada y si el nombre está vacío o es muy largo, devuelvo null
        {
            
        if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 20)
            {
             return null;
            }
            return usuarios.FirstOrDefault(u => u.Nombre == nombre);
        }

        public Usuario? BuscarPorNombre(string nombreBuscado)
        {
            // Validamos la entrada

            if (string.IsNullOrWhiteSpace(nombreBuscado) || nombreBuscado.Length > 20)
            {
             return null;
            }
            // Cambio la consulta concatenada por una parametrizada para evitar inyección SQL
            const string consulta = "SELECT * FROM usuarios WHERE nombre = @nombre";
            return EjecutarConsultaSimulada(consulta, nombreBuscado);
        }

        // Simulación simplificada de un motor de consultas, únicamente para
        // que el ejercicio se pueda ejecutar sin una base de datos real.
        // Interpreta la cadena "consulta" igual que lo haría un motor SQL básico.

        
        // Recibo el parámetro aparte y elimino el if que simulaba el ataque para demostrar que en un codigo normal eso es 
        // de mala practica 
        private Usuario? EjecutarConsultaSimulada(string consulta, string? parametro = null)
        {

               if (parametro == null)
            {
                return null;
            }
            return usuarios.FirstOrDefault(u => u.Nombre == parametro);
        }
    }
}
