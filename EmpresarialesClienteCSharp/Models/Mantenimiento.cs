using System;
using Newtonsoft.Json;

namespace EmpresarialesClienteCSharp.Models
{
    /// <summary>
    /// Clase Mantenimiento - Representa el detalle de mantenimientos de un Carro
    /// Relación: Carro (1) <---> (0..*) Mantenimiento
    /// </summary>
    public class Mantenimiento
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("placaCarro")]
        public string PlacaCarro { get; set; } = string.Empty;

        [JsonProperty("fechaMantenimiento")]
        public DateTime? FechaMantenimiento { get; set; }

        [JsonProperty("kilometraje")]
        public int Kilometraje { get; set; }

        [JsonProperty("tipoMantenimiento")]
        public string TipoMantenimiento { get; set; } = string.Empty;

        [JsonProperty("costo")]
        public double Costo { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonProperty("proximoMantenimiento")]
        public DateTime? ProximoMantenimiento { get; set; }

        [JsonProperty("completado")]
        public bool Completado { get; set; }

        [JsonProperty("fechaRegistro")]
        public DateTime? FechaRegistro { get; set; }

        // Propiedades calculadas (opcionales, pueden no venir del backend)
        [JsonProperty("estadoMantenimiento")]
        public string? EstadoMantenimiento { get; set; }

        [JsonProperty("esUrgente")]
        public bool? EsUrgente { get; set; }

        [JsonProperty("costoConImpuesto")]
        public double? CostoConImpuesto { get; set; }

        // Constructor por defecto
        public Mantenimiento()
        {
            Id = Guid.NewGuid().ToString();
            FechaRegistro = DateTime.Now;
            Completado = false;
        }

        // Constructor completo
        public Mantenimiento(string placaCarro, DateTime fechaMantenimiento, int kilometraje,
                           string tipoMantenimiento, double costo, string descripcion,
                           DateTime? proximoMantenimiento = null)
        {
            Id = Guid.NewGuid().ToString();
            PlacaCarro = placaCarro;
            FechaMantenimiento = fechaMantenimiento;
            Kilometraje = kilometraje;
            TipoMantenimiento = tipoMantenimiento;
            Costo = costo;
            Descripcion = descripcion;
            ProximoMantenimiento = proximoMantenimiento;
            Completado = false;
            FechaRegistro = DateTime.Now;
        }

        // Método para calcular si es urgente
        public bool CalcularEsUrgente()
        {
            if (ProximoMantenimiento == null)
            {
                return false;
            }
            DateTime ahora = DateTime.Now;
            return ProximoMantenimiento.Value < ahora.AddDays(7);
        }

        // Método para obtener el estado del mantenimiento
        public string ObtenerEstadoMantenimiento()
        {
            if (Completado)
            {
                return "COMPLETADO";
            }
            if (CalcularEsUrgente())
            {
                return "URGENTE";
            }
            return "PENDIENTE";
        }

        // Método para calcular costo con impuesto
        public double CalcularCostoConImpuesto()
        {
            return Costo * 1.19; // IVA del 19%
        }

        // Obtener descripción corta para mostrar en listas
        public string GetDescripcionCorta()
        {
            if (Descripcion.Length <= 50)
            {
                return Descripcion;
            }
            return Descripcion.Substring(0, 47) + "...";
        }

        // Formatear fecha para mostrar
        public string GetFechaMantenimientoFormateada()
        {
            return FechaMantenimiento?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
        }

        public string GetProximoMantenimientoFormateado()
        {
            return ProximoMantenimiento?.ToString("dd/MM/yyyy") ?? "N/A";
        }

        // ToString para mostrar información básica
        public override string ToString()
        {
            return $"{TipoMantenimiento} - {PlacaCarro} ({GetFechaMantenimientoFormateada()})";
        }

        // Método para obtener información completa
        public string GetInformacionCompleta()
        {
            return $@"
=== MANTENIMIENTO ===
ID: {Id}
Placa Carro: {PlacaCarro}
Fecha: {GetFechaMantenimientoFormateada()}
Kilometraje: {Kilometraje:N0} km
Tipo: {TipoMantenimiento}
Costo: ${Costo:N2}
Descripción: {Descripcion}
Próximo Mantenimiento: {GetProximoMantenimientoFormateado()}
Estado: {ObtenerEstadoMantenimiento()}
Completado: {(Completado ? "Sí" : "No")}
";
        }
    }

    /// <summary>
    /// Clase para estadísticas de mantenimientos
    /// </summary>
    public class MantenimientoEstadisticas
    {
        [JsonProperty("totalMantenimientos")]
        public int TotalMantenimientos { get; set; }

        [JsonProperty("costoTotal")]
        public double CostoTotal { get; set; }

        [JsonProperty("costoPromedio")]
        public double CostoPromedio { get; set; }

        [JsonProperty("mantenimientosUrgentes")]
        public int MantenimientosUrgentes { get; set; }

        public override string ToString()
        {
            return $@"
=== ESTADÍSTICAS DE MANTENIMIENTOS ===
Total de mantenimientos: {TotalMantenimientos}
Costo total: ${CostoTotal:N2}
Costo promedio: ${CostoPromedio:N2}
Mantenimientos urgentes: {MantenimientosUrgentes}
";
        }
    }

    /// <summary>
    /// DTO para crear y actualizar mantenimientos
    /// El backend espera un objeto "carro" con la placa, no solo "placaCarro"
    /// </summary>
    public class MantenimientoRequestDto
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("carro")]
        public CarroRef Carro { get; set; } = new CarroRef();

        [JsonProperty("fechaMantenimiento")]
        public DateTime? FechaMantenimiento { get; set; }

        [JsonProperty("kilometraje")]
        public int Kilometraje { get; set; }

        [JsonProperty("tipoMantenimiento")]
        public string TipoMantenimiento { get; set; } = string.Empty;

        [JsonProperty("costo")]
        public double Costo { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonProperty("proximoMantenimiento")]
        public DateTime? ProximoMantenimiento { get; set; }

        [JsonProperty("completado")]
        public bool Completado { get; set; }

        /// <summary>
        /// Clase interna para la referencia al carro
        /// </summary>
        public class CarroRef
        {
            [JsonProperty("placa")]
            public string Placa { get; set; } = string.Empty;
        }

        /// <summary>
        /// Convierte un Mantenimiento a MantenimientoRequestDto
        /// </summary>
        public static MantenimientoRequestDto FromMantenimiento(Mantenimiento mantenimiento)
        {
            return new MantenimientoRequestDto
            {
                Id = mantenimiento.Id,
                Carro = new CarroRef { Placa = mantenimiento.PlacaCarro },
                FechaMantenimiento = mantenimiento.FechaMantenimiento,
                Kilometraje = mantenimiento.Kilometraje,
                TipoMantenimiento = mantenimiento.TipoMantenimiento,
                Costo = mantenimiento.Costo,
                Descripcion = mantenimiento.Descripcion,
                ProximoMantenimiento = mantenimiento.ProximoMantenimiento,
                Completado = mantenimiento.Completado
            };
        }
    }
}
