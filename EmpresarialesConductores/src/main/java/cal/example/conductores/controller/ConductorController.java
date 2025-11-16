package cal.example.conductores.controller;

import cal.example.conductores.errors.ErrorMessage;
import cal.example.conductores.model.Conductor;
import cal.example.conductores.service.IConductorService;
import jakarta.validation.Valid;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.Optional;

@RestController
@RequestMapping("/api/conductor")
@CrossOrigin(origins = "*")
@Slf4j
public class ConductorController {

    @Autowired
    private IConductorService conductorService;

    // Health check
    @GetMapping("/healthCheck")
    public ResponseEntity<String> healthCheck() {
        log.info("Health check - Microservicio Conductores funcionando correctamente");
        return ResponseEntity.ok("Microservicio Conductores OK - Puerto 8081");
    }

    // CREATE - Crear un nuevo conductor
    @PostMapping
    public ResponseEntity<?> crearConductor(@Valid @RequestBody Conductor conductor) {
        try {
            log.info("POST /api/conductor - Crear conductor: {}", conductor.getCedula());
            Conductor nuevoConductor = conductorService.crearConductor(conductor);
            return new ResponseEntity<>(nuevoConductor, HttpStatus.CREATED);
        } catch (IllegalArgumentException e) {
            log.error("Error al crear conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.BAD_REQUEST.value(),
                "Error al crear conductor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.BAD_REQUEST);
        } catch (Exception e) {
            log.error("Error interno al crear conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // READ - Obtener conductor por cédula
    @GetMapping("/{cedula}")
    public ResponseEntity<?> obtenerConductorPorCedula(@PathVariable String cedula) {
        try {
            log.info("GET /api/conductor/{} - Obtener conductor", cedula);
            Optional<Conductor> conductor = conductorService.obtenerConductorPorCedula(cedula);

            if (conductor.isPresent()) {
                return ResponseEntity.ok(conductor.get());
            } else {
                ErrorMessage errorMessage = new ErrorMessage(
                    HttpStatus.NOT_FOUND.value(),
                    "Conductor no encontrado",
                    "No se encontró conductor con cédula: " + cedula
                );
                return new ResponseEntity<>(errorMessage, HttpStatus.NOT_FOUND);
            }
        } catch (Exception e) {
            log.error("Error al obtener conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // READ - Listar todos los conductores con filtros opcionales
    @GetMapping
    public ResponseEntity<?> listarConductores(
            @RequestParam(required = false) String nombre,
            @RequestParam(required = false) String apellido,
            @RequestParam(required = false) Boolean activo,
            @RequestParam(required = false) String licencia,
            @RequestParam(required = false) String action) {
        try {
            log.info("GET /api/conductor - Listar conductores con filtros");

            // Estadísticas
            if ("estadisticas".equalsIgnoreCase(action)) {
                Map<String, Object> estadisticas = conductorService.obtenerEstadisticas();
                return ResponseEntity.ok(estadisticas);
            }

            // Buscar por licencia
            if (licencia != null && !licencia.isEmpty()) {
                Optional<Conductor> conductor = conductorService.buscarPorLicencia(licencia);
                return ResponseEntity.ok(conductor.map(List::of).orElse(List.of()));
            }

            // Buscar por nombre
            if (nombre != null && !nombre.isEmpty()) {
                List<Conductor> conductores = conductorService.buscarPorNombre(nombre);
                return ResponseEntity.ok(conductores);
            }

            // Buscar por apellido
            if (apellido != null && !apellido.isEmpty()) {
                List<Conductor> conductores = conductorService.buscarPorApellido(apellido);
                return ResponseEntity.ok(conductores);
            }

            // Buscar por estado activo
            if (activo != null) {
                List<Conductor> conductores = activo ?
                    conductorService.buscarConductoresActivos() :
                    conductorService.buscarConductoresInactivos();
                return ResponseEntity.ok(conductores);
            }

            // Listar todos
            List<Conductor> conductores = conductorService.listarTodosConductores();
            return ResponseEntity.ok(conductores);

        } catch (Exception e) {
            log.error("Error al listar conductores: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // READ - Filtrar por rango de salario
    @GetMapping("/filtro/salario")
    public ResponseEntity<?> filtrarPorSalario(
            @RequestParam Double min,
            @RequestParam Double max) {
        try {
            log.info("GET /api/conductor/filtro/salario - Rango: {} - {}", min, max);
            List<Conductor> conductores = conductorService.filtrarPorRangoSalario(min, max);
            return ResponseEntity.ok(conductores);
        } catch (Exception e) {
            log.error("Error al filtrar por salario: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error al filtrar conductores",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // READ - Filtrar por rango de fecha de registro
    @GetMapping("/filtro/fecha")
    public ResponseEntity<?> filtrarPorFecha(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime inicio,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime fin) {
        try {
            log.info("GET /api/conductor/filtro/fecha - Rango: {} - {}", inicio, fin);
            List<Conductor> conductores = conductorService.filtrarPorRangoFechaRegistro(inicio, fin);
            return ResponseEntity.ok(conductores);
        } catch (Exception e) {
            log.error("Error al filtrar por fecha: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error al filtrar conductores",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // UPDATE - Actualizar conductor
    @PutMapping("/{cedula}")
    public ResponseEntity<?> actualizarConductor(
            @PathVariable String cedula,
            @Valid @RequestBody Conductor conductor) {
        try {
            log.info("PUT /api/conductor/{} - Actualizar conductor", cedula);
            Conductor conductorActualizado = conductorService.actualizarConductor(cedula, conductor);
            return ResponseEntity.ok(conductorActualizado);
        } catch (IllegalArgumentException e) {
            log.error("Error al actualizar conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.NOT_FOUND.value(),
                "Error al actualizar conductor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.NOT_FOUND);
        } catch (Exception e) {
            log.error("Error interno al actualizar conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
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
            log.info("DELETE /api/conductor/{} - Eliminar conductor", cedula);
            conductorService.eliminarConductor(cedula);
            return ResponseEntity.ok("Conductor eliminado exitosamente");
        } catch (IllegalArgumentException e) {
            log.error("Error al eliminar conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.NOT_FOUND.value(),
                "Error al eliminar conductor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.NOT_FOUND);
        } catch (Exception e) {
            log.error("Error interno al eliminar conductor: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error interno del servidor",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // Contar conductores activos
    @GetMapping("/contar/activos")
    public ResponseEntity<?> contarActivos() {
        try {
            Long count = conductorService.contarConductoresActivos();
            return ResponseEntity.ok(Map.of("conductoresActivos", count));
        } catch (Exception e) {
            log.error("Error al contar conductores activos: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error al contar conductores",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }

    // Contar conductores inactivos
    @GetMapping("/contar/inactivos")
    public ResponseEntity<?> contarInactivos() {
        try {
            Long count = conductorService.contarConductoresInactivos();
            return ResponseEntity.ok(Map.of("conductoresInactivos", count));
        } catch (Exception e) {
            log.error("Error al contar conductores inactivos: {}", e.getMessage());
            ErrorMessage errorMessage = new ErrorMessage(
                HttpStatus.INTERNAL_SERVER_ERROR.value(),
                "Error al contar conductores",
                e.getMessage()
            );
            return new ResponseEntity<>(errorMessage, HttpStatus.INTERNAL_SERVER_ERROR);
        }
    }
}
