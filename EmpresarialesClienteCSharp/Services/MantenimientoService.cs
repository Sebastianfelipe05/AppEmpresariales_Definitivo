using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EmpresarialesClienteCSharp.Models;

namespace EmpresarialesClienteCSharp.Services
{
    public class MantenimientoService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "http://localhost:8080/api/mantenimiento";

        public MantenimientoService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            // Configurar autenticación Basic Auth para Spring Security
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:admin"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            // Configurar headers adicionales
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Listar todos los mantenimientos
        public async Task<List<Mantenimiento>> ObtenerTodosLosMantenimientosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BASE_URL);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Mantenimiento>>(content) ?? new List<Mantenimiento>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los mantenimientos: {ex.Message}", ex);
            }
        }

        // Buscar mantenimiento por ID
        public async Task<Mantenimiento?> BuscarPorIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}?id={id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var mantenimientos = JsonConvert.DeserializeObject<List<Mantenimiento>>(content);
                return mantenimientos?.Count > 0 ? mantenimientos[0] : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar el mantenimiento: {ex.Message}", ex);
            }
        }

        // Buscar mantenimientos por placa de carro
        public async Task<List<Mantenimiento>> BuscarPorCarroAsync(string placaCarro)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}/carro/{placaCarro}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Mantenimiento>>(content) ?? new List<Mantenimiento>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar mantenimientos del carro: {ex.Message}", ex);
            }
        }

        // Buscar mantenimientos por criterios
        public async Task<List<Mantenimiento>> BuscarPorCriteriosAsync(Dictionary<string, string> criterios)
        {
            try
            {
                var query = string.Join("&", criterios.Select(kv => $"{kv.Key}={kv.Value}"));
                var response = await _httpClient.GetAsync($"{BASE_URL}?{query}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Mantenimiento>>(content) ?? new List<Mantenimiento>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar mantenimientos: {ex.Message}", ex);
            }
        }

        // Obtener mantenimientos urgentes
        public async Task<List<Mantenimiento>> ObtenerMantenimientosUrgentesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}?action=urgentes");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Mantenimiento>>(content) ?? new List<Mantenimiento>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener mantenimientos urgentes: {ex.Message}", ex);
            }
        }

        // Obtener estadísticas
        public async Task<MantenimientoEstadisticas> ObtenerEstadisticasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}?action=estadisticas");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<MantenimientoEstadisticas>(content)
                    ?? new MantenimientoEstadisticas();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}", ex);
            }
        }

        // Crear nuevo mantenimiento
        public async Task<Mantenimiento> CrearMantenimientoAsync(Mantenimiento mantenimiento)
        {
            try
            {
                // Convertir a DTO con el formato correcto para el backend
                var dto = MantenimientoRequestDto.FromMantenimiento(mantenimiento);

                var json = JsonConvert.SerializeObject(dto, new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    NullValueHandling = NullValueHandling.Ignore
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BASE_URL, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Mantenimiento>(responseContent)
                    ?? throw new Exception("No se pudo crear el mantenimiento");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el mantenimiento: {ex.Message}", ex);
            }
        }

        // Actualizar mantenimiento existente
        public async Task<Mantenimiento> ActualizarMantenimientoAsync(string id, Mantenimiento mantenimiento)
        {
            try
            {
                // Convertir a DTO con el formato correcto para el backend
                var dto = MantenimientoRequestDto.FromMantenimiento(mantenimiento);
                dto.Id = id;

                var json = JsonConvert.SerializeObject(dto, new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    NullValueHandling = NullValueHandling.Ignore
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BASE_URL}/{id}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Mantenimiento>(responseContent)
                    ?? throw new Exception("No se pudo actualizar el mantenimiento");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el mantenimiento: {ex.Message}", ex);
            }
        }

        // Eliminar mantenimiento
        public async Task<bool> EliminarMantenimientoAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BASE_URL}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el mantenimiento: {ex.Message}", ex);
            }
        }

        // Filtrar mantenimientos localmente (útil para búsquedas avanzadas)
        public List<Mantenimiento> FiltrarMantenimientos(List<Mantenimiento> mantenimientos,
            string? tipoMantenimiento = null,
            bool? completado = null,
            int? kilometrajeMin = null,
            int? kilometrajeMax = null)
        {
            var resultado = mantenimientos.AsEnumerable();

            if (!string.IsNullOrEmpty(tipoMantenimiento))
            {
                resultado = resultado.Where(m => m.TipoMantenimiento.Equals(tipoMantenimiento,
                    StringComparison.OrdinalIgnoreCase));
            }

            if (completado.HasValue)
            {
                resultado = resultado.Where(m => m.Completado == completado.Value);
            }

            if (kilometrajeMin.HasValue)
            {
                resultado = resultado.Where(m => m.Kilometraje >= kilometrajeMin.Value);
            }

            if (kilometrajeMax.HasValue)
            {
                resultado = resultado.Where(m => m.Kilometraje <= kilometrajeMax.Value);
            }

            return resultado.ToList();
        }

        // Ordenar mantenimientos
        public List<Mantenimiento> OrdenarMantenimientos(List<Mantenimiento> mantenimientos,
            string criterio = "fecha", bool descendente = true)
        {
            return criterio.ToLower() switch
            {
                "fecha" => descendente
                    ? mantenimientos.OrderByDescending(m => m.FechaMantenimiento).ToList()
                    : mantenimientos.OrderBy(m => m.FechaMantenimiento).ToList(),
                "costo" => descendente
                    ? mantenimientos.OrderByDescending(m => m.Costo).ToList()
                    : mantenimientos.OrderBy(m => m.Costo).ToList(),
                "kilometraje" => descendente
                    ? mantenimientos.OrderByDescending(m => m.Kilometraje).ToList()
                    : mantenimientos.OrderBy(m => m.Kilometraje).ToList(),
                _ => mantenimientos
            };
        }
    }
}
