package cal.example.POCEmpleado.controller;

import cal.example.POCEmpleado.model.Mantenimiento;
import cal.example.POCEmpleado.service.IMantenimientoService;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Controlador REST para Mantenimientos
 * Implementa CRUD completo y operaciones de búsqueda
 * Sigue arquitectura hexagonal y principios SOLID
 */
@RestController
@RequestMapping("/api/mantenimiento")
@CrossOrigin(origins = "*")
public class MantenimientoController {

    private final IMantenimientoService mantenimientoService;

    @Autowired
    public MantenimientoController(IMantenimientoService mantenimientoService) {
        this.mantenimientoService = mantenimientoService;
    }

    @GetMapping(value = "/healthCheck")
    public String healthCheck() {
        return "Mantenimientos API Status Ok!";
    }

    /**
     * MÉTODO UNIFICADO - Listar mantenimientos con filtros opcionales
     * GET /api/mantenimiento
     * GET /api/mantenimiento?id=xxx
     * GET /api/mantenimiento?placaCarro=ABC-123
     * GET /api/mantenimiento?tipoMantenimiento=PREVENTIVO
     * GET /api/mantenimiento?completado=true
     * GET /api/mantenimiento?action=estadisticas
     * GET /api/mantenimiento?action=urgentes
     */
    @GetMapping
    public ResponseEntity<?> listar(@RequestParam Map<String, String> params) {
        try {
            // Manejar acciones especiales
            String action = params.get("action");
            if ("estadisticas".equals(action)) {
                return obtenerEstadisticas();
            }
            if ("urgentes".equals(action)) {
                return obtenerUrgentes();
            }
            if ("carro".equals(action)) {
                String placa = params.get("placa");
                if (placa != null && !placa.trim().isEmpty()) {
                    return obtenerPorCarro(placa);
                }
                return ResponseEntity.badRequest().body("Placa requerida para consultar mantenimientos por carro");
            }

            // Convertir parámetros String a Object para el servicio
            Map<String, Object> filtros = new HashMap<>();

            for (Map.Entry<String, String> entry : params.entrySet()) {
                String key = entry.getKey();
                String value = entry.getValue();

                if (value != null && !value.trim().isEmpty() && !"action".equals(key)) {
                    // Conversión de tipos según el parámetro
                    switch (key.toLowerCase()) {
                        case "kilometraje":
                        case "kilometraje_min":
                        case "kilometraje_max":
                            try {
                                filtros.put(key, Integer.parseInt(value));
                            } catch (NumberFormatException e) {
                                return ResponseEntity.badRequest()
                                    .body("Valor inválido para kilometraje: " + value);
                            }
                            break;
                        case "costo_min":
                        case "costo_max":
                        case "costo":
                            try {
                                filtros.put(key, Double.parseDouble(value));
                            } catch (NumberFormatException e) {
                                return ResponseEntity.badRequest()
                                    .body("Valor inválido para costo: " + value);
                            }
                            break;
                        case "completado":
                        case "urgente":
                            filtros.put(key, Boolean.parseBoolean(value));
                            break;
                        default:
                            filtros.put(key, value);
                    }
                }
            }

            List<Mantenimiento> mantenimientos = mantenimientoService.listar(filtros);
            return ResponseEntity.ok(mantenimientos);

        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                .body("Error interno del servidor: " + e.getMessage());
        }
    }

    /**
     * CREATE - Crear mantenimiento (POST)
     * POST /api/mantenimiento
     */
    @PostMapping
    public ResponseEntity<?> crearMantenimiento(@Valid @RequestBody Mantenimiento mantenimiento,
                                                BindingResult bindingResult) {
        if (bindingResult.hasErrors()) {
            return ResponseEntity.badRequest().body(formatMessage(bindingResult));
        }

        try {
            Mantenimiento mantenimientoGuardado = mantenimientoService.save(mantenimiento);
            return ResponseEntity.status(HttpStatus.CREATED).body(mantenimientoGuardado);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR)
                    .body("Error al crear el mantenimiento: " + e.getMessage());
        }
    }

    /**
     * UPDATE - Actualizar mantenimiento (PUT)
     * PUT /api/mantenimiento/{id}
     */
    @PutMapping(value = "/{id}")
    public ResponseEntity<?> actualizarMantenimiento(@PathVariable("id") Long id,
                                                     @Valid @RequestBody Mantenimiento mantenimiento,
                                                     BindingResult bindingResult) {
        if (bindingResult.hasErrors()) {
            return ResponseEntity.badRequest().body(formatMessage(bindingResult));
        }

        // Buscar usando el método unificado
        Map<String, Object> filtros = new HashMap<>();
        filtros.put("id", id);
        List<Mantenimiento> mantenimientosEncontrados = mantenimientoService.listar(filtros);

        if (mantenimientosEncontrados.isEmpty()) {
            return ResponseEntity.notFound().build();
        }

        // Asegurar que el ID no cambie
        mantenimiento.setId(id);
        Mantenimiento mantenimientoActualizado = mantenimientoService.save(mantenimiento);
        return ResponseEntity.ok(mantenimientoActualizado);
    }

    /**
     * DELETE - Eliminar mantenimiento (DELETE)
     * DELETE /api/mantenimiento/{id}
     */
    @DeleteMapping(value = "/{id}")
    public ResponseEntity<Map<String, String>> eliminarMantenimiento(@PathVariable("id") Long id) {
        boolean eliminado = mantenimientoService.deleteById(id);

        Map<String, String> response = new HashMap<>();
        if (eliminado) {
            response.put("message", "Mantenimiento eliminado exitosamente");
            response.put("id", String.valueOf(id));
            return ResponseEntity.ok(response);
        } else {
            response.put("error", "No se encontró un mantenimiento con el ID: " + id);
            return ResponseEntity.notFound().build();
        }
    }

    /**
     * Obtener mantenimientos por carro
     * GET /api/mantenimiento/carro/{placa}
     */
    @GetMapping(value = "/carro/{placa}")
    public ResponseEntity<List<Mantenimiento>> getMantenimientosPorCarro(@PathVariable("placa") String placa) {
        List<Mantenimiento> mantenimientos = mantenimientoService.getMantenimientosPorCarro(placa);
        return ResponseEntity.ok(mantenimientos);
    }

    /**
     * Persistencia manual de JSON
     * POST /api/mantenimiento/guardar-json
     */
    @PostMapping(value = "/guardar-json")
    public ResponseEntity<Map<String, String>> guardarEnJson() {
        try {
            mantenimientoService.saveToJson();
            Map<String, String> response = new HashMap<>();
            response.put("message", "Datos guardados en JSON exitosamente");
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            Map<String, String> response = new HashMap<>();
            response.put("error", "Error al guardar en JSON: " + e.getMessage());
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    // Métodos auxiliares privados
    private ResponseEntity<Map<String, Object>> obtenerEstadisticas() {
        Map<String, Object> stats = new HashMap<>();
        stats.put("totalMantenimientos", mantenimientoService.count());
        stats.put("costoTotal", mantenimientoService.getCostoTotal());
        stats.put("costoPromedio", mantenimientoService.getCostoPromedio());
        stats.put("mantenimientosUrgentes", mantenimientoService.getMantenimientosUrgentes().size());
        return ResponseEntity.ok(stats);
    }

    private ResponseEntity<List<Mantenimiento>> obtenerUrgentes() {
        List<Mantenimiento> urgentes = mantenimientoService.getMantenimientosUrgentes();
        return ResponseEntity.ok(urgentes);
    }

    private ResponseEntity<List<Mantenimiento>> obtenerPorCarro(String placa) {
        List<Mantenimiento> mantenimientos = mantenimientoService.getMantenimientosPorCarro(placa);
        return ResponseEntity.ok(mantenimientos);
    }

    // Método para formatear mensajes de error
    private Map<String, String> formatMessage(BindingResult result) {
        Map<String, String> errors = new HashMap<>();
        result.getFieldErrors().forEach(error -> {
            errors.put(error.getField(), error.getDefaultMessage());
        });
        return errors;
    }
}
