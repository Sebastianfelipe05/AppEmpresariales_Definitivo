import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import type { ConductorCreateData } from '../types/Conductor';
import { createConductor } from '../services/conductorApi';

export default function CrearConductor() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [successMessage, setSuccessMessage] = useState<string>('');
  const navigate = useNavigate();

  const [formData, setFormData] = useState<ConductorCreateData>({
    cedula: '',
    nombre: '',
    apellido: '',
    telefono: '',
    licenciaNumero: '',
    fechaNacimiento: '',
    salario: 0,
    activo: true
  });

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
      // Convert date to ISO format with time
      const conductorData = {
        ...formData,
        fechaNacimiento: formData.fechaNacimiento + 'T00:00:00'
      };

      const result = await createConductor(conductorData);
      setSuccessMessage(`Conductor creado exitosamente: ${result.nombre} ${result.apellido}`);

      setTimeout(() => {
        navigate('/conductores/listar');
      }, 2000);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Error desconocido al crear el conductor';
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-blue-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center gap-4">
            <div className="p-3 bg-gradient-to-br from-green-600 to-green-700 rounded-xl">
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
            </div>
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Registrar Nuevo Conductor</h1>
              <p className="text-gray-600 mt-1">Complete todos los campos para agregar un conductor</p>
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
              {/* Cédula */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Cédula <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  name="cedula"
                  value={formData.cedula}
                  onChange={handleChange}
                  required
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Ej: 1098765432"
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
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Ej: Carlos"
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
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Ej: Rodríguez"
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
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Ej: 3201234567"
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
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Ej: LIC-2024-001"
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
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
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
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Ej: 2500000"
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
                    className="w-5 h-5 text-green-600 border-gray-300 rounded focus:ring-green-500"
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
                className="flex-1 bg-gradient-to-r from-green-600 to-green-700 text-white py-3 px-6 rounded-lg font-semibold hover:from-green-700 hover:to-green-800 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {isLoading ? 'Creando...' : 'Crear Conductor'}
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
