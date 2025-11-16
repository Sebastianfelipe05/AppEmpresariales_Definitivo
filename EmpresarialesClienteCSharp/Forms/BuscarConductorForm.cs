using System;
using System.Drawing;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Models;

namespace EmpresarialesClienteCSharp.Forms
{
    public class BuscarConductorForm : Form
    {
        private TextBox txtCedula;
        private Button btnBuscar;
        private Button btnLimpiar;
        private Button btnVolver;
        private Label lblTitulo;
        private GroupBox grpResultado;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtTelefono;
        private TextBox txtLicencia;
        private TextBox txtFechaNacimiento;
        private TextBox txtSalario;
        private TextBox txtEstado;
        private TextBox txtEdad;
        private Button btnEditar;
        private Button btnEliminar;
        private readonly ConductorService _conductorService;
        private Conductor _conductorEncontrado;

        public BuscarConductorForm()
        {
            _conductorService = new ConductorService();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Buscar Conductor";
            this.Size = new Size(650, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Título
            lblTitulo = new Label
            {
                Text = "BUSCAR CONDUCTOR POR CÉDULA",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(106, 90, 205),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Búsqueda
            var lblCedula = new Label
            {
                Text = "Ingrese la Cédula:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 70),
                Size = new Size(150, 25)
            };

            txtCedula = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(190, 67),
                Size = new Size(250, 30),
                MaxLength = 20
            };
            txtCedula.KeyPress += TxtCedula_KeyPress;

            btnBuscar = new Button
            {
                Text = "Buscar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(460, 65),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(106, 90, 205),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            // GroupBox de Resultado
            grpResultado = new GroupBox
            {
                Text = "Información del Conductor",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 120),
                Size = new Size(590, 360),
                Visible = false
            };

            // Nombre
            var lblNombre = new Label
            {
                Text = "Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 35),
                Size = new Size(150, 25)
            };

            txtNombre = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 35),
                Size = new Size(380, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Apellido
            var lblApellido = new Label
            {
                Text = "Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 70),
                Size = new Size(150, 25)
            };

            txtApellido = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 70),
                Size = new Size(380, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Teléfono
            var lblTelefono = new Label
            {
                Text = "Teléfono:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 105),
                Size = new Size(150, 25)
            };

            txtTelefono = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 105),
                Size = new Size(380, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Licencia
            var lblLicencia = new Label
            {
                Text = "Número de Licencia:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 140),
                Size = new Size(150, 25)
            };

            txtLicencia = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 140),
                Size = new Size(380, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Fecha Nacimiento
            var lblFechaNacimiento = new Label
            {
                Text = "Fecha Nacimiento:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 175),
                Size = new Size(150, 25)
            };

            txtFechaNacimiento = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 175),
                Size = new Size(180, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Edad
            var lblEdad = new Label
            {
                Text = "Edad:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(380, 175),
                Size = new Size(50, 25)
            };

            txtEdad = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(440, 175),
                Size = new Size(120, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Salario
            var lblSalario = new Label
            {
                Text = "Salario:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 210),
                Size = new Size(150, 25)
            };

            txtSalario = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 210),
                Size = new Size(380, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Estado
            var lblEstado = new Label
            {
                Text = "Estado:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 245),
                Size = new Size(150, 25)
            };

            txtEstado = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 245),
                Size = new Size(380, 25),
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Botones de acción
            btnEditar = new Button
            {
                Text = "Editar Conductor",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(80, 295),
                Size = new Size(160, 40),
                BackColor = Color.FromArgb(25, 118, 210),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += BtnEditar_Click;

            btnEliminar = new Button
            {
                Text = "Eliminar Conductor",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(260, 295),
                Size = new Size(170, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;

            // Agregar controles al GroupBox
            grpResultado.Controls.Add(lblNombre);
            grpResultado.Controls.Add(txtNombre);
            grpResultado.Controls.Add(lblApellido);
            grpResultado.Controls.Add(txtApellido);
            grpResultado.Controls.Add(lblTelefono);
            grpResultado.Controls.Add(txtTelefono);
            grpResultado.Controls.Add(lblLicencia);
            grpResultado.Controls.Add(txtLicencia);
            grpResultado.Controls.Add(lblFechaNacimiento);
            grpResultado.Controls.Add(txtFechaNacimiento);
            grpResultado.Controls.Add(lblEdad);
            grpResultado.Controls.Add(txtEdad);
            grpResultado.Controls.Add(lblSalario);
            grpResultado.Controls.Add(txtSalario);
            grpResultado.Controls.Add(lblEstado);
            grpResultado.Controls.Add(txtEstado);
            grpResultado.Controls.Add(btnEditar);
            grpResultado.Controls.Add(btnEliminar);

            // Botones inferiores
            btnLimpiar = new Button
            {
                Text = "Limpiar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(270, 500),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(255, 165, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.Click += BtnLimpiar_Click;

            btnVolver = new Button
            {
                Text = "Volver",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(410, 500),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => this.Close();

            // Agregar controles al formulario
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblCedula);
            this.Controls.Add(txtCedula);
            this.Controls.Add(btnBuscar);
            this.Controls.Add(grpResultado);
            this.Controls.Add(btnLimpiar);
            this.Controls.Add(btnVolver);
        }

        private void TxtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnBuscar_Click(sender, e);
            }
        }

        private async void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Por favor ingrese una cédula", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCedula.Focus();
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnBuscar.Enabled = false;

                _conductorEncontrado = await _conductorService.ObtenerConductorPorCedulaAsync(txtCedula.Text.Trim());

                // Mostrar información
                txtNombre.Text = _conductorEncontrado.Nombre;
                txtApellido.Text = _conductorEncontrado.Apellido;
                txtTelefono.Text = _conductorEncontrado.Telefono;
                txtLicencia.Text = _conductorEncontrado.LicenciaNumero;
                txtFechaNacimiento.Text = _conductorEncontrado.FechaNacimiento.ToString("dd/MM/yyyy");
                txtEdad.Text = $"{_conductorEncontrado.EdadAproximada} años";
                txtSalario.Text = _conductorEncontrado.Salario.ToString("C0");
                txtEstado.Text = _conductorEncontrado.Activo ? "ACTIVO" : "INACTIVO";
                txtEstado.ForeColor = _conductorEncontrado.Activo ? Color.Green : Color.Red;

                grpResultado.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar conductor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                grpResultado.Visible = false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnBuscar.Enabled = true;
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtCedula.Clear();
            grpResultado.Visible = false;
            _conductorEncontrado = null;
            txtCedula.Focus();
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (_conductorEncontrado != null)
            {
                var form = new ActualizarConductorForm(_conductorEncontrado.Cedula);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Recargar datos
                    BtnBuscar_Click(sender, e);
                }
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_conductorEncontrado != null)
            {
                var form = new EliminarConductorForm(_conductorEncontrado.Cedula);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    BtnLimpiar_Click(sender, e);
                }
            }
        }
    }
}
