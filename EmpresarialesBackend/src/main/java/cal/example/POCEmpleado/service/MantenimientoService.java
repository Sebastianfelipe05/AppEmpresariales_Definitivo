package cal.example.POCEmpleado.service;

import cal.example.POCEmpleado.model.Carro;
import cal.example.POCEmpleado.model.Mantenimiento;
import cal.example.POCEmpleado.repository.CarroRepository;
import cal.example.POCEmpleado.repository.MantenimientoRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

/**
 * Servicio de Mantenimiento usando JPA
 * Migrado desde almacenamiento en memoria/JSON a base de datos Oracle
 *
 * Implementa la relación @ManyToOne con Carro
 */
@Service
@Transactional
public class MantenimientoService implements IMantenimientoService {

    private final MantenimientoRepository mantenimientoRepository;
    private final CarroRepository carroRepository;

    @Autowired
    public MantenimientoService(MantenimientoRepository mantenimientoRepository,
                                CarroRepository carroRepository) {
        this.mantenimientoRepository = mantenimientoRepository;
        this.carroRepository = carroRepository;
    }

    @Override
    public Mantenimiento save(Mantenimiento mantenimiento) {
        // Validar que el carro existe antes de guardar el mantenimiento
        if (mantenimiento.getCarro() == null) {
            throw new IllegalArgumentException("El mantenimiento debe estar asociado a un carro");
        }

        String placaCarro = mantenimiento.getCarro().getPlaca();
        Carro carro = carroRepository.findById(placaCarro)
                .orElseThrow(() -> new IllegalArgumentException(
                        "No existe un carro con la placa: " + placaCarro));

        mantenimiento.setCarro(carro);
        return mantenimientoRepository.save(mantenimiento);
    }

    @Override
    public boolean deleteById(Long id) {
        if (mantenimientoRepository.existsById(id)) {
            mantenimientoRepository.deleteById(id);
            return true;
        }
        return false;
    }

    @Override
    @Transactional(readOnly = true)
    public List<Mantenimiento> listar(Map<String, Object> filtros) {
        // Sin filtros: retornar todos
        if (filtros == null || filtros.isEmpty()) {
            return mantenimientoRepository.findAll();
        }

        // Filtro por ID
        if (filtros.containsKey("id")) {
            Long id = Long.parseLong(filtros.get("id").toString());
            return mantenimientoRepository.findById(id)
                    .map(Collections::singletonList)
                    .orElse(Collections.emptyList());
        }

        // Filtro por placa del carro (usa query JPA personalizada)
        if (filtros.containsKey("placaCarro") || filtros.containsKey("placa_carro")) {
            String placa = filtros.getOrDefault("placaCarro",
                    filtros.get("placa_carro")).toString();
            return mantenimientoRepository.findByPlacaCarro(placa);
        }

        // Filtro por tipo de mantenimiento
        if (filtros.containsKey("tipoMantenimiento") || filtros.containsKey("tipo_mantenimiento")) {
            String tipo = filtros.getOrDefault("tipoMantenimiento",
                    filtros.get("tipo_mantenimiento")).toString();
            return mantenimientoRepository.findByTipoMantenimiento(tipo);
        }

        // Filtro por completado
        if (filtros.containsKey("completado")) {
            boolean completado = Boolean.parseBoolean(filtros.get("completado").toString());
            return mantenimientoRepository.findByCompletado(completado);
        }

        // Filtro por rango de costos
        if (filtros.containsKey("costo_min") && filtros.containsKey("costo_max")) {
            double min = Double.parseDouble(filtros.get("costo_min").toString());
            double max = Double.parseDouble(filtros.get("costo_max").toString());
            return mantenimientoRepository.findByCostoRange(min, max);
        }

        // Filtro por rango de fechas
        if (filtros.containsKey("fecha_inicio") && filtros.containsKey("fecha_fin")) {
            LocalDateTime inicio = LocalDateTime.parse(filtros.get("fecha_inicio").toString());
            LocalDateTime fin = LocalDateTime.parse(filtros.get("fecha_fin").toString());
            return mantenimientoRepository.findByFechaRange(inicio, fin);
        }

        // Para filtros complejos no mapeados, filtrar en memoria
        List<Mantenimiento> todos = mantenimientoRepository.findAll();
        return todos.stream()
                .filter(m -> aplicarFiltros(m, filtros))
                .collect(Collectors.toList());
    }

    /**
     * CONSULTA MAESTRO-DETALLE (Requerida por el PDF)
     * Retorna mantenimientos con información del carro asociado
     */
    @Override
    @Transactional(readOnly = true)
    public List<Mantenimiento> listarConCarro() {
        return mantenimientoRepository.findAllConCarro();
    }

    /**
     * Obtener mantenimientos de un carro específico (maestro-detalle)
     */
    @Override
    @Transactional(readOnly = true)
    public List<Mantenimiento> getMantenimientosPorCarro(String placa) {
        return mantenimientoRepository.findMantenimientosConCarroByPlaca(placa);
    }

    /**
     * Obtener mantenimientos urgentes (próximo mantenimiento en 7 días)
     */
    @Override
    @Transactional(readOnly = true)
    public List<Mantenimiento> getMantenimientosUrgentes() {
        LocalDateTime fechaLimite = LocalDateTime.now().plusDays(7);
        return mantenimientoRepository.findMantenimientosUrgentes(fechaLimite);
    }

    /**
     * Calcular costo total de mantenimientos de un carro
     */
    @Transactional(readOnly = true)
    public double calcularCostoTotalPorCarro(String placa) {
        Double total = mantenimientoRepository.calcularCostoTotalPorCarro(placa);
        return total != null ? total : 0.0;
    }

    /**
     * Filtrado en memoria para casos complejos
     */
    private boolean aplicarFiltros(Mantenimiento mantenimiento, Map<String, Object> filtros) {
        for (Map.Entry<String, Object> filtro : filtros.entrySet()) {
            String campo = filtro.getKey().toLowerCase();
            Object valor = filtro.getValue();

            if (valor == null) continue;

            switch (campo) {
                case "descripcion":
                    if (!mantenimiento.getDescripcion().toLowerCase()
                            .contains(valor.toString().toLowerCase())) {
                        return false;
                    }
                    break;
                case "kilometraje":
                    if (mantenimiento.getKilometraje() != null &&
                            !mantenimiento.getKilometraje().equals(Integer.parseInt(valor.toString()))) {
                        return false;
                    }
                    break;
                case "costo":
                    if (mantenimiento.getCosto() != Double.parseDouble(valor.toString())) {
                        return false;
                    }
                    break;
            }
        }
        return true;
    }

    @Override
    @Transactional(readOnly = true)
    public long count() {
        return mantenimientoRepository.count();
    }

    @Override
    @Transactional(readOnly = true)
    public double getCostoPromedio() {
        List<Mantenimiento> mantenimientos = mantenimientoRepository.findAll();
        return mantenimientos.stream()
                .mapToDouble(Mantenimiento::getCosto)
                .average()
                .orElse(0.0);
    }

    @Override
    @Transactional(readOnly = true)
    public long countByPlacaCarro(String placa) {
        return mantenimientoRepository.countByPlacaCarro(placa);
    }

    @Override
    @Transactional(readOnly = true)
    public double getCostoTotal() {
        List<Mantenimiento> mantenimientos = mantenimientoRepository.findAll();
        return mantenimientos.stream()
                .mapToDouble(Mantenimiento::getCosto)
                .sum();
    }

    // Métodos de persistencia JSON ya no son necesarios con JPA
    @Override
    public void saveToJson() {
        // No-op: JPA maneja la persistencia automáticamente
    }

    @Override
    public void loadFromJson() {
        // No-op: JPA carga los datos desde la base de datos
    }
}
