using System;
using System.Drawing;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Models;

namespace EmpresarialesClienteCSharp.Forms
{
    public class EliminarConductorForm : Form
    {
        private TextBox txtCedula;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtTelefono;
        private TextBox txtLicencia;
        private TextBox txtFechaNacimiento;
        private TextBox txtSalario;
        private TextBox txtEstado;
        private TextBox txtConfirmar;
        private Button btnEliminar;
        private Button btnCancelar;
        private Label lblTitulo;
        private Label lblAdvertencia;
        private readonly ConductorService _conductorService;
        private readonly string _cedula;
        private Conductor _conductor;

        public EliminarConductorForm(string cedula)
        {
            _conductorService = new ConductorService();
            _cedula = cedula;
            InitializeComponents();
            CargarDatosConductor();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Eliminar Conductor";
            this.Size = new Size(600, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 240, 240);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Título
            lblTitulo = new Label
            {
                Text = "ELIMINAR CONDUCTOR",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Advertencia
            lblAdvertencia = new Label
            {
                Text = "⚠️ ADVERTENCIA: Esta acción no se puede deshacer",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 87, 34),
                Location = new Point(20, 60),
                AutoSize = true
            };

            // Panel de información (solo lectura)
            var panelInfo = new Panel
            {
                Location = new Point(20, 100),
                Size = new Size(540, 320),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Cédula
            var lblCedula = new Label
            {
                Text = "Cédula:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(150, 25)
            };

            txtCedula = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 10),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Nombre
            var lblNombre = new Label
            {
                Text = "Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 45),
                Size = new Size(150, 25)
            };

            txtNombre = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 45),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Apellido
            var lblApellido = new Label
            {
                Text = "Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 80),
                Size = new Size(150, 25)
            };

            txtApellido = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 80),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Teléfono
            var lblTelefono = new Label
            {
                Text = "Teléfono:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 115),
                Size = new Size(150, 25)
            };

            txtTelefono = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 115),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Licencia
            var lblLicencia = new Label
            {
                Text = "Licencia:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 150),
                Size = new Size(150, 25)
            };

            txtLicencia = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 150),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Fecha Nacimiento
            var lblFechaNacimiento = new Label
            {
                Text = "Fecha Nacimiento:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 185),
                Size = new Size(150, 25)
            };

            txtFechaNacimiento = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 185),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Salario
            var lblSalario = new Label
            {
                Text = "Salario:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 220),
                Size = new Size(150, 25)
            };

            txtSalario = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 220),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Estado
            var lblEstado = new Label
            {
                Text = "Estado:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 255),
                Size = new Size(150, 25)
            };

            txtEstado = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 255),
                Size = new Size(340, 25),
                ReadOnly = true,
                BackColor = Color.LightGray
            };

            // Agregar controles al panel
            panelInfo.Controls.Add(lblCedula);
            panelInfo.Controls.Add(txtCedula);
            panelInfo.Controls.Add(lblNombre);
            panelInfo.Controls.Add(txtNombre);
            panelInfo.Controls.Add(lblApellido);
            panelInfo.Controls.Add(txtApellido);
            panelInfo.Controls.Add(lblTelefono);
            panelInfo.Controls.Add(txtTelefono);
            panelInfo.Controls.Add(lblLicencia);
            panelInfo.Controls.Add(txtLicencia);
            panelInfo.Controls.Add(lblFechaNacimiento);
            panelInfo.Controls.Add(txtFechaNacimiento);
            panelInfo.Controls.Add(lblSalario);
            panelInfo.Controls.Add(txtSalario);
            panelInfo.Controls.Add(lblEstado);
            panelInfo.Controls.Add(txtEstado);

            // Confirmación
            var lblConfirmar = new Label
            {
                Text = "Para confirmar escriba 'ELIMINAR':",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, 440),
                Size = new Size(250, 25)
            };

            txtConfirmar = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(290, 437),
                Size = new Size(270, 30),
                CharacterCasing = CharacterCasing.Upper
            };

            // Botón Eliminar
            btnEliminar = new Button
            {
                Text = "Confirmar Eliminación",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(120, 490),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.Click += BtnEliminar_Click;

            // Botón Cancelar
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(340, 490),
                Size = new Size(140, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            // Agregar controles al formulario
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblAdvertencia);
            this.Controls.Add(panelInfo);
            this.Controls.Add(lblConfirmar);
            this.Controls.Add(txtConfirmar);
            this.Controls.Add(btnEliminar);
            this.Controls.Add(btnCancelar);
        }

        private async void CargarDatosConductor()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                _conductor = await _conductorService.ObtenerConductorPorCedulaAsync(_cedula);

                txtCedula.Text = _conductor.Cedula;
                txtNombre.Text = _conductor.Nombre;
                txtApellido.Text = _conductor.Apellido;
                txtTelefono.Text = _conductor.Telefono;
                txtLicencia.Text = _conductor.LicenciaNumero;
                txtFechaNacimiento.Text = _conductor.FechaNacimiento.ToString("dd/MM/yyyy");
                txtSalario.Text = _conductor.Salario.ToString("C0");
                txtEstado.Text = _conductor.Activo ? "ACTIVO" : "INACTIVO";

                this.Text = $"Eliminar Conductor - {_conductor.NombreCompleto}";
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

        private async void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (txtConfirmar.Text.Trim() != "ELIMINAR")
            {
                MessageBox.Show(
                    "Para confirmar la eliminación debe escribir 'ELIMINAR' en el campo de confirmación",
                    "Confirmación Requerida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtConfirmar.Focus();
                return;
            }

            var confirmResult = MessageBox.Show(
                $"¿Está absolutamente seguro que desea eliminar al conductor {_conductor.NombreCompleto}?\n\n" +
                "Esta acción NO se puede deshacer.",
                "Confirmación Final",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnEliminar.Enabled = false;

                await _conductorService.EliminarConductorAsync(_cedula);

                MessageBox.Show(
                    $"Conductor {_conductor.NombreCompleto} eliminado exitosamente",
                    "Eliminado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar conductor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEliminar.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
