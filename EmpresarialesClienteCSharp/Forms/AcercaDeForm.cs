using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmpresarialesClienteCSharp.Forms
{
    public partial class AcercaDeForm : Form
    {
        public AcercaDeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Acerca de - Sistema de Gestión de Vehículos";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(249, 250, 251);
            this.MinimumSize = new Size(900, 700);
            this.MaximumSize = new Size(900, 700);

            // Modern title bar with close button
            var titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(59, 130, 246) // Blue-500
            };
            titleBar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    titleBar.ClientRectangle,
                    Color.FromArgb(59, 130, 246),
                    Color.FromArgb(37, 99, 235),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, titleBar.ClientRectangle);
                }
            };

            var lblTitulo = new Label
            {
                Text = "🚗 Sistema de Gestión de Vehículos",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 15),
                AutoSize = true
            };

            var lblSubtitulo = new Label
            {
                Text = "Cliente C# - .NET 8.0 Windows Forms",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(191, 219, 254),
                Location = new Point(30, 48),
                AutoSize = true
            };

            var btnCerrar = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(840, 20),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 99, 235);
            btnCerrar.Click += (s, e) => this.Close();

            titleBar.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, btnCerrar });

            // Main scrollable panel
            var mainPanel = new Panel
            {
                Location = new Point(0, 80),
                Size = new Size(900, 620),
                BackColor = Color.FromArgb(249, 250, 251),
                AutoScroll = true
            };

            int yPos = 20;

            // ===== SECCIÓN: INFORMACIÓN DEL PROYECTO =====
            var panelProyecto = CreateCard("🎯 Sobre el Proyecto", 30, yPos, 840, 200);

            var lblObjetivo = new Label
            {
                Text = "Objetivo",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(20, 20),
                AutoSize = true
            };

            var lblObjetivoDesc = new Label
            {
                Text = "Desarrollar una aplicación cliente en C# .NET que consume servicios REST de un\n" +
                       "microservicio Spring Boot para gestionar información de carros y mantenimientos\n" +
                       "almacenados en Oracle Database usando JPA/Hibernate.",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(20, 48),
                Size = new Size(800, 60)
            };

            var lblCaracteristicas = new Label
            {
                Text = "✨ Operaciones CRUD Completas     🔍 Búsqueda Avanzada     📱 Interfaz Moderna\n" +
                       "🔄 Integración REST API             🎨 Una Función por Ventana",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(20, 120),
                Size = new Size(800, 50)
            };

            panelProyecto.Controls.AddRange(new Control[] { lblObjetivo, lblObjetivoDesc, lblCaracteristicas });
            yPos += 220;

            // ===== SECCIÓN: VERSIÓN Y TECNOLOGÍAS =====
            var panelVersion = CreateCard("📋 Información de Versión", 30, yPos, 840, 180);

            var lblVersionCliente = CreateInfoLabel("Versión del Cliente:", "v3.0.0", 20, 20);
            var lblFecha = CreateInfoLabel("Fecha de Lanzamiento:", "Octubre 2025", 20, 50);
            var lblTecnologiaFront = CreateInfoLabel("Tecnología Frontend:", "C# .NET 8.0 + Windows Forms", 20, 80);
            var lblTecnologiaBack = CreateInfoLabel("Tecnología Backend:", "Spring Boot 3.5.5 + Java 17", 20, 110);
            var lblBaseDatos = CreateInfoLabel("Base de Datos:", "Oracle Database 21c XE + JPA/Hibernate", 20, 140);

            panelVersion.Controls.AddRange(new Control[] {
                lblVersionCliente, lblFecha, lblTecnologiaFront, lblTecnologiaBack, lblBaseDatos
            });
            yPos += 200;

            // ===== SECCIÓN: ESPECIFICACIONES TÉCNICAS =====
            var panelTech = CreateCard("⚙️ Especificaciones Técnicas", 30, yPos, 840, 200);

            var lblFrontendTech = new Label
            {
                Text = "Frontend:\n• .NET 8.0\n• Windows Forms\n• ModernUI Components\n• HTTP Client\n• JSON Serialization",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(20, 20),
                Size = new Size(250, 150)
            };

            var lblBackendTech = new Label
            {
                Text = "Backend Integration:\n• RESTful API\n• JSON Data Exchange\n• Error Handling\n• HTTP Status Codes\n• CORS Enabled",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(300, 20),
                Size = new Size(250, 150)
            };

            var lblFeaturesTech = new Label
            {
                Text = "Características:\n• Validación de Formularios\n• Async/Await Pattern\n• Loading States\n• MessageBox Alerts\n• Custom UI Controls",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(580, 20),
                Size = new Size(240, 150)
            };

            panelTech.Controls.AddRange(new Control[] { lblFrontendTech, lblBackendTech, lblFeaturesTech });
            yPos += 220;

            // ===== SECCIÓN: INFORMACIÓN ACADÉMICA =====
            var panelUniversidad = CreateCard("🎓 Información Académica", 30, yPos, 840, 180);

            var lblUniversidadIcon = new Label
            {
                Text = "🏛️",
                Font = new Font("Segoe UI", 48, FontStyle.Regular),
                Location = new Point(20, 40),
                AutoSize = true
            };

            var lblUniversidad = new Label
            {
                Text = "Universidad de Ibagué\nFacultad de Ingeniería",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(120, 30),
                Size = new Size(700, 60)
            };

            var lblMateria = new Label
            {
                Text = "Materia: Desarrollo de Aplicaciones Empresariales\n" +
                       "Proyecto: Tercer Prototipo - Arquitectura Distribuida\n" +
                       "Fecha: Octubre 2025",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(120, 90),
                Size = new Size(700, 70)
            };

            panelUniversidad.Controls.AddRange(new Control[] { lblUniversidadIcon, lblUniversidad, lblMateria });
            yPos += 200;

            // ===== SECCIÓN: CONTACTO =====
            var panelContacto = CreateCard("📞 Contacto e Información", 30, yPos, 840, 130);

            var lblEmail = CreateInfoLabel("📧 Email Académico:", "carlos.lugo@unibague.edu.co", 20, 20);
            var lblRepo = CreateInfoLabel("🌐 Repositorio:", "GitHub - EmpresarialesProyecto", 20, 50);
            var lblAPI = CreateInfoLabel("🔗 API Base URL:", "http://localhost:8080/api", 20, 80);

            panelContacto.Controls.AddRange(new Control[] { lblEmail, lblRepo, lblAPI });
            yPos += 150;

            // Footer
            var lblFooter = new Label
            {
                Text = "© 2025 Universidad de Ibagué - Facultad de Ingeniería\n" +
                       "Sistema de Gestión de Vehículos - Proyecto Académico",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(30, yPos),
                Size = new Size(840, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            mainPanel.Controls.AddRange(new Control[] {
                panelProyecto, panelVersion, panelTech, panelUniversidad, panelContacto, lblFooter
            });

            this.Controls.AddRange(new Control[] { titleBar, mainPanel });
        }

        private Panel CreateCard(string title, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(panel.ClientRectangle, 12))
                {
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(229, 231, 235), 1), path);
                }
            };

            // Title for the card
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Separator line
            var separator = new Panel
            {
                Location = new Point(20, 50),
                Size = new Size(width - 40, 2),
                BackColor = Color.FromArgb(229, 231, 235)
            };

            panel.Controls.AddRange(new Control[] { lblTitle, separator });

            return panel;
        }

        private Label CreateInfoLabel(string caption, string value, int x, int y)
        {
            var label = new Label
            {
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(x, y),
                AutoSize = true
            };

            label.Text = $"{caption} ";

            // Add value part with different style
            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(x + TextRenderer.MeasureText(caption + " ", label.Font).Width, y),
                AutoSize = true
            };

            label.Parent = lblValue.Parent;

            return label;
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
    }
}
