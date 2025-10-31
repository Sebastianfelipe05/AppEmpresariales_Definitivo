package cal.example.POCEmpleado.repository;

import cal.example.POCEmpleado.model.Mantenimiento;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

/**
 * Repositorio JPA para la entidad Mantenimiento
 * Incluye consultas personalizadas según los requisitos del proyecto
 */
@Repository
public interface MantenimientoRepository extends JpaRepository<Mantenimiento, Long> {

    // ===== CONSULTAS BÁSICAS (heredadas de JpaRepository) =====
    // - findAll(): List<Mantenimiento>
    // - findById(Long id): Optional<Mantenimiento>
    // - save(Mantenimiento m): Mantenimiento
    // - deleteById(Long id): void

    // ===== CONSULTAS PERSONALIZADAS (requeridas por el PDF) =====

    /**
     * CONSULTA PERSONALIZADA 1 (REQUERIDA EN PDF):
     * Mostrar datos de la tabla maestro (Carro) y detalle (Mantenimiento)
     * "Debe permitir mostrar los datos de la tabla maestro y dos de detalle"
     */
    @Query("SELECT m FROM Mantenimiento m " +
           "JOIN FETCH m.carro c " +
           "WHERE c.placa = :placa " +
           "ORDER BY m.fechaMantenimiento DESC")
    List<Mantenimiento> findMantenimientosConCarroByPlaca(@Param("placa") String placa);

    /**
     * CONSULTA PERSONALIZADA 2 (REQUERIDA EN PDF):
     * Listar todos los mantenimientos con información del carro (maestro-detalle)
     */
    @Query("SELECT m FROM Mantenimiento m " +
           "JOIN FETCH m.carro c " +
           "ORDER BY m.fechaMantenimiento DESC")
    List<Mantenimiento> findAllConCarro();

    /**
     * CONSULTA PERSONALIZADA 3: Buscar por placa del carro
     */
    @Query("SELECT m FROM Mantenimiento m WHERE m.carro.placa = :placa")
    List<Mantenimiento> findByPlacaCarro(@Param("placa") String placa);

    /**
     * CONSULTA PERSONALIZADA 4: Buscar por tipo de mantenimiento
     */
    @Query("SELECT m FROM Mantenimiento m WHERE m.tipoMantenimiento = :tipo")
    List<Mantenimiento> findByTipoMantenimiento(@Param("tipo") String tipo);

    /**
     * CONSULTA PERSONALIZADA 5: Buscar mantenimientos por rango de fechas
     */
    @Query("SELECT m FROM Mantenimiento m " +
           "WHERE m.fechaMantenimiento BETWEEN :fechaInicio AND :fechaFin " +
           "ORDER BY m.fechaMantenimiento DESC")
    List<Mantenimiento> findByFechaRange(@Param("fechaInicio") LocalDateTime fechaInicio,
                                         @Param("fechaFin") LocalDateTime fechaFin);

    /**
     * CONSULTA PERSONALIZADA 6: Buscar mantenimientos por rango de costo
     */
    @Query("SELECT m FROM Mantenimiento m WHERE m.costo BETWEEN :costoMin AND :costoMax")
    List<Mantenimiento> findByCostoRange(@Param("costoMin") double costoMin,
                                         @Param("costoMax") double costoMax);

    /**
     * CONSULTA PERSONALIZADA 7: Buscar mantenimientos completados
     */
    @Query("SELECT m FROM Mantenimiento m WHERE m.completado = :completado")
    List<Mantenimiento> findByCompletado(@Param("completado") boolean completado);

    /**
     * CONSULTA PERSONALIZADA 8: Buscar mantenimientos urgentes
     * (próximo mantenimiento en los próximos 7 días)
     */
    @Query("SELECT m FROM Mantenimiento m " +
           "WHERE m.proximoMantenimiento BETWEEN CURRENT_TIMESTAMP AND :fechaLimite " +
           "AND m.completado = false")
    List<Mantenimiento> findMantenimientosUrgentes(@Param("fechaLimite") LocalDateTime fechaLimite);

    /**
     * CONSULTA PERSONALIZADA 9: Contar mantenimientos por placa
     */
    @Query("SELECT COUNT(m) FROM Mantenimiento m WHERE m.carro.placa = :placa")
    long countByPlacaCarro(@Param("placa") String placa);

    /**
     * CONSULTA PERSONALIZADA 10: Calcular costo total de mantenimientos por carro
     */
    @Query("SELECT SUM(m.costo) FROM Mantenimiento m WHERE m.carro.placa = :placa")
    Double calcularCostoTotalPorCarro(@Param("placa") String placa);

    /**
     * CONSULTA PERSONALIZADA 11: Buscar últimos N mantenimientos de un carro
     */
    @Query("SELECT m FROM Mantenimiento m " +
           "WHERE m.carro.placa = :placa " +
           "ORDER BY m.fechaMantenimiento DESC")
    List<Mantenimiento> findUltimosMantenimientosByPlaca(@Param("placa") String placa);

    /**
     * CONSULTA PERSONALIZADA 12: Mantenimientos con información completa (DTO-like)
     * Retorna Mantenimiento con Carro cargado (FETCH JOIN)
     */
    @Query("SELECT DISTINCT m FROM Mantenimiento m " +
           "LEFT JOIN FETCH m.carro " +
           "WHERE m.tipoMantenimiento = :tipo")
    List<Mantenimiento> findByTipoWithCarro(@Param("tipo") String tipo);
}
