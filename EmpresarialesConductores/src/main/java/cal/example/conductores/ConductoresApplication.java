package cal.example.conductores;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
public class ConductoresApplication {

	public static void main(String[] args) {
		SpringApplication.run(ConductoresApplication.class, args);
		System.out.println("==============================================");
		System.out.println("Microservicio Conductores iniciado en puerto 8081");
		System.out.println("Base de datos: PostgreSQL - conductores_db");
		System.out.println("==============================================");
	}

}
