import type { Conductor, ConductorCreateData, ConductorUpdateData } from '../types/Conductor';

const API_BASE_URL = 'http://localhost:8080/api/conductor';
const AUTH_USERNAME = 'admin';
const AUTH_PASSWORD = 'admin';

// Crear headers con autenticación básica
const createHeaders = (): HeadersInit => {
  const basicAuth = 'Basic ' + btoa(`${AUTH_USERNAME}:${AUTH_PASSWORD}`);
  return {
    'Content-Type': 'application/json',
    'Authorization': basicAuth
  };
};

// Manejar respuesta de error
const handleResponse = async (response: Response) => {
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    // console.error('❌ Error response:', errorData);
    // console.error('❌ Status:', response.status, response.statusText);

    let errorMessage = errorData.description || errorData.message || `Error ${response.status}: ${response.statusText}`;

    // Mejorar mensajes de error específicos
    if (errorMessage.includes('ConstraintViolation') || errorMessage.includes('could not execute statement')) {
      if (errorMessage.includes('CEDULA') || errorMessage.includes('cedula')) {
        errorMessage = '❌ Esta cédula ya está registrada en el sistema';
      } else if (errorMessage.includes('LICENCIA') || errorMessage.includes('licencia')) {
        errorMessage = '❌ Este número de licencia ya está registrado en el sistema';
      } else {
        errorMessage = '❌ Los datos ingresados ya existen en el sistema (cédula o licencia duplicada)';
      }
    } else if (errorMessage.includes('fechaNacimiento') || errorMessage.includes('debe ser en el pasado')) {
      errorMessage = '❌ La fecha de nacimiento debe ser en el pasado';
    } else if (errorMessage.includes('salario')) {
      errorMessage = '❌ El salario debe ser un valor positivo mayor a 0';
    } else if (errorMessage.includes('cédula debe contener')) {
      errorMessage = '❌ La cédula debe contener solo números (6 a 20 dígitos)';
    } else if (errorMessage.includes('teléfono debe contener')) {
      errorMessage = '❌ El teléfono debe contener solo números (7 a 20 dígitos)';
    }

    throw new Error(errorMessage);
  }
  const data = await response.json();
  // console.log('✅ Success response:', data);
  return data;
};

// CREATE - Crear conductor
export const createConductor = async (conductor: ConductorCreateData): Promise<Conductor> => {
  // console.log('🚀 Creating conductor:', conductor);
  // console.log('📤 Request body:', JSON.stringify(conductor));

  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: createHeaders(),
    body: JSON.stringify(conductor)
  });

  // console.log('📡 Response status:', response.status);
  // console.log('📡 Response ok:', response.ok);

  return handleResponse(response);
};

// READ - Obtener conductor por cédula
export const getConductorByCedula = async (cedula: string): Promise<Conductor> => {
  const response = await fetch(`${API_BASE_URL}/${cedula}`, {
    method: 'GET',
    headers: createHeaders()
  });
  return handleResponse(response);
};

// READ - Listar todos los conductores
export const getAllConductores = async (): Promise<Conductor[]> => {
  const response = await fetch(API_BASE_URL, {
    method: 'GET',
    headers: createHeaders()
  });
  return handleResponse(response);
};

// READ - Buscar conductores por nombre
export const getConductoresByNombre = async (nombre: string): Promise<Conductor[]> => {
  const response = await fetch(`${API_BASE_URL}?nombre=${encodeURIComponent(nombre)}`, {
    method: 'GET',
    headers: createHeaders()
  });
  return handleResponse(response);
};

// READ - Buscar conductores activos
export const getConductoresActivos = async (): Promise<Conductor[]> => {
  const response = await fetch(`${API_BASE_URL}?activo=true`, {
    method: 'GET',
    headers: createHeaders()
  });
  return handleResponse(response);
};

// READ - Obtener estadísticas
export const getEstadisticas = async (): Promise<any> => {
  const response = await fetch(`${API_BASE_URL}?action=estadisticas`, {
    method: 'GET',
    headers: createHeaders()
  });
  return handleResponse(response);
};

// UPDATE - Actualizar conductor
export const updateConductor = async (cedula: string, conductor: ConductorUpdateData): Promise<Conductor> => {
  const response = await fetch(`${API_BASE_URL}/${cedula}`, {
    method: 'PUT',
    headers: createHeaders(),
    body: JSON.stringify(conductor)
  });
  return handleResponse(response);
};

// DELETE - Eliminar conductor
export const deleteConductor = async (cedula: string): Promise<string> => {
  console.log('🗑️ Deleting conductor with cedula:', cedula);
  console.log('🔗 DELETE URL:', `${API_BASE_URL}/${cedula}`);

  const response = await fetch(`${API_BASE_URL}/${cedula}`, {
    method: 'DELETE',
    headers: createHeaders()
  });

  console.log('📡 DELETE Response status:', response.status);

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    console.error('❌ DELETE Error:', errorData);

    let errorMessage = errorData.description || errorData.message || `Error ${response.status}: ${response.statusText}`;

    // Mejorar mensaje de error
    if (response.status === 404) {
      errorMessage = `❌ El conductor con cédula ${cedula} no existe en el sistema`;
    } else if (errorMessage.includes('could not execute statement') || errorMessage.includes('foreign key')) {
      errorMessage = '❌ No se puede eliminar este conductor porque tiene registros asociados (carros asignados)';
    }

    throw new Error(errorMessage);
  }

  console.log('✅ Conductor deleted successfully');
  return response.text();
};

// Health check del microservicio
export const healthCheck = async (): Promise<string> => {
  const response = await fetch(`${API_BASE_URL}/healthCheck`, {
    method: 'GET',
    headers: createHeaders()
  });
  return response.text();
};
