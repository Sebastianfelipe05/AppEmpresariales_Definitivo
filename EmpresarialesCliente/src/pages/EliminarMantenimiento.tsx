import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { getMantenimientoById, deleteMantenimiento } from '../services/mantenimientoApi';
import type { Mantenimiento } from '../types/Mantenimiento';

export default function EliminarMantenimiento() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const idFromUrl = searchParams.get('id');

  const [id, setId] = useState(idFromUrl || '');
  const [mantenimiento, setMantenimiento] = useState<Mantenimiento | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showConfirmModal, setShowConfirmModal] = useState(false);

  const buscarMantenimiento = async (searchId?: string) => {
    const idToSearch = searchId || id.trim();

    if (!idToSearch) {
      setError('Por favor ingrese un ID');
      return;
    }

    try {
      setLoading(true);
      setError('');
      const data = await getMantenimientoById(idToSearch);
      if (data) {
        setMantenimiento(data);
      } else {
        setError('No se encontró un mantenimiento con ese ID');
        setMantenimiento(null);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al buscar el mantenimiento');
      setMantenimiento(null);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (idFromUrl) {
      setId(idFromUrl);
      buscarMantenimiento(idFromUrl);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [idFromUrl]);

  const handleEliminar = async () => {
    if (!mantenimiento) return;

    try {
      setLoading(true);
      setError('');
      await deleteMantenimiento(String(mantenimiento.id));
      alert('Mantenimiento eliminado exitosamente');
      setMantenimiento(null);
      setId('');
      setShowConfirmModal(false);
      navigate('/mantenimientos');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al eliminar el mantenimiento');
      setShowConfirmModal(false);
    } finally {
      setLoading(false);
    }
  };

  const formatearFecha = (fecha: string): string => {
    if (!fecha) return 'N/A';
    const date = new Date(fecha.replace(' ', 'T'));
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
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

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-red-50 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        {/* Breadcrumb */}
        <nav className="mb-6 flex items-center gap-2 text-sm text-gray-600">
          <button
            onClick={() => navigate('/mantenimientos')}
            className="hover:text-red-600 transition-colors"
          >
            Mantenimientos
          </button>
          <span>/</span>
          <span className="text-gray-900 font-medium">Eliminar</span>
        </nav>

        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2 flex items-center gap-3">
            <div className="w-12 h-12 bg-gradient-to-br from-red-500 to-red-700 rounded-xl flex items-center justify-center">
              <svg className="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </div>
            Eliminar Mantenimiento
          </h1>
          <p className="text-gray-600 ml-15">
            Busque y elimine un mantenimiento del sistema
          </p>
        </div>

        {/* Search Card */}
        <div className="bg-white rounded-2xl shadow-xl overflow-hidden mb-6">
          <div className="p-8">
            <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
              <svg className="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              Buscar Mantenimiento
            </h2>

            {error && (
              <div className="bg-gradient-to-r from-red-50 to-orange-50 border-l-4 border-red-500 p-4 rounded-lg mb-4">
                <div className="flex items-center gap-3">
                  <svg className="w-6 h-6 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <p className="text-red-800 font-medium">{error}</p>
                </div>
              </div>
            )}

            <div className="flex gap-3">
              <input
                type="text"
                id="id"
                value={id}
                onChange={(e) => setId(e.target.value)}
                placeholder="Ingrese el ID del mantenimiento (UUID)"
                className="flex-1 px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent transition-all"
                onKeyPress={(e) => e.key === 'Enter' && buscarMantenimiento()}
              />
              <button
                onClick={() => buscarMantenimiento()}
                disabled={loading}
                className="px-8 py-3 bg-gradient-to-r from-red-500 to-red-700 text-white rounded-lg hover:from-red-600 hover:to-red-800 font-medium shadow-lg shadow-red-500/30 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
              >
                {loading ? (
                  <span className="flex items-center gap-2">
                    <svg className="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Buscando...
                  </span>
                ) : (
                  'Buscar'
                )}
              </button>
            </div>
          </div>
        </div>

        {/* Maintenance Info Card */}
        {mantenimiento && (
          <div className="bg-white rounded-2xl shadow-xl overflow-hidden">
            <div className="p-8">
              <h2 className="text-lg font-semibold text-gray-900 mb-6 flex items-center gap-2">
                <svg className="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                Información del Mantenimiento
              </h2>

              {/* Warning Section */}
              <div className="bg-gradient-to-r from-red-50 to-orange-50 border-l-4 border-red-500 p-6 rounded-lg mb-6">
                <div className="flex items-start gap-3">
                  <svg className="w-8 h-8 text-red-600 flex-shrink-0 mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                  </svg>
                  <div>
                    <p className="text-red-800 font-semibold text-lg">¡Advertencia!</p>
                    <p className="text-red-700 mt-1">Esta acción eliminará permanentemente este mantenimiento del sistema. Esta operación no se puede deshacer.</p>
                  </div>
                </div>
              </div>

              {/* Maintenance Details */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">ID</p>
                  <p className="text-gray-900 font-mono text-sm break-all">{mantenimiento.id}</p>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">Placa del Vehículo</p>
                  <p className="text-gray-900 font-semibold text-lg">{mantenimiento.placaCarro}</p>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">Tipo de Mantenimiento</p>
                  <span className="inline-block px-3 py-1 bg-indigo-100 text-indigo-800 rounded-full text-sm font-medium">
                    {mantenimiento.tipoMantenimiento.replace('_', ' ')}
                  </span>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">Fecha</p>
                  <p className="text-gray-900 font-medium">{formatearFecha(mantenimiento.fechaMantenimiento)}</p>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">Kilometraje</p>
                  <p className="text-gray-900 font-medium">{mantenimiento.kilometraje.toLocaleString()} km</p>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">Costo</p>
                  <p className="text-gray-900 font-semibold text-lg">{formatearPrecio(mantenimiento.costo)}</p>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg">
                  <p className="text-sm text-gray-600 mb-1">Estado</p>
                  <span className={`inline-block px-3 py-1 rounded-full text-sm font-medium ${
                    mantenimiento.completado
                      ? 'bg-green-100 text-green-800'
                      : 'bg-yellow-100 text-yellow-800'
                  }`}>
                    {mantenimiento.completado ? 'COMPLETADO' : 'PENDIENTE'}
                  </span>
                </div>

                <div className="bg-gray-50 p-4 rounded-lg md:col-span-2">
                  <p className="text-sm text-gray-600 mb-1">Descripción</p>
                  <p className="text-gray-900">{mantenimiento.descripcion}</p>
                </div>
              </div>

              {/* Action Buttons */}
              <div className="flex flex-col sm:flex-row gap-3 pt-6 border-t border-gray-200">
                <button
                  onClick={() => setShowConfirmModal(true)}
                  className="flex-1 px-6 py-3 bg-gradient-to-r from-red-500 to-red-700 text-white rounded-lg hover:from-red-600 hover:to-red-800 font-medium shadow-lg shadow-red-500/30 transition-all"
                >
                  Eliminar Mantenimiento
                </button>
                <button
                  onClick={() => {
                    setMantenimiento(null);
                    setId('');
                  }}
                  className="flex-1 px-6 py-3 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-100 font-medium transition-all"
                >
                  Cancelar
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Confirmation Modal */}
        {showConfirmModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">
              <div className="bg-gradient-to-r from-red-500 to-red-700 p-6">
                <h3 className="text-2xl font-bold text-white flex items-center gap-3">
                  <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                  </svg>
                  Confirmar Eliminación
                </h3>
              </div>
              <div className="p-6">
                <p className="text-gray-700 mb-4">
                  ¿Está completamente seguro que desea eliminar este mantenimiento?
                </p>
                <div className="bg-gray-50 p-4 rounded-lg mb-6">
                  <p className="text-sm text-gray-600">Placa:</p>
                  <p className="text-gray-900 font-semibold">{mantenimiento?.placaCarro}</p>
                  <p className="text-sm text-gray-600 mt-2">Tipo:</p>
                  <p className="text-gray-900 font-semibold">{mantenimiento?.tipoMantenimiento.replace('_', ' ')}</p>
                </div>
                <p className="text-red-600 font-semibold text-sm mb-6">
                  ⚠️ Esta acción no se puede deshacer
                </p>
                <div className="flex gap-3">
                  <button
                    onClick={handleEliminar}
                    disabled={loading}
                    className="flex-1 px-6 py-3 bg-gradient-to-r from-red-500 to-red-700 text-white rounded-lg hover:from-red-600 hover:to-red-800 font-medium shadow-lg shadow-red-500/30 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                  >
                    {loading ? 'Eliminando...' : 'Sí, Eliminar'}
                  </button>
                  <button
                    onClick={() => setShowConfirmModal(false)}
                    disabled={loading}
                    className="flex-1 px-6 py-3 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-100 font-medium transition-all"
                  >
                    Cancelar
                  </button>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
