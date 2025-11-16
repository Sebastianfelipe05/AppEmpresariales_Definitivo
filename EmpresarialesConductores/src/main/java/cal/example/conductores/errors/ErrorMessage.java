package cal.example.conductores.errors;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ErrorMessage {
    private int statusCode;
    private LocalDateTime timestamp;
    private String message;
    private String description;

    public ErrorMessage(int statusCode, String message, String description) {
        this.statusCode = statusCode;
        this.timestamp = LocalDateTime.now();
        this.message = message;
        this.description = description;
    }
}
