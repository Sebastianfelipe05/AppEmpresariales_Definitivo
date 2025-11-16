package cal.example.conductores.service;

import cal.example.conductores.model.Conductor;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.Optional;

public interface IConductorService {

    // CRUD Operations
    Conductor crearConductor(Conductor conductor);
    Optional<Conductor> obtenerConductorPorCedula(String cedula);
    List<Conductor> listarTodosConductores();
    Conductor actualizarConductor(String cedula, Conductor conductor);
    void eliminarConductor(String cedula);

    // Search Operations
    List<Conductor> buscarPorNombre(String nombre);
    List<Conductor> buscarPorApellido(String apellido);
    Optional<Conductor> buscarPorLicencia(String licenciaNumero);
    List<Conductor> buscarConductoresActivos();
    List<Conductor> buscarConductoresInactivos();

    // Filter Operations
    List<Conductor> filtrarPorRangoSalario(Double salarioMin, Double salarioMax);
    List<Conductor> filtrarPorRangoFechaRegistro(LocalDateTime fechaInicio, LocalDateTime fechaFin);

    // Statistics
    Map<String, Object> obtenerEstadisticas();
    Long contarConductoresActivos();
    Long contarConductoresInactivos();
}
