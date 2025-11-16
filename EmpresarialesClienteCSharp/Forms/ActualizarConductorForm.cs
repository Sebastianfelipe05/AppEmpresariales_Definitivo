using System;
using System.Drawing;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Models;

namespace EmpresarialesClienteCSharp.Forms
{
    public class ActualizarConductorForm : Form
    {
        private TextBox txtCedula;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtTelefono;
        private TextBox txtLicencia;
        private DateTimePicker dtpFechaNacimiento;
        private NumericUpDown numSalario;
        private CheckBox chkActivo;
        private Button btnActualizar;
        private Button btnCancelar;
        private Label lblTitulo;
        private readonly ConductorService _conductorService;
        private readonly string _cedulaOriginal;
        private Conductor _conductorOriginal;

        public ActualizarConductorForm(string cedula)
        {
            _conductorService = new ConductorService();
            _cedulaOriginal = cedula;
            InitializeComponents();
            CargarDatosConductor();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Actualizar Conductor";
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Título
            lblTitulo = new Label
            {
                Text = "ACTUALIZAR DATOS DEL CONDUCTOR",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 118, 210),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Cédula (solo lectura)
            var lblCedula = new Label
            {
                Text = "Cédula:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 70),
                Size = new Size(150, 25)
            };

            txtCedula = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 70),
                Size = new Size(350, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Nombre
            var lblNombre = new Label
            {
                Text = "Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 110),
                Size = new Size(150, 25)
            };

            txtNombre = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 110),
                Size = new Size(350, 25),
                MaxLength = 100
            };

            // Apellido
            var lblApellido = new Label
            {
                Text = "Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 150),
                Size = new Size(150, 25)
            };

            txtApellido = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 150),
                Size = new Size(350, 25),
                MaxLength = 100
            };

            // Teléfono
            var lblTelefono = new Label
            {
                Text = "Teléfono:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 190),
                Size = new Size(150, 25)
            };

            txtTelefono = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 190),
                Size = new Size(350, 25),
                MaxLength = 20
            };

            // Número de Licencia
            var lblLicencia = new Label
            {
                Text = "Número de Licencia:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 230),
                Size = new Size(150, 25)
            };

            txtLicencia = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 230),
                Size = new Size(350, 25),
                MaxLength = 50
            };

            // Fecha de Nacimiento
            var lblFechaNacimiento = new Label
            {
                Text = "Fecha de Nacimiento:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 270),
                Size = new Size(150, 25)
            };

            dtpFechaNacimiento = new DateTimePicker
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 270),
                Size = new Size(350, 25),
                Format = DateTimePickerFormat.Long,
                MaxDate = DateTime.Now.AddYears(-18)
            };

            // Salario
            var lblSalario = new Label
            {
                Text = "Salario:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 310),
                Size = new Size(150, 25)
            };

            numSalario = new NumericUpDown
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(200, 310),
                Size = new Size(350, 25),
                Minimum = 0,
                Maximum = 999999999,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };

            // Activo
            chkActivo = new CheckBox
            {
                Text = "Conductor Activo",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(200, 350),
                AutoSize = true
            };

            // Botón Actualizar
            btnActualizar = new Button
            {
                Text = "Actualizar Conductor",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(110, 400),
                Size = new Size(190, 40),
                BackColor = Color.FromArgb(25, 118, 210),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnActualizar.FlatAppearance.BorderSize = 0;
            btnActualizar.Click += BtnActualizar_Click;

            // Botón Cancelar
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(320, 400),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            // Agregar controles
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblCedula);
            this.Controls.Add(txtCedula);
            this.Controls.Add(lblNombre);
            this.Controls.Add(txtNombre);
            this.Controls.Add(lblApellido);
            this.Controls.Add(txtApellido);
            this.Controls.Add(lblTelefono);
            this.Controls.Add(txtTelefono);
            this.Controls.Add(lblLicencia);
            this.Controls.Add(txtLicencia);
            this.Controls.Add(lblFechaNacimiento);
            this.Controls.Add(dtpFechaNacimiento);
            this.Controls.Add(lblSalario);
            this.Controls.Add(numSalario);
            this.Controls.Add(chkActivo);
            this.Controls.Add(btnActualizar);
            this.Controls.Add(btnCancelar);
        }

        private async void CargarDatosConductor()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                _conductorOriginal = await _conductorService.ObtenerConductorPorCedulaAsync(_cedulaOriginal);

                txtCedula.Text = _conductorOriginal.Cedula;
                txtNombre.Text = _conductorOriginal.Nombre;
                txtApellido.Text = _conductorOriginal.Apellido;
                txtTelefono.Text = _conductorOriginal.Telefono;
                txtLicencia.Text = _conductorOriginal.LicenciaNumero;
                dtpFechaNacimiento.Value = _conductorOriginal.FechaNacimiento;
                numSalario.Value = (decimal)_conductorOriginal.Salario;
                chkActivo.Checked = _conductorOriginal.Activo;

                this.Text = $"Actualizar Conductor - {_conductorOriginal.NombreCompleto}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar conductor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void BtnActualizar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("El teléfono es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTelefono.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLicencia.Text))
            {
                MessageBox.Show("El número de licencia es obligatorio", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLicencia.Focus();
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnActualizar.Enabled = false;

                var conductor = new Conductor
                {
                    Cedula = txtCedula.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    LicenciaNumero = txtLicencia.Text.Trim(),
                    FechaNacimiento = dtpFechaNacimiento.Value,
                    Salario = (double)numSalario.Value,
                    Activo = chkActivo.Checked
                };

                var resultado = await _conductorService.ActualizarConductorAsync(_cedulaOriginal, conductor);

                MessageBox.Show(
                    $"Conductor {resultado.NombreCompleto} actualizado exitosamente",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar conductor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnActualizar.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
