using System;

namespace EmpresarialesClienteCSharp.Models
{
    public class Conductor
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string LicenciaNumero { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public double Salario { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        // Método auxiliar para obtener nombre completo
        public string NombreCompleto => $"{Nombre} {Apellido}";

        // Método para calcular edad aproximada
        public int EdadAproximada
        {
            get
            {
                var hoy = DateTime.Now;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }

        public override string ToString()
        {
            return $"{Cedula} - {NombreCompleto} ({(Activo ? "Activo" : "Inactivo")})";
        }
    }
}
