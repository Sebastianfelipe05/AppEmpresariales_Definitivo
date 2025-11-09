using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class EliminarMantenimientoForm : Form
    {
        private readonly MantenimientoService _mantenimientoService;
        private Mantenimiento? _mantenimientoActual;

        private TextBox txtPlaca = null!;
        private Button btnBuscar = null!;
        private DataGridView dgvMantenimientos = null!;
        private Panel panelInfo = null!;
        private Button btnEliminar = null!;
        private Label lblId = null!, lblPlaca = null!, lblFecha = null!, lblTipo = null!;
        private Label lblKilometraje = null!, lblCosto = null!, lblEstado = null!, lblCompletado = null!;
        private Label lblDescripcion = null!, lblProximo = null!;

        public EliminarMantenimientoForm()
        {
            _mantenimientoService = new MantenimientoService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Eliminar Mantenimiento";
            this.Size = new Size(750, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(249, 250, 251);
            this.MinimumSize = new Size(750, 700);
            this.MaximumSize = new Size(750, 700);

            // Modern title bar with close button
            var titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(220, 38, 38) // Red theme for delete
            };
            titleBar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            };

            var lblTitulo = new Label
            {
                Text = "🗑️ Eliminar Mantenimiento",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 18),
                AutoSize = true
            };

            var btnCerrar = new Button
            {
                Text = "✕",
                Size = new Size(35, 35),
                Location = new Point(690, 17),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(185, 28, 28);
            btnCerrar.Click += (s, e) => this.Close();

            titleBar.Controls.AddRange(new Control[] { lblTitulo, btnCerrar });

            // Main content panel
            var mainPanel = new Panel
            {
                Location = new Point(30, 100),
                Size = new Size(690, 570),
                BackColor = Color.White
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
            var lblInstruccion = new Label
            {
                Text = "🔍 Paso 1: Buscar Mantenimientos por Placa del Carro",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(30, 30),
                AutoSize = true
            };

            var lblPlacaLabel = new Label
            {
                Text = "Placa del Carro:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, 70),
                AutoSize = true
            };

            txtPlaca = new TextBox
            {
                Location = new Point(30, 95),
                Size = new Size(200, 35),
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

            btnBuscar = CreateModernButton("🔎 Buscar", Color.FromArgb(147, 51, 234), 250, 92, 150, 42);
            btnBuscar.Click += BtnBuscar_Click;

            // DataGridView para mostrar mantenimientos encontrados
            dgvMantenimientos = new DataGridView
            {
                Location = new Point(30, 150),
                Size = new Size(630, 180),
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
                    BackColor = Color.FromArgb(220, 38, 38),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                Visible = false
            };
            dgvMantenimientos.CellDoubleClick += DgvMantenimientos_CellDoubleClick;

            // Info panel (hidden by default)
            panelInfo = new Panel
            {
                Location = new Point(30, 350),
                Size = new Size(630, 320),
                BackColor = Color.FromArgb(254, 242, 242),
                Visible = false
            };
            panelInfo.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(panelInfo.ClientRectangle, 10))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.FromArgb(254, 242, 242)), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(252, 165, 165), 2), path);
                }
            };

            var lblInfoTitulo = new Label
            {
                Text = "⚠️ Información del Mantenimiento a Eliminar",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(153, 27, 27),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Create info labels in two columns
            int col1X = 20;
            int col2X = 330;
            int yStart = 50;
            int ySpacing = 30;

            lblId = CreateInfoLabel("🔖 ID:", "", col1X, yStart);
            lblPlaca = CreateInfoLabel("🚗 Placa:", "", col1X, yStart + ySpacing);
            lblFecha = CreateInfoLabel("📅 Fecha:", "", col1X, yStart + ySpacing * 2);
            lblTipo = CreateInfoLabel("🔧 Tipo:", "", col1X, yStart + ySpacing * 3);
            lblKilometraje = CreateInfoLabel("📏 Kilometraje:", "", col1X, yStart + ySpacing * 4);

            lblCosto = CreateInfoLabel("💰 Costo:", "", col2X, yStart);
            lblEstado = CreateInfoLabel("📊 Estado:", "", col2X, yStart + ySpacing);
            lblCompletado = CreateInfoLabel("✅ Completado:", "", col2X, yStart + ySpacing * 2);
            lblProximo = CreateInfoLabel("📆 Próximo:", "", col2X, yStart + ySpacing * 3);

            lblDescripcion = new Label
            {
                Text = "📝 Descripción: ",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(col1X, yStart + ySpacing * 5 + 10),
                Size = new Size(590, 60),
                BackColor = Color.White
            };

            var lblAdvertencia = new Label
            {
                Text = "⚠️ ADVERTENCIA: Esta acción es permanente y no se puede deshacer",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(20, 280),
                Size = new Size(590, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(254, 226, 226)
            };

            panelInfo.Controls.AddRange(new Control[] {
                lblInfoTitulo, lblId, lblPlaca, lblFecha, lblTipo, lblKilometraje,
                lblCosto, lblEstado, lblCompletado, lblProximo, lblDescripcion, lblAdvertencia
            });

            // Action buttons
            btnEliminar = CreateModernButton("🗑️ Eliminar Mantenimiento", Color.FromArgb(220, 38, 38), 30, 500, 300, 50);
            btnEliminar.Enabled = false;
            btnEliminar.Click += BtnEliminar_Click;

            var btnCancelar = CreateModernButton("❌ Cancelar", Color.FromArgb(107, 114, 128), 360, 500, 300, 50);
            btnCancelar.Click += (s, e) => this.Close();

            mainPanel.Controls.AddRange(new Control[] { lblInstruccion, lblPlacaLabel, txtPlaca, btnBuscar, dgvMantenimientos, panelInfo, btnEliminar, btnCancelar });
            this.Controls.AddRange(new Control[] { titleBar, mainPanel });
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

        private Label CreateInfoLabel(string caption, string value, int x, int y)
        {
            return new Label
            {
                Text = $"{caption} {value}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(x, y),
                AutoSize = true
            };
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
                panelInfo.Visible = false;
                btnEliminar.Enabled = false;

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

                MessageBox.Show($"✅ Se encontraron {mantenimientos.Count} mantenimiento(s).\n\nHaga doble clic en uno para eliminarlo.",
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
                    MostrarInformacion(mantenimiento);
                    panelInfo.Visible = true;
                    btnEliminar.Enabled = true;
                }
            }
        }

        private void MostrarInformacion(Mantenimiento mantenimiento)
        {
            lblId.Text = $"🔖 ID: {mantenimiento.Id}";
            lblPlaca.Text = $"🚗 Placa: {mantenimiento.PlacaCarro}";
            lblFecha.Text = $"📅 Fecha: {mantenimiento.GetFechaMantenimientoFormateada()}";
            lblTipo.Text = $"🔧 Tipo: {mantenimiento.TipoMantenimiento.Replace("_", " ")}";
            lblKilometraje.Text = $"📏 Kilometraje: {mantenimiento.Kilometraje:N0} km";
            lblCosto.Text = $"💰 Costo: ${mantenimiento.Costo:N0}";
            lblEstado.Text = $"📊 Estado: {mantenimiento.ObtenerEstadoMantenimiento()}";
            lblCompletado.Text = $"✅ Completado: {(mantenimiento.Completado ? "Sí" : "No")}";
            lblProximo.Text = $"📆 Próximo: {mantenimiento.GetProximoMantenimientoFormateado()}";

            string descripcionCorta = mantenimiento.Descripcion.Length > 100
                ? mantenimiento.Descripcion.Substring(0, 100) + "..."
                : mantenimiento.Descripcion;
            lblDescripcion.Text = $"📝 Descripción: {descripcionCorta}";
        }

        private async void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (_mantenimientoActual == null)
            {
                MessageBox.Show("❌ No hay mantenimiento cargado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(
                $"⚠️ ¿Está COMPLETAMENTE SEGURO que desea eliminar este mantenimiento?\n\n" +
                $"📋 ID: {_mantenimientoActual.Id}\n" +
                $"🚗 Placa: {_mantenimientoActual.PlacaCarro}\n" +
                $"🔧 Tipo: {_mantenimientoActual.TipoMantenimiento}\n" +
                $"💰 Costo: ${_mantenimientoActual.Costo:N0}\n\n" +
                $"⛔ Esta acción es PERMANENTE y NO se puede deshacer.\n" +
                $"Se perderán todos los registros de este mantenimiento.",
                "⚠️ Confirmar Eliminación Permanente",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                btnEliminar.Enabled = false;
                btnEliminar.Text = "⏳ Eliminando...";

                bool eliminado = await _mantenimientoService.EliminarMantenimientoAsync(_mantenimientoActual.Id);

                if (eliminado)
                {
                    MessageBox.Show("✅ Mantenimiento eliminado exitosamente del sistema.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("❌ No se pudo eliminar el mantenimiento. Intente nuevamente.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnEliminar.Enabled = true;
                    btnEliminar.Text = "🗑️ Eliminar Mantenimiento";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al eliminar el mantenimiento:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEliminar.Enabled = true;
                btnEliminar.Text = "🗑️ Eliminar Mantenimiento";
            }
        }
    }
}
