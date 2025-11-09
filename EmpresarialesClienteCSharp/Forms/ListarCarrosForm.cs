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
    public partial class ListarCarrosForm : Form
    {
        private readonly CarroService _carroService;
        private DataGridView dgvCarros = null!;
        private TextBox txtFiltroMarca = null!;
        private TextBox txtFiltroColor = null!;
        private TextBox txtFiltroModelo = null!;
        private Label lblResultadoInfo;
        private List<Carro> todosLosCarros = new List<Carro>();
        private List<Carro> carrosFiltrados = new List<Carro>();

        public ListarCarrosForm()
        {
            _carroService = new CarroService();
            InitializeComponent();
            ModernUI.MakeResponsive(this);
            CargarCarros();
        }

        private void InitializeComponent()
        {
            this.Text = "Inventario de Vehículos - Filtros Avanzados";
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

            // Info Label
            lblResultadoInfo = new Label
            {
                Text = "Cargando vehículos...",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = true,
                Location = new Point(30, 310),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblResultadoInfo);

            // DataGridView
            dgvCarros = ModernUI.CreateModernDataGrid();
            dgvCarros.Location = new Point(30, 340);
            dgvCarros.Size = new Size(1210, 420);
            dgvCarros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            mainPanel.Controls.Add(dgvCarros);

            this.Controls.Add(mainPanel);
        }

        private Panel CreateHeader()
        {
            var header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1140, 100),
                BackColor = Color.Transparent
            };

            // Botón volver
            var btnVolver = CreateBackButton();
            btnVolver.Location = new Point(0, 10);
            header.Controls.Add(btnVolver);

            // Título
            var lblTitulo = new Label
            {
                Text = "📋 Inventario de Vehículos",
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
                Text = "Explore y filtre el inventario completo de vehículos por marca, color o modelo",
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
                ForeColor = ModernUI.Colors.Primary,
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
                Size = new Size(1210, 170),
                BackColor = Color.White
            };

            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 1206, 166), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 1209, 169), 12))
                using (var brush = new SolidBrush(Color.White))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Header del Panel
            var lblHeader = new Label
            {
                Text = "🔍 Filtros de Búsqueda",
                Font = ModernUI.Fonts.BodyBold,
                ForeColor = ModernUI.Colors.Gray900,
                AutoSize = true,
                Location = new Point(25, 15),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblHeader);

            // Primera fila de filtros
            int yPos = 45;
            int xMargin = 25;

            // Filtro por Marca
            var lblMarca = new Label
            {
                Text = "Marca",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(xMargin, yPos),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblMarca);

            var txtContainerMarca = CreateTextBoxContainer(350, 40);
            txtContainerMarca.Location = new Point(xMargin, yPos + 20);
            txtFiltroMarca = CreateFilterTextBox(330);
            txtContainerMarca.Controls.Add(txtFiltroMarca);
            panel.Controls.Add(txtContainerMarca);

            // Filtro por Color
            var lblColor = new Label
            {
                Text = "Color",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(xMargin + 380, yPos),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblColor);

            var txtContainerColor = CreateTextBoxContainer(280, 40);
            txtContainerColor.Location = new Point(xMargin + 380, yPos + 20);
            txtFiltroColor = CreateFilterTextBox(260);
            txtContainerColor.Controls.Add(txtFiltroColor);
            panel.Controls.Add(txtContainerColor);

            // Filtro por Modelo
            var lblModelo = new Label
            {
                Text = "Modelo",
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray700,
                AutoSize = true,
                Location = new Point(xMargin + 690, yPos),
                BackColor = Color.Transparent
            };
            panel.Controls.Add(lblModelo);

            var txtContainerModelo = CreateTextBoxContainer(280, 40);
            txtContainerModelo.Location = new Point(xMargin + 690, yPos + 20);
            txtFiltroModelo = CreateFilterTextBox(260);
            txtContainerModelo.Controls.Add(txtFiltroModelo);
            panel.Controls.Add(txtContainerModelo);

            // Segunda fila - Botones de acción
            int yPosButtons = yPos + 70;

            // Botón Filtrar
            var btnFiltrar = ModernUI.CreatePrimaryButton("🔍 Aplicar Filtros", (s, e) => AplicarFiltro());
            btnFiltrar.Location = new Point(xMargin, yPosButtons);
            btnFiltrar.Size = new Size(200, 45);
            btnFiltrar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnFiltrar);

            // Botón Limpiar
            var btnLimpiar = ModernUI.CreateSecondaryButton("🔄 Limpiar Filtros", (s, e) => LimpiarFiltro());
            btnLimpiar.Location = new Point(xMargin + 220, yPosButtons);
            btnLimpiar.Size = new Size(200, 45);
            btnLimpiar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnLimpiar);

            // Botón Refrescar
            var btnRefrescar = ModernUI.CreateButton("♻️ Recargar Datos", ModernUI.Colors.Success, Color.White, (s, e) => CargarCarros());
            btnRefrescar.Location = new Point(xMargin + 440, yPosButtons);
            btnRefrescar.Size = new Size(220, 45);
            btnRefrescar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnRefrescar);

            return panel;
        }

        private Panel CreateTextBoxContainer(int width, int height)
        {
            var container = new Panel
            {
                Size = new Size(width, height),
                BackColor = Color.White
            };

            container.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(container.ClientRectangle, 8))
                using (var pen = new Pen(ModernUI.Colors.Border, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            return container;
        }

        private TextBox CreateFilterTextBox(int width)
        {
            var textBox = new TextBox
            {
                Location = new Point(10, 8),
                Width = width,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900
            };

            textBox.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    AplicarFiltro();
                }
            };

            return textBox;
        }

        private async void CargarCarros()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lblResultadoInfo.Text = "⏳ Cargando vehículos...";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Primary;

                todosLosCarros = await _carroService.ObtenerTodosLosCarrosAsync();
                carrosFiltrados = new List<Carro>(todosLosCarros);

                ActualizarDataGrid();

                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"✅ {todosLosCarros.Count} vehículos cargados correctamente";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                lblResultadoInfo.Text = $"❌ Error al cargar: {ex.Message}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;

                MessageBox.Show($"Error al cargar los vehículos:\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarFiltro()
        {
            var marcaBuscada = txtFiltroMarca.Text.Trim().ToLower();
            var colorBuscado = txtFiltroColor.Text.Trim().ToLower();
            var modeloBuscado = txtFiltroModelo.Text.Trim().ToLower();

            // Si no hay filtros aplicados, mostrar todos
            if (string.IsNullOrWhiteSpace(marcaBuscada) &&
                string.IsNullOrWhiteSpace(colorBuscado) &&
                string.IsNullOrWhiteSpace(modeloBuscado))
            {
                carrosFiltrados = new List<Carro>(todosLosCarros);
                lblResultadoInfo.Text = $"ℹ️ {todosLosCarros.Count} vehículos en total (sin filtros)";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Primary;
                ActualizarDataGrid();
                return;
            }

            // Aplicar filtros combinados (AND)
            carrosFiltrados = todosLosCarros.Where(c =>
            {
                bool cumpleMarca = string.IsNullOrWhiteSpace(marcaBuscada) ||
                                   (c.Marca?.ToLower().Contains(marcaBuscada) == true);

                bool cumpleColor = string.IsNullOrWhiteSpace(colorBuscado) ||
                                   (c.Color?.ToLower().Contains(colorBuscado) == true);

                bool cumpleModelo = string.IsNullOrWhiteSpace(modeloBuscado) ||
                                    (c.Modelo?.ToLower().Contains(modeloBuscado) == true);

                return cumpleMarca && cumpleColor && cumpleModelo;
            }).ToList();

            ActualizarDataGrid();

            // Construir mensaje de resultado
            var filtrosAplicados = new List<string>();
            if (!string.IsNullOrWhiteSpace(marcaBuscada)) filtrosAplicados.Add($"Marca: {txtFiltroMarca.Text}");
            if (!string.IsNullOrWhiteSpace(colorBuscado)) filtrosAplicados.Add($"Color: {txtFiltroColor.Text}");
            if (!string.IsNullOrWhiteSpace(modeloBuscado)) filtrosAplicados.Add($"Modelo: {txtFiltroModelo.Text}");

            if (carrosFiltrados.Count == 0)
            {
                lblResultadoInfo.Text = $"❌ No se encontraron vehículos con los filtros: {string.Join(", ", filtrosAplicados)}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;
            }
            else
            {
                lblResultadoInfo.Text = $"✅ {carrosFiltrados.Count} vehículo(s) encontrado(s) | Filtros: {string.Join(", ", filtrosAplicados)}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
            }
        }

        private void LimpiarFiltro()
        {
            txtFiltroMarca.Clear();
            txtFiltroColor.Clear();
            txtFiltroModelo.Clear();
            carrosFiltrados = new List<Carro>(todosLosCarros);
            ActualizarDataGrid();

            lblResultadoInfo.Text = $"✅ {todosLosCarros.Count} vehículos en total (filtros limpiados)";
            lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
            txtFiltroMarca.Focus();
        }

        private void ActualizarDataGrid()
        {
            dgvCarros.DataSource = null;
            dgvCarros.DataSource = carrosFiltrados;

            // Personalizar columnas
            if (dgvCarros.Columns.Count > 0)
            {
                dgvCarros.Columns["Placa"].HeaderText = "Placa";
                dgvCarros.Columns["Marca"].HeaderText = "Marca";
                dgvCarros.Columns["Modelo"].HeaderText = "Modelo";
                dgvCarros.Columns["Anio"].HeaderText = "Año";
                dgvCarros.Columns["Color"].HeaderText = "Color";
                dgvCarros.Columns["Precio"].HeaderText = "Precio";
                dgvCarros.Columns["Precio"].DefaultCellStyle.Format = "C0";
                dgvCarros.Columns["TipoTransmision"].HeaderText = "Transmisión";
                dgvCarros.Columns["NumeroPuertas"].HeaderText = "Puertas";
                dgvCarros.Columns["TieneAireAcondicionado"].HeaderText = "A/C";
                dgvCarros.Columns["Estado"].HeaderText = "Estado";
                dgvCarros.Columns["Combustible"].HeaderText = "Combustible";

                if (dgvCarros.Columns.Contains("FechaRegistro"))
                {
                    dgvCarros.Columns["FechaRegistro"].HeaderText = "Fecha Registro";
                    dgvCarros.Columns["FechaRegistro"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
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
