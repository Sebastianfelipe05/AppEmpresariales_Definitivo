package cal.example.POCEmpleado.config;

import cal.example.POCEmpleado.model.Carro;
import cal.example.POCEmpleado.service.CarroService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.CommandLineRunner;
import org.springframework.stereotype.Component;

import java.time.LocalDateTime;

/**
 * Inicializador de datos simple para carros
 * DESACTIVADO: La base de datos inicia vacía
 */
//@Component  // Comentado para desactivar la inicialización automática
public class DataInitializer implements CommandLineRunner {

    @Autowired
    private CarroService carroService;

    @Override
    public void run(String... args) throws Exception {
        // ⚠️ DESACTIVADO: La base de datos debe iniciar vacía
        // Si necesitas datos de prueba, descomenta la línea @Component arriba

        // Verificar si ya existen datos
        // if (carroService.count() == 0) {
        //     initializeCarros();
        // }
    }

    private void initializeCarros() {
        try {
            // Carro 1
            Carro carro1 = new Carro();
            carro1.setMarca("TOYOTA");
            carro1.setColor("ROJO");
            carro1.setPlaca("ABC-123");
            carro1.setCombustible("GASOLINA");
            carro1.setModelo("COROLLA");
            carro1.setAnio(2020);
            carro1.setEstado("USADO");
            carro1.setNumeroPuertas(4);
            carro1.setTieneAireAcondicionado(true);
            carro1.setPrecio(45000000.0);
            carro1.setTipoTransmision("AUTOMATICA");
            carro1.setFechaRegistro(LocalDateTime.now().minusDays(30));

            // Carro 2
            Carro carro2 = new Carro();
            carro2.setMarca("HONDA");
            carro2.setColor("AZUL");
            carro2.setPlaca("DEF-456");
            carro2.setCombustible("GASOLINA");
            carro2.setModelo("CIVIC");
            carro2.setAnio(2022);
            carro2.setEstado("NUEVO");
            carro2.setNumeroPuertas(4);
            carro2.setTieneAireAcondicionado(true);
            carro2.setPrecio(55000000.0);
            carro2.setTipoTransmision("MANUAL");
            carro2.setFechaRegistro(LocalDateTime.now().minusDays(15));

            // Carro 3
            Carro carro3 = new Carro();
            carro3.setMarca("CHEVROLET");
            carro3.setColor("BLANCO");
            carro3.setPlaca("GHI-789");
            carro3.setCombustible("HIBRIDO");
            carro3.setModelo("CRUZE");
            carro3.setAnio(2021);
            carro3.setEstado("EXCELENTE");
            carro3.setNumeroPuertas(4);
            carro3.setTieneAireAcondicionado(true);
            carro3.setPrecio(50000000.0);
            carro3.setTipoTransmision("AUTOMATICA");
            carro3.setFechaRegistro(LocalDateTime.now().minusDays(10));

            // Carro 4
            Carro carro4 = new Carro();
            carro4.setMarca("NISSAN");
            carro4.setColor("NEGRO");
            carro4.setPlaca("JKL-012");
            carro4.setCombustible("ELECTRICO");
            carro4.setModelo("LEAF");
            carro4.setAnio(2023);
            carro4.setEstado("NUEVO");
            carro4.setNumeroPuertas(5);
            carro4.setTieneAireAcondicionado(true);
            carro4.setPrecio(65000000.0);
            carro4.setTipoTransmision("AUTOMATICA");
            carro4.setFechaRegistro(LocalDateTime.now().minusDays(5));

            // Guardar carros
            carroService.save(carro1);
            carroService.save(carro2);
            carroService.save(carro3);
            carroService.save(carro4);

            System.out.println("✅ Datos de prueba inicializados correctamente:");
            System.out.println("   - 4 carros creados");
            System.out.println("   - API REST disponible en: /api/carro");

        } catch (Exception e) {
            System.err.println("❌ Error al inicializar datos de prueba: " + e.getMessage());
        }
    }
}

/*
 * NOTA: Para reactivar los datos de prueba automáticos:
 * 1. Descomenta la anotación @Component en la línea 15
 * 2. Descomenta las líneas 26-29 del método run()
 * 3. Reinicia el backend
 *
 * Para mantener la base de datos vacía:
 * - Deja todo comentado como está actualmente
 */
