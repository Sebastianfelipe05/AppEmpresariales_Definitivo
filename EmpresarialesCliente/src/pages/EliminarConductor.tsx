import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import type { Conductor } from '../types/Conductor';
import { getConductorByCedula, deleteConductor } from '../services/conductorApi';

export default function EliminarConductor() {
  const { cedula } = useParams<{ cedula: string }>();
  const [conductor, setConductor] = useState<Conductor | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(true);
  const [error, setError] = useState<string>('');
  const [confirmText, setConfirmText] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    if (cedula && cedula.trim() !== '') {
      loadConductor(cedula);
    } else if (!cedula) {
      setIsFetching(false);
      setError('No se especificó una cédula válida en la URL');
    }
  }, [cedula]);

  const loadConductor = async (cedulaBuscar: string) => {
    setIsFetching(true);
    setError('');

    try {
      const data = await getConductorByCedula(cedulaBuscar);
      setConductor(data);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Conductor no encontrado';
      setError(errorMessage);
    } finally {
      setIsFetching(false);
    }
  };

  const handleDelete = async () => {
    if (!cedula || !conductor) return;

    if (confirmText !== 'ELIMINAR') {
      setError('Por favor escriba "ELIMINAR" para confirmar');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      await deleteConductor(cedula);
      navigate('/conductores/listar', {
        state: {
          message: `Conductor ${conductor.nombre} ${conductor.apellido} eliminado exitosamente`,
          type: 'success'
        }
      });
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error al eliminar el conductor';
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

  if (isFetching) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-red-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block w-12 h-12 border-4 border-red-600 border-t-transparent rounded-full animate-spin"></div>
          <p className="text-gray-600 mt-4 font-medium">Cargando conductor...</p>
        </div>
      </div>
    );
  }

  if (error && !conductor) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-red-50 py-8">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="rounded-xl p-8 bg-red-50 border border-red-200">
            <div className="flex items-start gap-3">
              <svg className="w-6 h-6 text-red-600 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
              <div>
                <p className="text-red-800 font-bold text-lg">{error}</p>
                <button
                  onClick={() => navigate('/conductores/listar')}
                  className="mt-4 bg-red-600 text-white px-6 py-2 rounded-lg hover:bg-red-700 transition-all"
                >
                  Volver al Listado
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-red-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-red-300 p-6 mb-6">
          <div className="flex items-center gap-4">
            <div className="p-3 bg-gradient-to-br from-red-600 to-red-700 rounded-xl">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </div>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Eliminar Conductor</h1>
              <p className="text-gray-600 mt-1">Esta acción no se puede deshacer</p>
            </div>
          </div>
        </div>

        {/* Warning */}
        <div className="rounded-xl p-6 mb-6 bg-yellow-50 border-2 border-yellow-300">
          <div className="flex items-start gap-3">
            <svg className="w-6 h-6 text-yellow-600 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
            <div>
              <p className="text-yellow-900 font-bold text-lg">¡Advertencia!</p>
              <p className="text-yellow-800 mt-1">
                Está a punto de eliminar permanentemente este conductor. Esta acción no se puede revertir.
              </p>
            </div>
          </div>
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
          <div className="bg-white rounded-2xl shadow-sm border border-red-300 p-8 mb-6">
            <div className="flex items-center justify-between mb-6">
              <h2 className="text-2xl font-bold text-gray-900">Datos del Conductor a Eliminar</h2>
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
            </div>
          </div>
        )}

        {/* Confirmation */}
        <div className="bg-white rounded-2xl shadow-sm border border-red-300 p-8">
          <h3 className="text-xl font-bold text-gray-900 mb-4">Confirmar Eliminación</h3>
          <p className="text-gray-600 mb-4">
            Para confirmar la eliminación, escriba <span className="font-bold text-red-600">ELIMINAR</span> en el campo de abajo:
          </p>

          <input
            type="text"
            value={confirmText}
            onChange={(e) => setConfirmText(e.target.value.toUpperCase())}
            placeholder="Escriba ELIMINAR"
            className="w-full px-4 py-3 border-2 border-red-300 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-transparent mb-6"
          />

          <div className="flex gap-4">
            <button
              onClick={handleDelete}
              disabled={isLoading || confirmText !== 'ELIMINAR'}
              className="flex-1 bg-gradient-to-r from-red-600 to-red-700 text-white py-3 px-6 rounded-lg font-semibold hover:from-red-700 hover:to-red-800 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isLoading ? 'Eliminando...' : 'Confirmar Eliminación'}
            </button>
            <button
              type="button"
              onClick={() => navigate('/conductores/listar')}
              disabled={isLoading}
              className="flex-1 bg-gray-200 text-gray-700 py-3 px-6 rounded-lg font-semibold hover:bg-gray-300 transition-all disabled:opacity-50"
            >
              Cancelar
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
