import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import type { Carro } from '../types/Carro';
import { updateCarro } from '../services/carroApi';

const ActualizarCarroFormulario: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const carroEncontrado = location.state?.carro as Carro;
  
  const [formData, setFormData] = useState<Carro>({
    placa: '',
    marca: '',
    modelo: '',
    anio: new Date().getFullYear(),
    color: '',
    estado: '',
    combustible: '',
    tipoTransmision: '',
    numeroPuertas: 4,
    tieneAireAcondicionado: false,
    precio: 0,
    fechaRegistro: new Date().toISOString()
  });

  const [isLoading, setIsLoading] = useState(false);
  const [message, setMessage] = useState('');

  useEffect(() => {
    if (!carroEncontrado) {
      navigate('/actualizar');
      return;
    }
    setFormData(carroEncontrado);
  }, [carroEncontrado, navigate]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? (e.target as HTMLInputElement).checked : 
               name === 'numeroPuertas' || name === 'anio' || name === 'precio' ? Number(value) : 
               value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setMessage('');
    
    try {
      await updateCarro(formData.placa, formData);
      setMessage('¡Carro actualizado exitosamente!');
      setTimeout(() => navigate('/'), 2000);
    } catch (error) {
      setMessage(`Error al actualizar el carro: ${error}`);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    navigate('/actualizar');
  };

  if (!carroEncontrado) {
    return null;
  }

  return (
    <div className="page-container">
      {/* Header */}
      <div className="success-header">
        <div className="success-icon">✅</div>
        <h1 className="success-title">Carro Encontrado</h1>
        <div className="action-buttons">
          <button className="btn-actualizar active">✏️ Actualizar</button>
          <button
            className="btn-eliminar"
            onClick={() => navigate('/eliminar-formulario', { state: { carro: carroEncontrado } })}
          >
            🗑️ Eliminar
          </button>
        </div>
      </div>

      {/* Proceso Steps */}
      <div className="proceso-steps">
        <span className="step completed">🔍 1. Buscar Carro</span>
        <span className="step-arrow">→</span>
        <span className="step active">✏️ 2. Actualizar Datos</span>
      </div>

      <form onSubmit={handleSubmit} className="content-container">
        {/* Información Básica */}
        <div className="info-section">
          <h2 className="section-title">
            📋 Información Básica
          </h2>
          <div className="info-grid">
            <div className="info-row">
              <div className="info-item">
                <label>Placa:</label>
                <input
                  type="text"
                  name="placa"
                  value={formData.placa}
                  onChange={handleInputChange}
                  className="info-input readonly"
                  readOnly
                />
              </div>
              <div className="info-item">
                <label>Marca:</label>
                <input
                  type="text"
                  name="marca"
                  value={formData.marca}
                  onChange={handleInputChange}
                  className="info-input"
                  required
                />
              </div>
              <div className="info-item">
                <label>Modelo:</label>
                <input
                  type="text"
                  name="modelo"
                  value={formData.modelo}
                  onChange={handleInputChange}
                  className="info-input"
                  required
                />
              </div>
            </div>
            <div className="info-row">
              <div className="info-item">
                <label>Año:</label>
                <input
                  type="number"
                  name="anio"
                  value={formData.anio}
                  onChange={handleInputChange}
                  className="info-input"
                  min="1900"
                  max={new Date().getFullYear() + 1}
                  required
                />
              </div>
              <div className="info-item">
                <label>Color:</label>
                <input
                  type="text"
                  name="color"
                  value={formData.color}
                  onChange={handleInputChange}
                  className="info-input"
                  required
                />
              </div>
              <div className="info-item">
                <label>Estado:</label>
                <select
                  name="estado"
                  value={formData.estado}
                  onChange={handleInputChange}
                  className="info-input"
                  required
                >
                  <option value="">Seleccionar</option>
                  <option value="NUEVO">NUEVO</option>
                  <option value="USADO">USADO</option>
                </select>
              </div>
            </div>
          </div>
        </div>

        {/* Especificaciones Técnicas */}
        <div className="info-section">
          <h2 className="section-title">
            ⚙️ Especificaciones Técnicas
          </h2>
          <div className="info-grid">
            <div className="info-row">
              <div className="info-item">
                <label>Combustible:</label>
                <select
                  name="combustible"
                  value={formData.combustible}
                  onChange={handleInputChange}
                  className="info-input"
                  required
                >
                  <option value="">Seleccionar</option>
                  <option value="GASOLINA">GASOLINA</option>
                  <option value="DIESEL">DIESEL</option>
                  <option value="ELECTRICO">ELÉCTRICO</option>
                  <option value="HIBRIDO">HÍBRIDO</option>
                </select>
              </div>
              <div className="info-item">
                <label>Transmisión:</label>
                <select
                  name="tipoTransmision"
                  value={formData.tipoTransmision}
                  onChange={handleInputChange}
                  className="info-input"
                  required
                >
                  <option value="">Seleccionar</option>
                  <option value="MANUAL">MANUAL</option>
                  <option value="AUTOMATICA">AUTOMÁTICA</option>
                </select>
              </div>
              <div className="info-item">
                <label>Puertas:</label>
                <input
                  type="number"
                  name="numeroPuertas"
                  value={formData.numeroPuertas}
                  onChange={handleInputChange}
                  className="info-input"
                  min="2"
                  max="5"
                  required
                />
              </div>
            </div>
            <div className="info-row">
              <div className="info-item aire-item">
                <label>
                  <input
                    type="checkbox"
                    name="tieneAireAcondicionado"
                    checked={formData.tieneAireAcondicionado}
                    onChange={handleInputChange}
                    className="checkbox-input"
                  />
                  Aire Acondicionado
                </label>
              </div>
            </div>
          </div>
        </div>

        {/* Información Comercial */}
        <div className="info-section">
          <h2 className="section-title">
            💰 Información Comercial
          </h2>
          <div className="info-grid">
            <div className="info-row">
              <div className="info-item">
                <label>Precio:</label>
                <input
                  type="number"
                  name="precio"
                  value={formData.precio}
                  onChange={handleInputChange}
                  className="info-input precio-input"
                  min="0"
                  step="1000"
                  required
                />
              </div>
              <div className="info-item">
                <label>Fecha de Registro:</label>
                <span className="info-value readonly">
                  {new Date(formData.fechaRegistro).toLocaleDateString('es-ES', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                  })} p. m.
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Botones de Acción */}
        <div className="form-actions">
          <button
            type="button"
            onClick={handleCancel}
            className="btn btn-secondary"
            disabled={isLoading}
          >
            ← Volver a Buscar
          </button>
          <button
            type="submit"
            className="btn btn-primary"
            disabled={isLoading}
          >
            {isLoading ? 'Actualizando...' : '✏️ Actualizar Carro'}
          </button>
        </div>

        {message && (
          <div className={`message ${message.includes('Error') ? 'error' : 'success'}`}>
            {message}
          </div>
        )}
      </form>
    </div>
  );
};

export default ActualizarCarroFormulario;
