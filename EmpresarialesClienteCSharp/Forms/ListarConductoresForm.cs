using System;
using System.Drawing;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Models;

namespace EmpresarialesClienteCSharp.Forms
{
    public class ListarConductoresForm : Form
    {
        private DataGridView dgvConductores;
        private Button btnRefrescar;
        private Button btnCrear;
        private Button btnBuscar;
        private Button btnVolver;
        private TextBox txtBuscar;
        private Label lblTitulo;
        private Label lblBuscar;
        private CheckBox chkSoloActivos;
        private readonly ConductorService _conductorService;

        public ListarConductoresForm()
        {
            _conductorService = new ConductorService();
            InitializeComponents();
            CargarConductores();
        }

        private void InitializeComponents()
        {
            // Configuración del formulario
            this.Text = "Listado de Conductores";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Título
            lblTitulo = new Label
            {
                Text = "GESTIÓN DE CONDUCTORES",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Label Buscar
            lblBuscar = new Label
            {
                Text = "Buscar por nombre:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 70),
                AutoSize = true
            };

            // TextBox Buscar
            txtBuscar = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(160, 67),
                Size = new Size(250, 25)
            };

            // CheckBox Solo Activos
            chkSoloActivos = new CheckBox
            {
                Text = "Solo activos",
                Font = new Font("Segoe UI", 10),
                Location = new Point(430, 67),
                AutoSize = true
            };

            // Botón Buscar
            btnBuscar = new Button
            {
                Text = "Buscar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(550, 65),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(106, 90, 205),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            // DataGridView
            dgvConductores = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(940, 380),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            // Botones de acción
            btnRefrescar = new Button
            {
                Text = "Refrescar",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(20, 510),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefrescar.FlatAppearance.BorderSize = 0;
            btnRefrescar.Click += BtnRefrescar_Click;

            btnCrear = new Button
            {
                Text = "Nuevo Conductor",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(160, 510),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(46, 139, 87),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCrear.FlatAppearance.BorderSize = 0;
            btnCrear.Click += BtnCrear_Click;

            btnVolver = new Button
            {
                Text = "Volver al Menú",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(830, 510),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVolver.FlatAppearance.BorderSize = 0;
            btnVolver.Click += (s, e) => this.Close();

            // Agregar controles al formulario
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblBuscar);
            this.Controls.Add(txtBuscar);
            this.Controls.Add(chkSoloActivos);
            this.Controls.Add(btnBuscar);
            this.Controls.Add(dgvConductores);
            this.Controls.Add(btnRefrescar);
            this.Controls.Add(btnCrear);
            this.Controls.Add(btnVolver);

            // Evento doble clic para editar
            dgvConductores.CellDoubleClick += DgvConductores_CellDoubleClick;
        }

        private async void CargarConductores()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                var conductores = await _conductorService.ListarTodosConductoresAsync();

                dgvConductores.DataSource = null;
                dgvConductores.DataSource = conductores;

                // Configurar columnas
                if (dgvConductores.Columns.Count > 0)
                {
                    dgvConductores.Columns["Cedula"].HeaderText = "Cédula";
                    dgvConductores.Columns["Nombre"].HeaderText = "Nombre";
                    dgvConductores.Columns["Apellido"].HeaderText = "Apellido";
                    dgvConductores.Columns["Telefono"].HeaderText = "Teléfono";
                    dgvConductores.Columns["LicenciaNumero"].HeaderText = "Licencia";
                    dgvConductores.Columns["FechaNacimiento"].HeaderText = "F. Nacimiento";
                    dgvConductores.Columns["Salario"].HeaderText = "Salario";
                    dgvConductores.Columns["Salario"].DefaultCellStyle.Format = "C0";
                    dgvConductores.Columns["Activo"].HeaderText = "Estado";
                    dgvConductores.Columns["FechaRegistro"].HeaderText = "F. Registro";

                    // Ocultar columnas no necesarias
                    if (dgvConductores.Columns.Contains("NombreCompleto"))
                        dgvConductores.Columns["NombreCompleto"].Visible = false;
                    if (dgvConductores.Columns.Contains("EdadAproximada"))
                        dgvConductores.Columns["EdadAproximada"].Visible = false;

                    // Colorear filas según estado
                    foreach (DataGridViewRow row in dgvConductores.Rows)
                    {
                        var conductor = row.DataBoundItem as Conductor;
                        if (conductor != null && !conductor.Activo)
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 228, 225);
                        }
                    }
                }

                this.Text = $"Listado de Conductores ({conductores.Count} registros)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar conductores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void BtnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (chkSoloActivos.Checked)
                {
                    var conductores = await _conductorService.BuscarConductoresActivosAsync();
                    dgvConductores.DataSource = conductores;
                }
                else if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    var conductores = await _conductorService.BuscarConductoresPorNombreAsync(txtBuscar.Text);
                    dgvConductores.DataSource = conductores;
                }
                else
                {
                    CargarConductores();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void BtnRefrescar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            chkSoloActivos.Checked = false;
            CargarConductores();
        }

        private void BtnCrear_Click(object sender, EventArgs e)
        {
            var form = new CrearConductorForm();
            form.ShowDialog();
            CargarConductores(); // Refrescar después de crear
        }

        private void DgvConductores_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var conductor = dgvConductores.Rows[e.RowIndex].DataBoundItem as Conductor;
                if (conductor != null)
                {
                    var result = MessageBox.Show(
                        $"¿Qué desea hacer con el conductor {conductor.NombreCompleto}?",
                        "Opciones",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1,
                        (MessageBoxOptions)0,
                        false);

                    if (result == DialogResult.Yes)
                    {
                        // Editar
                        var form = new ActualizarConductorForm(conductor.Cedula);
                        form.ShowDialog();
                        CargarConductores();
                    }
                    else if (result == DialogResult.No)
                    {
                        // Eliminar
                        var form = new EliminarConductorForm(conductor.Cedula);
                        form.ShowDialog();
                        CargarConductores();
                    }
                }
            }
        }
    }
}
