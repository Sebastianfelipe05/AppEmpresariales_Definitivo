import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import type { ConductorUpdateData, Conductor } from '../types/Conductor';
import { getConductorByCedula, updateConductor } from '../services/conductorApi';

export default function ActualizarConductor() {
  const { cedula } = useParams<{ cedula: string }>();
  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(true);
  const [error, setError] = useState<string>('');
  const [successMessage, setSuccessMessage] = useState<string>('');
  const navigate = useNavigate();

  const [conductorOriginal, setConductorOriginal] = useState<Conductor | null>(null);
  const [formData, setFormData] = useState<ConductorUpdateData>({
    cedula: '',
    nombre: '',
    apellido: '',
    telefono: '',
    licenciaNumero: '',
    fechaNacimiento: '',
    salario: 0,
    activo: true
  });

  useEffect(() => {
    if (cedula) {
      loadConductor(cedula);
    }
  }, [cedula]);

  const loadConductor = async (cedulaBuscar: string) => {
    setIsFetching(true);
    setError('');

    try {
      const data = await getConductorByCedula(cedulaBuscar);
      setConductorOriginal(data);

      // Extract date from ISO string (YYYY-MM-DDTHH:mm:ss -> YYYY-MM-DD)
      const fechaNacimiento = data.fechaNacimiento.split('T')[0];

      setFormData({
        cedula: data.cedula,
        nombre: data.nombre,
        apellido: data.apellido,
        telefono: data.telefono,
        licenciaNumero: data.licenciaNumero,
        fechaNacimiento: fechaNacimiento,
        salario: data.salario,
        activo: data.activo
      });
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Conductor no encontrado';
      setError(errorMessage);
    } finally {
      setIsFetching(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;

    if (type === 'checkbox') {
      setFormData(prev => ({
        ...prev,
        [name]: (e.target as HTMLInputElement).checked
      }));
    } else if (name === 'salario') {
      setFormData(prev => ({
        ...prev,
        [name]: parseFloat(value) || 0
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: value
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');
    setSuccessMessage('');

    try {
      if (!cedula) {
        throw new Error('Cédula no especificada');
      }

      // Convert date to ISO format with time
      const conductorData = {
        ...formData,
        fechaNacimiento: formData.fechaNacimiento + 'T00:00:00'
      };

      const result = await updateConductor(cedula, conductorData);
      setSuccessMessage(`Conductor actualizado exitosamente: ${result.nombre} ${result.apellido}`);

      setTimeout(() => {
        navigate('/conductores/listar');
      }, 2000);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al actualizar el conductor';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  if (isFetching) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block w-12 h-12 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
          <p className="text-gray-600 mt-4 font-medium">Cargando conductor...</p>
        </div>
      </div>
    );
  }

  if (error && !conductorOriginal) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-8">
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
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center gap-4">
            <div className="p-3 bg-gradient-to-br from-blue-600 to-blue-700 rounded-xl">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
            </div>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Actualizar Conductor</h1>
              <p className="text-gray-600 mt-1">Modifique los datos del conductor</p>
            </div>
          </div>
        </div>

        {/* Messages */}
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

        {successMessage && (
          <div className="rounded-xl p-4 mb-6 bg-green-50 border border-green-200">
            <div className="flex items-start gap-3">
              <svg className="w-5 h-5 text-green-600 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
              <p className="text-green-800 font-medium">{successMessage}</p>
            </div>
          </div>
        )}

        {/* Form */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">
          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {/* Cédula (read-only) */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Cédula <span className="text-gray-400">(No modificable)</span>
                </label>
                <input
                  type="text"
                  value={formData.cedula}
                  readOnly
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg bg-gray-100 cursor-not-allowed"
                />
              </div>

              {/* Nombre */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Nombre <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="nombre"
                  value={formData.nombre}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Apellido */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Apellido <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="apellido"
                  value={formData.apellido}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Teléfono */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Teléfono <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="telefono"
                  value={formData.telefono}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Licencia */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Número de Licencia <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="licenciaNumero"
                  value={formData.licenciaNumero}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Fecha Nacimiento */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Fecha de Nacimiento <span className="text-red-500">*</span>
                </label>
                <input
                  type="date"
                  name="fechaNacimiento"
                  value={formData.fechaNacimiento}
                  onChange={handleChange}
                  required
                  max={new Date().toISOString().split('T')[0]}
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Salario */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Salario <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  name="salario"
                  value={formData.salario}
                  onChange={handleChange}
                  required
                  min="0"
                  step="0.01"
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              {/* Activo */}
              <div className="flex items-center">
                <label className="flex items-center gap-3 cursor-pointer">
                  <input
                    type="checkbox"
                    name="activo"
                    checked={formData.activo}
                    onChange={handleChange}
                    className="w-5 h-5 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                  />
                  <span className="text-sm font-semibold text-gray-700">Conductor Activo</span>
                </label>
              </div>
            </div>

            {/* Buttons */}
            <div className="flex gap-4 pt-6 border-t border-gray-200">
              <button
                type="submit"
                disabled={isLoading}
                className="flex-1 bg-gradient-to-r from-blue-600 to-blue-700 text-white py-3 px-6 rounded-lg font-semibold hover:from-blue-700 hover:to-blue-800 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isLoading ? 'Actualizando...' : 'Actualizar Conductor'}
              </button>
              <button
                type="button"
                onClick={() => navigate('/conductores/listar')}
                className="flex-1 bg-gray-200 text-gray-700 py-3 px-6 rounded-lg font-semibold hover:bg-gray-300 transition-all"
              >
                Cancelar
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
