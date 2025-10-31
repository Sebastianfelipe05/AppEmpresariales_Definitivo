import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Mantenimiento } from '../types/Mantenimiento';
import { listMantenimientos, getMantenimientosPorCarro } from '../services/mantenimientoApi';

export default function ListarMantenimientos() {
  const navigate = useNavigate();
  const [mantenimientos, setMantenimientos] = useState<Mantenimiento[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  // UN SOLO FILTRO: por placa del carro
  const [filtroPlaca, setFiltroPlaca] = useState('');

  useEffect(() => {
    cargarMantenimientos();
  }, []);

  const cargarMantenimientos = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await listMantenimientos();
      setMantenimientos(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar mantenimientos');
    } finally {
      setLoading(false);
    }
  };

  const buscarPorCarro = async () => {
    if (!filtroPlaca.trim()) {
      cargarMantenimientos();
      return;
    }

    try {
      setLoading(true);
      setError('');
      const data = await getMantenimientosPorCarro(filtroPlaca.trim());
      setMantenimientos(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al buscar mantenimientos');
    } finally {
      setLoading(false);
    }
  };

  const limpiarFiltro = () => {
    setFiltroPlaca('');
    cargarMantenimientos();
  };

  const formatearFecha = (fecha: string): string => {
    if (!fecha) return 'N/A';
    const date = new Date(fecha.replace(' ', 'T'));
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const formatearPrecio = (precio: number): string => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(precio);
  };

  const getEstadoBadge = (mantenimiento: Mantenimiento) => {
    if (mantenimiento.completado) {
      return <span className="px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">COMPLETADO</span>;
    }
    if (mantenimiento.esUrgente || mantenimiento.estadoMantenimiento === 'URGENTE') {
      return <span className="px-2 py-1 text-xs font-semibold rounded-full bg-red-100 text-red-800">URGENTE</span>;
    }
    return <span className="px-2 py-1 text-xs font-semibold rounded-full bg-yellow-100 text-yellow-800">PENDIENTE</span>;
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="text-xl">Cargando mantenimientos...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Lista de Mantenimientos</h1>
        <button
          onClick={() => navigate('/mantenimientos/crear')}
          className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 flex items-center gap-2"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Nuevo Mantenimiento
        </button>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {/* FILTRO SIMPLE - UN SOLO PARÁMETRO: PLACA DEL CARRO */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <h2 className="text-xl font-semibold mb-4">Filtrar por Placa del Carro</h2>
        <div className="flex gap-4">
          <div className="flex-1">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Placa del Vehículo
            </label>
            <input
              type="text"
              value={filtroPlaca}
              onChange={(e) => setFiltroPlaca(e.target.value.toUpperCase())}
              placeholder="Ej: ABC-123"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div className="flex items-end gap-2">
            <button
              onClick={buscarPorCarro}
              className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 flex items-center gap-2"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              Buscar
            </button>
            {filtroPlaca && (
              <button
                onClick={limpiarFiltro}
                className="px-6 py-2 bg-gray-500 text-white rounded-md hover:bg-gray-600 flex items-center gap-2"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
                Limpiar
              </button>
            )}
          </div>
        </div>
        {filtroPlaca && (
          <p className="mt-3 text-sm text-gray-600">
            Mostrando mantenimientos para la placa: <strong className="text-blue-600">{filtroPlaca}</strong>
          </p>
        )}
      </div>

      {/* Estadísticas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
        <div className="bg-blue-100 p-4 rounded-lg">
          <div className="text-sm text-gray-600">Total Mantenimientos</div>
          <div className="text-2xl font-bold">{mantenimientos.length}</div>
        </div>
        <div className="bg-green-100 p-4 rounded-lg">
          <div className="text-sm text-gray-600">Completados</div>
          <div className="text-2xl font-bold">
            {mantenimientos.filter(m => m.completado).length}
          </div>
        </div>
        <div className="bg-yellow-100 p-4 rounded-lg">
          <div className="text-sm text-gray-600">Costo Total</div>
          <div className="text-2xl font-bold">
            {formatearPrecio(mantenimientos.reduce((sum, m) => sum + m.costo, 0))}
          </div>
        </div>
      </div>

      {/* Tabla */}
      {mantenimientos.length === 0 ? (
        <div className="text-center py-8 text-gray-500">
          No se encontraron mantenimientos
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Placa Carro
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Fecha
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Tipo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Kilometraje
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Costo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Estado
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Descripción
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {mantenimientos.map((mantenimiento) => (
                  <tr key={mantenimiento.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {mantenimiento.placaCarro}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {formatearFecha(mantenimiento.fechaMantenimiento)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {mantenimiento.tipoMantenimiento.replace('_', ' ')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {mantenimiento.kilometraje.toLocaleString()} km
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {formatearPrecio(mantenimiento.costo)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      {getEstadoBadge(mantenimiento)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-500 max-w-xs truncate">
                      {mantenimiento.descripcion}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <div className="flex gap-2">
                        <button
                          onClick={() => navigate(`/mantenimientos/actualizar/${mantenimiento.id}`)}
                          className="text-blue-600 hover:text-blue-900"
                          title="Actualizar"
                        >
                          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                          </svg>
                        </button>
                        <button
                          onClick={() => navigate(`/mantenimientos/eliminar/${mantenimiento.id}`)}
                          className="text-red-600 hover:text-red-900"
                          title="Eliminar"
                        >
                          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                          </svg>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
