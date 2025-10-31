using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EmpresarialesClienteCSharp.Utils;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class MainForm : Form
    {
        private Panel mainPanel;
        private Panel contentPanel;

        public MainForm()
        {
            InitializeComponent();
            ModernUI.MakeResponsive(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Concesionario AAA - Sistema de Gestión Empresarial";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ModernUI.Colors.Background;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.MinimumSize = new Size(1000, 700);

            // Crear MenuStrip moderno
            CreateModernMenuStrip();

            // Panel principal con scroll
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ModernUI.Colors.Background
            };

            // Panel de contenido
            contentPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1200, 1400),
                BackColor = Color.Transparent
            };

            CreateHeroSection();
            CreateContentSection();

            mainPanel.Controls.Add(contentPanel);
            this.Controls.Add(mainPanel);

            // Hacer responsive
            this.Resize += MainForm_Resize;
        }

        private void CreateModernMenuStrip()
        {
            var menuStrip = new MenuStrip
            {
                BackColor = Color.White,
                Font = ModernUI.Fonts.Body,
                Padding = new Padding(10, 5, 10, 5),
                Height = 45
            };

            menuStrip.Renderer = new ModernMenuStripRenderer();

            // Menú Archivo
            var archivoMenu = new ToolStripMenuItem("📁 Archivo");
            archivoMenu.DropDownItems.Add("Salir", null, (s, e) => Application.Exit());

            // Menú Vehículos
            var carrosMenu = new ToolStripMenuItem("🚗 Vehículos");
            carrosMenu.DropDownItems.Add("➕ Registrar Vehículo", null, AbrirFormCrear);
            carrosMenu.DropDownItems.Add(new ToolStripSeparator());
            carrosMenu.DropDownItems.Add("📋 Listar Todos", null, AbrirFormListar);
            carrosMenu.DropDownItems.Add("🔍 Buscar por Placa", null, AbrirFormBuscar);
            carrosMenu.DropDownItems.Add(new ToolStripSeparator());
            carrosMenu.DropDownItems.Add("✏️ Actualizar Registro", null, AbrirFormActualizar);
            carrosMenu.DropDownItems.Add("🗑️ Eliminar Registro", null, AbrirFormEliminar);

            // Menú Mantenimientos
            var mantenimientosMenu = new ToolStripMenuItem("🔧 Mantenimientos");
            mantenimientosMenu.DropDownItems.Add("➕ Registrar Mantenimiento", null, AbrirFormCrearMantenimiento);
            mantenimientosMenu.DropDownItems.Add(new ToolStripSeparator());
            mantenimientosMenu.DropDownItems.Add("📋 Listar Mantenimientos", null, AbrirFormListarMantenimientos);
            mantenimientosMenu.DropDownItems.Add("🔍 Buscar por ID", null, AbrirFormBuscarMantenimiento);
            mantenimientosMenu.DropDownItems.Add(new ToolStripSeparator());
            mantenimientosMenu.DropDownItems.Add("✏️ Actualizar Mantenimiento", null, AbrirFormActualizarMantenimiento);
            mantenimientosMenu.DropDownItems.Add("🗑️ Eliminar Mantenimiento", null, AbrirFormEliminarMantenimiento);

            // Menú Ayuda
            var ayudaMenu = new ToolStripMenuItem("❓ Ayuda");
            ayudaMenu.DropDownItems.Add("ℹ️ Acerca de...", null, MostrarAcercaDe);

            menuStrip.Items.AddRange(new[] { archivoMenu, carrosMenu, mantenimientosMenu, ayudaMenu });
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private void CreateHeroSection()
        {
            // Panel Hero con gradiente
            var heroPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(1200, 280),
                BackColor = ModernUI.Colors.Primary
            };

            heroPanel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    heroPanel.ClientRectangle,
                    ModernUI.Colors.Primary,
                    ModernUI.Colors.PrimaryDark,
                    45f))
                {
                    e.Graphics.FillRectangle(brush, heroPanel.ClientRectangle);
                }
            };

            // Badge de versión
            var badgePanel = new Panel
            {
                Location = new Point(480, 30),
                Size = new Size(240, 35),
                BackColor = Color.FromArgb(30, 255, 255, 255)
            };

            badgePanel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(badgePanel.ClientRectangle, 20))
                using (var brush = new SolidBrush(Color.FromArgb(30, 255, 255, 255)))
                {
                    e.Graphics.FillPath(brush, path);
                }
            };

            var lblBadge = new Label
            {
                Text = "Sistema Empresarial v2.0.1",
                Font = ModernUI.Fonts.Small,
                ForeColor = ModernUI.Colors.PrimaryLight,
                AutoSize = true,
                Location = new Point(35, 10),
                BackColor = Color.Transparent
            };

            badgePanel.Controls.Add(lblBadge);

            // Título principal
            var lblTitulo1 = new Label
            {
                Text = "Gestión Integral de",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(320, 80),
                BackColor = Color.Transparent
            };

            var lblTitulo2 = new Label
            {
                Text = "Vehículos Empresariales",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = ModernUI.Colors.PrimaryLight,
                AutoSize = true,
                Location = new Point(220, 130),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = "Plataforma empresarial para la administración completa del inventario automotriz",
                Font = ModernUI.Fonts.Body,
                ForeColor = Color.FromArgb(191, 219, 254),
                AutoSize = false,
                Size = new Size(800, 50),
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(200, 190),
                BackColor = Color.Transparent
            };

            heroPanel.Controls.AddRange(new Control[] { badgePanel, lblTitulo1, lblTitulo2, lblSubtitulo });
            contentPanel.Controls.Add(heroPanel);
        }

        private void CreateContentSection()
        {
            // Título de sección
            var lblSeccion = ModernUI.CreateLabel(
                "Módulos del Sistema",
                ModernUI.Fonts.Heading1,
                ModernUI.Colors.Gray900
            );
            lblSeccion.Location = new Point(80, 320);
            contentPanel.Controls.Add(lblSeccion);

            // Grid de tarjetas - 3 columnas responsive
            int cardWidth = 340;
            int cardHeight = 180;
            int gap = 30;
            int startX = 80;
            int startY = 380;

            // Fila 1 - Vehículos
            contentPanel.Controls.Add(CreateModernCard(
                "🚗 Gestión de Vehículos",
                "CRUD completo para administrar el inventario de automóviles",
                startX, startY, cardWidth, cardHeight,
                ModernUI.Colors.Primary,
                (s, e) => AbrirFormListar(s, e)
            ));

            contentPanel.Controls.Add(CreateModernCard(
                "➕ Registrar Vehículo",
                "Agregar nuevos vehículos al sistema con validaciones",
                startX + cardWidth + gap, startY, cardWidth, cardHeight,
                ModernUI.Colors.Success,
                (s, e) => AbrirFormCrear(s, e)
            ));

            contentPanel.Controls.Add(CreateModernCard(
                "🔍 Buscar Vehículo",
                "Buscar vehículos por placa y criterios específicos",
                startX + (cardWidth + gap) * 2, startY, cardWidth, cardHeight,
                ModernUI.Colors.Info,
                (s, e) => AbrirFormBuscar(s, e)
            ));

            // Fila 2 - Operaciones
            startY += cardHeight + gap;

            contentPanel.Controls.Add(CreateModernCard(
                "✏️ Actualizar Vehículo",
                "Modificar información de vehículos existentes",
                startX, startY, cardWidth, cardHeight,
                ModernUI.Colors.Warning,
                (s, e) => AbrirFormActualizar(s, e)
            ));

            contentPanel.Controls.Add(CreateModernCard(
                "🗑️ Eliminar Vehículo",
                "Eliminar registros de vehículos del sistema",
                startX + cardWidth + gap, startY, cardWidth, cardHeight,
                ModernUI.Colors.Danger,
                (s, e) => AbrirFormEliminar(s, e)
            ));

            contentPanel.Controls.Add(CreateModernCard(
                "🔧 Mantenimientos",
                "Gestión completa de mantenimientos vehiculares",
                startX + (cardWidth + gap) * 2, startY, cardWidth, cardHeight,
                Color.FromArgb(99, 102, 241), // Indigo
                (s, e) => AbrirFormListarMantenimientos(s, e)
            ));

            // Fila 3 - Mantenimientos
            startY += cardHeight + gap;

            contentPanel.Controls.Add(CreateModernCard(
                "➕ Registrar Mantenimiento",
                "Agregar nuevos registros de mantenimiento",
                startX, startY, cardWidth, cardHeight,
                Color.FromArgb(16, 185, 129), // Emerald
                (s, e) => AbrirFormCrearMantenimiento(s, e)
            ));

            contentPanel.Controls.Add(CreateModernCard(
                "🔍 Buscar Mantenimiento",
                "Buscar mantenimientos por ID único",
                startX + cardWidth + gap, startY, cardWidth, cardHeight,
                Color.FromArgb(139, 92, 246), // Violet
                (s, e) => AbrirFormBuscarMantenimiento(s, e)
            ));

            contentPanel.Controls.Add(CreateModernCard(
                "📊 Reportes",
                "Visualizar estadísticas y reportes del sistema",
                startX + (cardWidth + gap) * 2, startY, cardWidth, cardHeight,
                Color.FromArgb(236, 72, 153), // Pink
                (s, e) => MessageBox.Show("Módulo en desarrollo", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ));

            // Footer
            startY += cardHeight + 50;
            var lblFooter = new Label
            {
                Text = "© 2025 Universidad de Ibagué - Desarrollo de Aplicaciones Empresariales\n" +
                       "Juan David Reyes • Julio David Suarez • Sebastian Felipe Solano",
                Font = ModernUI.Fonts.Small,
                ForeColor = ModernUI.Colors.Gray500,
                AutoSize = false,
                Size = new Size(1100, 60),
                TextAlign = ContentAlignment.TopCenter,
                Location = new Point(50, startY),
                BackColor = Color.Transparent
            };
            contentPanel.Controls.Add(lblFooter);
        }

        private Panel CreateModernCard(string title, string description, int x, int y, int width, int height, Color accentColor, EventHandler clickHandler)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(4, 4, width - 8, height - 8), 16))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo del card
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, width - 1, height - 1), 16))
                using (var brush = new SolidBrush(card.BackColor))
                using (var pen = new Pen(ModernUI.Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Barra de color superior
            var colorBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(width, 6),
                BackColor = accentColor
            };

            colorBar.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(accentColor))
                {
                    var rect = new Rectangle(0, 0, width, 6);
                    e.Graphics.FillRectangle(brush, rect);

                    // Bordes redondeados superiores
                    using (var path = new GraphicsPath())
                    {
                        path.AddArc(0, 0, 16, 16, 180, 90);
                        path.AddArc(width - 16, 0, 16, 16, 270, 90);
                        path.AddLine(width, 6, 0, 6);
                        path.CloseFigure();
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            // Ícono circular
            var iconPanel = new Panel
            {
                Location = new Point(25, 35),
                Size = new Size(50, 50),
                BackColor = Color.FromArgb(20, accentColor.R, accentColor.G, accentColor.B)
            };

            iconPanel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(new SolidBrush(iconPanel.BackColor), iconPanel.ClientRectangle);
            };

            // Título
            var lblTitle = new Label
            {
                Text = title,
                Font = ModernUI.Fonts.Heading3,
                ForeColor = ModernUI.Colors.Gray900,
                AutoSize = true,
                Location = new Point(25, 100),
                BackColor = Color.Transparent
            };

            // Descripción
            var lblDescription = new Label
            {
                Text = description,
                Font = ModernUI.Fonts.Body,
                ForeColor = ModernUI.Colors.Gray600,
                AutoSize = false,
                Size = new Size(width - 50, 50),
                Location = new Point(25, 130),
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { colorBar, iconPanel, lblTitle, lblDescription });

            // Efectos de hover
            card.MouseEnter += (s, e) => {
                card.BackColor = ModernUI.Colors.Gray50;
            };
            card.MouseLeave += (s, e) => {
                card.BackColor = Color.White;
            };

            // Click handler recursivo
            AsignarClickRecursivo(card, clickHandler);

            return card;
        }

        private void AsignarClickRecursivo(Control control, EventHandler handler)
        {
            control.Click += handler;
            foreach (Control child in control.Controls)
            {
                AsignarClickRecursivo(child, handler);
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

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (contentPanel != null)
            {
                contentPanel.Width = Math.Max(this.ClientSize.Width, 1200);
                contentPanel.Refresh();
            }
        }

        #region Event Handlers - Carros
        private void AbrirFormCrear(object? sender, EventArgs e)
        {
            var formCrear = new CrearCarroForm();
            formCrear.ShowDialog();
        }

        private void AbrirFormListar(object? sender, EventArgs e)
        {
            var formListar = new ListarCarrosForm();
            formListar.ShowDialog();
        }

        private void AbrirFormBuscar(object? sender, EventArgs e)
        {
            var formBuscar = new BuscarCarroForm();
            formBuscar.ShowDialog();
        }

        private void AbrirFormActualizar(object? sender, EventArgs e)
        {
            var formActualizar = new ActualizarCarroForm();
            formActualizar.ShowDialog();
        }

        private void AbrirFormEliminar(object? sender, EventArgs e)
        {
            var formEliminar = new EliminarCarroForm();
            formEliminar.ShowDialog();
        }
        #endregion

        #region Event Handlers - Mantenimientos
        private void AbrirFormCrearMantenimiento(object? sender, EventArgs e)
        {
            var formCrear = new CrearMantenimientoForm();
            formCrear.ShowDialog();
        }

        private void AbrirFormListarMantenimientos(object? sender, EventArgs e)
        {
            var formListar = new ListarMantenimientosForm();
            formListar.ShowDialog();
        }

        private void AbrirFormBuscarMantenimiento(object? sender, EventArgs e)
        {
            var formBuscar = new BuscarMantenimientoForm();
            formBuscar.ShowDialog();
        }

        private void AbrirFormActualizarMantenimiento(object? sender, EventArgs e)
        {
            var formActualizar = new ActualizarMantenimientoForm();
            formActualizar.ShowDialog();
        }

        private void AbrirFormEliminarMantenimiento(object? sender, EventArgs e)
        {
            var formEliminar = new EliminarMantenimientoForm();
            formEliminar.ShowDialog();
        }
        #endregion

        private void MostrarAcercaDe(object? sender, EventArgs e)
        {
            var mensaje = "🚗 Sistema de Gestión de Concesionario AAA\n\n" +
                         "Versión: 2.0.1\n" +
                         "Arquitectura: Cliente-Servidor REST\n\n" +
                         "Desarrollado por:\n" +
                         "• Juan David Reyes\n" +
                         "• Julio David Suarez\n" +
                         "• Sebastian Felipe Solano\n\n" +
                         "Universidad de Ibagué\n" +
                         "Facultad de Ingeniería\n" +
                         "Desarrollo de Aplicaciones Empresariales\n" +
                         "2025-A";

            MessageBox.Show(mensaje, "Acerca de - Concesionario AAA",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Renderer personalizado para el MenuStrip
    public class ModernMenuStripRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuStripRenderer() : base(new ModernColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                using (var brush = new SolidBrush(ModernUI.Colors.Gray100))
                {
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }
    }

    public class ModernColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => ModernUI.Colors.Gray100;
        public override Color MenuItemSelectedGradientBegin => ModernUI.Colors.Gray100;
        public override Color MenuItemSelectedGradientEnd => ModernUI.Colors.Gray100;
        public override Color MenuBorder => ModernUI.Colors.Border;
        public override Color MenuItemBorder => ModernUI.Colors.Border;
    }
}
