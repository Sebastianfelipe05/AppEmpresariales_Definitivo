using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Models;
using EmpresarialesClienteCSharp.Services;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class ListarMantenimientosForm : Form
    {
        private readonly MantenimientoService _mantenimientoService;
        private DataGridView dgvMantenimientos = null!;
        private TextBox txtFiltroPlaca = null!;
        private Label lblResultadoInfo;
        private Label lblEstadisticas;
        private List<Mantenimiento> todosLosMantenimientos = new List<Mantenimiento>();
        private List<Mantenimiento> mantenimientosFiltrados = new List<Mantenimiento>();

        public ListarMantenimientosForm()
        {
            _mantenimientoService = new MantenimientoService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
            CargarMantenimientos();
        }

        private void InitializeComponent()
        {
            this.Text = "Lista de Mantenimientos - Filtrar por Placa";
            this.Size = new Size(1300, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(1100, 750);

            // Panel principal con scroll
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ModernUI.Colors.Background,
                Padding = new Padding(30)
            };

            // Header
            var headerPanel = CreateHeader();
            mainPanel.Controls.Add(headerPanel);

            // Filter Panel
            var filterPanel = CreateFilterPanel();
            filterPanel.Location = new Point(30, 120);
            mainPanel.Controls.Add(filterPanel);

            // Info Labels
            lblResultadoInfo = new Label
            {
                Text = "Cargando mantenimientos...",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = true,
                Location = new Point(30, 260),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblResultadoInfo);

            lblEstadisticas = new Label
            {
                Text = "Estadísticas: Calculando...",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Info,
                AutoSize = true,
                Location = new Point(30, 285),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblEstadisticas);

            // DataGridView
            dgvMantenimientos = ModernUI.CreateModernDataGrid();
            dgvMantenimientos.Location = new Point(30, 320);
            dgvMantenimientos.Size = new Size(1210, 450);
            dgvMantenimientos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvMantenimientos.CellFormatting += DgvMantenimientos_CellFormatting;
            mainPanel.Controls.Add(dgvMantenimientos);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1240, 100),
                BackColor = Color.Transparent
            };

            // Botón volver
            var btnVolver = CreateBackButton();
            btnVolver.Location = new Point(0, 10);
            header.Controls.Add(btnVolver);

            // Título
            var lblTitulo = new Label
            {
                Text = "🔧 Lista de Mantenimientos",
                Font = ModernUI.Fonts.Heading1,
                ForeColor = ModernUI.Colors.Gray900,
                AutoSize = true,
                Location = new Point(0, 50),
                BackColor = Color.Transparent
            };
            header.Controls.Add(lblTitulo);

            // Subtítulo
            var lblSubtitulo = new Label
            {
                Text = "Vea y filtre todos los mantenimientos registrados por placa del vehículo",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = true,
                Location = new Point(0, 80),
                BackColor = Color.Transparent
            };
            header.Controls.Add(lblSubtitulo);

            return header;
        }

        private Button CreateBackButton()
        {
            var btn = new Button
            {
                Text = "← Volver",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Info,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 35),
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ModernUI.Colors.Gray100;

            btn.Click += (s, e) => this.Close();

            return btn;
        }

        private Panel CreateFilterPanel()
        {
            var panel = new Panel
            {
                Size = new Size(1210, 120),
                BackColor = Color.White
            };

            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 1206, 116), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 1209, 119), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Label
            var lblFiltro = new Label
            {
                Text = "Filtrar por Placa del Carro",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(25, 25),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblFiltro);

            // TextBox Container
            var txtContainer = new Panel
            {
                Location = new Point(25, 50),
                Size = new Size(450, 45),
                BackColor = Color.White
            };

            txtContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(txtContainer.ClientRectangle, 8))
                using (var pen = new Pen(ModernUI.Colors.Border, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            txtFiltroPlaca = new TextBox
            {
                Location = new Point(15, 10),
                Width = 420,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900,
                CharacterCasing = CharacterCasing.Upper,
                MaxLength = 10
            };

            txtFiltroPlaca.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    AplicarFiltro();
                }
            };

            txtContainer.Controls.Add(txtFiltroPlaca);
            panel.Controls.Add(txtContainer);

            // Botón Filtrar
            var btnFiltrar = ModernUI.CreateButton("🔍 Filtrar", ModernUI.Colors.Info, Color.White, (s, e) => AplicarFiltro());
            btnFiltrar.Location = new Point(500, 50);
            btnFiltrar.Size = new Size(150, 45);
            btnFiltrar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnFiltrar);

            // Botón Limpiar
            var btnLimpiar = ModernUI.CreateSecondaryButton("🔄 Limpiar Filtro", (s, e) => LimpiarFiltro());
            btnLimpiar.Location = new Point(670, 50);
            btnLimpiar.Size = new Size(170, 45);
            btnLimpiar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnLimpiar);

            // Botón Refrescar
            var btnRefrescar = ModernUI.CreateButton("♻️ Recargar Datos", ModernUI.Colors.Success, Color.White, (s, e) => CargarMantenimientos());
            btnRefrescar.Location = new Point(860, 50);
            btnRefrescar.Size = new Size(200, 45);
            btnRefrescar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnRefrescar);

            return panel;
        }

        private async void CargarMantenimientos()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lblResultadoInfo.Text = "⏳ Cargando mantenimientos...";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Info;

                todosLosMantenimientos = await _mantenimientoService.ObtenerTodosLosMantenimientosAsync();
                mantenimientosFiltrados = new List<Mantenimiento>(todosLosMantenimientos);

                ActualizarDataGrid();

                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"✅ {todosLosMantenimientos.Count} mantenimientos cargados correctamente";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Success;

                ActualizarEstadisticas();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"❌ Error al cargar: {ex.Message}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;

                MessageBox.Show($"Error al cargar mantenimientos:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void AplicarFiltro()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFiltroPlaca.Text))
                {
                    mantenimientosFiltrados = new List<Mantenimiento>(todosLosMantenimientos);
                    ActualizarDataGrid();
                    ActualizarEstadisticas();

                    lblResultadoInfo.Text = $"✅ {todosLosMantenimientos.Count} mantenimientos en total (sin filtro)";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
                    return;
                }

                this.Cursor = Cursors.WaitCursor;
                lblResultadoInfo.Text = "🔍 Filtrando...";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Info;

                // Filtrar por placa
                var placa = txtFiltroPlaca.Text.Trim().ToUpper();
                mantenimientosFiltrados = await _mantenimientoService.BuscarPorCarroAsync(placa);

                ActualizarDataGrid();

                this.Cursor = Cursors.Default;

                if (mantenimientosFiltrados.Count == 0)
                {
                    lblResultadoInfo.Text = $"❌ No se encontraron mantenimientos para la placa: {placa}";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;
                }
                else
                {
                    lblResultadoInfo.Text = $"✅ {mantenimientosFiltrados.Count} mantenimiento(s) encontrado(s) - Placa: {placa}";
                    lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
                }

                ActualizarEstadisticas();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"❌ Error al filtrar: {ex.Message}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;

                MessageBox.Show($"Error al filtrar mantenimientos:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFiltro()
        {
            txtFiltroPlaca.Clear();
            mantenimientosFiltrados = new List<Mantenimiento>(todosLosMantenimientos);
            ActualizarDataGrid();

            lblResultadoInfo.Text = $"✅ {todosLosMantenimientos.Count} mantenimientos en total (sin filtro)";
            lblResultadoInfo.ForeColor = ModernUI.Colors.Success;

            ActualizarEstadisticas();
            txtFiltroPlaca.Focus();
        }

        private void ActualizarDataGrid()
        {
            dgvMantenimientos.DataSource = null;
            dgvMantenimientos.DataSource = mantenimientosFiltrados;

            // Personalizar columnas
            if (dgvMantenimientos.Columns.Count > 0)
            {
                if (dgvMantenimientos.Columns.Contains("Id"))
                    dgvMantenimientos.Columns["Id"].HeaderText = "ID";

                if (dgvMantenimientos.Columns.Contains("PlacaCarro"))
                    dgvMantenimientos.Columns["PlacaCarro"].HeaderText = "Placa del Carro";

                if (dgvMantenimientos.Columns.Contains("TipoMantenimiento"))
                    dgvMantenimientos.Columns["TipoMantenimiento"].HeaderText = "Tipo";

                if (dgvMantenimientos.Columns.Contains("FechaMantenimiento"))
                {
                    dgvMantenimientos.Columns["FechaMantenimiento"].HeaderText = "Fecha";
                    dgvMantenimientos.Columns["FechaMantenimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                if (dgvMantenimientos.Columns.Contains("Kilometraje"))
                {
                    dgvMantenimientos.Columns["Kilometraje"].HeaderText = "Kilometraje (km)";
                    dgvMantenimientos.Columns["Kilometraje"].DefaultCellStyle.Format = "N0";
                }

                if (dgvMantenimientos.Columns.Contains("Costo"))
                {
                    dgvMantenimientos.Columns["Costo"].HeaderText = "Costo";
                    dgvMantenimientos.Columns["Costo"].DefaultCellStyle.Format = "C0";
                }

                if (dgvMantenimientos.Columns.Contains("Completado"))
                    dgvMantenimientos.Columns["Completado"].HeaderText = "Completado";

                if (dgvMantenimientos.Columns.Contains("Descripcion"))
                    dgvMantenimientos.Columns["Descripcion"].HeaderText = "Descripción";

                if (dgvMantenimientos.Columns.Contains("FechaRegistro"))
                {
                    dgvMantenimientos.Columns["FechaRegistro"].HeaderText = "Fecha Registro";
                    dgvMantenimientos.Columns["FechaRegistro"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }
        }

        private void ActualizarEstadisticas()
        {
            int total = mantenimientosFiltrados.Count;
            int completados = mantenimientosFiltrados.Count(m => m.Completado);
            int pendientes = total - completados;
            double costoTotal = mantenimientosFiltrados.Sum(m => m.Costo);

            lblEstadisticas.Text = $"📊 Total: {total} | ✅ Completados: {completados} | ⏳ Pendientes: {pendientes} | 💰 Costo Total: {costoTotal:C0}";
        }

        private void DgvMantenimientos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var mantenimiento = dgvMantenimientos.Rows[e.RowIndex].DataBoundItem as Mantenimiento;
            if (mantenimiento == null) return;

            // Colorear filas según estado de completado
            if (mantenimiento.Completado)
            {
                dgvMantenimientos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231); // Verde claro
                dgvMantenimientos.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = ModernUI.Colors.Success;
            }
            else
            {
                dgvMantenimientos.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195); // Amarillo claro
                dgvMantenimientos.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor = ModernUI.Colors.Warning;
            }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
