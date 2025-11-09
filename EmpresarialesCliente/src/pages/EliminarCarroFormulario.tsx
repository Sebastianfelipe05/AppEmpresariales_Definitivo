import { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import type { Carro } from '../types/Carro';
import { deleteCarro } from '../services/carroApi';

export default function EliminarCarroFormulario() {
  const location = useLocation();
  const navigate = useNavigate();
  const carroEncontrado = location.state?.carro as Carro;

  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' } | null>(null);
  const [showConfirmModal, setShowConfirmModal] = useState(false);

  useEffect(() => {
    if (!carroEncontrado) {
      navigate('/search-list');
      return;
    }
  }, [carroEncontrado, navigate]);

  const handleDelete = async () => {
    setIsLoading(true);
    setMessage(null);

    try {
      await deleteCarro(carroEncontrado.placa);
      setMessage({ text: '¡Carro eliminado exitosamente!', type: 'success' });
      setShowConfirmModal(false);
      setTimeout(() => {
        navigate('/search-list', {
          state: {
            message: `Vehículo ${carroEncontrado.placa} eliminado correctamente`,
            type: 'success'
          }
        });
      }, 1500);
    } catch (error) {
      setMessage({
        text: `Error al eliminar el carro: ${error instanceof Error ? error.message : 'Error desconocido'}`,
        type: 'error'
      });
      setShowConfirmModal(false);
    } finally {
      setIsLoading(false);
    }
  };

  if (!carroEncontrado) {
    return null;
  }

  const formatCurrency = (value: number): string => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0
    }).format(value);
  };

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-red-50 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-5xl mx-auto">

        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={() => navigate('/search-list')}
              className="inline-flex items-center gap-2 px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors font-medium"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
              </svg>
              Volver
            </button>
            <div className="flex gap-2">
              <button
                onClick={() => navigate('/actualizar-formulario', { state: { carro: carroEncontrado } })}
                className="inline-flex items-center gap-2 px-4 py-2 bg-amber-50 text-amber-700 hover:bg-amber-100 rounded-lg transition-colors font-medium border border-amber-200"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                </svg>
                Actualizar
              </button>
            </div>
          </div>

          <div className="text-center">
            <div className="inline-flex items-center justify-center w-16 h-16 bg-red-100 rounded-full mb-4">
              <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </div>
            <h1 className="text-3xl font-bold text-gray-900 mb-2">Eliminar Vehículo</h1>
            <p className="text-gray-600">Revise los datos antes de eliminar el vehículo <span className="font-semibold text-red-600">{carroEncontrado.placa}</span></p>
          </div>

          {/* Breadcrumb */}
          <div className="flex items-center justify-center gap-3 mt-6 text-sm">
            <div className="flex items-center gap-2 text-green-600">
              <div className="flex items-center justify-center w-6 h-6 bg-green-100 rounded-full">
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                </svg>
              </div>
              <span className="font-medium">Buscar</span>
            </div>
            <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
            <div className="flex items-center gap-2 text-red-600">
              <div className="flex items-center justify-center w-6 h-6 bg-red-100 rounded-full">
                <span className="text-xs font-bold">2</span>
              </div>
              <span className="font-medium">Eliminar</span>
            </div>
          </div>
        </div>

        {/* Success/Error Messages */}
        {message && (
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

        {/* Vehicle Information */}
        <div className="space-y-6">

          {/* Información Básica */}
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <div className="p-2 bg-blue-100 rounded-lg">
                <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
              </div>
              Información Básica
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Placa</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.placa}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Marca</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.marca}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Modelo</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.modelo}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Año</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.anio}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Color</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.color}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Estado</span>
                <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium w-fit ${
                  carroEncontrado.estado === 'NUEVO'
                    ? 'bg-green-100 text-green-800'
                    : carroEncontrado.estado === 'EXCELENTE'
                    ? 'bg-blue-100 text-blue-800'
                    : 'bg-amber-100 text-amber-800'
                }`}>
                  {carroEncontrado.estado}
                </span>
              </div>
            </div>
          </div>

          {/* Especificaciones Técnicas */}
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <div className="p-2 bg-purple-100 rounded-lg">
                <svg className="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
              </div>
              Especificaciones Técnicas
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Combustible</span>
                <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium w-fit ${
                  carroEncontrado.combustible === 'ELECTRICO'
                    ? 'bg-green-100 text-green-800'
                    : carroEncontrado.combustible === 'HIBRIDO'
                    ? 'bg-blue-100 text-blue-800'
                    : 'bg-gray-100 text-gray-800'
                }`}>
                  {carroEncontrado.combustible}
                </span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Transmisión</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.tipoTransmision}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Puertas</span>
                <span className="text-lg font-semibold text-gray-900">{carroEncontrado.numeroPuertas}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Aire Acondicionado</span>
                <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium w-fit ${
                  carroEncontrado.tieneAireAcondicionado
                    ? 'bg-green-100 text-green-800'
                    : 'bg-gray-100 text-gray-800'
                }`}>
                  {carroEncontrado.tieneAireAcondicionado ? 'Sí' : 'No'}
                </span>
              </div>
            </div>
          </div>

          {/* Información Comercial */}
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <h2 className="text-xl font-bold text-gray-900 mb-4 flex items-center gap-2">
              <div className="p-2 bg-emerald-100 rounded-lg">
                <svg className="w-5 h-5 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              Información Comercial
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Precio</span>
                <span className="text-2xl font-bold text-emerald-600">{formatCurrency(carroEncontrado.precio)}</span>
              </div>
              <div className="flex flex-col gap-1">
                <span className="text-sm text-gray-500">Fecha de Registro</span>
                <span className="text-lg font-semibold text-gray-900">{formatDate(carroEncontrado.fechaRegistro)}</span>
              </div>
            </div>
          </div>

          {/* Warning Section */}
          <div className="bg-gradient-to-r from-red-50 to-orange-50 border-2 border-red-200 rounded-2xl p-6">
            <div className="flex items-start gap-4">
              <div className="flex-shrink-0">
                <div className="flex items-center justify-center w-12 h-12 bg-red-100 rounded-full">
                  <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                  </svg>
                </div>
              </div>
              <div className="flex-1">
                <h3 className="text-lg font-bold text-red-900 mb-2">⚠️ Advertencia de Eliminación</h3>
                <p className="text-red-800 mb-2">
                  Está a punto de <strong>eliminar permanentemente</strong> este vehículo del sistema.
                </p>
                <ul className="list-disc list-inside text-red-700 text-sm space-y-1">
                  <li>Esta acción <strong>no se puede deshacer</strong></li>
                  <li>Se eliminarán todos los registros asociados</li>
                  <li>Los datos no podrán ser recuperados</li>
                </ul>
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
            <div className="flex flex-col sm:flex-row gap-4 justify-end">
              <button
                type="button"
                onClick={() => navigate('/search-list')}
                className="inline-flex items-center justify-center gap-2 px-6 py-3 bg-white border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 hover:border-gray-400 transition-all font-medium"
                disabled={isLoading}
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                </svg>
                Cancelar
              </button>
              <button
                type="button"
                onClick={() => setShowConfirmModal(true)}
                className="inline-flex items-center justify-center gap-2 px-6 py-3 bg-gradient-to-r from-red-600 to-red-700 text-white rounded-lg hover:from-red-700 hover:to-red-800 transition-all font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                disabled={isLoading}
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
                Eliminar Vehículo
              </button>
            </div>
          </div>

        </div>

        {/* Confirmation Modal */}
        {showConfirmModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-2xl shadow-2xl max-w-md w-full p-6 transform transition-all">
              <div className="text-center mb-6">
                <div className="inline-flex items-center justify-center w-16 h-16 bg-red-100 rounded-full mb-4">
                  <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                  </svg>
                </div>
                <h3 className="text-2xl font-bold text-gray-900 mb-2">Confirmar Eliminación</h3>
                <p className="text-gray-600 mb-4">
                  ¿Está seguro que desea eliminar el vehículo?
                </p>
                <div className="bg-gray-50 rounded-lg p-4 mb-4">
                  <p className="text-sm text-gray-600 mb-2">Placa del vehículo:</p>
                  <p className="text-2xl font-bold text-red-600">{carroEncontrado.placa}</p>
                </div>
                <p className="text-sm text-red-600 font-medium">
                  ⚠️ Esta acción no se puede deshacer
                </p>
              </div>

              <div className="flex flex-col sm:flex-row gap-3">
                <button
                  onClick={() => setShowConfirmModal(false)}
                  className="flex-1 inline-flex items-center justify-center gap-2 px-6 py-3 bg-white border-2 border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 hover:border-gray-400 transition-all font-medium"
                  disabled={isLoading}
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                  Cancelar
                </button>
                <button
                  onClick={handleDelete}
                  className="flex-1 inline-flex items-center justify-center gap-2 px-6 py-3 bg-gradient-to-r from-red-600 to-red-700 text-white rounded-lg hover:from-red-700 hover:to-red-800 transition-all font-medium shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                  disabled={isLoading}
                >
                  {isLoading ? (
                    <>
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                      Eliminando...
                    </>
                  ) : (
                    <>
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                      </svg>
                      Sí, Eliminar
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        )}

      </div>
    </div>
  );
}
