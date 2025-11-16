import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Conductor } from '../types/Conductor';
import { getAllConductores, getConductoresActivos, getConductoresByNombre } from '../services/conductorApi';

export default function ListarConductores() {
  const [conductores, setConductores] = useState<Conductor[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string>('');
  const [filtroNombre, setFiltroNombre] = useState('');
  const [filtroActivo, setFiltroActivo] = useState<'TODOS' | 'ACTIVOS'>('TODOS');
  const navigate = useNavigate();

  useEffect(() => {
    loadConductores();
  }, []);

  const loadConductores = async () => {
    setIsLoading(true);
    setError('');

    try {
      const data = await getAllConductores();
      setConductores(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error al cargar conductores';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const handleBuscar = async () => {
    setIsLoading(true);
    setError('');

    try {
      let data: Conductor[];

      if (filtroNombre.trim()) {
        data = await getConductoresByNombre(filtroNombre);
      } else if (filtroActivo === 'ACTIVOS') {
        data = await getConductoresActivos();
      } else {
        data = await getAllConductores();
      }

      setConductores(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error al buscar conductores';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const handleLimpiar = () => {
    setFiltroNombre('');
    setFiltroActivo('TODOS');
    loadConductores();
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
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-gradient-to-br from-blue-600 to-blue-700 rounded-xl">
                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              <div>
                <h1 className="text-3xl font-bold text-gray-900">Listado de Conductores</h1>
                <p className="text-gray-600 mt-1">Gestiona la información de todos los conductores</p>
              </div>
            </div>
            <button
              onClick={() => navigate('/conductores/crear')}
              className="bg-gradient-to-r from-green-600 to-green-700 text-white px-6 py-3 rounded-lg font-semibold hover:from-green-700 hover:to-green-800 transition-all flex items-center gap-2"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
              </svg>
              Nuevo Conductor
            </button>
          </div>
        </div>

        {/* Filtros */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Filtros de Búsqueda</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Buscar por Nombre</label>
              <input
                type="text"
                value={filtroNombre}
                onChange={(e) => setFiltroNombre(e.target.value)}
                placeholder="Nombre del conductor..."
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">Estado</label>
              <select
                value={filtroActivo}
                onChange={(e) => setFiltroActivo(e.target.value as 'TODOS' | 'ACTIVOS')}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              >
                <option value="TODOS">Todos</option>
                <option value="ACTIVOS">Solo Activos</option>
              </select>
            </div>

            <div className="flex items-end gap-2">
              <button
                onClick={handleBuscar}
                className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-all"
              >
                Buscar
              </button>
              <button
                onClick={handleLimpiar}
                className="flex-1 bg-gray-200 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-300 transition-all"
              >
                Limpiar
              </button>
            </div>
          </div>
        </div>

        {/* Error Message */}
        {error && (
          <div className="rounded-xl p-4 mb-6 bg-red-50 border border-red-200">
            <p className="text-red-800 font-medium">{error}</p>
          </div>
        )}

        {/* Loading */}
        {isLoading && (
          <div className="text-center py-12">
            <div className="inline-block w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
            <p className="text-gray-600 mt-4 font-medium">Cargando conductores...</p>
          </div>
        )}

        {/* Tabla de Conductores */}
        {!isLoading && conductores.length > 0 && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Cédula</th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Nombre Completo</th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Teléfono</th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Licencia</th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Salario</th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Estado</th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Acciones</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {conductores.map((conductor) => (
                    <tr key={conductor.cedula} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        {conductor.cedula}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {conductor.nombre} {conductor.apellido}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {conductor.telefono}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {conductor.licenciaNumero}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">
                        {formatearSalario(conductor.salario)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`px-3 py-1 rounded-full text-xs font-semibold ${
                          conductor.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}>
                          {conductor.activo ? 'Activo' : 'Inactivo'}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm space-x-2">
                        <button
                          onClick={() => navigate(`/conductores/actualizar/${conductor.cedula}`)}
                          className="text-blue-600 hover:text-blue-800 font-medium"
                        >
                          Editar
                        </button>
                        <button
                          onClick={() => navigate(`/conductores/eliminar/${conductor.cedula}`)}
                          className="text-red-600 hover:text-red-800 font-medium"
                        >
                          Eliminar
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="bg-gray-50 px-6 py-4 border-t border-gray-200">
              <p className="text-sm text-gray-600">
                Total de conductores: <span className="font-semibold">{conductores.length}</span>
              </p>
            </div>
          </div>
        )}

        {/* No Results */}
        {!isLoading && conductores.length === 0 && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-12 text-center">
            <svg className="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
            <p className="text-gray-600 text-lg font-medium">No se encontraron conductores</p>
            <p className="text-gray-500 mt-2">Intenta ajustar los filtros o crear un nuevo conductor</p>
          </div>
        )}
      </div>
    </div>
  );
}
