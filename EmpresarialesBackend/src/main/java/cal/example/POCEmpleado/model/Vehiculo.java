package cal.example.POCEmpleado.model;

import jakarta.persistence.*;
import jakarta.validation.constraints.*;

@MappedSuperclass  // Indica que esta clase es una superclase JPA (no tiene tabla propia)
public abstract class Vehiculo {

    @NotBlank(message = "La marca es obligatoria")
    @Column(nullable = false, length = 50)
    private String marca;

    @NotBlank(message = "El color es obligatorio")
    @Column(nullable = false, length = 30)
    private String color;

    @Id  // Clave primaria
    @NotBlank(message = "La placa es obligatoria")
    @Pattern(regexp = "^[A-Z]{3}-[0-9]{3}$", message = "La placa debe tener el formato ABC-123")
    @Column(name = "placa", length = 10)
    private String placa;

    @NotBlank(message = "El combustible es obligatorio")
    @Pattern(regexp = "GASOLINA|DIESEL|HIBRIDO|ELECTRICO", message = "El combustible debe ser GASOLINA, DIESEL, HIBRIDO o ELECTRICO")
    @Column(nullable = false, length = 20)
    private String combustible;

    @NotBlank(message = "El modelo es obligatorio")
    @Column(nullable = false, length = 50)
    private String modelo;

    @NotNull(message = "El año es obligatorio")
    @Min(value = 1950, message = "El año debe ser mayor o igual a 1950")
    @Max(value = 2025, message = "El año debe ser menor o igual al año actual")
    @Column(nullable = false)
    private int anio;

    @NotBlank(message = "El estado es obligatorio")
    @Pattern(regexp = "NUEVO|USADO|EXCELENTE|BUENO|REGULAR", message = "El estado debe ser NUEVO, USADO, EXCELENTE, BUENO o REGULAR")
    @Column(nullable = false, length = 20)
    private String estado;

    // Constructor por defecto
    public Vehiculo() {
    }

    public Vehiculo(String marca, String color, String placa, String combustible, String modelo, int anio, String estado) {
        this.marca = marca;
        this.color = color;
        this.placa = placa;
        this.combustible = combustible;
        this.modelo = modelo;
        this.anio = anio;
        this.estado = estado;
    }

    //POLIMORFISMO: Metodo que cada clase hija implementara de manera diferente
    public abstract String getTipoVehiculo();

    public abstract String getDetallesEspecificos();

    public abstract double calcularValorComercial();

    //Metodos se comporta diferente segun cada clase hija
    public abstract String obtenerInformacionMantenimiento();

    //Metodo para mostrar la info completa
    public String getInformacionCompleta() {
        return "=== " + getTipoVehiculo() + " ===\n"
                + "Marca: " + marca + "\n"
                + "Color: " + color + "\n"
                + "Placa: " + placa + "\n"
                + "Modelo: " + modelo + "\n"
                + "Año: " + anio + "\n"
                + "Estado: " + estado + "\n"
                + getDetallesEspecificos() + "\n"
                + "Valor Comercial: $" + String.format("%.2f", calcularValorComercial());
    }

    // Getters y Setters
    public String getMarca() {
        return marca;
    }

    public void setMarca(String marca) {
        this.marca = marca;
    }

    public String getColor() {
        return color;
    }

    public void setColor(String color) {
        this.color = color;
    }

    public String getPlaca() {
        return placa;
    }

    public void setPlaca(String placa) {
        this.placa = placa;
    }

    public String getCombustible() {
        return combustible;
    }

    public void setCombustible(String combustible) {
        this.combustible = combustible;
    }

    public String getModelo() {
        return modelo;
    }

    public void setModelo(String modelo) {
        this.modelo = modelo;
    }

    public int getAnio() {
        return anio;
    }

    public void setAnio(int anio) {
        this.anio = anio;
    }

    public String getEstado() {
        return estado;
    }

    public void setEstado(String estado) {
        this.estado = estado;
    }

    @Override
    public String toString() {
        return "Vehiculo{" +
                "marca='" + marca + '\'' +
                ", color='" + color + '\'' +
                ", placa='" + placa + '\'' +
                ", combustible='" + combustible + '\'' +
                ", modelo='" + modelo + '\'' +
                ", anio=" + anio +
                ", estado='" + estado + '\'' +
                '}';
    }
}
