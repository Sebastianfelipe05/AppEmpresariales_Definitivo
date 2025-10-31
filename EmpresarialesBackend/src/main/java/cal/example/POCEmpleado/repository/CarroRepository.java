package cal.example.POCEmpleado.repository;

import cal.example.POCEmpleado.model.Carro;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

/**
 * Repositorio JPA para la entidad Carro
 * Incluye consultas personalizadas según los requisitos del proyecto
 */
@Repository
public interface CarroRepository extends JpaRepository<Carro, String> {

    // ===== CONSULTAS BÁSICAS (heredadas de JpaRepository) =====
    // - findAll(): List<Carro>
    // - findById(String placa): Optional<Carro>
    // - save(Carro carro): Carro
    // - deleteById(String placa): void
    // - existsById(String placa): boolean

    // ===== CONSULTAS PERSONALIZADAS (requeridas por el PDF) =====

    /**
     * CONSULTA PERSONALIZADA 1: Buscar carros por estado
     * Requerimiento del PDF: "Debe permitir la consulta de los objetos a partir de diferentes criterios"
     */
    @Query("SELECT c FROM Carro c WHERE c.estado = :estado")
    List<Carro> findByEstado(@Param("estado") String estado);

    /**
     * CONSULTA PERSONALIZADA 2: Buscar carros por marca y modelo
     */
    @Query("SELECT c FROM Carro c WHERE UPPER(c.marca) LIKE UPPER(CONCAT('%', :marca, '%')) " +
           "AND UPPER(c.modelo) LIKE UPPER(CONCAT('%', :modelo, '%'))")
    List<Carro> findByMarcaAndModelo(@Param("marca") String marca, @Param("modelo") String modelo);

    /**
     * CONSULTA PERSONALIZADA 3: Buscar carros por rango de año
     */
    @Query("SELECT c FROM Carro c WHERE c.anio BETWEEN :anioInicio AND :anioFin")
    List<Carro> findByAnioRange(@Param("anioInicio") int anioInicio, @Param("anioFin") int anioFin);

    /**
     * CONSULTA PERSONALIZADA 4: Buscar carros por rango de precio
     */
    @Query("SELECT c FROM Carro c WHERE c.precio BETWEEN :precioMin AND :precioMax")
    List<Carro> findByPrecioRange(@Param("precioMin") double precioMin, @Param("precioMax") double precioMax);

    /**
     * CONSULTA PERSONALIZADA 5: Buscar carros por tipo de transmisión
     */
    @Query("SELECT c FROM Carro c WHERE c.tipoTransmision = :tipo")
    List<Carro> findByTipoTransmision(@Param("tipo") String tipo);

    /**
     * CONSULTA PERSONALIZADA 6: Listar carros con filtro por marca
     * Usa LIKE para búsqueda parcial (case-insensitive)
     */
    @Query("SELECT c FROM Carro c WHERE UPPER(c.marca) LIKE UPPER(CONCAT('%', :marca, '%'))")
    List<Carro> findByMarcaContaining(@Param("marca") String marca);

    /**
     * CONSULTA PERSONALIZADA 7: Buscar carros que tengan aire acondicionado
     */
    @Query("SELECT c FROM Carro c WHERE c.tieneAireAcondicionado = true")
    List<Carro> findCarrosConAireAcondicionado();

    /**
     * CONSULTA PERSONALIZADA 8: Buscar carro por placa (alternativa a findById)
     */
    Optional<Carro> findByPlaca(String placa);

    /**
     * CONSULTA PERSONALIZADA 9: Contar carros por estado
     */
    @Query("SELECT COUNT(c) FROM Carro c WHERE c.estado = :estado")
    long countByEstado(@Param("estado") String estado);

    /**
     * CONSULTA PERSONALIZADA 10: Obtener carros ordenados por precio
     */
    @Query("SELECT c FROM Carro c ORDER BY c.precio DESC")
    List<Carro> findAllOrderByPrecioDesc();
}
