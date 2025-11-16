import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Conductor } from '../types/Conductor';
import { getConductorByCedula } from '../services/conductorApi';

export default function BuscarConductor() {
  const [cedula, setCedula] = useState('');
  const [conductor, setConductor] = useState<Conductor | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const navigate = useNavigate();

  const handleBuscar = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!cedula.trim()) {
      setError('Por favor ingrese una cédula');
      return;
    }

    setIsLoading(true);
    setError('');
    setConductor(null);

    try {
      const data = await getConductorByCedula(cedula);
      setConductor(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Conductor no encontrado';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const formatearSalario = (salario: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(salario);
  };

  const formatearFecha = (fecha: string) => {
    return new Date(fecha).toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center gap-4">
            <div className="p-3 bg-gradient-to-br from-purple-600 to-purple-700 rounded-xl">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Buscar Conductor</h1>
              <p className="text-gray-600 mt-1">Ingrese la cédula del conductor a buscar</p>
            </div>
          </div>
        </div>

        {/* Search Form */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-8 mb-6">
          <form onSubmit={handleBuscar} className="space-y-4">
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-2">
                Cédula del Conductor
              </label>
              <input
                type="text"
                value={cedula}
                onChange={(e) => setCedula(e.target.value)}
                placeholder="Ej: 1098765432"
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
            </div>

            <div className="flex gap-4">
              <button
                type="submit"
                disabled={isLoading}
                className="flex-1 bg-gradient-to-r from-purple-600 to-purple-700 text-white py-3 px-6 rounded-lg font-semibold hover:from-purple-700 hover:to-purple-800 transition-all disabled:opacity-50"
              >
                {isLoading ? 'Buscando...' : 'Buscar Conductor'}
              </button>
              <button
                type="button"
                onClick={() => navigate('/conductores/listar')}
                className="flex-1 bg-gray-200 text-gray-700 py-3 px-6 rounded-lg font-semibold hover:bg-gray-300 transition-all"
              >
                Volver al Listado
              </button>
            </div>
          </form>
        </div>

        {/* Error Message */}
        {error && (
          <div className="rounded-xl p-4 mb-6 bg-red-50 border border-red-200">
            <div className="flex items-start gap-3">
              <svg className="w-5 h-5 text-red-600 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
              <p className="text-red-800 font-medium">{error}</p>
            </div>
          </div>
        )}

        {/* Conductor Details */}
        {conductor && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-bold text-gray-900">Información del Conductor</h2>
              <span className={`px-4 py-2 rounded-full text-sm font-semibold ${
                conductor.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
              }`}>
                {conductor.activo ? 'Activo' : 'Inactivo'}
              </span>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-500 mb-1">Cédula</p>
                <p className="text-lg font-semibold text-gray-900">{conductor.cedula}</p>
              </div>

              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-500 mb-1">Nombre Completo</p>
                <p className="text-lg font-semibold text-gray-900">{conductor.nombre} {conductor.apellido}</p>
              </div>

              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-500 mb-1">Teléfono</p>
                <p className="text-lg font-semibold text-gray-900">{conductor.telefono}</p>
              </div>

              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-500 mb-1">Número de Licencia</p>
                <p className="text-lg font-semibold text-gray-900">{conductor.licenciaNumero}</p>
              </div>

              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-500 mb-1">Fecha de Nacimiento</p>
                <p className="text-lg font-semibold text-gray-900">{formatearFecha(conductor.fechaNacimiento)}</p>
              </div>

              <div className="bg-gray-50 rounded-lg p-4">
                <p className="text-sm font-medium text-gray-500 mb-1">Salario</p>
                <p className="text-lg font-semibold text-gray-900">{formatearSalario(conductor.salario)}</p>
              </div>

              {conductor.fechaRegistro && (
                <div className="bg-gray-50 rounded-lg p-4 md:col-span-2">
                  <p className="text-sm font-medium text-gray-500 mb-1">Fecha de Registro</p>
                  <p className="text-lg font-semibold text-gray-900">{formatearFecha(conductor.fechaRegistro)}</p>
                </div>
              )}
            </div>

            {/* Actions */}
            <div className="flex gap-4 mt-8 pt-6 border-t border-gray-200">
              <button
                onClick={() => navigate(`/conductores/actualizar/${conductor.cedula}`)}
                className="flex-1 bg-blue-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-blue-700 transition-all"
              >
                Editar Conductor
              </button>
              <button
                onClick={() => navigate(`/conductores/eliminar/${conductor.cedula}`)}
                className="flex-1 bg-red-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-red-700 transition-all"
              >
                Eliminar Conductor
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
