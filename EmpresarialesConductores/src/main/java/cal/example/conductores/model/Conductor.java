package cal.example.conductores.model;

import jakarta.persistence.*;
import jakarta.validation.constraints.*;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;

@Entity
@Table(name = "CONDUCTOR")
@Data
@NoArgsConstructor
@AllArgsConstructor
public class Conductor {

    @Id
    @Column(name = "cedula", nullable = false, unique = true, length = 20)
    @NotBlank(message = "La cédula es obligatoria")
    @Pattern(regexp = "^[0-9]{6,20}$", message = "La cédula debe contener entre 6 y 20 dígitos")
    private String cedula;

    @Column(name = "nombre", nullable = false, length = 100)
    @NotBlank(message = "El nombre es obligatorio")
    @Size(min = 2, max = 100, message = "El nombre debe tener entre 2 y 100 caracteres")
    private String nombre;

    @Column(name = "apellido", nullable = false, length = 100)
    @NotBlank(message = "El apellido es obligatorio")
    @Size(min = 2, max = 100, message = "El apellido debe tener entre 2 y 100 caracteres")
    private String apellido;

    @Column(name = "telefono", length = 20)
    @Pattern(regexp = "^[0-9]{7,20}$", message = "El teléfono debe contener entre 7 y 20 dígitos")
    private String telefono;

    @Column(name = "licencia_numero", nullable = false, unique = true, length = 50)
    @NotBlank(message = "El número de licencia es obligatorio")
    @Size(min = 5, max = 50, message = "El número de licencia debe tener entre 5 y 50 caracteres")
    private String licenciaNumero;

    @Column(name = "fecha_nacimiento", nullable = false)
    @NotNull(message = "La fecha de nacimiento es obligatoria")
    @Past(message = "La fecha de nacimiento debe ser en el pasado")
    private LocalDateTime fechaNacimiento;

    @Column(name = "salario", nullable = false)
    @NotNull(message = "El salario es obligatorio")
    @Positive(message = "El salario debe ser positivo")
    private Double salario;

    @Column(name = "activo", nullable = false)
    @NotNull(message = "El estado activo es obligatorio")
    private Boolean activo;

    @Column(name = "fecha_registro", nullable = false, updatable = false)
    private LocalDateTime fechaRegistro;

    @PrePersist
    protected void onCreate() {
        fechaRegistro = LocalDateTime.now();
        if (activo == null) {
            activo = true;
        }
    }

    // Método para obtener nombre completo
    public String getNombreCompleto() {
        return nombre + " " + apellido;
    }

    // Método para calcular edad aproximada
    public int getEdadAproximada() {
        if (fechaNacimiento == null) {
            return 0;
        }
        return LocalDateTime.now().getYear() - fechaNacimiento.getYear();
    }
}
