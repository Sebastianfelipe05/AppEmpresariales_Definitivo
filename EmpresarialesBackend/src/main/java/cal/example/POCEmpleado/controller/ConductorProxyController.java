package cal.example.POCEmpleado.controller;

import cal.example.POCEmpleado.errors.ErrorMessage;
import cal.example.POCEmpleado.model.ConductorDTO;
import cal.example.POCEmpleado.service.ConductorProxyService;
import jakarta.validation.Valid;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.Optional;

/**
 * Controlador Proxy para Conductores
 * Expone endpoints en el microservicio principal que redirigen al microservicio de Conductores
 * Los clientes GUI solo se comunican con este controlador (puerto 8080)
 */
@RestController
@RequestMapping("/api/conductor")
@CrossOrigin(origins = "*")
@Slf4j
public class ConductorProxyController {

    @Autowired
    private ConductorProxyService conductorProxyService;

    // READ - Listar todos los conductores con filtros opcionales
    @GetMapping
    public ResponseEntity<?> listarConductores(
            @RequestParam(required = false) String nombre,
            @RequestParam(required = false) Boolean activo,
            @RequestParam(required = false) String action) {
        try {
            log.info("ProxyController: GET /api/conductor - Listar conductores");

            // Estadísticas
            if ("estadisticas".equalsIgnoreCase(action)) {
                Map<String, Object> estadisticas = conductorProxyService.obtenerEstadisticas();
                return ResponseEntity.ok(estadisticas);
            }

            // Buscar por nombre
            if (nombre != null && !nombre.isEmpty()) {
                List<ConductorDTO> conductores = conductorProxyService.buscarPorNombre(nombre);
                return ResponseEntity.ok(conductores);
            }

            // Buscar por estado activo
            if (activo != null) {
                List<ConductorDTO> conductores = conductorProxyService.buscarPorEstadoActivo(activo);
                return ResponseEntity.ok(conductores);
            }

            // Listar todos
            List<ConductorDTO> conductores = conductorProxyService.listarTodosConductores();
            return ResponseEntity.ok(conductores);

        } catch (Exception e) {
            log.error("ProxyController: Error al listar conductores: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                LocalDateTime.now(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // CREATE - Crear un nuevo conductor
    @PostMapping
    public ResponseEntity<?> crearConductor(@Valid @RequestBody ConductorDTO conductor) {
        try {
            log.info("ProxyController: POST /api/conductor - Crear conductor: {}", conductor.getCedula());
            ConductorDTO nuevoConductor = conductorProxyService.crearConductor(conductor);
            return new ResponseEntity<>(nuevoConductor, HttpStatus.CREATED);
        } catch (RuntimeException e) {
            log.error("ProxyController: Error al crear conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.BAD_REQUEST.value(),
                LocalDateTime.now(),
                "Error al crear conductor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.BAD_REQUEST);
        } catch (Exception e) {
            log.error("ProxyController: Error interno: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                LocalDateTime.now(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // Health check
    @GetMapping("/healthCheck")
    public ResponseEntity<String> healthCheck() {
        try {
            boolean isHealthy = conductorProxyService.isHealthy();
            if (isHealthy) {
                return ResponseEntity.ok("Proxy Conductor OK - Microservicio Conductores disponible");
            } else {
                return ResponseEntity.status(HttpStatus.SERVICE_UNAVAILABLE)
                    .body("Microservicio de Conductores no disponible");
            }
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.SERVICE_UNAVAILABLE)
                .body("Error al verificar microservicio de Conductores");
        }
    }

    // READ - Obtener conductor por cédula
    @GetMapping("/{cedula}")
    public ResponseEntity<?> obtenerConductorPorCedula(@PathVariable String cedula) {
        try {
            log.info("ProxyController: GET /api/conductor/{} - Obtener conductor", cedula);
            Optional<ConductorDTO> conductor = conductorProxyService.obtenerConductorPorCedula(cedula);

            if (conductor.isPresent()) {
                return ResponseEntity.ok(conductor.get());
            } else {
                ErrorMessage errorMessage = new ErrorMessage(
                    HttpStatus.NOT_FOUND.value(),
                    LocalDateTime.now(),
                    "Conductor no encontrado",
                    "No se encontró conductor con cédula: " + cedula
                );
                return new ResponseEntity<>(errorMessage, HttpStatus.NOT_FOUND);
            }
        } catch (Exception e) {
            log.error("ProxyController: Error al obtener conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                LocalDateTime.now(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // UPDATE - Actualizar conductor
    @PutMapping("/{cedula}")
    public ResponseEntity<?> actualizarConductor(
            @PathVariable String cedula,
            @Valid @RequestBody ConductorDTO conductor) {
        try {
            log.info("ProxyController: PUT /api/conductor/{} - Actualizar conductor", cedula);
            ConductorDTO conductorActualizado = conductorProxyService.actualizarConductor(cedula, conductor);
            return ResponseEntity.ok(conductorActualizado);
        } catch (RuntimeException e) {
            log.error("ProxyController: Error al actualizar conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.NOT_FOUND.value(),
                LocalDateTime.now(),
                "Error al actualizar conductor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.NOT_FOUND);
        } catch (Exception e) {
            log.error("ProxyController: Error interno: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                LocalDateTime.now(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // DELETE - Eliminar conductor
    @DeleteMapping("/{cedula}")
    public ResponseEntity<?> eliminarConductor(@PathVariable String cedula) {
        try {
            log.info("ProxyController: DELETE /api/conductor/{} - Eliminar conductor", cedula);
            conductorProxyService.eliminarConductor(cedula);
            return ResponseEntity.ok("Conductor eliminado exitosamente");
        } catch (RuntimeException e) {
            log.error("ProxyController: Error al eliminar conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.NOT_FOUND.value(),
                LocalDateTime.now(),
                "Error al eliminar conductor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.NOT_FOUND);
        } catch (Exception e) {
            log.error("ProxyController: Error interno: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                LocalDateTime.now(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
