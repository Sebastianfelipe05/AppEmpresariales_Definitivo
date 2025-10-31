package cal.example.POCEmpleado.service;

import cal.example.POCEmpleado.model.Carro;
import cal.example.POCEmpleado.repository.CarroRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;
import java.util.stream.Collectors;

/**
 * Implementación del servicio de Carros usando JPA
 * Migrado desde almacenamiento en memoria/JSON a base de datos Oracle
 *
 * Principios SOLID aplicados:
 * - SRP: Responsabilidad única de gestionar carros
 * - OCP: Abierto para extensión mediante la interfaz
 * - LSP: Sustituible por cualquier implementación de ICarroService
 * - ISP: Interfaz segregada con operaciones específicas
 * - DIP: Depende de abstracciones (ICarroService y CarroRepository)
 */
@Service
@Transactional
public class CarroService implements ICarroService {

    private final CarroRepository carroRepository;

    @Autowired
    public CarroService(CarroRepository carroRepository) {
        this.carroRepository = carroRepository;
    }

    @Override
    public Carro save(Carro carro) {
        // JPA maneja automáticamente INSERT o UPDATE según si existe
        return carroRepository.save(carro);
    }

    @Override
    public boolean deleteByPlaca(String placa) {
        if (carroRepository.existsById(placa)) {
            carroRepository.deleteById(placa);
            return true;
        }
        return false;
    }

    /**
     * MÉTODO UNIFICADO - Maneja todos los casos de consulta:
     * - Sin filtros: retorna todos los carros
     * - Con placa: retorna un carro específico (como lista de 1 elemento)
     * - Con otros filtros: usa consultas JPA personalizadas o filtrado en memoria
     */
    @Override
    @Transactional(readOnly = true)
    public List<Carro> listar(Map<String, Object> filtros) {
        // Sin filtros: retornar todos
        if (filtros == null || filtros.isEmpty()) {
            return carroRepository.findAll();
        }

        // Con filtros: usar consultas optimizadas JPA cuando sea posible
        List<Carro> resultado = null;

        // Filtro por placa (búsqueda exacta)
        if (filtros.containsKey("placa")) {
            String placa = filtros.get("placa").toString();
            return carroRepository.findById(placa)
                    .map(Collections::singletonList)
                    .orElse(Collections.emptyList());
        }

        // Filtro por estado (usa query JPA personalizada)
        if (filtros.containsKey("estado") && filtros.size() == 1) {
            return carroRepository.findByEstado(filtros.get("estado").toString());
        }

        // Filtro por tipo de transmisión
        if (filtros.containsKey("transmision") || filtros.containsKey("tipotransmision")) {
            String tipo = filtros.getOrDefault("transmision",
                    filtros.get("tipotransmision")).toString();
            return carroRepository.findByTipoTransmision(tipo);
        }

        // Filtro por marca
        if (filtros.containsKey("marca") && filtros.size() == 1) {
            return carroRepository.findByMarcaContaining(filtros.get("marca").toString());
        }

        // Filtro por marca Y modelo
        if (filtros.containsKey("marca") && filtros.containsKey("modelo")) {
            String marca = filtros.get("marca").toString();
            String modelo = filtros.get("modelo").toString();
            return carroRepository.findByMarcaAndModelo(marca, modelo);
        }

        // Filtro por rango de precios
        if (filtros.containsKey("precio_min") && filtros.containsKey("precio_max")) {
            double min = Double.parseDouble(filtros.get("precio_min").toString());
            double max = Double.parseDouble(filtros.get("precio_max").toString());
            return carroRepository.findByPrecioRange(min, max);
        }

        // Filtro por aire acondicionado
        if (filtros.containsKey("aire_acondicionado") && filtros.size() == 1) {
            boolean tieneAire = Boolean.parseBoolean(filtros.get("aire_acondicionado").toString());
            if (tieneAire) {
                return carroRepository.findCarrosConAireAcondicionado();
            }
        }

        // Para filtros complejos no mapeados, cargar todos y filtrar en memoria
        List<Carro> todosLosCarros = carroRepository.findAll();
        return todosLosCarros.stream()
                .filter(carro -> aplicarFiltros(carro, filtros))
                .collect(Collectors.toList());
    }

    /**
     * Filtrado en memoria para casos complejos
     * (Se mantiene para compatibilidad con filtros no implementados en JPA)
     */
    private boolean aplicarFiltros(Carro carro, Map<String, Object> filtros) {
        for (Map.Entry<String, Object> filtro : filtros.entrySet()) {
            String campo = filtro.getKey().toLowerCase();
            Object valor = filtro.getValue();

            if (valor == null) continue;

            switch (campo) {
                case "color":
                    if (!carro.getColor().toLowerCase().contains(valor.toString().toLowerCase())) {
                        return false;
                    }
                    break;
                case "modelo":
                    if (!carro.getModelo().toLowerCase().contains(valor.toString().toLowerCase())) {
                        return false;
                    }
                    break;
                case "combustible":
                    if (!carro.getCombustible().toLowerCase().contains(valor.toString().toLowerCase())) {
                        return false;
                    }
                    break;
                case "anio":
                    if (carro.getAnio() != Integer.parseInt(valor.toString())) {
                        return false;
                    }
                    break;
                case "numeropuertas":
                case "numero_puertas":
                    if (carro.getNumeroPuertas() != Integer.parseInt(valor.toString())) {
                        return false;
                    }
                    break;
                case "precio":
                    if (carro.getPrecio() != Double.parseDouble(valor.toString())) {
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
        return carroRepository.count();
    }

    @Override
    @Transactional(readOnly = true)
    public double getPrecioPromedio() {
        List<Carro> carros = carroRepository.findAll();
        return carros.stream()
                .mapToDouble(Carro::getPrecio)
                .average()
                .orElse(0.0);
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
