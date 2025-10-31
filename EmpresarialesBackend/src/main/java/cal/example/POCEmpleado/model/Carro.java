package cal.example.POCEmpleado.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import jakarta.persistence.*;
import jakarta.validation.constraints.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

@Entity
@Table(name = "CARRO")
@JsonIgnoreProperties(ignoreUnknown = true)
public class Carro extends Vehiculo {

    @NotNull(message = "El número de puertas es obligatorio")
    @Min(value = 2, message = "El número de puertas debe ser mínimo 2")
    @Max(value = 5, message = "El número de puertas debe ser máximo 5")
    private int numeroPuertas;

    private boolean tieneAireAcondicionado;

    @NotNull(message = "El precio es obligatorio")
    @DecimalMin(value = "0.0", inclusive = false, message = "El precio debe ser mayor que 0")
    @Column(name = "precio", columnDefinition = "NUMBER(10,2)")
    private double precio;

    @JsonFormat(pattern = "yyyy-MM-dd HH:mm:ss")
    private LocalDateTime fechaRegistro;

    @NotBlank(message = "El tipo de transmisión es obligatorio")
    @Pattern(regexp = "MANUAL|AUTOMATICA", message = "El tipo de transmisión debe ser MANUAL o AUTOMATICA")
    private String tipoTransmision;

    // ===== RELACIÓN @OneToMany CON MANTENIMIENTO =====
    // Un CARRO puede tener MUCHOS Mantenimientos
    @OneToMany(mappedBy = "carro", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.LAZY)
    @JsonManagedReference  // Evita recursión infinita en JSON
    private List<Mantenimiento> mantenimientos = new ArrayList<>();

    // Constructor por defecto
    public Carro() {
        super();
        this.fechaRegistro = LocalDateTime.now();
        this.mantenimientos = new ArrayList<>();
    }

    public Carro(String marca, String color, String placa, String combustible, String modelo, int anio,
                 String estado, int numeroPuertas, boolean tieneAireAcondicionado, double precio, String tipoTransmision) {
        super(marca, color, placa, combustible, modelo, anio, estado);
        this.numeroPuertas = numeroPuertas;
        this.tieneAireAcondicionado = tieneAireAcondicionado;
        this.precio = precio;
        this.fechaRegistro = LocalDateTime.now();
        this.tipoTransmision = tipoTransmision;
    }

    //POLIMORFISMO
    @Override
    public String getTipoVehiculo() {
        return "AUTOMÓVIL";
    }

    @Override
    public String getDetallesEspecificos() {
        return String.format("Puertas: %d | Aire A/C: %s | Transmisión: %s | Precio: $%.2f",
                numeroPuertas,
                tieneAireAcondicionado ? "Sí" : "No",
                tipoTransmision,
                precio);
    }

    @Override
    public double calcularValorComercial() {
        double valorBase = this.precio > 0 ? this.precio : 45000000;
        double depreciacion = (2025 - getAnio()) * 0.08; // 8% anual

        // Bonificaciones específicas del CARRO
        if (tieneAireAcondicionado) {
            valorBase += 3000000;
        }
        if (numeroPuertas == 4) {
            valorBase += 2000000;
        }
        if ("AUTOMATICA".equals(tipoTransmision)) {
            valorBase += 5000000;
        }

        return valorBase * (1 - Math.min(depreciacion, 0.75));
    }

    @Override
    public String obtenerInformacionMantenimiento() {
        int kilometraje = (2025 - getAnio()) * 15000; // Estimado
        return String.format("Mantenimiento cada 10.000 km | Estimado: %d km", kilometraje);
    }

    // Getters y Setters
    public int getNumeroPuertas() {
        return numeroPuertas;
    }

    public void setNumeroPuertas(int numeroPuertas) {
        this.numeroPuertas = numeroPuertas;
    }

    public boolean isTieneAireAcondicionado() {
        return tieneAireAcondicionado;
    }

    public void setTieneAireAcondicionado(boolean tieneAireAcondicionado) {
        this.tieneAireAcondicionado = tieneAireAcondicionado;
    }

    public double getPrecio() {
        return precio;
    }

    public void setPrecio(double precio) {
        this.precio = precio;
    }

    public LocalDateTime getFechaRegistro() {
        return fechaRegistro;
    }

    public void setFechaRegistro(LocalDateTime fechaRegistro) {
        this.fechaRegistro = fechaRegistro;
    }

    public String getTipoTransmision() {
        return tipoTransmision;
    }

    public void setTipoTransmision(String tipoTransmision) {
        this.tipoTransmision = tipoTransmision;
    }

    // Getters y Setters para la relación
    public List<Mantenimiento> getMantenimientos() {
        return mantenimientos;
    }

    public void setMantenimientos(List<Mantenimiento> mantenimientos) {
        this.mantenimientos = mantenimientos;
    }

    // Métodos helper para gestionar la relación bidireccional
    public void addMantenimiento(Mantenimiento mantenimiento) {
        mantenimientos.add(mantenimiento);
        mantenimiento.setCarro(this);
    }

    public void removeMantenimiento(Mantenimiento mantenimiento) {
        mantenimientos.remove(mantenimiento);
        mantenimiento.setCarro(null);
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Carro carro = (Carro) o;
        return Objects.equals(getPlaca(), carro.getPlaca());
    }

    @Override
    public int hashCode() {
        return Objects.hash(getPlaca());
    }

    @Override
    public String toString() {
        return "Carro{" +
                "numeroPuertas=" + numeroPuertas +
                ", tieneAireAcondicionado=" + tieneAireAcondicionado +
                ", precio=" + precio +
                ", fechaRegistro=" + fechaRegistro +
                ", tipoTransmision='" + tipoTransmision + '\'' +
                "} " + super.toString();
    }
}
