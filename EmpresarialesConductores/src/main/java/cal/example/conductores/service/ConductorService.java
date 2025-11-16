package cal.example.conductores.service;

import cal.example.conductores.model.Conductor;
import cal.example.conductores.repository.ConductorRepository;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;

@Service
@Transactional
@Slf4j
public class ConductorService implements IConductorService {

    @Autowired
    private ConductorRepository conductorRepository;

    @Override
    public Conductor crearConductor(Conductor conductor) {
        log.info("Creando conductor con cédula: {}", conductor.getCedula());

        // Validar que no exista conductor con la misma cédula
        if (conductorRepository.findByCedula(conductor.getCedula()).isPresent()) {
            log.error("Ya existe un conductor con la cédula: {}", conductor.getCedula());
            throw new IllegalArgumentException("Ya existe un conductor con la cédula: " + conductor.getCedula());
        }

        // Validar que no exista conductor con el mismo número de licencia
        if (conductorRepository.findByLicenciaNumero(conductor.getLicenciaNumero()).isPresent()) {
            log.error("Ya existe un conductor con el número de licencia: {}", conductor.getLicenciaNumero());
            throw new IllegalArgumentException("Ya existe un conductor con el número de licencia: " + conductor.getLicenciaNumero());
        }

        Conductor savedConductor = conductorRepository.save(conductor);
        log.info("Conductor creado exitosamente: {} {}", savedConductor.getNombre(), savedConductor.getApellido());
        return savedConductor;
    }

    @Override
    @Transactional(readOnly = true)
    public Optional<Conductor> obtenerConductorPorCedula(String cedula) {
        log.info("Buscando conductor por cédula: {}", cedula);
        return conductorRepository.findByCedula(cedula);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> listarTodosConductores() {
        log.info("Listando todos los conductores");
        return conductorRepository.findAllByOrderByNombreAsc();
    }

    @Override
    public Conductor actualizarConductor(String cedula, Conductor conductor) {
        log.info("Actualizando conductor con cédula: {}", cedula);

        Conductor conductorExistente = conductorRepository.findByCedula(cedula)
            .orElseThrow(() -> {
                log.error("Conductor no encontrado con cédula: {}", cedula);
                return new IllegalArgumentException("Conductor no encontrado con cédula: " + cedula);
            });

        // Validar cambio de número de licencia
        if (!conductorExistente.getLicenciaNumero().equals(conductor.getLicenciaNumero())) {
            Optional<Conductor> conductorConMismaLicencia = conductorRepository.findByLicenciaNumero(conductor.getLicenciaNumero());
            if (conductorConMismaLicencia.isPresent() && !conductorConMismaLicencia.get().getCedula().equals(cedula)) {
                log.error("Ya existe otro conductor con el número de licencia: {}", conductor.getLicenciaNumero());
                throw new IllegalArgumentException("Ya existe otro conductor con el número de licencia: " + conductor.getLicenciaNumero());
            }
        }

        // Actualizar campos
        conductorExistente.setNombre(conductor.getNombre());
        conductorExistente.setApellido(conductor.getApellido());
        conductorExistente.setTelefono(conductor.getTelefono());
        conductorExistente.setLicenciaNumero(conductor.getLicenciaNumero());
        conductorExistente.setFechaNacimiento(conductor.getFechaNacimiento());
        conductorExistente.setSalario(conductor.getSalario());
        conductorExistente.setActivo(conductor.getActivo());

        Conductor conductorActualizado = conductorRepository.save(conductorExistente);
        log.info("Conductor actualizado exitosamente: {}", cedula);
        return conductorActualizado;
    }

    @Override
    public void eliminarConductor(String cedula) {
        log.info("Eliminando conductor con cédula: {}", cedula);

        Conductor conductor = conductorRepository.findByCedula(cedula)
            .orElseThrow(() -> {
                log.error("Conductor no encontrado con cédula: {}", cedula);
                return new IllegalArgumentException("Conductor no encontrado con cédula: " + cedula);
            });

        conductorRepository.delete(conductor);
        log.info("Conductor eliminado exitosamente: {}", cedula);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> buscarPorNombre(String nombre) {
        log.info("Buscando conductores por nombre: {}", nombre);
        return conductorRepository.findByNombreContainingIgnoreCase(nombre);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> buscarPorApellido(String apellido) {
        log.info("Buscando conductores por apellido: {}", apellido);
        return conductorRepository.findByApellidoContainingIgnoreCase(apellido);
    }

    @Override
    @Transactional(readOnly = true)
    public Optional<Conductor> buscarPorLicencia(String licenciaNumero) {
        log.info("Buscando conductor por licencia: {}", licenciaNumero);
        return conductorRepository.findByLicenciaNumero(licenciaNumero);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> buscarConductoresActivos() {
        log.info("Buscando conductores activos");
        return conductorRepository.findByActivo(true);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> buscarConductoresInactivos() {
        log.info("Buscando conductores inactivos");
        return conductorRepository.findByActivo(false);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> filtrarPorRangoSalario(Double salarioMin, Double salarioMax) {
        log.info("Filtrando conductores por rango de salario: {} - {}", salarioMin, salarioMax);
        return conductorRepository.findBySalarioRange(salarioMin, salarioMax);
    }

    @Override
    @Transactional(readOnly = true)
    public List<Conductor> filtrarPorRangoFechaRegistro(LocalDateTime fechaInicio, LocalDateTime fechaFin) {
        log.info("Filtrando conductores por rango de fecha de registro: {} - {}", fechaInicio, fechaFin);
        return conductorRepository.findByFechaRegistroRange(fechaInicio, fechaFin);
    }

    @Override
    @Transactional(readOnly = true)
    public Map<String, Object> obtenerEstadisticas() {
        log.info("Obteniendo estadísticas de conductores");

        Map<String, Object> estadisticas = new HashMap<>();
        estadisticas.put("totalConductores", conductorRepository.count());
        estadisticas.put("conductoresActivos", conductorRepository.countByActivo(true));
        estadisticas.put("conductoresInactivos", conductorRepository.countByActivo(false));
        estadisticas.put("salarioPromedio", conductorRepository.calcularSalarioPromedio());
        estadisticas.put("salarioMaximo", conductorRepository.obtenerSalarioMaximo());
        estadisticas.put("salarioMinimo", conductorRepository.obtenerSalarioMinimo());

        return estadisticas;
    }

    @Override
    @Transactional(readOnly = true)
    public Long contarConductoresActivos() {
        return conductorRepository.countByActivo(true);
    }

    @Override
    @Transactional(readOnly = true)
    public Long contarConductoresInactivos() {
        return conductorRepository.countByActivo(false);
    }
}
