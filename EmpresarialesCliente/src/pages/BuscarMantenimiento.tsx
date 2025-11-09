import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Mantenimiento } from '../types/Mantenimiento';
import { getMantenimientosPorCarro } from '../services/mantenimientoApi';

export default function BuscarMantenimiento() {
  const [searchPlaca, setSearchPlaca] = useState<string>('');
  const [mantenimientos, setMantenimientos] = useState<Mantenimiento[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [hasSearched, setHasSearched] = useState(false);
  const navigate = useNavigate();

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!searchPlaca.trim()) {
      setError('Por favor ingrese una placa de carro para buscar');
      return;
    }

    setIsLoading(true);
    setError('');
    setMantenimientos([]);
    setHasSearched(true);

    try {
      const result = await getMantenimientosPorCarro(searchPlaca.trim());

      if (result && result.length > 0) {
        setMantenimientos(result);
      } else {
        setError(`No se encontraron mantenimientos para el carro con placa: ${searchPlaca}`);
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al buscar mantenimientos';
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
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  };

  const getTipoMantenimientoBadge = (tipo: string) => {
    const badges: Record<string, string> = {
      'PREVENTIVO': 'bg-blue-100 text-blue-800',
      'CORRECTIVO': 'bg-red-100 text-red-800',
      'REVISION': 'bg-purple-100 text-purple-800',
      'CAMBIO_ACEITE': 'bg-amber-100 text-amber-800',
      'CAMBIO_LLANTAS': 'bg-green-100 text-green-800',
      'OTROS': 'bg-gray-100 text-gray-800'
    };
    return badges[tipo] || badges['OTROS'];
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-indigo-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-6xl mx-auto">

        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <button
            onClick={() => navigate('/mantenimientos')}
            className="inline-flex items-center gap-2 px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors font-medium mb-4"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            Volver a Mantenimientos
          </button>

          <div className="text-center">
            <div className="inline-flex items-center justify-center w-16 h-16 bg-indigo-100 rounded-full mb-4">
              <svg className="w-8 h-8 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Buscar Mantenimientos</h1>
            <p className="text-gray-600">Ingrese la placa del vehículo para ver su historial de mantenimientos</p>
          </div>
        </div>

        {/* Search Form */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <form onSubmit={handleSearch} className="max-w-2xl mx-auto">
            <div className="mb-4">
              <label htmlFor="placa" className="block text-sm font-medium text-gray-700 mb-2">
                Placa del Carro
              </label>
              <input
                type="text"
                id="placa"
                value={searchPlaca}
                onChange={(e) => setSearchPlaca(e.target.value.toUpperCase())}
                placeholder="Ej: ABC-123"
                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all text-lg"
                disabled={isLoading}
              />
            </div>

            <button
              type="submit"
              className="w-full inline-flex items-center justify-center gap-2 px-6 py-3 bg-gradient-to-r from-indigo-600 to-indigo-700 text-white rounded-lg hover:from-indigo-700 hover:to-indigo-800 transition-all font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
              disabled={isLoading || !searchPlaca.trim()}
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
                  Buscar Mantenimientos
                </>
              )}
            </button>
          </form>
        </div>

        {/* Error Messages */}
        {error && (
          <div className="rounded-xl p-4 mb-6 border bg-red-50 border-red-200">
            <div className="flex items-center gap-3">
              <svg className="w-5 h-5 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <span className="font-medium text-red-800">{error}</span>
            </div>
          </div>
        )}

        {/* Search Results */}
        {hasSearched && !isLoading && mantenimientos.length > 0 && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-6">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">Mantenimientos Encontrados</h2>
                <p className="text-gray-600 mt-1">
                  {mantenimientos.length} mantenimiento(s) para el vehículo <span className="font-semibold text-indigo-600">{searchPlaca}</span>
                </p>
              </div>
            </div>

            <div className="grid gap-4">
              {mantenimientos.map((mant) => (
                <div key={mant.id} className="border border-gray-200 rounded-xl p-6 hover:shadow-md transition-shadow">
                  <div className="flex items-start justify-between mb-4">
                    <div className="flex items-center gap-3">
                      <div className="p-2 bg-indigo-100 rounded-lg">
                        <svg className="w-6 h-6 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                        </svg>
                      </div>
                      <div>
                        <h3 className="text-lg font-bold text-gray-900">Mantenimiento #{mant.id}</h3>
                        <p className="text-sm text-gray-500">{formatDate(mant.fechaMantenimiento)}</p>
                      </div>
                    </div>
                    <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${getTipoMantenimientoBadge(mant.tipoMantenimiento)}`}>
                      {mant.tipoMantenimiento.replace('_', ' ')}
                    </span>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                    <div>
                      <span className="text-sm text-gray-500">Kilometraje</span>
                      <p className="text-lg font-semibold text-gray-900">{mant.kilometraje?.toLocaleString('es-CO')} km</p>
                    </div>
                    <div>
                      <span className="text-sm text-gray-500">Costo</span>
                      <p className="text-lg font-semibold text-green-600">{formatCurrency(mant.costo)}</p>
                    </div>
                  </div>

                  <div className="mb-4">
                    <span className="text-sm text-gray-500">Descripción</span>
                    <p className="text-gray-700 mt-1">{mant.descripcion}</p>
                  </div>

                  {mant.proximoMantenimiento && (
                    <div className="mb-4 p-3 bg-amber-50 border border-amber-200 rounded-lg">
                      <div className="flex items-center gap-2">
                        <svg className="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <div>
                          <span className="text-sm font-medium text-amber-800">Próximo Mantenimiento</span>
                          <p className="text-sm text-amber-700">{formatDate(mant.proximoMantenimiento)}</p>
                        </div>
                      </div>
                    </div>
                  )}

                  <div className="flex items-center gap-2 pt-4 border-t border-gray-200">
                    <button
                      onClick={() => navigate(`/mantenimientos/actualizar?id=${mant.id}`)}
                      className="inline-flex items-center gap-1 px-4 py-2 bg-amber-50 text-amber-700 hover:bg-amber-100 rounded-lg transition-colors text-sm font-medium border border-amber-200"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                      </svg>
                      Actualizar
                    </button>
                    <button
                      onClick={() => navigate(`/mantenimientos/eliminar?id=${mant.id}`)}
                      className="inline-flex items-center gap-1 px-4 py-2 bg-red-50 text-red-700 hover:bg-red-100 rounded-lg transition-colors text-sm font-medium border border-red-200"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                      </svg>
                      Eliminar
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* No Results */}
        {hasSearched && !isLoading && mantenimientos.length === 0 && !error && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-12 text-center">
            <svg className="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <h3 className="text-xl font-bold text-gray-900 mb-2">No se encontraron mantenimientos</h3>
            <p className="text-gray-600 mb-4">
              No hay registros de mantenimiento para el vehículo con placa: <strong>{searchPlaca}</strong>
            </p>
            <button
              onClick={() => {
                setSearchPlaca('');
                setHasSearched(false);
                setError('');
              }}
              className="inline-flex items-center gap-2 px-6 py-2.5 bg-white border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 hover:border-gray-400 transition-all font-medium"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              Buscar Otra Placa
            </button>
          </div>
        )}

        {/* Instructions */}
        {!hasSearched && (
          <div className="bg-gradient-to-r from-indigo-50 to-purple-50 rounded-2xl border border-indigo-200 p-6">
            <h3 className="text-lg font-bold text-gray-900 mb-3 flex items-center gap-2">
              <svg className="w-6 h-6 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              Instrucciones
            </h3>
            <ol className="list-decimal list-inside space-y-2 text-gray-700">
              <li>Ingrese la placa del vehículo (ej: ABC-123)</li>
              <li>Haga clic en "Buscar Mantenimientos"</li>
              <li>Verá el historial completo de mantenimientos del vehículo</li>
              <li>Puede actualizar o eliminar cualquier registro desde la lista</li>
            </ol>
          </div>
        )}

      </div>
    </div>
  );
}
