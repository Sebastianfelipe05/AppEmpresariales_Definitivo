using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class ActualizarMantenimientoForm : Form
    {
        private readonly MantenimientoService _mantenimientoService;
        private Mantenimiento? _mantenimientoActual;

        private TextBox txtPlaca = null!;
        private Button btnBuscar = null!;
        private DataGridView dgvMantenimientos = null!;
        private Panel panelFormulario = null!;

        private TextBox txtPlacaCarro = null!;
        private DateTimePicker dtpFechaMantenimiento = null!;
        private NumericUpDown nudKilometraje = null!;
        private ComboBox cboTipoMantenimiento = null!;
        private NumericUpDown nudCosto = null!;
        private TextBox txtDescripcion = null!;
        private DateTimePicker dtpProximoMantenimiento = null!;
        private CheckBox chkProximoMantenimiento = null!;
        private CheckBox chkCompletado = null!;
        private Button btnActualizar = null!;

        public ActualizarMantenimientoForm()
        {
            _mantenimientoService = new MantenimientoService();
            InitializeComponent();
        }

        public ActualizarMantenimientoForm(string placa) : this()
        {
            // Cargar los mantenimientos después de inicializar componentes
            this.Load += (s, e) => CargarMantenimientosPorPlaca(placa);
        }

        private void CargarMantenimientosPorPlaca(string placa)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(placa))
                {
                    txtPlaca.Text = placa;
                    BtnBuscar_Click(null, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("La placa del carro no es válida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los mantenimientos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Actualizar Mantenimiento";
            this.Size = new Size(900, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(249, 250, 251);
            this.MinimumSize = new Size(900, 750);
            this.MaximumSize = new Size(900, 750);

            // Modern title bar with close button
            var titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(147, 51, 234) // Purple theme for maintenance
            };
            titleBar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            };

            var lblTitulo = new Label
            {
                Text = "🔧 Actualizar Mantenimiento",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 18),
                AutoSize = true
            };

            var btnCerrar = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                Location = new Point(840, 17),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(126, 34, 206);
            btnCerrar.Click += (s, e) => this.Close();

            titleBar.Controls.AddRange(new Control[] { lblTitulo, btnCerrar });

            // Main content panel
            var mainPanel = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(840, 620),
                BackColor = Color.White,
                AutoScroll = true
            };
            mainPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(mainPanel.ClientRectangle, 15))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(229, 231, 235), 1), path);
                }
            };

            // Search section
            var searchPanel = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(780, 100),
                BackColor = Color.FromArgb(243, 244, 246)
            };
            searchPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(searchPanel.ClientRectangle, 10))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.FromArgb(243, 244, 246)), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(209, 213, 219), 1), path);
                }
            };

            var lblInstruccion = new Label
            {
                Text = "🔍 Paso 1: Buscar Mantenimientos por Placa del Carro",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(20, 15),
                AutoSize = true
            };

            var lblPlaca = new Label
            {
                Text = "Placa del Carro:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(20, 50),
                AutoSize = true
            };

            txtPlaca = new TextBox
            {
                Location = new Point(140, 47),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                CharacterCasing = CharacterCasing.Upper,
                MaxLength = 10
            };
            txtPlaca.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BtnBuscar_Click(s, e);
                }
            };

            btnBuscar = CreateModernButton("🔎 Buscar", Color.FromArgb(147, 51, 234), 360, 45, 150, 35);
            btnBuscar.Click += BtnBuscar_Click;

            searchPanel.Controls.AddRange(new Control[] { lblInstruccion, lblPlaca, txtPlaca, btnBuscar });

            // DataGridView para mostrar mantenimientos encontrados
            dgvMantenimientos = new DataGridView
            {
                Location = new Point(30, 130),
                Size = new Size(780, 180),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(147, 51, 234),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                Visible = false
            };
            dgvMantenimientos.CellDoubleClick += DgvMantenimientos_CellDoubleClick;

            // Form panel (hidden by default)
            panelFormulario = new Panel
            {
                Location = new Point(30, 320),
                Size = new Size(780, 270),
                Visible = false,
                AutoScroll = true
            };

            CrearFormularioEdicion();

            mainPanel.Controls.AddRange(new Control[] { searchPanel, dgvMantenimientos, panelFormulario });
            this.Controls.AddRange(new Control[] { titleBar, mainPanel });
        }

        private void CrearFormularioEdicion()
        {
            var lblPaso2 = new Label
            {
                Text = "✏️ Paso 2: Editar Información del Mantenimiento",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(0, 0),
                AutoSize = true
            };
            panelFormulario.Controls.Add(lblPaso2);

            int yPos = 40;
            int col1X = 0;
            int col2X = 390;
            int labelWidth = 160;
            int controlWidth = 200;

            // Column 1: Basic info
            var lblPlaca = CreateLabel("🚗 Placa del Carro:", col1X, yPos);
            txtPlacaCarro = new TextBox
            {
                Location = new Point(col1X, yPos + 25),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Enabled = false,
                BackColor = Color.FromArgb(243, 244, 246)
            };
            panelFormulario.Controls.AddRange(new Control[] { lblPlaca, txtPlacaCarro });

            // Column 2: Date
            var lblFecha = CreateLabel("📅 Fecha Mantenimiento:", col2X, yPos);
            dtpFechaMantenimiento = new DateTimePicker
            {
                Location = new Point(col2X, yPos + 25),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm"
            };
            panelFormulario.Controls.AddRange(new Control[] { lblFecha, dtpFechaMantenimiento });
            yPos += 70;

            // Column 1: Kilometraje
            var lblKilometraje = CreateLabel("📏 Kilometraje (km):", col1X, yPos);
            nudKilometraje = new NumericUpDown
            {
                Location = new Point(col1X, yPos + 25),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 1000000,
                ThousandsSeparator = true
            };
            panelFormulario.Controls.AddRange(new Control[] { lblKilometraje, nudKilometraje });

            // Column 2: Tipo
            var lblTipo = CreateLabel("🔧 Tipo Mantenimiento:", col2X, yPos);
            cboTipoMantenimiento = new ComboBox
            {
                Location = new Point(col2X, yPos + 25),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboTipoMantenimiento.Items.AddRange(new object[]
            {
                "PREVENTIVO",
                "CORRECTIVO",
                "REVISION",
                "CAMBIO_ACEITE",
                "CAMBIO_LLANTAS",
                "OTROS"
            });
            panelFormulario.Controls.AddRange(new Control[] { lblTipo, cboTipoMantenimiento });
            yPos += 70;

            // Column 1: Costo
            var lblCosto = CreateLabel("💰 Costo:", col1X, yPos);
            nudCosto = new NumericUpDown
            {
                Location = new Point(col1X, yPos + 25),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                Minimum = 0,
                Maximum = 100000000,
                DecimalPlaces = 0,
                ThousandsSeparator = true
            };
            panelFormulario.Controls.AddRange(new Control[] { lblCosto, nudCosto });

            // Column 2: Completado checkbox
            chkCompletado = new CheckBox
            {
                Text = "✅ Mantenimiento Completado",
                Location = new Point(col2X, yPos + 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };
            panelFormulario.Controls.Add(chkCompletado);
            yPos += 70;

            // Description (full width)
            var lblDescripcion = CreateLabel("📝 Descripción:", col1X, yPos);
            txtDescripcion = new TextBox
            {
                Location = new Point(col1X, yPos + 25),
                Size = new Size(590, 80),
                Font = new Font("Segoe UI", 10),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 500
            };
            panelFormulario.Controls.AddRange(new Control[] { lblDescripcion, txtDescripcion });
            yPos += 115;

            // Proximo mantenimiento section
            chkProximoMantenimiento = new CheckBox
            {
                Text = "📆 Programar Próximo Mantenimiento",
                Location = new Point(col1X, yPos),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(147, 51, 234)
            };
            chkProximoMantenimiento.CheckedChanged += (s, e) =>
            {
                dtpProximoMantenimiento.Enabled = chkProximoMantenimiento.Checked;
            };
            panelFormulario.Controls.Add(chkProximoMantenimiento);
            yPos += 30;

            dtpProximoMantenimiento = new DateTimePicker
            {
                Location = new Point(col1X, yPos),
                Size = new Size(controlWidth, 30),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Enabled = false
            };
            panelFormulario.Controls.Add(dtpProximoMantenimiento);
            yPos += 50;

            // Action buttons
            btnActualizar = CreateModernButton("💾 Actualizar Mantenimiento", Color.FromArgb(147, 51, 234), col1X, yPos, 280, 45);
            btnActualizar.Click += BtnActualizar_Click;

            var btnCancelar = CreateModernButton("❌ Cancelar", Color.FromArgb(107, 114, 128), col1X + 300, yPos, 280, 45);
            btnCancelar.Click += (s, e) => this.Close();

            panelFormulario.Controls.AddRange(new Control[] { btnActualizar, btnCancelar });
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(75, 85, 99)
            };
        }

        private Button CreateModernButton(string text, Color bgColor, int x, int y, int width, int height)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            Color hoverColor = ControlPaint.Dark(bgColor, 0.1f);
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = bgColor;

            return btn;
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private async void BtnBuscar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlaca.Text))
            {
                MessageBox.Show("⚠️ Por favor ingrese una placa", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnBuscar.Enabled = false;
                btnBuscar.Text = "🔍 Buscando...";
                dgvMantenimientos.Visible = false;
                panelFormulario.Visible = false;

                var mantenimientos = await _mantenimientoService.BuscarPorCarroAsync(txtPlaca.Text.Trim());

                if (mantenimientos == null || !mantenimientos.Any())
                {
                    MessageBox.Show($"❌ No se encontraron mantenimientos para la placa {txtPlaca.Text.Trim()}",
                        "No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Mostrar mantenimientos en el grid
                dgvMantenimientos.DataSource = mantenimientos;
                ConfigurarColumnasGrid();
                dgvMantenimientos.Visible = true;

                MessageBox.Show($"✅ Se encontraron {mantenimientos.Count} mantenimiento(s).\n\nHaga doble clic en uno para editarlo.",
                    "Mantenimientos Encontrados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al buscar mantenimientos:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBuscar.Enabled = true;
                btnBuscar.Text = "🔎 Buscar";
            }
        }

        private void ConfigurarColumnasGrid()
        {
            if (dgvMantenimientos.Columns.Count > 0)
            {
                dgvMantenimientos.Columns["Id"].HeaderText = "ID";
                dgvMantenimientos.Columns["Id"].Width = 60;
                dgvMantenimientos.Columns["PlacaCarro"].Visible = false;
                dgvMantenimientos.Columns["FechaMantenimiento"].HeaderText = "Fecha";
                dgvMantenimientos.Columns["FechaMantenimiento"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgvMantenimientos.Columns["TipoMantenimiento"].HeaderText = "Tipo";
                dgvMantenimientos.Columns["Kilometraje"].HeaderText = "KM";
                dgvMantenimientos.Columns["Costo"].HeaderText = "Costo";
                dgvMantenimientos.Columns["Costo"].DefaultCellStyle.Format = "C2";
                dgvMantenimientos.Columns["Descripcion"].HeaderText = "Descripción";
                dgvMantenimientos.Columns["Completado"].HeaderText = "Completado";
                dgvMantenimientos.Columns["ProximoMantenimiento"].Visible = false;
                dgvMantenimientos.Columns["FechaRegistro"].Visible = false;
            }
        }

        private void DgvMantenimientos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var mantenimiento = dgvMantenimientos.Rows[e.RowIndex].DataBoundItem as Mantenimiento;
                if (mantenimiento != null)
                {
                    _mantenimientoActual = mantenimiento;
                    CargarDatosEnFormulario(mantenimiento);
                    panelFormulario.Visible = true;
                }
            }
        }

        private void CargarDatosEnFormulario(Mantenimiento mantenimiento)
        {
            txtPlacaCarro.Text = mantenimiento.PlacaCarro;
            dtpFechaMantenimiento.Value = mantenimiento.FechaMantenimiento ?? DateTime.Now;
            nudKilometraje.Value = mantenimiento.Kilometraje;
            cboTipoMantenimiento.SelectedItem = mantenimiento.TipoMantenimiento;
            nudCosto.Value = (decimal)mantenimiento.Costo;
            txtDescripcion.Text = mantenimiento.Descripcion;
            chkCompletado.Checked = mantenimiento.Completado;

            if (mantenimiento.ProximoMantenimiento.HasValue)
            {
                chkProximoMantenimiento.Checked = true;
                dtpProximoMantenimiento.Value = mantenimiento.ProximoMantenimiento.Value;
            }
            else
            {
                chkProximoMantenimiento.Checked = false;
            }
        }

        private async void BtnActualizar_Click(object? sender, EventArgs e)
        {
            if (_mantenimientoActual == null)
            {
                MessageBox.Show("❌ No hay mantenimiento cargado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text) || txtDescripcion.Text.Length < 10)
            {
                MessageBox.Show("⚠️ La descripción debe tener al menos 10 caracteres", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescripcion.Focus();
                return;
            }

            if (cboTipoMantenimiento.SelectedItem == null)
            {
                MessageBox.Show("⚠️ Por favor seleccione un tipo de mantenimiento", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboTipoMantenimiento.Focus();
                return;
            }

            try
            {
                _mantenimientoActual.FechaMantenimiento = dtpFechaMantenimiento.Value;
                _mantenimientoActual.Kilometraje = (int)nudKilometraje.Value;
                _mantenimientoActual.TipoMantenimiento = cboTipoMantenimiento.SelectedItem?.ToString() ?? "PREVENTIVO";
                _mantenimientoActual.Costo = (double)nudCosto.Value;
                _mantenimientoActual.Descripcion = txtDescripcion.Text.Trim();
                _mantenimientoActual.ProximoMantenimiento = chkProximoMantenimiento.Checked ? dtpProximoMantenimiento.Value : null;
                _mantenimientoActual.Completado = chkCompletado.Checked;

                btnActualizar.Enabled = false;
                btnActualizar.Text = "⏳ Actualizando...";

                await _mantenimientoService.ActualizarMantenimientoAsync(_mantenimientoActual.Id, _mantenimientoActual);

                MessageBox.Show("✅ Mantenimiento actualizado exitosamente",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al actualizar el mantenimiento:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnActualizar.Enabled = true;
                btnActualizar.Text = "💾 Actualizar Mantenimiento";
            }
        }
    }
}
