package cal.example.POCEmpleado.model;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import jakarta.persistence.*;
import jakarta.validation.constraints.*;
import java.time.LocalDateTime;
import java.util.Objects;

/**
 * Clase Mantenimiento - Representa el detalle de los mantenimientos de un Carro
 * Relación: Carro (1) <---> (N) Mantenimiento
 * Cumple con los requisitos:
 * - int: kilometraje
 * - double: costo
 * - String: tipoMantenimiento, descripcion
 * - LocalDateTime: fechaMantenimiento, proximoMantenimiento
 * - boolean: completado
 */
@Entity
@Table(name = "MANTENIMIENTO")
@JsonIgnoreProperties(ignoreUnknown = true)
public class Mantenimiento {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id")
    private Long id;

    // ===== RELACIÓN @ManyToOne CON CARRO =====
    // MUCHOS Mantenimientos pertenecen a UN Carro
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "placa_carro", nullable = false)
    @JsonBackReference  // Evita recursión infinita en JSON
    private Carro carro;

    @NotNull(message = "La fecha de mantenimiento es obligatoria")
    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    @Column(name = "fecha_mantenimiento", nullable = false)
    private LocalDateTime fechaMantenimiento;

    @NotNull(message = "El kilometraje es obligatorio")
    @Min(value = 0, message = "El kilometraje debe ser mayor o igual a 0")
    @Max(value = 1000000, message = "El kilometraje debe ser menor a 1,000,000 km")
    @Column(nullable = true)
    private Integer kilometraje;

    @NotBlank(message = "El tipo de mantenimiento es obligatorio")
    @Pattern(regexp = "PREVENTIVO|CORRECTIVO|REVISION|CAMBIO_ACEITE|CAMBIO_LLANTAS|OTROS",
             message = "El tipo debe ser: PREVENTIVO, CORRECTIVO, REVISION, CAMBIO_ACEITE, CAMBIO_LLANTAS u OTROS")
    @Column(name = "tipo_mantenimiento", nullable = false, length = 50)
    private String tipoMantenimiento;

    @NotNull(message = "El costo es obligatorio")
    @DecimalMin(value = "0.0", inclusive = true, message = "El costo debe ser mayor o igual a 0")
    @Column(name = "costo", nullable = false, columnDefinition = "NUMBER(10,2)")
    private double costo;

    @NotBlank(message = "La descripción es obligatoria")
    @Size(min = 10, max = 500, message = "La descripción debe tener entre 10 y 500 caracteres")
    @Column(length = 500)
    private String descripcion;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    @Column(name = "proximo_mantenimiento")
    private LocalDateTime proximoMantenimiento;

    @Column(nullable = false)
    private boolean completado;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    @Column(name = "fecha_registro")
    private LocalDateTime fechaRegistro;

    // Constructor por defecto
    public Mantenimiento() {
        this.fechaRegistro = LocalDateTime.now();
        this.completado = false;
    }

    // Constructor completo
    public Mantenimiento(Carro carro, LocalDateTime fechaMantenimiento, Integer kilometraje,
                        String tipoMantenimiento, double costo, String descripcion,
                        LocalDateTime proximoMantenimiento) {
        this.carro = carro;
        this.fechaMantenimiento = fechaMantenimiento;
        this.kilometraje = kilometraje;
        this.tipoMantenimiento = tipoMantenimiento;
        this.costo = costo;
        this.descripcion = descripcion;
        this.proximoMantenimiento = proximoMantenimiento;
        this.completado = false;
        this.fechaRegistro = LocalDateTime.now();
    }

    // Métodos de negocio
    public boolean esUrgente() {
        if (proximoMantenimiento == null) {
            return false;
        }
        LocalDateTime ahora = LocalDateTime.now();
        return proximoMantenimiento.isBefore(ahora.plusDays(7));
    }

    public String getEstadoMantenimiento() {
        if (completado) {
            return "COMPLETADO";
        }
        if (esUrgente()) {
            return "URGENTE";
        }
        return "PENDIENTE";
    }

    public double calcularCostoConImpuesto() {
        return costo * 1.19; // IVA del 19%
    }

    // Getters y Setters
    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public Carro getCarro() {
        return carro;
    }

    public void setCarro(Carro carro) {
        this.carro = carro;
    }

    // Helper para obtener la placa del carro (compatibilidad con código existente)
    public String getPlacaCarro() {
        return carro != null ? carro.getPlaca() : null;
    }

    public LocalDateTime getFechaMantenimiento() {
        return fechaMantenimiento;
    }

    public void setFechaMantenimiento(LocalDateTime fechaMantenimiento) {
        this.fechaMantenimiento = fechaMantenimiento;
    }

    public Integer getKilometraje() {
        return kilometraje;
    }

    public void setKilometraje(Integer kilometraje) {
        this.kilometraje = kilometraje;
    }

    public String getTipoMantenimiento() {
        return tipoMantenimiento;
    }

    public void setTipoMantenimiento(String tipoMantenimiento) {
        this.tipoMantenimiento = tipoMantenimiento;
    }

    public double getCosto() {
        return costo;
    }

    public void setCosto(double costo) {
        this.costo = costo;
    }

    public String getDescripcion() {
        return descripcion;
    }

    public void setDescripcion(String descripcion) {
        this.descripcion = descripcion;
    }

    public LocalDateTime getProximoMantenimiento() {
        return proximoMantenimiento;
    }

    public void setProximoMantenimiento(LocalDateTime proximoMantenimiento) {
        this.proximoMantenimiento = proximoMantenimiento;
    }

    public boolean isCompletado() {
        return completado;
    }

    public void setCompletado(boolean completado) {
        this.completado = completado;
    }

    public LocalDateTime getFechaRegistro() {
        return fechaRegistro;
    }

    public void setFechaRegistro(LocalDateTime fechaRegistro) {
        this.fechaRegistro = fechaRegistro;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Mantenimiento that = (Mantenimiento) o;
        return Objects.equals(id, that.id);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }

    @Override
    public String toString() {
        return "Mantenimiento{" +
                "id=" + id +
                ", placaCarro='" + getPlacaCarro() + '\'' +
                ", fechaMantenimiento=" + fechaMantenimiento +
                ", kilometraje=" + kilometraje +
                ", tipoMantenimiento='" + tipoMantenimiento + '\'' +
                ", costo=" + costo +
                ", completado=" + completado +
                '}';
    }
}
