package cal.example.POCEmpleado.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;

/**
 * DTO para representar un Conductor del microservicio externo
 * Este DTO permite la comunicación con el microservicio de Conductores
 * sin crear dependencias circulares
 */
@Data
@NoArgsConstructor
@AllArgsConstructor
public class ConductorDTO {

    private String cedula;
    private String nombre;
    private String apellido;
    private String telefono;
    private String licenciaNumero;

    @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss")
    private LocalDateTime fechaNacimiento;

    private Double salario;
    private Boolean activo;

    @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss")
    private LocalDateTime fechaRegistro;

    // Método auxiliar para obtener nombre completo
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
