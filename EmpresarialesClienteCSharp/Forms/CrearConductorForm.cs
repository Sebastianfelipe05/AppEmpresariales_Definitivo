using System;
using System.Drawing;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Models;

namespace EmpresarialesClienteCSharp.Forms
{
    public class CrearConductorForm : Form
    {
        private TextBox txtCedula;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtTelefono;
        private TextBox txtLicencia;
        private DateTimePicker dtpFechaNacimiento;
        private NumericUpDown numSalario;
        private CheckBox chkActivo;
        private Button btnGuardar;
        private Button btnCancelar;
        private Label lblTitulo;
        private readonly ConductorService _conductorService;

        public CrearConductorForm()
        {
            _conductorService = new ConductorService();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Crear Nuevo Conductor";
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Título
            lblTitulo = new Label
            {
                Text = "REGISTRAR NUEVO CONDUCTOR",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Cédula
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
                MaxLength = 20
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
                MaxDate = DateTime.Now.AddYears(-18),
                Value = DateTime.Now.AddYears(-30)
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
                ThousandsSeparator = true,
                Value = 2500000
            };

            // Activo
            chkActivo = new CheckBox
            {
                Text = "Conductor Activo",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(200, 350),
                AutoSize = true,
                Checked = true
            };

            // Botón Guardar
            btnGuardar = new Button
            {
                Text = "Guardar Conductor",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(120, 400),
                Size = new Size(180, 40),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

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
            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnCancelar);
        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("La cédula es obligatoria", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCedula.Focus();
                return;
            }

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
                btnGuardar.Enabled = false;

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

                var resultado = await _conductorService.CrearConductorAsync(conductor);

                MessageBox.Show(
                    $"Conductor {resultado.NombreCompleto} creado exitosamente",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear conductor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
