package cal.example.POCEmpleado.errors;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class ErrorMessage {
    private int statusCode;

    @JsonFormat(pattern = "yyyy-MM-dd'T'HH:mm:ss")
    private LocalDateTime timestamp;

    private String message;
    private String description;

    // Campos adicionales para compatibilidad con otros usos
    private String code;
    private List<String> details;
    private List<Map<String, String>> mensajes;

    // Constructor simplificado para uso común
    public ErrorMessage(int statusCode, LocalDateTime timestamp, String message, String description) {
        this.statusCode = statusCode;
        this.timestamp = timestamp;
        this.message = message;
        this.description = description;
    }

    // Constructor alternativo con code
    public ErrorMessage(String code, String message, List<String> details, List<Map<String, String>> mensajes) {
        this.code = code;
        this.message = message;
        this.details = details;
        this.mensajes = mensajes;
    }
}
