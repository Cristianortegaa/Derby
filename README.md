# Derby - Solución Integrada Frontend + Backend

## Estructura del Proyecto

- **Derby.Backend**: API REST en C# con .NET
- **Derby.Frontend**: Aplicación Angular

## Cómo ejecutar

### Backend
1. Navega a la carpeta `Derby.Backend`
2. Asegúrate de que PostgreSQL está corriendo
3. Ejecuta: `dotnet run`
4. El servidor estará disponible en `https://localhost:5000`

### Frontend
1. Navega a la carpeta `Derby.Frontend`
2. Instala las dependencias: `npm install`
3. Ejecuta: `npm start` o `ng serve`
4. Accede a `http://localhost:4200`

## Integración

### Endpoints disponibles:
- `GET /api/equipos` - Obtener todos los equipos
- `GET /api/equipos/{id}` - Obtener equipo por ID
- `POST /api/equipos` - Crear equipo
- `PUT /api/equipos/{id}` - Actualizar equipo
- `DELETE /api/equipos/{id}` - Eliminar equipo

### Rutas del Frontend:
- `/` - Home
- `/login` - Login
- `/clubes` - Clubes
- `/competiciones` - Competiciones
- `/equipos` - Equipos (consumiendo el API del backend)

## Configuración de CORS

El backend está configurado para permitir peticiones desde:
- `http://localhost:4200`
- `https://localhost:4200`

## Notas

- El frontend Angular se conecta al backend usando la URL `http://localhost:5000/api/equipos`
- CORS está habilitado para permitir la comunicación entre frontend (puerto 4200) y backend (puerto 5000)
- Se utiliza `HttpClient` de Angular para las peticiones HTTP

