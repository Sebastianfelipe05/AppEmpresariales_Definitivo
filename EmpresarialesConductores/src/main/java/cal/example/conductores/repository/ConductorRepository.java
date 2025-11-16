package cal.example.conductores.repository;

import cal.example.conductores.model.Conductor;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;

@Repository
public interface ConductorRepository extends JpaRepository<Conductor, String> {

    // Buscar por cédula (llave primaria)
    Optional<Conductor> findByCedula(String cedula);

    // Buscar por número de licencia
    Optional<Conductor> findByLicenciaNumero(String licenciaNumero);

    // Buscar por nombre (contiene)
    List<Conductor> findByNombreContainingIgnoreCase(String nombre);

    // Buscar por apellido (contiene)
    List<Conductor> findByApellidoContainingIgnoreCase(String apellido);

    // Buscar por nombre completo
    @Query("SELECT c FROM Conductor c WHERE LOWER(c.nombre) LIKE LOWER(CONCAT('%', :nombre, '%')) " +
           "AND LOWER(c.apellido) LIKE LOWER(CONCAT('%', :apellido, '%'))")
    List<Conductor> findByNombreAndApellido(@Param("nombre") String nombre, @Param("apellido") String apellido);

    // Buscar conductores activos
    List<Conductor> findByActivo(Boolean activo);

    // Buscar conductores por rango de salario
    @Query("SELECT c FROM Conductor c WHERE c.salario BETWEEN :salarioMin AND :salarioMax ORDER BY c.salario DESC")
    List<Conductor> findBySalarioRange(@Param("salarioMin") Double salarioMin, @Param("salarioMax") Double salarioMax);

    // Buscar conductores por edad aproximada (rango de fechas de nacimiento)
    @Query("SELECT c FROM Conductor c WHERE c.fechaNacimiento BETWEEN :fechaMin AND :fechaMax ORDER BY c.fechaNacimiento DESC")
    List<Conductor> findByFechaNacimientoRange(@Param("fechaMin") LocalDateTime fechaMin, @Param("fechaMax") LocalDateTime fechaMax);

    // Contar conductores activos
    Long countByActivo(Boolean activo);

    // Buscar conductores registrados en rango de fechas
    @Query("SELECT c FROM Conductor c WHERE c.fechaRegistro BETWEEN :fechaInicio AND :fechaFin ORDER BY c.fechaRegistro DESC")
    List<Conductor> findByFechaRegistroRange(@Param("fechaInicio") LocalDateTime fechaInicio, @Param("fechaFin") LocalDateTime fechaFin);

    // Buscar conductores con salario mayor a un valor
    List<Conductor> findBySalarioGreaterThanOrderBySalarioDesc(Double salario);

    // Query para obtener todos ordenados por nombre
    List<Conductor> findAllByOrderByNombreAsc();

    // Query para obtener estadísticas de salarios
    @Query("SELECT AVG(c.salario) FROM Conductor c WHERE c.activo = true")
    Double calcularSalarioPromedio();

    @Query("SELECT MAX(c.salario) FROM Conductor c WHERE c.activo = true")
    Double obtenerSalarioMaximo();

    @Query("SELECT MIN(c.salario) FROM Conductor c WHERE c.activo = true")
    Double obtenerSalarioMinimo();
}
