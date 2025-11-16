using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EmpresarialesClienteCSharp.Models;
using Newtonsoft.Json;

namespace EmpresarialesClienteCSharp.Services
{
    public class ConductorService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "http://localhost:8080/api/conductor";
        private const string USERNAME = "admin";
        private const string PASSWORD = "admin";

        public ConductorService()
        {
            _httpClient = new HttpClient();

            // Configurar autenticación básica
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{PASSWORD}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // CREATE - Crear conductor
        public async Task<Conductor> CrearConductorAsync(Conductor conductor)
        {
            try
            {
                // Convertir fechaNacimiento a formato ISO 8601
                var json = JsonConvert.SerializeObject(new
                {
                    cedula = conductor.Cedula,
                    nombre = conductor.Nombre,
                    apellido = conductor.Apellido,
                    telefono = conductor.Telefono,
                    licenciaNumero = conductor.LicenciaNumero,
                    fechaNacimiento = conductor.FechaNacimiento.ToString("yyyy-MM-ddTHH:mm:ss"),
                    salario = conductor.Salario,
                    activo = conductor.Activo
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BASE_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Conductor>(responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Mejorar mensajes de error específicos
                    string errorMessage = errorContent;

                    if (errorContent.Contains("CEDULA") || errorContent.Contains("cedula") || errorContent.Contains("ConstraintViolation"))
                    {
                        errorMessage = "❌ Esta cédula ya está registrada en el sistema";
                    }
                    else if (errorContent.Contains("LICENCIA") || errorContent.Contains("licencia"))
                    {
                        errorMessage = "❌ Este número de licencia ya está registrado en el sistema";
                    }
                    else if (errorContent.Contains("fechaNacimiento") || errorContent.Contains("debe ser en el pasado"))
                    {
                        errorMessage = "❌ La fecha de nacimiento debe ser en el pasado";
                    }
                    else if (errorContent.Contains("salario"))
                    {
                        errorMessage = "❌ El salario debe ser un valor positivo mayor a 0";
                    }
                    else if (errorContent.Contains("cédula debe contener"))
                    {
                        errorMessage = "❌ La cédula debe contener solo números (6 a 20 dígitos)";
                    }
                    else if (errorContent.Contains("teléfono debe contener"))
                    {
                        errorMessage = "❌ El teléfono debe contener solo números (7 a 20 dígitos)";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        errorMessage = "❌ Los datos ingresados no cumplen con las validaciones requeridas. Verifique:\n" +
                                      "• Cédula: solo números (6-20 dígitos), única\n" +
                                      "• Teléfono: solo números (7-20 dígitos)\n" +
                                      "• Licencia: 5-50 caracteres, única\n" +
                                      "• Fecha nacimiento: debe ser en el pasado\n" +
                                      "• Salario: mayor a 0";
                    }

                    throw new Exception($"Error al crear conductor: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error de conexión: {ex.Message}", ex);
            }
        }

        // READ - Obtener conductor por cédula
        public async Task<Conductor> ObtenerConductorPorCedulaAsync(string cedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}/{cedula}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Conductor>(content);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Conductor con cédula {cedula} no encontrado");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener conductor: {ex.Message}", ex);
            }
        }

        // READ - Listar todos los conductores
        public async Task<List<Conductor>> ListarTodosConductoresAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BASE_URL);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Conductor>>(content) ?? new List<Conductor>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar conductores: {ex.Message}", ex);
            }
        }

        // READ - Buscar conductores por nombre
        public async Task<List<Conductor>> BuscarConductoresPorNombreAsync(string nombre)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}?nombre={Uri.EscapeDataString(nombre)}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Conductor>>(content) ?? new List<Conductor>();
                }
                else
                {
                    throw new Exception($"Error al buscar conductores: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar conductores: {ex.Message}", ex);
            }
        }

        // READ - Buscar conductores activos
        public async Task<List<Conductor>> BuscarConductoresActivosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}?activo=true");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Conductor>>(content) ?? new List<Conductor>();
                }
                else
                {
                    throw new Exception($"Error al buscar conductores activos: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}", ex);
            }
        }

        // UPDATE - Actualizar conductor
        public async Task<Conductor> ActualizarConductorAsync(string cedula, Conductor conductor)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new
                {
                    cedula = conductor.Cedula,
                    nombre = conductor.Nombre,
                    apellido = conductor.Apellido,
                    telefono = conductor.Telefono,
                    licenciaNumero = conductor.LicenciaNumero,
                    fechaNacimiento = conductor.FechaNacimiento.ToString("yyyy-MM-ddTHH:mm:ss"),
                    salario = conductor.Salario,
                    activo = conductor.Activo
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BASE_URL}/{cedula}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Conductor>(responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Mejorar mensajes de error específicos
                    string errorMessage = errorContent;

                    if (errorContent.Contains("LICENCIA") || errorContent.Contains("licencia"))
                    {
                        errorMessage = "❌ Este número de licencia ya está registrado en el sistema";
                    }
                    else if (errorContent.Contains("fechaNacimiento") || errorContent.Contains("debe ser en el pasado"))
                    {
                        errorMessage = "❌ La fecha de nacimiento debe ser en el pasado";
                    }
                    else if (errorContent.Contains("salario"))
                    {
                        errorMessage = "❌ El salario debe ser un valor positivo mayor a 0";
                    }
                    else if (errorContent.Contains("teléfono debe contener"))
                    {
                        errorMessage = "❌ El teléfono debe contener solo números (7 a 20 dígitos)";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        errorMessage = "❌ Los datos ingresados no cumplen con las validaciones requeridas. Verifique:\n" +
                                      "• Teléfono: solo números (7-20 dígitos)\n" +
                                      "• Licencia: 5-50 caracteres, única\n" +
                                      "• Fecha nacimiento: debe ser en el pasado\n" +
                                      "• Salario: mayor a 0";
                    }

                    throw new Exception($"Error al actualizar conductor: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error de conexión: {ex.Message}", ex);
            }
        }

        // DELETE - Eliminar conductor
        public async Task<bool> EliminarConductorAsync(string cedula)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BASE_URL}/{cedula}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception($"Conductor con cédula {cedula} no encontrado");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar conductor: {ex.Message}", ex);
            }
        }

        // Health check
        public async Task<bool> VerificarConexionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BASE_URL}/healthCheck");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
