import { useState, useEffect } from 'react';
import type { Mantenimiento, MantenimientoCreateData } from '../types/Mantenimiento';
import { TIPOS_MANTENIMIENTO } from '../types/Mantenimiento';

interface MantenimientoFormProps {
  onSubmit: (data: MantenimientoCreateData) => Promise<void>;
  initialData?: Mantenimiento;
  isEditing?: boolean;
}

export default function MantenimientoForm({ onSubmit, initialData, isEditing = false }: MantenimientoFormProps) {
  const [placa, setPlaca] = useState(initialData?.placaCarro || '');
  const [formData, setFormData] = useState<Omit<MantenimientoCreateData, 'carro'>>({
    fechaMantenimiento: initialData?.fechaMantenimiento || '',
    kilometraje: initialData?.kilometraje || 0,
    tipoMantenimiento: initialData?.tipoMantenimiento || 'PREVENTIVO',
    costo: initialData?.costo || 0,
    descripcion: initialData?.descripcion || '',
    proximoMantenimiento: initialData?.proximoMantenimiento || null,
    completado: initialData?.completado || false,
  });

  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (initialData) {
      setPlaca(initialData.placaCarro);
      setFormData({
        fechaMantenimiento: initialData.fechaMantenimiento,
        kilometraje: initialData.kilometraje,
        tipoMantenimiento: initialData.tipoMantenimiento,
        costo: initialData.costo,
        descripcion: initialData.descripcion,
        proximoMantenimiento: initialData.proximoMantenimiento,
        completado: initialData.completado,
      });
    }
  }, [initialData]);

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

    try {
      // Construir el objeto completo con el formato que espera el backend
      const dataToSend: MantenimientoCreateData = {
        carro: {
          placa: placa
        },
        ...formData
      };
      await onSubmit(dataToSend);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al enviar el formulario');
    } finally {
      setIsSubmitting(false);
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

  // Función para convertir fecha local a formato del backend
  const formatDateForBackend = (localDatetime: string): string => {
    if (!localDatetime) return '';
    const date = new Date(localDatetime);
    return date.toISOString().slice(0, 19).replace('T', ' ');
  };

  // Función para convertir fecha del backend a formato input datetime-local
  const formatDateForInput = (backendDatetime: string | null): string => {
    if (!backendDatetime) return '';
    // El backend devuelve "YYYY-MM-DD HH:mm:ss"
    // Input datetime-local necesita "YYYY-MM-DDTHH:mm"
    return backendDatetime.replace(' ', 'T').slice(0, 16);
  };

  const handleDateChange = (name: 'fechaMantenimiento' | 'proximoMantenimiento') => (e: React.ChangeEvent<HTMLInputElement>) => {
    const localValue = e.target.value;
    const backendValue = localValue ? formatDateForBackend(localValue) : null;
    setFormData(prev => ({ ...prev, [name]: backendValue }));
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-w-2xl mx-auto p-6 bg-white rounded-lg shadow">
      <h2 className="text-2xl font-bold mb-4">
        {isEditing ? 'Editar Mantenimiento' : 'Crear Nuevo Mantenimiento'}
      </h2>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
          {error}
        </div>
      )}

      <div>
        <label htmlFor="placaCarro" className="block text-sm font-medium text-gray-700 mb-1">
          Placa del Carro *
        </label>
        <input
          type="text"
          id="placaCarro"
          name="placaCarro"
          value={placa}
          onChange={(e) => setPlaca(e.target.value.toUpperCase())}
          placeholder="ABC-123"
          pattern="[A-Z]{3}-[0-9]{3}"
          required
          disabled={isEditing}
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 uppercase disabled:bg-gray-100"
        />
      </div>

      <div>
        <label htmlFor="fechaMantenimiento" className="block text-sm font-medium text-gray-700 mb-1">
          Fecha de Mantenimiento *
        </label>
        <input
          type="datetime-local"
          id="fechaMantenimiento"
          name="fechaMantenimiento"
          value={formatDateForInput(formData.fechaMantenimiento)}
          onChange={handleDateChange('fechaMantenimiento')}
          required
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div>
        <label htmlFor="kilometraje" className="block text-sm font-medium text-gray-700 mb-1">
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
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div>
        <label htmlFor="tipoMantenimiento" className="block text-sm font-medium text-gray-700 mb-1">
          Tipo de Mantenimiento *
        </label>
        <select
          id="tipoMantenimiento"
          name="tipoMantenimiento"
          value={formData.tipoMantenimiento}
          onChange={handleChange}
          required
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          {TIPOS_MANTENIMIENTO.map((tipo) => (
            <option key={tipo} value={tipo}>
              {tipo.replace('_', ' ')}
            </option>
          ))}
        </select>
      </div>

      <div>
        <label htmlFor="costo" className="block text-sm font-medium text-gray-700 mb-1">
          Costo *
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
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div>
        <label htmlFor="descripcion" className="block text-sm font-medium text-gray-700 mb-1">
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
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
        <div className="text-sm text-gray-500 mt-1">
          {formData.descripcion.length} / 500 caracteres
        </div>
      </div>

      <div>
        <label htmlFor="proximoMantenimiento" className="block text-sm font-medium text-gray-700 mb-1">
          Próximo Mantenimiento (Opcional)
        </label>
        <input
          type="datetime-local"
          id="proximoMantenimiento"
          name="proximoMantenimiento"
          value={formatDateForInput(formData.proximoMantenimiento)}
          onChange={handleDateChange('proximoMantenimiento')}
          className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <div className="flex items-center">
        <input
          type="checkbox"
          id="completado"
          name="completado"
          checked={formData.completado}
          onChange={handleChange}
          className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
        />
        <label htmlFor="completado" className="ml-2 block text-sm text-gray-900">
          Mantenimiento completado
        </label>
      </div>

      <div className="flex gap-4 pt-4">
        <button
          type="submit"
          disabled={isSubmitting}
          className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-400"
        >
          {isSubmitting ? 'Guardando...' : (isEditing ? 'Actualizar' : 'Crear')}
        </button>
      </div>
    </form>
  );
}
