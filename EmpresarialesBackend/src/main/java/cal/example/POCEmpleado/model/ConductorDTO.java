package cal.example.POCEmpleado.model;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
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
@JsonIgnoreProperties(ignoreUnknown = true)
public class ConductorDTO {

    private String cedula;
    private String nombre;
    private String apellido;
    private String telefono;
    private String licenciaNumero;

    private LocalDateTime fechaNacimiento;

    private Double salario;
    private Boolean activo;

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
