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
    const errorMessage = errorData.message || errorData.description || `Error ${response.status}: ${response.statusText}`;
    throw new Error(errorMessage);
  }
  return response.json();
};

// CREATE - Crear conductor
export const createConductor = async (conductor: ConductorCreateData): Promise<Conductor> => {
  const response = await fetch(API_BASE_URL, {
    method: 'POST',
    headers: createHeaders(),
    body: JSON.stringify(conductor)
  });
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
  const response = await fetch(`${API_BASE_URL}/${cedula}`, {
    method: 'DELETE',
    headers: createHeaders()
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    const errorMessage = errorData.message || errorData.description || `Error ${response.status}: ${response.statusText}`;
    throw new Error(errorMessage);
  }

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
