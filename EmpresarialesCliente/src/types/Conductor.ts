export interface Conductor {
  cedula: string;
  nombre: string;
  apellido: string;
  telefono: string;
  licenciaNumero: string;
  fechaNacimiento: string;
  salario: number;
  activo: boolean;
  fechaRegistro?: string;
}

export interface ConductorCreateData {
  cedula: string;
  nombre: string;
  apellido: string;
  telefono: string;
  licenciaNumero: string;
  fechaNacimiento: string;
  salario: number;
  activo: boolean;
}

export interface ConductorUpdateData extends ConductorCreateData {}
