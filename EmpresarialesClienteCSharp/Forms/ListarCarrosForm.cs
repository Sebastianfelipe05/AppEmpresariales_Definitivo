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
            this.Text = "Inventario de Vehículos - Listar por Marca";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(1000, 700);

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
                Location = new Point(30, 260),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(lblResultadoInfo);

            // DataGridView
            dgvCarros = ModernUI.CreateModernDataGrid();
            dgvCarros.Location = new Point(30, 290);
            dgvCarros.Size = new Size(1110, 420);
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
                Text = "Explore y filtre el inventario completo de vehículos por marca",
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
                Size = new Size(1110, 120),
                BackColor = Color.White
            };

            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, 1106, 116), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, 1109, 119), 12))
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
                Text = "Filtrar por Marca",
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

            txtFiltroMarca = new TextBox
            {
                Location = new Point(15, 10),
                Width = 420,
                Height = 25,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = ModernUI.Colors.Gray900
            };

            txtFiltroMarca.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    AplicarFiltro();
                }
            };

            txtContainer.Controls.Add(txtFiltroMarca);
            panel.Controls.Add(txtContainer);

            // Botón Filtrar
            var btnFiltrar = ModernUI.CreatePrimaryButton("🔍 Filtrar", (s, e) => AplicarFiltro());
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
            var btnRefrescar = ModernUI.CreateButton("♻️ Recargar Datos", ModernUI.Colors.Success, Color.White, (s, e) => CargarCarros());
            btnRefrescar.Location = new Point(860, 50);
            btnRefrescar.Size = new Size(200, 45);
            btnRefrescar.Font = ModernUI.Fonts.Button;
            panel.Controls.Add(btnRefrescar);

            return panel;
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
            if (string.IsNullOrWhiteSpace(txtFiltroMarca.Text))
            {
                carrosFiltrados = new List<Carro>(todosLosCarros);
            }
            else
            {
                var marcaBuscada = txtFiltroMarca.Text.Trim().ToLower();
                carrosFiltrados = todosLosCarros
                    .Where(c => c.Marca?.ToLower().Contains(marcaBuscada) == true)
                    .ToList();
            }

            ActualizarDataGrid();

            if (carrosFiltrados.Count == 0)
            {
                lblResultadoInfo.Text = $"❌ No se encontraron vehículos de la marca: {txtFiltroMarca.Text}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Danger;
            }
            else
            {
                lblResultadoInfo.Text = $"✅ {carrosFiltrados.Count} vehículo(s) encontrado(s) - Marca: {txtFiltroMarca.Text}";
                lblResultadoInfo.ForeColor = ModernUI.Colors.Success;
            }
        }

        private void LimpiarFiltro()
        {
            txtFiltroMarca.Clear();
            carrosFiltrados = new List<Carro>(todosLosCarros);
            ActualizarDataGrid();

            lblResultadoInfo.Text = $"✅ {todosLosCarros.Count} vehículos en total (sin filtro)";
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
