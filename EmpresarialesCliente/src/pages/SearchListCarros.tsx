import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import type { Carro } from '../types/Carro';
import { listCarros } from '../services/carroApi';

export default function SearchListCarros() {
  const [carros, setCarros] = useState<Carro[]>([]);
  const [filteredCarros, setFilteredCarros] = useState<Carro[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' }>({ text: '', type: 'success' });

  // Filtros simples
  const [marcaFilter, setMarcaFilter] = useState('');
  const [colorFilter, setColorFilter] = useState('');
  const [modeloFilter, setModeloFilter] = useState('');

  const navigate = useNavigate();
  const location = useLocation();

  // Check for messages from navigation state
  useEffect(() => {
    if (location.state?.message) {
      setMessage({
        text: location.state.message,
        type: location.state.type || 'success'
      });

      setTimeout(() => setMessage({ text: '', type: 'success' }), 5000);
      navigate(location.pathname, { replace: true });
    }
  }, [location.state, navigate, location.pathname]);

  // Load all carros on component mount
  useEffect(() => {
    loadCarros();
  }, []);

  const loadCarros = async () => {
    setIsLoading(true);
    setError('');

    try {
      const result = await listCarros();
      setCarros(result);
      setFilteredCarros(result);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al cargar los carros';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  const applyFilters = () => {
    const marca = marcaFilter.trim().toLowerCase();
    const color = colorFilter.trim().toLowerCase();
    const modelo = modeloFilter.trim().toLowerCase();

    // Si no hay filtros, mostrar todos
    if (!marca && !color && !modelo) {
      setFilteredCarros(carros);
      return;
    }

    // Aplicar filtros combinados (AND)
    const filtered = carros.filter(carro => {
      const cumpleMarca = !marca || carro.marca?.toLowerCase().includes(marca);
      const cumpleColor = !color || carro.color?.toLowerCase().includes(color);
      const cumpleModelo = !modelo || carro.modelo?.toLowerCase().includes(modelo);

      return cumpleMarca && cumpleColor && cumpleModelo;
    });

    setFilteredCarros(filtered);
  };

  const clearFilters = () => {
    setMarcaFilter('');
    setColorFilter('');
    setModeloFilter('');
    setFilteredCarros(carros);
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      applyFilters();
    }
  };

  const formatCurrency = (value?: number): string => {
    if (!value) return 'N/A';
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(value);
  };

  const getActiveFiltersText = (): string => {
    const filters = [];
    if (marcaFilter.trim()) filters.push(`Marca: ${marcaFilter}`);
    if (colorFilter.trim()) filters.push(`Color: ${colorFilter}`);
    if (modeloFilter.trim()) filters.push(`Modelo: ${modeloFilter}`);
    return filters.length > 0 ? filters.join(', ') : 'Sin filtros';
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-purple-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">

        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={() => navigate('/')}
              className="inline-flex items-center gap-2 px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors font-medium"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
              </svg>
              Volver al Inicio
            </button>
          </div>

          <div className="text-center">
            <div className="inline-flex items-center justify-center w-16 h-16 bg-purple-100 rounded-full mb-4">
              <svg className="w-8 h-8 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
              </svg>
            </div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Inventario de Vehículos</h1>
            <p className="text-gray-600">Explore y filtre el inventario completo de vehículos por marca, color o modelo</p>
          </div>
        </div>

        {/* Success/Error Messages */}
        {message.text && (
          <div className={`rounded-xl p-4 mb-6 border ${
            message.type === 'success'
              ? 'bg-emerald-50 border-emerald-200'
              : 'bg-red-50 border-red-200'
          }`}>
            <div className="flex items-center gap-3">
              {message.type === 'success' ? (
                <svg className="w-5 h-5 text-emerald-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              ) : (
                <svg className="w-5 h-5 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              )}
              <span className={`font-medium ${message.type === 'success' ? 'text-emerald-800' : 'text-red-800'}`}>
                {message.text}
              </span>
            </div>
          </div>
        )}

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

        {/* Filters Panel */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
            <div className="p-2 bg-purple-100 rounded-lg">
              <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
              </svg>
            </div>
            Filtros de Búsqueda
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
            {/* Filtro Marca */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Marca
              </label>
              <input
                type="text"
                value={marcaFilter}
                onChange={(e) => setMarcaFilter(e.target.value)}
                onKeyPress={handleKeyPress}
                placeholder="Ej: Toyota"
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all"
              />
            </div>

            {/* Filtro Color */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Color
              </label>
              <input
                type="text"
                value={colorFilter}
                onChange={(e) => setColorFilter(e.target.value)}
                onKeyPress={handleKeyPress}
                placeholder="Ej: Blanco"
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all"
              />
            </div>

            {/* Filtro Modelo */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Modelo
              </label>
              <input
                type="text"
                value={modeloFilter}
                onChange={(e) => setModeloFilter(e.target.value)}
                onKeyPress={handleKeyPress}
                placeholder="Ej: Corolla"
                className="w-full px-4 py-2.5 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent transition-all"
              />
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex flex-wrap gap-3">
            <button
              onClick={applyFilters}
              className="inline-flex items-center gap-2 px-6 py-2.5 bg-gradient-to-r from-purple-600 to-purple-700 text-white rounded-lg hover:from-purple-700 hover:to-purple-800 transition-all font-medium shadow-sm"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              Aplicar Filtros
            </button>

            <button
              onClick={clearFilters}
              className="inline-flex items-center gap-2 px-6 py-2.5 bg-white border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 hover:border-gray-400 transition-all font-medium"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              Limpiar Filtros
            </button>

            <button
              onClick={loadCarros}
              disabled={isLoading}
              className="inline-flex items-center gap-2 px-6 py-2.5 bg-gradient-to-r from-green-600 to-green-700 text-white rounded-lg hover:from-green-700 hover:to-green-800 transition-all font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? (
                <>
                  <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                  Cargando...
                </>
              ) : (
                <>
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  Recargar Datos
                </>
              )}
            </button>
          </div>
        </div>

        {/* Results Info */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-4 mb-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
              <span className="text-gray-900 font-medium">
                {filteredCarros.length} vehículo(s) encontrado(s)
              </span>
            </div>
            <span className="text-sm text-gray-600">
              Filtros: {getActiveFiltersText()}
            </span>
          </div>
        </div>

        {/* Vehicles Table */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gradient-to-r from-purple-50 to-purple-100 border-b border-purple-200">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Placa</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Marca</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Modelo</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Año</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Color</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Estado</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Precio</th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-purple-900 uppercase tracking-wider">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {isLoading ? (
                  <tr>
                    <td colSpan={8} className="px-6 py-12 text-center">
                      <div className="flex flex-col items-center gap-3">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
                        <span className="text-gray-600">Cargando vehículos...</span>
                      </div>
                    </td>
                  </tr>
                ) : filteredCarros.length === 0 ? (
                  <tr>
                    <td colSpan={8} className="px-6 py-12 text-center">
                      <div className="flex flex-col items-center gap-3">
                        <svg className="w-16 h-16 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                        <div>
                          <p className="text-gray-900 font-medium">No se encontraron vehículos</p>
                          <p className="text-gray-500 text-sm mt-1">
                            {marcaFilter || colorFilter || modeloFilter
                              ? 'Intente modificar los filtros de búsqueda'
                              : 'No hay vehículos registrados en el sistema'}
                          </p>
                        </div>
                      </div>
                    </td>
                  </tr>
                ) : (
                  filteredCarros.map((carro) => (
                    <tr key={carro.placa} className="hover:bg-purple-50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-semibold bg-purple-100 text-purple-800">
                          {carro.placa}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{carro.marca}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{carro.modelo}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{carro.anio}</td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">{carro.color}</td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          carro.estado === 'NUEVO'
                            ? 'bg-green-100 text-green-800'
                            : carro.estado === 'EXCELENTE'
                            ? 'bg-blue-100 text-blue-800'
                            : 'bg-amber-100 text-amber-800'
                        }`}>
                          {carro.estado}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-green-600">
                        {formatCurrency(carro.precio)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm">
                        <div className="flex items-center gap-2">
                          <button
                            onClick={() => navigate('/actualizar-formulario', { state: { carro } })}
                            className="inline-flex items-center gap-1 px-3 py-1.5 bg-amber-50 text-amber-700 hover:bg-amber-100 rounded-lg transition-colors text-xs font-medium border border-amber-200"
                            title="Actualizar"
                          >
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                            </svg>
                            Editar
                          </button>
                          <button
                            onClick={() => navigate('/eliminar-formulario', { state: { carro } })}
                            className="inline-flex items-center gap-1 px-3 py-1.5 bg-red-50 text-red-700 hover:bg-red-100 rounded-lg transition-colors text-xs font-medium border border-red-200"
                            title="Eliminar"
                          >
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                            </svg>
                            Eliminar
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

      </div>
    </div>
  );
}
