import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import type { Mantenimiento, MantenimientoUpdateData } from '../types/Mantenimiento';
import { getMantenimientoById, updateMantenimiento } from '../services/mantenimientoApi';
import { TIPOS_MANTENIMIENTO } from '../types/Mantenimiento';

export default function ActualizarMantenimiento() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const id = searchParams.get('id');

  const [mantenimiento, setMantenimiento] = useState<Mantenimiento | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [placa, setPlaca] = useState('');

  const [formData, setFormData] = useState<Omit<MantenimientoUpdateData, 'id' | 'carro'>>({
    fechaMantenimiento: '',
    kilometraje: 0,
    tipoMantenimiento: 'PREVENTIVO',
    costo: 0,
    descripcion: '',
    proximoMantenimiento: null,
    completado: false,
  });

  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!id) {
      setLoadError('ID de mantenimiento no proporcionado');
      setLoading(false);
      return;
    }

    cargarMantenimiento();
  }, [id]);

  const cargarMantenimiento = async () => {
    if (!id) return;

    try {
      setLoading(true);
      const data = await getMantenimientoById(id);
      if (data) {
        setMantenimiento(data);
        setPlaca(data.placaCarro);
        setFormData({
          fechaMantenimiento: data.fechaMantenimiento,
          kilometraje: data.kilometraje,
          tipoMantenimiento: data.tipoMantenimiento,
          costo: data.costo,
          descripcion: data.descripcion,
          proximoMantenimiento: data.proximoMantenimiento,
          completado: data.completado,
        });
      } else {
        setLoadError('Mantenimiento no encontrado');
      }
    } catch (err) {
      setLoadError(err instanceof Error ? err.message : 'Error al cargar el mantenimiento');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value, type } = e.target;

    if (type === 'checkbox') {
      const checked = (e.target as HTMLInputElement).checked;
      setFormData(prev => ({ ...prev, [name]: checked }));
    } else if (type === 'number') {
      setFormData(prev => ({ ...prev, [name]: parseFloat(value) || 0 }));
    } else if (name === 'proximoMantenimiento' && value === '') {
      setFormData(prev => ({ ...prev, [name]: null }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }
  };

  const formatDateForBackend = (localDatetime: string): string => {
    if (!localDatetime) return '';
    const date = new Date(localDatetime);
    return date.toISOString().slice(0, 19).replace('T', ' ');
  };

  const formatDateForInput = (backendDatetime: string | null): string => {
    if (!backendDatetime) return '';
    return backendDatetime.replace(' ', 'T').slice(0, 16);
  };

  const handleDateChange = (name: 'fechaMantenimiento' | 'proximoMantenimiento') => (e: React.ChangeEvent<HTMLInputElement>) => {
    const localValue = e.target.value;
    const backendValue = localValue ? formatDateForBackend(localValue) : null;
    setFormData(prev => ({ ...prev, [name]: backendValue }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);

    // Validaciones
    if (!placa.match(/^[A-Z]{3}-[0-9]{3}$/)) {
      setError('La placa debe tener el formato ABC-123');
      setIsSubmitting(false);
      return;
    }

    if (formData.kilometraje < 0 || formData.kilometraje > 1000000) {
      setError('El kilometraje debe estar entre 0 y 1,000,000 km');
      setIsSubmitting(false);
      return;
    }

    if (formData.costo < 0) {
      setError('El costo debe ser mayor o igual a 0');
      setIsSubmitting(false);
      return;
    }

    if (formData.descripcion.length < 10 || formData.descripcion.length > 500) {
      setError('La descripción debe tener entre 10 y 500 caracteres');
      setIsSubmitting(false);
      return;
    }

    if (!id) {
      setError('ID de mantenimiento no disponible');
      setIsSubmitting(false);
      return;
    }

    try {
      // Construir el objeto completo con el formato que espera el backend
      const dataToSend: MantenimientoUpdateData = {
        id,
        carro: {
          placa: placa
        },
        ...formData
      };
      await updateMantenimiento(id, dataToSend);
      alert('Mantenimiento actualizado exitosamente');
      navigate('/mantenimientos');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al actualizar el mantenimiento');
      setIsSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-amber-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-16 w-16 border-t-4 border-b-4 border-amber-500 mb-4"></div>
          <p className="text-gray-700 text-lg font-medium">Cargando mantenimiento...</p>
        </div>
      </div>
    );
  }

  if (loadError) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-amber-50 py-8 px-4">
        <div className="max-w-2xl mx-auto">
          <div className="bg-gradient-to-r from-red-50 to-orange-50 border-l-4 border-red-500 p-6 rounded-lg shadow-lg mb-6">
            <div className="flex items-center gap-3">
              <svg className="w-8 h-8 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <p className="text-red-800 font-medium text-lg">{loadError}</p>
            </div>
          </div>
          <button
            onClick={() => navigate('/mantenimientos')}
            className="px-6 py-3 bg-white text-gray-700 rounded-lg hover:bg-gray-100 font-medium shadow-md transition-all"
          >
            ← Volver a la lista
          </button>
        </div>
      </div>
    );
  }

  if (!mantenimiento) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-amber-50 py-8 px-4">
        <div className="max-w-2xl mx-auto text-center">
          <div className="bg-white rounded-lg shadow-lg p-8 mb-6">
            <svg className="w-20 h-20 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p className="text-gray-700 text-lg font-medium">Mantenimiento no encontrado</p>
          </div>
          <button
            onClick={() => navigate('/mantenimientos')}
            className="px-6 py-3 bg-white text-gray-700 rounded-lg hover:bg-gray-100 font-medium shadow-md transition-all"
          >
            ← Volver a la lista
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-amber-50 py-8 px-4">
      <div className="max-w-4xl mx-auto">
        {/* Breadcrumb */}
        <nav className="mb-6 flex items-center gap-2 text-sm text-gray-600">
          <button
            onClick={() => navigate('/mantenimientos')}
            className="hover:text-amber-600 transition-colors"
          >
            Mantenimientos
          </button>
          <span>/</span>
          <span className="text-gray-900 font-medium">Actualizar</span>
        </nav>

        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2 flex items-center gap-3">
            <div className="w-12 h-12 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl flex items-center justify-center">
              <svg className="w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
            </div>
            Actualizar Mantenimiento
          </h1>
          <p className="text-gray-600 ml-15">
            Modifique la información del mantenimiento registrado
          </p>
        </div>

        {/* Form Card */}
        <form onSubmit={handleSubmit} className="bg-white rounded-2xl shadow-xl overflow-hidden">
          {error && (
            <div className="bg-gradient-to-r from-red-50 to-orange-50 border-l-4 border-red-500 p-4 m-6 rounded-lg">
              <div className="flex items-center gap-3">
                <svg className="w-6 h-6 text-red-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p className="text-red-800 font-medium">{error}</p>
              </div>
            </div>
          )}

          <div className="p-8 space-y-8">
            {/* Información del Vehículo */}
            <div>
              <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <svg className="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                Información del Vehículo
              </h2>
              <div className="grid grid-cols-1 gap-6">
                <div>
                  <label htmlFor="placaCarro" className="block text-sm font-medium text-gray-700 mb-2">
                    Placa del Vehículo *
                  </label>
                  <input
                    type="text"
                    id="placaCarro"
                    name="placaCarro"
                    value={placa}
                    onChange={(e) => setPlaca(e.target.value.toUpperCase())}
                    disabled
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg bg-gray-100 text-gray-600 cursor-not-allowed uppercase"
                  />
                  <p className="mt-1 text-sm text-gray-500">La placa no se puede modificar</p>
                </div>
              </div>
            </div>

            {/* Detalles del Mantenimiento */}
            <div>
              <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <svg className="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                Detalles del Mantenimiento
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label htmlFor="tipoMantenimiento" className="block text-sm font-medium text-gray-700 mb-2">
                    Tipo de Mantenimiento *
                  </label>
                  <select
                    id="tipoMantenimiento"
                    name="tipoMantenimiento"
                    value={formData.tipoMantenimiento}
                    onChange={handleChange}
                    required
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-all"
                  >
                    {TIPOS_MANTENIMIENTO.map((tipo) => (
                      <option key={tipo} value={tipo}>
                        {tipo.replace('_', ' ')}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label htmlFor="fechaMantenimiento" className="block text-sm font-medium text-gray-700 mb-2">
                    Fecha del Mantenimiento *
                  </label>
                  <input
                    type="datetime-local"
                    id="fechaMantenimiento"
                    name="fechaMantenimiento"
                    value={formatDateForInput(formData.fechaMantenimiento)}
                    onChange={handleDateChange('fechaMantenimiento')}
                    required
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-all"
                  />
                </div>

                <div>
                  <label htmlFor="kilometraje" className="block text-sm font-medium text-gray-700 mb-2">
                    Kilometraje *
                  </label>
                  <input
                    type="number"
                    id="kilometraje"
                    name="kilometraje"
                    value={formData.kilometraje}
                    onChange={handleChange}
                    min="0"
                    max="1000000"
                    required
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-all"
                  />
                  <p className="mt-1 text-sm text-gray-500">Máximo: 1,000,000 km</p>
                </div>

                <div>
                  <label htmlFor="costo" className="block text-sm font-medium text-gray-700 mb-2">
                    Costo (COP) *
                  </label>
                  <input
                    type="number"
                    id="costo"
                    name="costo"
                    value={formData.costo}
                    onChange={handleChange}
                    min="0"
                    step="0.01"
                    required
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-all"
                  />
                </div>

                <div className="md:col-span-2">
                  <label htmlFor="descripcion" className="block text-sm font-medium text-gray-700 mb-2">
                    Descripción * (10-500 caracteres)
                  </label>
                  <textarea
                    id="descripcion"
                    name="descripcion"
                    value={formData.descripcion}
                    onChange={handleChange}
                    minLength={10}
                    maxLength={500}
                    rows={4}
                    required
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-all resize-none"
                    placeholder="Describa el mantenimiento realizado..."
                  />
                  <div className="mt-1 text-sm text-gray-500 text-right">
                    {formData.descripcion.length} / 500 caracteres
                  </div>
                </div>
              </div>
            </div>

            {/* Información Adicional */}
            <div>
              <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                <svg className="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                Información Adicional
              </h2>
              <div className="grid grid-cols-1 gap-6">
                <div>
                  <label htmlFor="proximoMantenimiento" className="block text-sm font-medium text-gray-700 mb-2">
                    Próximo Mantenimiento (Opcional)
                  </label>
                  <input
                    type="datetime-local"
                    id="proximoMantenimiento"
                    name="proximoMantenimiento"
                    value={formatDateForInput(formData.proximoMantenimiento)}
                    onChange={handleDateChange('proximoMantenimiento')}
                    className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent transition-all"
                  />
                </div>

                <div className="flex items-center gap-3 p-4 bg-gray-50 rounded-lg">
                  <input
                    type="checkbox"
                    id="completado"
                    name="completado"
                    checked={formData.completado}
                    onChange={handleChange}
                    className="h-5 w-5 text-amber-600 focus:ring-amber-500 border-gray-300 rounded cursor-pointer"
                  />
                  <label htmlFor="completado" className="text-sm font-medium text-gray-900 cursor-pointer">
                    Marcar como mantenimiento completado
                  </label>
                </div>
              </div>
            </div>
          </div>

          {/* Actions */}
          <div className="bg-gray-50 px-8 py-6 flex flex-col sm:flex-row gap-3 justify-end border-t border-gray-200">
            <button
              type="button"
              onClick={() => navigate('/mantenimientos')}
              className="px-6 py-3 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-100 font-medium transition-all"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-8 py-3 bg-gradient-to-r from-amber-500 to-orange-600 text-white rounded-lg hover:from-amber-600 hover:to-orange-700 font-medium shadow-lg shadow-amber-500/30 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
            >
              {isSubmitting ? (
                <span className="flex items-center gap-2">
                  <svg className="animate-spin h-5 w-5" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Actualizando...
                </span>
              ) : (
                'Actualizar Mantenimiento'
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
