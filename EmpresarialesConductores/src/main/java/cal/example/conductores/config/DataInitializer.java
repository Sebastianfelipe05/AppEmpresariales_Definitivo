package cal.example.conductores.config;

import cal.example.conductores.model.Conductor;
import cal.example.conductores.repository.ConductorRepository;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;

@Component
@Slf4j
public class DataInitializer implements CommandLineRunner {

    @Autowired
    private ConductorRepository conductorRepository;

    @Override
    public void run(String... args) throws Exception {
        log.info("Inicializando datos de prueba para Conductores...");

        if (conductorRepository.count() == 0) {
            // Crear conductores de prueba
            Conductor conductor1 = new Conductor();
            conductor1.setCedula("1098765432");
            conductor1.setNombre("Carlos");
            conductor1.setApellido("Rodríguez");
            conductor1.setTelefono("3201234567");
            conductor1.setLicenciaNumero("LIC-2020-001");
            conductor1.setFechaNacimiento(LocalDateTime.of(1985, 5, 15, 0, 0));
            conductor1.setSalario(2500000.0);
            conductor1.setActivo(true);

            Conductor conductor2 = new Conductor();
            conductor2.setCedula("1087654321");
            conductor2.setNombre("María");
            conductor2.setApellido("González");
            conductor2.setTelefono("3109876543");
            conductor2.setLicenciaNumero("LIC-2019-002");
            conductor2.setFechaNacimiento(LocalDateTime.of(1990, 8, 22, 0, 0));
            conductor2.setSalario(3000000.0);
            conductor2.setActivo(true);

            Conductor conductor3 = new Conductor();
            conductor3.setCedula("1076543210");
            conductor3.setNombre("Juan");
            conductor3.setApellido("Martínez");
            conductor3.setTelefono("3157654321");
            conductor3.setLicenciaNumero("LIC-2021-003");
            conductor3.setFechaNacimiento(LocalDateTime.of(1988, 3, 10, 0, 0));
            conductor3.setSalario(2800000.0);
            conductor3.setActivo(false);

            conductorRepository.save(conductor1);
            conductorRepository.save(conductor2);
            conductorRepository.save(conductor3);

            log.info("✓ Datos de prueba creados: 3 conductores");
        } else {
            log.info("Base de datos ya contiene datos. Total conductores: {}", conductorRepository.count());
        }
    }
}
