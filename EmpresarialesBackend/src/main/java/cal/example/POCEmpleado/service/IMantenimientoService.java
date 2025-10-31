package cal.example.POCEmpleado.service;

import cal.example.POCEmpleado.model.Mantenimiento;
import java.util.List;
import java.util.Map;

/**
 * Interfaz de servicio para Mantenimiento
 * Define el contrato de operaciones CRUD y búsquedas
 */
public interface IMantenimientoService {

    /**
     * Guarda o actualiza un mantenimiento
     * @param mantenimiento El mantenimiento a guardar
     * @return El mantenimiento guardado
     */
    Mantenimiento save(Mantenimiento mantenimiento);

    /**
     * Lista mantenimientos con filtros opcionales
     * @param filtros Mapa de filtros (id, placaCarro, tipoMantenimiento, etc.)
     * @return Lista de mantenimientos que cumplen los filtros
     */
    List<Mantenimiento> listar(Map<String, Object> filtros);

    /**
     * Elimina un mantenimiento por su ID
     * @param id El ID del mantenimiento a eliminar
     * @return true si se eliminó, false si no existe
     */
    boolean deleteById(Long id);

    /**
     * Cuenta el total de mantenimientos
     * @return Número total de mantenimientos
     */
    long count();

    /**
     * Calcula el costo promedio de mantenimientos
     * @return Promedio de los costos
     */
    double getCostoPromedio();

    /**
     * Cuenta mantenimientos por placa de carro
     * @param placa La placa del carro
     * @return Número de mantenimientos del carro
     */
    long countByPlacaCarro(String placa);

    /**
     * Lista mantenimientos con información del carro (maestro-detalle)
     * @return Lista de mantenimientos con carro asociado
     */
    List<Mantenimiento> listarConCarro();

    /**
     * Guarda los datos en archivo JSON (persistencia manual)
     */
    void saveToJson();

    /**
     * Carga los datos desde archivo JSON
     */
    void loadFromJson();

    /**
     * Obtiene mantenimientos de un carro específico
     * @param placa La placa del carro
     * @return Lista de mantenimientos del carro
     */
    List<Mantenimiento> getMantenimientosPorCarro(String placa);

    /**
     * Obtiene mantenimientos urgentes (próximos en 7 días)
     * @return Lista de mantenimientos urgentes
     */
    List<Mantenimiento> getMantenimientosUrgentes();

    /**
     * Calcula el costo total de todos los mantenimientos
     * @return Costo total
     */
    double getCostoTotal();
}
