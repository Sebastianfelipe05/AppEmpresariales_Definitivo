# 📖 Documentación de Endpoints - API REST

## 🔐 Autenticación

La API utiliza **Basic Authentication** con las siguientes credenciales:



### Configuración en cURL:
```bash
curl -u admin:admin http://localhost:8080/api/carro
```

---

## 🚗 ENDPOINTS DE CARROS

### Base URL: `http://localhost:8080/api/carro`

---

### 1. **Listar Todos los Carros**

**Descripción:** Obtiene todos los carros registrados en la base de datos.

**Endpoint:**
```
GET /api/carro
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro
Authorization: Basic Auth (admin:admin)
```

**Respuesta Exitosa (200 OK):**
```json
[
  {
    "placa": "ABC123",
    "marca": "Toyota",
    "modelo": "Corolla",
    "color": "Blanco",
    "anio": 2023,
    "estado": "DISPONIBLE",
    "combustible": "GASOLINA",
    "numeroPuertas": 4,
    "tieneAireAcondicionado": true,
    "precio": 45000000.0,
    "tipoTransmision": "AUTOMATICA",
    "fechaRegistro": "2025-01-10 10:30:00"
  }
]
```

---

### 2. **Buscar Carro por Placa**

**Descripción:** Busca un carro específico por su placa.

**Repository (CarroRepository.java línea 76):**
```java
Optional<Carro> findByPlaca(String placa);
```

**Service (CarroService.java línea 67-72):**
```java
if (filtros.containsKey("placa")) {
    String placa = filtros.get("placa").toString();
    return carroRepository.findById(placa)
            .map(Collections::singletonList)
            .orElse(Collections.emptyList());
}
```

**Endpoint:**
```
GET /api/carro?placa={placa}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?placa=ABC123
Authorization: Basic Auth (admin:admin)
```

---

### 3. **Buscar Carros por Estado**

**Descripción:** Filtra carros por su estado (DISPONIBLE, VENDIDO, RESERVADO, EN_REPARACION).

**Repository (CarroRepository.java línea 32-33):**
```java
@Query("SELECT c FROM Carro c WHERE c.estado = :estado")
List<Carro> findByEstado(@Param("estado") String estado);
```

**Service (CarroService.java línea 75-77):**
```java
if (filtros.containsKey("estado") && filtros.size() == 1) {
    return carroRepository.findByEstado(filtros.get("estado").toString());
}
```

**Endpoint:**
```
GET /api/carro?estado={estado}
```

**Valores válidos:** `DISPONIBLE`, `VENDIDO`, `RESERVADO`, `EN_REPARACION`

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?estado=DISPONIBLE
Authorization: Basic Auth (admin:admin)
```

---

### 4. **Buscar Carros por Marca**

**Descripción:** Busca carros cuya marca contenga el texto especificado (case-insensitive).

**Repository (CarroRepository.java línea 64-65):**
```java
@Query("SELECT c FROM Carro c WHERE UPPER(c.marca) LIKE UPPER(CONCAT('%', :marca, '%'))")
List<Carro> findByMarcaContaining(@Param("marca") String marca);
```

**Service (CarroService.java línea 87-89):**
```java
if (filtros.containsKey("marca") && filtros.size() == 1) {
    return carroRepository.findByMarcaContaining(filtros.get("marca").toString());
}
```

**Endpoint:**
```
GET /api/carro?marca={marca}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?marca=Toyota
Authorization: Basic Auth (admin:admin)
```

---

### 5. **Buscar Carros por Marca y Modelo**

**Descripción:** Busca carros por marca y modelo simultáneamente.

**Repository (CarroRepository.java línea 38-40):**
```java
@Query("SELECT c FROM Carro c WHERE UPPER(c.marca) LIKE UPPER(CONCAT('%', :marca, '%')) " +
       "AND UPPER(c.modelo) LIKE UPPER(CONCAT('%', :modelo, '%'))")
List<Carro> findByMarcaAndModelo(@Param("marca") String marca, @Param("modelo") String modelo);
```

**Service (CarroService.java línea 92-96):**
```java
if (filtros.containsKey("marca") && filtros.containsKey("modelo")) {
    String marca = filtros.get("marca").toString();
    String modelo = filtros.get("modelo").toString();
    return carroRepository.findByMarcaAndModelo(marca, modelo);
}
```

**Endpoint:**
```
GET /api/carro?marca={marca}&modelo={modelo}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?marca=Toyota&modelo=Corolla
Authorization: Basic Auth (admin:admin)
```

---

### 6. **Buscar Carros por Tipo de Transmisión**

**Descripción:** Filtra carros por tipo de transmisión (MANUAL o AUTOMATICA).

**Repository (CarroRepository.java línea 57-58):**
```java
@Query("SELECT c FROM Carro c WHERE c.tipoTransmision = :tipo")
List<Carro> findByTipoTransmision(@Param("tipo") String tipo);
```

**Service (CarroService.java línea 80-84):**
```java
if (filtros.containsKey("transmision") || filtros.containsKey("tipotransmision")) {
    String tipo = filtros.getOrDefault("transmision",
            filtros.get("tipotransmision")).toString();
    return carroRepository.findByTipoTransmision(tipo);
}
```

**Endpoint:**
```
GET /api/carro?transmision={tipo}
```

**Valores válidos:** `MANUAL`, `AUTOMATICA`

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?transmision=AUTOMATICA
Authorization: Basic Auth (admin:admin)
```

---

### 7. **Buscar Carros por Rango de Precio**

**Descripción:** Filtra carros dentro de un rango de precios.

**Repository (CarroRepository.java línea 51-52):**
```java
@Query("SELECT c FROM Carro c WHERE c.precio BETWEEN :precioMin AND :precioMax")
List<Carro> findByPrecioRange(@Param("precioMin") double precioMin, @Param("precioMax") double precioMax);
```

**Service (CarroService.java línea 99-103):**
```java
if (filtros.containsKey("precio_min") && filtros.containsKey("precio_max")) {
    double min = Double.parseDouble(filtros.get("precio_min").toString());
    double max = Double.parseDouble(filtros.get("precio_max").toString());
    return carroRepository.findByPrecioRange(min, max);
}
```

**Endpoint:**
```
GET /api/carro?precio_min={min}&precio_max={max}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?precio_min=20000000&precio_max=50000000
Authorization: Basic Auth (admin:admin)
```

---

### 8. **Buscar Carros por Rango de Año**

**Descripción:** Filtra carros fabricados entre dos años específicos.

**Repository (CarroRepository.java línea 45-46):**
```java
@Query("SELECT c FROM Carro c WHERE c.anio BETWEEN :anioInicio AND :anioFin")
List<Carro> findByAnioRange(@Param("anioInicio") int anioInicio, @Param("anioFin") int anioFin);
```

**Service (CarroService.java línea 104-108):**
```java
if (filtros.containsKey("anio_inicio") && filtros.containsKey("anio_fin")) {
    int inicio = Integer.parseInt(filtros.get("anio_inicio").toString());
    int fin = Integer.parseInt(filtros.get("anio_fin").toString());
    return carroRepository.findByAnioRange(inicio, fin);
}
```

**Endpoint:**
```
GET /api/carro?anio_inicio={inicio}&anio_fin={fin}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?anio_inicio=2020&anio_fin=2024
Authorization: Basic Auth (admin:admin)
```

---

### 9. **Buscar Carros con Aire Acondicionado**

**Descripción:** Filtra carros que tengan o no aire acondicionado.

**Repository (CarroRepository.java línea 70-71):**
```java
@Query("SELECT c FROM Carro c WHERE c.tieneAireAcondicionado = true")
List<Carro> findCarrosConAireAcondicionado();
```

**Service (CarroService.java línea 110-112):**
```java
if (filtros.containsKey("aire_acondicionado")) {
    boolean tieneAire = Boolean.parseBoolean(filtros.get("aire_acondicionado").toString());
    // Filtra en memoria todos los carros
}
```

**Endpoint:**
```
GET /api/carro?aire_acondicionado={true/false}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/carro?aire_acondicionado=true
Authorization: Basic Auth (admin:admin)
```

---

### 10. **Crear Nuevo Carro**

**Descripción:** Crea un nuevo carro en la base de datos.

**Endpoint:**
```
POST /api/carro
```

**Headers:**
```
Content-Type: application/json
Authorization: Basic Auth (admin:admin)
```

**Body (JSON):**
```json
{
  "placa": "XYZ789",
  "marca": "Honda",
  "modelo": "Civic",
  "color": "Negro",
  "anio": 2024,
  "estado": "DISPONIBLE",
  "combustible": "GASOLINA",
  "numeroPuertas": 4,
  "tieneAireAcondicionado": true,
  "precio": 55000000.0,
  "tipoTransmision": "MANUAL"
}
```

**Respuesta Exitosa (201 Created):**
```json
{
  "placa": "XYZ789",
  "marca": "Honda",
  "modelo": "Civic",
  "color": "Negro",
  "anio": 2024,
  "estado": "DISPONIBLE",
  "combustible": "GASOLINA",
  "numeroPuertas": 4,
  "tieneAireAcondicionado": true,
  "precio": 55000000.0,
  "tipoTransmision": "MANUAL",
  "fechaRegistro": "2025-11-10 18:30:00"
}
```

---

### 11. **Actualizar Carro**

**Descripción:** Actualiza un carro existente por su placa.

**Endpoint:**
```
PUT /api/carro/{placa}
```

**Ejemplo Postman:**
```
PUT http://localhost:8080/api/carro/ABC123
Authorization: Basic Auth (admin:admin)
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "placa": "ABC123",
  "marca": "Toyota",
  "modelo": "Corolla",
  "color": "Rojo",
  "anio": 2023,
  "estado": "VENDIDO",
  "combustible": "GASOLINA",
  "numeroPuertas": 4,
  "tieneAireAcondicionado": true,
  "precio": 42000000.0,
  "tipoTransmision": "AUTOMATICA"
}
```

---

### 12. **Eliminar Carro**

**Descripción:** Elimina un carro de la base de datos por su placa.

**Endpoint:**
```
DELETE /api/carro/{placa}
```

**Ejemplo Postman:**
```
DELETE http://localhost:8080/api/carro/ABC123
Authorization: Basic Auth (admin:admin)
```

**Respuesta Exitosa (200 OK):**
```json
{
  "message": "Carro eliminado exitosamente",
  "placa": "ABC123"
}
```

---

## 🔧 ENDPOINTS DE MANTENIMIENTOS

### Base URL: `http://localhost:8080/api/mantenimiento`

---

### 1. **Listar Todos los Mantenimientos**

**Descripción:** Obtiene todos los mantenimientos registrados con información del carro asociado (relación ManyToOne).

**Repository (MantenimientoRepository.java línea 42-45):**
```java
@Query("SELECT m FROM Mantenimiento m " +
       "JOIN FETCH m.carro c " +
       "ORDER BY m.fechaMantenimiento DESC")
List<Mantenimiento> findAllConCarro();
```

**Endpoint:**
```
GET /api/mantenimiento
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento
Authorization: Basic Auth (admin:admin)
```

**Respuesta Exitosa (200 OK):**
```json
[
  {
    "id": 1,
    "placaCarro": "ABC123",
    "fechaMantenimiento": "2025-01-15 10:30:00",
    "kilometraje": 15000,
    "tipoMantenimiento": "PREVENTIVO",
    "costo": 250000.0,
    "descripcion": "Cambio de aceite y filtros",
    "proximoMantenimiento": "2025-04-15 10:30:00",
    "completado": true,
    "fechaRegistro": "2025-01-10 08:00:00",
    "estadoMantenimiento": "COMPLETADO",
    "esUrgente": false,
    "costoConImpuesto": 297500.0
  }
]
```

---

### 2. **Buscar Mantenimientos por Placa de Carro (Relación ManyToOne)**

**Descripción:** Obtiene todos los mantenimientos de un carro específico (consulta maestro-detalle).

**Repository (MantenimientoRepository.java línea 32-36):**
```java
@Query("SELECT m FROM Mantenimiento m " +
       "JOIN FETCH m.carro c " +
       "WHERE c.placa = :placa " +
       "ORDER BY m.fechaMantenimiento DESC")
List<Mantenimiento> findMantenimientosConCarroByPlaca(@Param("placa") String placa);
```

**Endpoint 1 (Path Variable):**
```
GET /api/mantenimiento/carro/{placa}
```

**Endpoint 2 (Query Parameter):**
```
GET /api/mantenimiento?placaCarro={placa}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento/carro/ABC123
Authorization: Basic Auth (admin:admin)
```

O también:
```
GET http://localhost:8080/api/mantenimiento?placaCarro=ABC123
Authorization: Basic Auth (admin:admin)
```

---

### 3. **Buscar Mantenimientos por ID**

**Descripción:** Busca un mantenimiento específico por su ID.

**Endpoint:**
```
GET /api/mantenimiento?id={id}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?id=1
Authorization: Basic Auth (admin:admin)
```

---

### 4. **Buscar Mantenimientos por Tipo**

**Descripción:** Filtra mantenimientos por su tipo.

**Repository (MantenimientoRepository.java línea 56-57):**
```java
@Query("SELECT m FROM Mantenimiento m WHERE m.tipoMantenimiento = :tipo")
List<Mantenimiento> findByTipoMantenimiento(@Param("tipo") String tipo);
```

**Service (MantenimientoService.java línea 84-88):**
```java
if (filtros.containsKey("tipoMantenimiento") || filtros.containsKey("tipo_mantenimiento")) {
    String tipo = filtros.getOrDefault("tipoMantenimiento",
            filtros.get("tipo_mantenimiento")).toString();
    return mantenimientoRepository.findByTipoMantenimiento(tipo);
}
```

**Endpoint:**
```
GET /api/mantenimiento?tipoMantenimiento={tipo}
```

**Valores válidos:**
- `PREVENTIVO`
- `CORRECTIVO`
- `REVISION`
- `CAMBIO_ACEITE`
- `CAMBIO_LLANTAS`
- `OTROS`

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?tipoMantenimiento=PREVENTIVO
Authorization: Basic Auth (admin:admin)
```

---

### 5. **Buscar Mantenimientos por Estado de Completado**

**Descripción:** Filtra mantenimientos completados o pendientes.

**Repository (MantenimientoRepository.java línea 78-79):**
```java
@Query("SELECT m FROM Mantenimiento m WHERE m.completado = :completado")
List<Mantenimiento> findByCompletado(@Param("completado") boolean completado);
```

**Service (MantenimientoService.java línea 91-94):**
```java
if (filtros.containsKey("completado")) {
    boolean completado = Boolean.parseBoolean(filtros.get("completado").toString());
    return mantenimientoRepository.findByCompletado(completado);
}
```

**Endpoint:**
```
GET /api/mantenimiento?completado={true/false}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?completado=false
Authorization: Basic Auth (admin:admin)
```

---

### 6. **Buscar Mantenimientos por Rango de Costo**

**Descripción:** Filtra mantenimientos dentro de un rango de costos.

**Repository (MantenimientoRepository.java línea 71-73):**
```java
@Query("SELECT m FROM Mantenimiento m WHERE m.costo BETWEEN :costoMin AND :costoMax")
List<Mantenimiento> findByCostoRange(@Param("costoMin") double costoMin,
                                     @Param("costoMax") double costoMax);
```

**Service (MantenimientoService.java línea 97-101):**
```java
if (filtros.containsKey("costo_min") && filtros.containsKey("costo_max")) {
    double min = Double.parseDouble(filtros.get("costo_min").toString());
    double max = Double.parseDouble(filtros.get("costo_max").toString());
    return mantenimientoRepository.findByCostoRange(min, max);
}
```

**Endpoint:**
```
GET /api/mantenimiento?costo_min={min}&costo_max={max}
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?costo_min=100000&costo_max=500000
Authorization: Basic Auth (admin:admin)
```

---

### 7. **Buscar Mantenimientos por Rango de Fechas**

**Descripción:** Filtra mantenimientos realizados entre dos fechas.

**Repository (MantenimientoRepository.java línea 62-66):**
```java
@Query("SELECT m FROM Mantenimiento m " +
       "WHERE m.fechaMantenimiento BETWEEN :fechaInicio AND :fechaFin " +
       "ORDER BY m.fechaMantenimiento DESC")
List<Mantenimiento> findByFechaRange(@Param("fechaInicio") LocalDateTime fechaInicio,
                                     @Param("fechaFin") LocalDateTime fechaFin);
```

**Endpoint:**
```
GET /api/mantenimiento?fecha_inicio={inicio}&fecha_fin={fin}
```

**Formato de fecha:** `yyyy-MM-ddTHH:mm:ss`

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?fecha_inicio=2025-01-01T00:00:00&fecha_fin=2025-12-31T23:59:59
Authorization: Basic Auth (admin:admin)
```

---

### 8. **Obtener Mantenimientos Urgentes**

**Descripción:** Obtiene mantenimientos cuyo próximo mantenimiento es en los próximos 7 días.

**Repository (MantenimientoRepository.java línea 85-88):**
```java
@Query("SELECT m FROM Mantenimiento m " +
       "WHERE m.proximoMantenimiento BETWEEN CURRENT_TIMESTAMP AND :fechaLimite " +
       "AND m.completado = false")
List<Mantenimiento> findMantenimientosUrgentes(@Param("fechaLimite") LocalDateTime fechaLimite);
```

**Service (MantenimientoService.java línea 139-144):**
```java
@Override
@Transactional(readOnly = true)
public List<Mantenimiento> getMantenimientosUrgentes() {
    LocalDateTime fechaLimite = LocalDateTime.now().plusDays(7);
    return mantenimientoRepository.findMantenimientosUrgentes(fechaLimite);
}
```

**Endpoint:**
```
GET /api/mantenimiento?action=urgentes
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?action=urgentes
Authorization: Basic Auth (admin:admin)
```

---

### 9. **Obtener Estadísticas de Mantenimientos**

**Descripción:** Obtiene estadísticas generales (total, costo total, promedio, urgentes).

**Endpoint:**
```
GET /api/mantenimiento?action=estadisticas
```

**Ejemplo Postman:**
```
GET http://localhost:8080/api/mantenimiento?action=estadisticas
Authorization: Basic Auth (admin:admin)
```

**Respuesta Exitosa (200 OK):**
```json
{
  "totalMantenimientos": 25,
  "costoTotal": 5750000.00,
  "costoPromedio": 230000.00,
  "mantenimientosUrgentes": 3
}
```

---

### 10. **Crear Nuevo Mantenimiento**

**Descripción:** Crea un nuevo mantenimiento asociado a un carro.

**Endpoint:**
```
POST /api/mantenimiento
```

**Headers:**
```
Content-Type: application/json
Authorization: Basic Auth (admin:admin)
```

**Body (JSON):**
```json
{
  "carro": {
    "placa": "ABC123"
  },
  "fechaMantenimiento": "2025-11-10 14:30:00",
  "kilometraje": 20000,
  "tipoMantenimiento": "PREVENTIVO",
  "costo": 350000,
  "descripcion": "Cambio de aceite, filtros y revisión general del vehículo",
  "proximoMantenimiento": "2026-02-10 14:30:00",
  "completado": false
}
```

**Respuesta Exitosa (201 Created):**
```json
{
  "id": 5,
  "placaCarro": "ABC123",
  "fechaMantenimiento": "2025-11-10 14:30:00",
  "kilometraje": 20000,
  "tipoMantenimiento": "PREVENTIVO",
  "costo": 350000.0,
  "descripcion": "Cambio de aceite, filtros y revisión general del vehículo",
  "proximoMantenimiento": "2026-02-10 14:30:00",
  "completado": false,
  "fechaRegistro": "2025-11-10 18:45:00"
}
```

---

### 11. **Actualizar Mantenimiento**

**Descripción:** Actualiza un mantenimiento existente por su ID.

**Endpoint:**
```
PUT /api/mantenimiento/{id}
```

**Ejemplo Postman:**
```
PUT http://localhost:8080/api/mantenimiento/1
Authorization: Basic Auth (admin:admin)
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "carro": {
    "placa": "ABC123"
  },
  "fechaMantenimiento": "2025-01-15 10:30:00",
  "kilometraje": 15000,
  "tipoMantenimiento": "PREVENTIVO",
  "costo": 250000,
  "descripcion": "Cambio de aceite y filtros completo",
  "proximoMantenimiento": "2025-04-15 10:30:00",
  "completado": true
}
```

---

### 12. **Eliminar Mantenimiento**

**Descripción:** Elimina un mantenimiento de la base de datos por su ID.

**Endpoint:**
```
DELETE /api/mantenimiento/{id}
```

**Ejemplo Postman:**
```
DELETE http://localhost:8080/api/mantenimiento/1
Authorization: Basic Auth (admin:admin)
```

**Respuesta Exitosa (200 OK):**
```json
{
  "message": "Mantenimiento eliminado exitosamente",
  "id": "1"
}
```

---

## 📊 RELACIONES OneToMany y ManyToOne

### **Relación: Un Carro tiene Muchos Mantenimientos (@OneToMany)**

**En Carro.java (línea 37-41):**
```java
@OneToMany(mappedBy = "carro", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.LAZY)
@JsonManagedReference
private List<Mantenimiento> mantenimientos = new ArrayList<>();
```

**Consulta para obtener Carro con sus Mantenimientos:**

En este proyecto, **no se expone directamente** esta consulta porque el frontend la maneja de forma separada:
1. Primero busca el carro: `GET /api/carro?placa=ABC123`
2. Luego busca sus mantenimientos: `GET /api/mantenimiento/carro/ABC123`

---

### **Relación: Muchos Mantenimientos pertenecen a Un Carro (@ManyToOne)**

**En Mantenimiento.java:**
```java
@ManyToOne(fetch = FetchType.LAZY)
@JoinColumn(name = "placa_carro", nullable = false)
@JsonBackReference
private Carro carro;
```

**Consulta Maestro-Detalle (JOIN FETCH):**

```java
@Query("SELECT m FROM Mantenimiento m " +
       "JOIN FETCH m.carro c " +
       "WHERE c.placa = :placa " +
       "ORDER BY m.fechaMantenimiento DESC")
List<Mantenimiento> findMantenimientosConCarroByPlaca(@Param("placa") String placa);
```

**Endpoint:**
```
GET /api/mantenimiento/carro/ABC123
```

Esta consulta retorna los mantenimientos **CON** la información del carro asociado cargada en memoria (optimización con `JOIN FETCH`).

---

## 🔍 TABLA RESUMEN: REPOSITORY → ENDPOINT

### **CARROS**

| Consulta Repository | Endpoint | Ejemplo |
|---|---|---|
| `findAll()` | `GET /api/carro` | `http://localhost:8080/api/carro` |
| `findById(placa)` | `GET /api/carro?placa={placa}` | `http://localhost:8080/api/carro?placa=ABC123` |
| `findByEstado(estado)` | `GET /api/carro?estado={estado}` | `http://localhost:8080/api/carro?estado=DISPONIBLE` |
| `findByMarcaContaining(marca)` | `GET /api/carro?marca={marca}` | `http://localhost:8080/api/carro?marca=Toyota` |
| `findByMarcaAndModelo(...)` | `GET /api/carro?marca={m}&modelo={mod}` | `http://localhost:8080/api/carro?marca=Toyota&modelo=Corolla` |
| `findByTipoTransmision(tipo)` | `GET /api/carro?transmision={tipo}` | `http://localhost:8080/api/carro?transmision=MANUAL` |
| `findByPrecioRange(min,max)` | `GET /api/carro?precio_min={min}&precio_max={max}` | `http://localhost:8080/api/carro?precio_min=20000000&precio_max=50000000` |
| `findByAnioRange(inicio,fin)` | `GET /api/carro?anio_inicio={i}&anio_fin={f}` | `http://localhost:8080/api/carro?anio_inicio=2020&anio_fin=2024` |
| `findCarrosConAireAcondicionado()` | `GET /api/carro?aire_acondicionado=true` | `http://localhost:8080/api/carro?aire_acondicionado=true` |
| `save(carro)` | `POST /api/carro` | `POST http://localhost:8080/api/carro` + Body JSON |
| `save(carro)` (update) | `PUT /api/carro/{placa}` | `PUT http://localhost:8080/api/carro/ABC123` + Body JSON |
| `deleteById(placa)` | `DELETE /api/carro/{placa}` | `DELETE http://localhost:8080/api/carro/ABC123` |

---

### **MANTENIMIENTOS**

| Consulta Repository | Endpoint | Ejemplo |
|---|---|---|
| `findAll()` | `GET /api/mantenimiento` | `http://localhost:8080/api/mantenimiento` |
| `findAllConCarro()` | `GET /api/mantenimiento` | `http://localhost:8080/api/mantenimiento` (mismo) |
| `findById(id)` | `GET /api/mantenimiento?id={id}` | `http://localhost:8080/api/mantenimiento?id=1` |
| `findByPlacaCarro(placa)` | `GET /api/mantenimiento?placaCarro={placa}` | `http://localhost:8080/api/mantenimiento?placaCarro=ABC123` |
| `findMantenimientosConCarroByPlaca(placa)` | `GET /api/mantenimiento/carro/{placa}` | `http://localhost:8080/api/mantenimiento/carro/ABC123` |
| `findByTipoMantenimiento(tipo)` | `GET /api/mantenimiento?tipoMantenimiento={tipo}` | `http://localhost:8080/api/mantenimiento?tipoMantenimiento=PREVENTIVO` |
| `findByCompletado(bool)` | `GET /api/mantenimiento?completado={bool}` | `http://localhost:8080/api/mantenimiento?completado=false` |
| `findByCostoRange(min,max)` | `GET /api/mantenimiento?costo_min={min}&costo_max={max}` | `http://localhost:8080/api/mantenimiento?costo_min=100000&costo_max=500000` |
| `findByFechaRange(inicio,fin)` | `GET /api/mantenimiento?fecha_inicio={i}&fecha_fin={f}` | `http://localhost:8080/api/mantenimiento?fecha_inicio=2025-01-01T00:00:00&fecha_fin=2025-12-31T23:59:59` |
| `findMantenimientosUrgentes(...)` | `GET /api/mantenimiento?action=urgentes` | `http://localhost:8080/api/mantenimiento?action=urgentes` |
| Estadísticas | `GET /api/mantenimiento?action=estadisticas` | `http://localhost:8080/api/mantenimiento?action=estadisticas` |
| `save(mantenimiento)` | `POST /api/mantenimiento` | `POST http://localhost:8080/api/mantenimiento` + Body JSON |
| `save(mantenimiento)` (update) | `PUT /api/mantenimiento/{id}` | `PUT http://localhost:8080/api/mantenimiento/1` + Body JSON |
| `deleteById(id)` | `DELETE /api/mantenimiento/{id}` | `DELETE http://localhost:8080/api/mantenimiento/1` |

---

## 💡 TIPS PARA USAR EN POSTMAN

### 1. **Configurar Authorization Globalmente**

Para no tener que configurar `admin:admin` en cada request:

1. Click derecho en tu colección → **Edit**
2. Ve a la pestaña **Authorization**
3. Selecciona **Basic Auth**
4. Username: `admin`, Password: `admin`
5. Todos los requests heredarán esta configuración

---

### 2. **Variables de Entorno**

Crea variables para no repetir la URL base:

1. Click en **Environments** (icono de ojo arriba a la derecha)
2. Crea un nuevo environment: **"Local Development"**
3. Agrega variables:
   - `base_url` = `http://localhost:8080`
   - `username` = `admin`
   - `password` = `admin`
4. Úsalas en tus requests: `{{base_url}}/api/carro`

---

### 3. **Importar Collection**

Puedes crear una colección con todos estos endpoints y compartirla con tu equipo.

---

## 📞 HEALTH CHECKS

**Verificar que el backend esté corriendo:**

```
GET http://localhost:8080/api/carro/healthCheck
Authorization: Basic Auth (admin:admin)
```

**Respuesta:**
```
Carros API Status Ok!
```

**Verificar mantenimientos:**
```
GET http://localhost:8080/api/mantenimiento/healthCheck
Authorization: Basic Auth (admin:admin)
```

**Respuesta:**
```
Mantenimientos API Status Ok!
```

---

## 🚨 CÓDIGOS DE ERROR COMUNES

| Código | Descripción | Solución |
|---|---|---|
| `401 Unauthorized` | No se envió autenticación o es incorrecta | Configurar Basic Auth con `admin:admin` |
| `400 Bad Request` | Validación fallida en el body JSON | Verificar que todos los campos requeridos estén presentes |
| `404 Not Found` | Recurso no encontrado | Verificar que la placa/ID existe en la BD |
| `500 Internal Server Error` | Error en el backend | Revisar logs del backend para más detalles |

---

## 📝 NOTAS FINALES

- **Base de Datos:** Oracle XE (puerto 1521, servicio `xepdb1`)
- **Usuario BD:** `DAE2025` / `DAE2025`
- **Puerto Backend:** `8080`
- **Autenticación:** Basic Auth (`admin:admin`)
- **Formato de Fecha:** `yyyy-MM-dd HH:mm:ss`

---

**Desarrollado por:**
- Juan David Reyes
- Julio David Suarez
- Sebastian Felipe Solano

**Universidad de Ibagué**
Facultad de Ingeniería
Desarrollo de Aplicaciones Empresariales
