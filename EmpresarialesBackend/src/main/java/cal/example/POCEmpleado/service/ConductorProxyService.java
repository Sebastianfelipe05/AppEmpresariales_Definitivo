package cal.example.POCEmpleado.service;

import cal.example.POCEmpleado.model.ConductorDTO;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.*;
import org.springframework.stereotype.Service;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.web.client.RestTemplate;

import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;
import java.util.*;

/**
 * Servicio Proxy para comunicarse con el Microservicio de Conductores
 * Implementa el patrón Proxy para hacer transparente la comunicación entre microservicios
 */
@Service
@Slf4j
public class ConductorProxyService {

    @Value("${conductor.service.url:http://localhost:8081/api/conductor}")
    private String conductorServiceUrl;

    @Value("${conductor.service.username:admin}")
    private String username;

    @Value("${conductor.service.password:admin}")
    private String password;

    private final RestTemplate restTemplate;

    public ConductorProxyService(RestTemplate restTemplate) {
        this.restTemplate = restTemplate;
    }

    /**
     * Crear headers con autenticación HTTP Basic
     */
    private HttpHeaders createHeaders() {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        headers.setAccept(Collections.singletonList(MediaType.APPLICATION_JSON));

        String auth = username + ":" + password;
        byte[] encodedAuth = Base64.getEncoder().encode(auth.getBytes(StandardCharsets.UTF_8));
        String authHeader = "Basic " + new String(encodedAuth);
        headers.set("Authorization", authHeader);

        return headers;
    }

    /**
     * CREATE - Crear un nuevo conductor
     */
    public ConductorDTO crearConductor(ConductorDTO conductor) {
        try {
            log.info("Proxy: Creando conductor en microservicio - Cédula: {}", conductor.getCedula());

            HttpHeaders headers = createHeaders();
            HttpEntity<ConductorDTO> request = new HttpEntity<>(conductor, headers);

            ResponseEntity<ConductorDTO> response = restTemplate.exchange(
                conductorServiceUrl,
                HttpMethod.POST,
                request,
                ConductorDTO.class
            );

            log.info("Proxy: Conductor creado exitosamente - {}", conductor.getCedula());
            return response.getBody();

        } catch (HttpClientErrorException e) {
            log.error("Proxy: Error al crear conductor - Status: {} - Error: {}",
                e.getStatusCode(), e.getResponseBodyAsString());
            throw new RuntimeException("Error al crear conductor en microservicio: " + e.getMessage());
        } catch (Exception e) {
            log.error("Proxy: Error de conexión con microservicio de conductores: {}", e.getMessage());
            throw new RuntimeException("No se pudo conectar con el microservicio de conductores: " + e.getMessage());
        }
    }

    /**
     * READ - Obtener conductor por cédula
     */
    public Optional<ConductorDTO> obtenerConductorPorCedula(String cedula) {
        try {
            log.info("Proxy: Obteniendo conductor por cédula: {}", cedula);

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            ResponseEntity<ConductorDTO> response = restTemplate.exchange(
                conductorServiceUrl + "/" + cedula,
                HttpMethod.GET,
                request,
                ConductorDTO.class
            );

            log.info("Proxy: Conductor encontrado - {}", cedula);
            return Optional.ofNullable(response.getBody());

        } catch (HttpClientErrorException.NotFound e) {
            log.warn("Proxy: Conductor no encontrado - {}", cedula);
            return Optional.empty();
        } catch (Exception e) {
            log.error("Proxy: Error al obtener conductor: {}", e.getMessage());
            throw new RuntimeException("Error al obtener conductor del microservicio: " + e.getMessage());
        }
    }

    /**
     * READ - Listar todos los conductores
     */
    public List<ConductorDTO> listarTodosConductores() {
        try {
            log.info("Proxy: Listando todos los conductores");

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            ResponseEntity<List<ConductorDTO>> response = restTemplate.exchange(
                conductorServiceUrl,
                HttpMethod.GET,
                request,
                new ParameterizedTypeReference<List<ConductorDTO>>() {}
            );

            List<ConductorDTO> conductores = response.getBody();
            log.info("Proxy: {} conductores encontrados", conductores != null ? conductores.size() : 0);
            return conductores != null ? conductores : new ArrayList<>();

        } catch (Exception e) {
            log.error("Proxy: Error al listar conductores: {}", e.getMessage());
            throw new RuntimeException("Error al listar conductores del microservicio: " + e.getMessage());
        }
    }

    /**
     * READ - Buscar conductores por nombre
     */
    public List<ConductorDTO> buscarPorNombre(String nombre) {
        try {
            log.info("Proxy: Buscando conductores por nombre: {}", nombre);

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            String url = conductorServiceUrl + "?nombre=" + nombre;

            ResponseEntity<List<ConductorDTO>> response = restTemplate.exchange(
                url,
                HttpMethod.GET,
                request,
                new ParameterizedTypeReference<List<ConductorDTO>>() {}
            );

            return response.getBody() != null ? response.getBody() : new ArrayList<>();

        } catch (Exception e) {
            log.error("Proxy: Error al buscar por nombre: {}", e.getMessage());
            throw new RuntimeException("Error al buscar conductores: " + e.getMessage());
        }
    }

    /**
     * READ - Buscar conductores activos
     */
    public List<ConductorDTO> buscarConductoresActivos() {
        try {
            log.info("Proxy: Buscando conductores activos");

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            String url = conductorServiceUrl + "?activo=true";

            ResponseEntity<List<ConductorDTO>> response = restTemplate.exchange(
                url,
                HttpMethod.GET,
                request,
                new ParameterizedTypeReference<List<ConductorDTO>>() {}
            );

            return response.getBody() != null ? response.getBody() : new ArrayList<>();

        } catch (Exception e) {
            log.error("Proxy: Error al buscar conductores activos: {}", e.getMessage());
            throw new RuntimeException("Error al buscar conductores activos: " + e.getMessage());
        }
    }

    /**
     * READ - Buscar conductores por estado activo (true/false)
     */
    public List<ConductorDTO> buscarPorEstadoActivo(Boolean activo) {
        try {
            log.info("Proxy: Buscando conductores con estado activo: {}", activo);

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            String url = conductorServiceUrl + "?activo=" + activo;

            ResponseEntity<List<ConductorDTO>> response = restTemplate.exchange(
                url,
                HttpMethod.GET,
                request,
                new ParameterizedTypeReference<List<ConductorDTO>>() {}
            );

            return response.getBody() != null ? response.getBody() : new ArrayList<>();

        } catch (Exception e) {
            log.error("Proxy: Error al buscar conductores por estado: {}", e.getMessage());
            throw new RuntimeException("Error al buscar conductores por estado: " + e.getMessage());
        }
    }

    /**
     * UPDATE - Actualizar conductor
     */
    public ConductorDTO actualizarConductor(String cedula, ConductorDTO conductor) {
        try {
            log.info("Proxy: Actualizando conductor: {}", cedula);

            HttpHeaders headers = createHeaders();
            HttpEntity<ConductorDTO> request = new HttpEntity<>(conductor, headers);

            ResponseEntity<ConductorDTO> response = restTemplate.exchange(
                conductorServiceUrl + "/" + cedula,
                HttpMethod.PUT,
                request,
                ConductorDTO.class
            );

            log.info("Proxy: Conductor actualizado exitosamente - {}", cedula);
            return response.getBody();

        } catch (HttpClientErrorException e) {
            log.error("Proxy: Error al actualizar conductor: {}", e.getMessage());
            throw new RuntimeException("Error al actualizar conductor: " + e.getMessage());
        } catch (Exception e) {
            log.error("Proxy: Error de conexión: {}", e.getMessage());
            throw new RuntimeException("Error de conexión con microservicio: " + e.getMessage());
        }
    }

    /**
     * DELETE - Eliminar conductor
     */
    public void eliminarConductor(String cedula) {
        try {
            log.info("Proxy: Eliminando conductor: {}", cedula);

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            // El microservicio devuelve JSON, no String simple
            restTemplate.exchange(
                conductorServiceUrl + "/" + cedula,
                HttpMethod.DELETE,
                request,
                Object.class  // Cambiar de String.class a Object.class para manejar JSON
            );

            log.info("Proxy: Conductor eliminado exitosamente - {}", cedula);

        } catch (HttpClientErrorException.NotFound e) {
            log.error("Proxy: Conductor no encontrado para eliminar - {}", cedula);
            throw new RuntimeException("Conductor no encontrado: " + cedula);
        } catch (Exception e) {
            log.error("Proxy: Error al eliminar conductor: {}", e.getMessage());
            // No lanzar error si el status code fue exitoso
            if (!e.getMessage().contains("Error while extracting response")) {
                throw new RuntimeException("Error al eliminar conductor: " + e.getMessage());
            }
            // Si es solo un error de extracción pero se eliminó, continuar
            log.warn("Proxy: Conductor eliminado pero hubo error al extraer respuesta");
        }
    }

    /**
     * Obtener estadísticas del microservicio de conductores
     */
    public Map<String, Object> obtenerEstadisticas() {
        try {
            log.info("Proxy: Obteniendo estadísticas de conductores");

            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            String url = conductorServiceUrl + "?action=estadisticas";

            ResponseEntity<Map<String, Object>> response = restTemplate.exchange(
                url,
                HttpMethod.GET,
                request,
                new ParameterizedTypeReference<Map<String, Object>>() {}
            );

            return response.getBody() != null ? response.getBody() : new HashMap<>();

        } catch (Exception e) {
            log.error("Proxy: Error al obtener estadísticas: {}", e.getMessage());
            throw new RuntimeException("Error al obtener estadísticas: " + e.getMessage());
        }
    }

    /**
     * Health check del microservicio de conductores
     */
    public boolean isHealthy() {
        try {
            HttpHeaders headers = createHeaders();
            HttpEntity<String> request = new HttpEntity<>(headers);

            ResponseEntity<String> response = restTemplate.exchange(
                conductorServiceUrl + "/healthCheck",
                HttpMethod.GET,
                request,
                String.class
            );

            return response.getStatusCode() == HttpStatus.OK;

        } catch (Exception e) {
            log.error("Proxy: Microservicio de conductores no disponible: {}", e.getMessage());
            return false;
        }
    }
}
