import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Mantenimiento } from '../types/Mantenimiento';
import { getMantenimientoById } from '../services/mantenimientoApi';

export default function BuscarMantenimiento() {
  const [searchId, setSearchId] = useState<string>('');
  const [mantenimiento, setMantenimiento] = useState<Mantenimiento | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [hasSearched, setHasSearched] = useState(false);
  const navigate = useNavigate();

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!searchId.trim()) {
      setError('Por favor ingrese un ID de mantenimiento para buscar');
      return;
    }

    setIsLoading(true);
    setError('');
    setMantenimiento(null);
    setHasSearched(true);

    try {
      const result = await getMantenimientoById(searchId.trim());

      if (result) {
        setMantenimiento(result);
      } else {
        setError(`No se encontró un mantenimiento con el ID: ${searchId}`);
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al buscar el mantenimiento';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(amount);
  };

  const formatDate = (dateString: string): string => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString.replace(' ', 'T'));
    return new Intl.DateTimeFormat('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  };

  const getEstadoBadge = () => {
    if (!mantenimiento) return null;

    if (mantenimiento.completado) {
      return <span className="px-3 py-1 text-sm font-semibold rounded-full bg-green-100 text-green-800">COMPLETADO</span>;
    }
    if (mantenimiento.esUrgente || mantenimiento.estadoMantenimiento === 'URGENTE') {
      return <span className="px-3 py-1 text-sm font-semibold rounded-full bg-red-100 text-red-800">URGENTE</span>;
    }
    return <span className="px-3 py-1 text-sm font-semibold rounded-full bg-yellow-100 text-yellow-800">PENDIENTE</span>;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-purple-50 py-8">
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <button onClick={() => navigate('/')} className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
            Volver al Inicio
          </button>
          <h2 className="text-3xl font-bold text-gray-900 mb-2">Buscar Mantenimiento</h2>
          <p className="text-gray-600">Busque un mantenimiento específico por su ID único.</p>
        </div>

        {/* Search Form */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <form onSubmit={handleSearch}>
            <div className="mb-4">
              <label htmlFor="id" className="block text-sm font-medium text-gray-700 mb-2">
                ID del Mantenimiento
              </label>
              <input
                type="text"
                id="id"
                value={searchId}
                onChange={(e) => setSearchId(e.target.value)}
                placeholder="Ingrese el ID del mantenimiento (UUID)"
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all"
                disabled={isLoading}
              />
            </div>

            <button
              type="submit"
              className="w-full inline-flex items-center justify-center gap-2 px-6 py-3 bg-gradient-to-r from-purple-600 to-purple-700 text-white rounded-lg hover:from-purple-700 hover:to-purple-800 transition-all font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
              disabled={isLoading || !searchId.trim()}
            >
              {isLoading ? (
                <>
                  <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                  Buscando...
                </>
              ) : (
                <>
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  Buscar Mantenimiento
                </>
              )}
            </button>
          </form>
        </div>

        {/* Error Messages */}
        {error && (
          <div className="rounded-xl p-4 mb-6 bg-red-50 border border-red-200 text-red-800">
            <div className="flex items-center gap-3">
              <svg className="w-5 h-5 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
              <span className="font-medium">{error}</span>
            </div>
          </div>
        )}

        {/* Search Results */}
        {hasSearched && !isLoading && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
            {mantenimiento ? (
              <div>
                <div className="bg-gradient-to-r from-purple-600 to-indigo-600 text-white p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="text-2xl font-bold mb-2">Mantenimiento Encontrado</h3>
                      <p className="text-purple-100">ID: {mantenimiento.id}</p>
                    </div>
                    <div>
                      {getEstadoBadge()}
                    </div>
                  </div>
                </div>

                <div className="p-6 space-y-6">
                  {/* Información del Vehículo */}
                  <div className="border-b border-gray-200 pb-6">
                    <h4 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                      <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      Información del Vehículo
                    </h4>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <label className="text-sm text-gray-500">Placa del Carro:</label>
                        <p className="text-lg font-semibold text-gray-900">{mantenimiento.placaCarro}</p>
                      </div>
                      <div>
                        <label className="text-sm text-gray-500">Kilometraje:</label>
                        <p className="text-lg font-semibold text-gray-900">{mantenimiento.kilometraje.toLocaleString()} km</p>
                      </div>
                    </div>
                  </div>

                  {/* Detalles del Mantenimiento */}
                  <div className="border-b border-gray-200 pb-6">
                    <h4 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                      <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      Detalles del Servicio
                    </h4>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <label className="text-sm text-gray-500">Tipo de Mantenimiento:</label>
                        <p className="text-lg font-semibold text-gray-900">{mantenimiento.tipoMantenimiento.replace('_', ' ')}</p>
                      </div>
                      <div>
                        <label className="text-sm text-gray-500">Fecha del Mantenimiento:</label>
                        <p className="text-lg font-semibold text-gray-900">{formatDate(mantenimiento.fechaMantenimiento)}</p>
                      </div>
                      <div className="md:col-span-2">
                        <label className="text-sm text-gray-500">Descripción:</label>
                        <p className="text-base text-gray-900 mt-1">{mantenimiento.descripcion}</p>
                      </div>
                    </div>
                  </div>

                  {/* Información Financiera */}
                  <div className="border-b border-gray-200 pb-6">
                    <h4 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                      <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      Información Financiera
                    </h4>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div className="bg-purple-50 p-4 rounded-lg">
                        <label className="text-sm text-gray-500">Costo:</label>
                        <p className="text-2xl font-bold text-purple-700">{formatCurrency(mantenimiento.costo)}</p>
                      </div>
                      {mantenimiento.costoConImpuesto && (
                        <div className="bg-gray-50 p-4 rounded-lg">
                          <label className="text-sm text-gray-500">Costo con Impuesto (19%):</label>
                          <p className="text-2xl font-bold text-gray-900">{formatCurrency(mantenimiento.costoConImpuesto)}</p>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Próximo Mantenimiento */}
                  {mantenimiento.proximoMantenimiento && (
                    <div className="border-b border-gray-200 pb-6">
                      <h4 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                        <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                        </svg>
                        Próximo Mantenimiento
                      </h4>
                      <div>
                        <label className="text-sm text-gray-500">Fecha Programada:</label>
                        <p className="text-lg font-semibold text-gray-900">{formatDate(mantenimiento.proximoMantenimiento)}</p>
                        {mantenimiento.esUrgente && (
                          <p className="mt-2 text-sm text-red-600 font-semibold">
                            ⚠️ Este mantenimiento es urgente (próximo mantenimiento dentro de 7 días)
                          </p>
                        )}
                      </div>
                    </div>
                  )}

                  {/* Fechas del Sistema */}
                  <div>
                    <h4 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                      <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      Información del Sistema
                    </h4>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <label className="text-sm text-gray-500">Fecha de Registro:</label>
                        <p className="text-base text-gray-900">{formatDate(mantenimiento.fechaRegistro)}</p>
                      </div>
                      <div>
                        <label className="text-sm text-gray-500">Estado del Mantenimiento:</label>
                        <p className="text-base text-gray-900">{mantenimiento.estadoMantenimiento || 'N/A'}</p>
                      </div>
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="flex gap-4 pt-6">
                    <button
                      onClick={() => navigate(`/mantenimientos/actualizar?id=${mantenimiento.id}`)}
                      className="flex-1 inline-flex items-center justify-center gap-2 px-6 py-3 bg-amber-500 text-white rounded-lg hover:bg-amber-600 transition-all font-medium shadow-sm"
                    >
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                      Actualizar
                    </button>
                    <button
                      onClick={() => navigate(`/mantenimientos/eliminar?id=${mantenimiento.id}`)}
                      className="flex-1 inline-flex items-center justify-center gap-2 px-6 py-3 bg-red-500 text-white rounded-lg hover:bg-red-600 transition-all font-medium shadow-sm"
                    >
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                      Eliminar
                    </button>
                  </div>
                </div>
              </div>
            ) : (
              <div className="p-12 text-center">
                <div className="inline-flex items-center justify-center w-16 h-16 bg-gray-100 rounded-full mb-4">
                  <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </div>
                <h3 className="text-xl font-bold text-gray-900 mb-2">Mantenimiento No Encontrado</h3>
                <p className="text-gray-600 mb-6">No se encontró ningún mantenimiento con el ID: <strong>{searchId}</strong></p>
                <button
                  onClick={() => {
                    setSearchId('');
                    setHasSearched(false);
                    setError('');
                  }}
                  className="inline-flex items-center gap-2 px-6 py-3 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-all font-medium"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  Buscar Otro
                </button>
              </div>
            )}
          </div>
        )}

        {/* Instructions */}
        {!hasSearched && (
          <div className="bg-blue-50 border border-blue-200 rounded-2xl p-6">
            <h3 className="text-lg font-bold text-blue-900 mb-3 flex items-center gap-2">
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              Instrucciones
            </h3>
            <ol className="list-decimal list-inside space-y-2 text-blue-800">
              <li>Ingrese el ID (UUID) del mantenimiento que desea buscar</li>
              <li>Haga clic en "Buscar Mantenimiento"</li>
              <li>Si el mantenimiento existe, verá toda su información detallada</li>
              <li>Podrá acceder directamente a actualizar o eliminar el mantenimiento</li>
            </ol>
          </div>
        )}
      </div>
    </div>
  );
}
