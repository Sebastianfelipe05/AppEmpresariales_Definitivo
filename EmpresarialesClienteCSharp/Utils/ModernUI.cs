using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EmpresarialesClienteCSharp.Utils
{
    /// <summary>
    /// Clase de utilidades para crear interfaces modernas y responsive
    /// </summary>
    public static class ModernUI
    {
        #region Colores Modernos
        public static class Colors
        {
            // Paleta principal
            public static readonly Color Primary = Color.FromArgb(59, 130, 246);      // Blue-500
            public static readonly Color PrimaryDark = Color.FromArgb(37, 99, 235);   // Blue-600
            public static readonly Color PrimaryLight = Color.FromArgb(147, 197, 253);// Blue-300

            // Paleta secundaria
            public static readonly Color Success = Color.FromArgb(34, 197, 94);       // Green-500
            public static readonly Color Warning = Color.FromArgb(251, 146, 60);      // Orange-400
            public static readonly Color Danger = Color.FromArgb(239, 68, 68);        // Red-500
            public static readonly Color Info = Color.FromArgb(147, 51, 234);         // Purple-600

            // Grises
            public static readonly Color Gray50 = Color.FromArgb(249, 250, 251);
            public static readonly Color Gray100 = Color.FromArgb(243, 244, 246);
            public static readonly Color Gray200 = Color.FromArgb(229, 231, 235);
            public static readonly Color Gray300 = Color.FromArgb(209, 213, 219);
            public static readonly Color Gray400 = Color.FromArgb(156, 163, 175);
            public static readonly Color Gray500 = Color.FromArgb(107, 114, 128);
            public static readonly Color Gray600 = Color.FromArgb(75, 85, 99);
            public static readonly Color Gray700 = Color.FromArgb(55, 65, 81);
            public static readonly Color Gray800 = Color.FromArgb(31, 41, 55);
            public static readonly Color Gray900 = Color.FromArgb(17, 24, 39);

            // Fondos
            public static readonly Color Background = Gray50;
            public static readonly Color Surface = Color.White;
            public static readonly Color Border = Gray200;
        }
        #endregion

        #region Fuentes
        public static class Fonts
        {
            public static Font Title => new Font("Segoe UI", 24, FontStyle.Bold);
            public static Font Heading1 => new Font("Segoe UI", 20, FontStyle.Bold);
            public static Font Heading2 => new Font("Segoe UI", 16, FontStyle.Bold);
            public static Font Heading3 => new Font("Segoe UI", 14, FontStyle.Bold);
            public static Font Body => new Font("Segoe UI", 10, FontStyle.Regular);
            public static Font BodyBold => new Font("Segoe UI", 10, FontStyle.Bold);
            public static Font Small => new Font("Segoe UI", 9, FontStyle.Regular);
            public static Font Button => new Font("Segoe UI", 10, FontStyle.Bold);
        }
        #endregion

        #region Crear Botones Modernos
        public static Button CreateButton(string text, Color backgroundColor, Color textColor, EventHandler clickHandler = null)
        {
            var button = new Button
            {
                Text = text,
                BackColor = backgroundColor,
                ForeColor = textColor,
                FlatStyle = FlatStyle.Flat,
                Font = Fonts.Button,
                Cursor = Cursors.Hand,
                Height = 40,
                AutoSize = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backgroundColor, 0.1f);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backgroundColor, 0.1f);

            // Efecto de hover
            button.MouseEnter += (s, e) => {
                button.BackColor = ControlPaint.Light(backgroundColor, 0.1f);
                button.Cursor = Cursors.Hand;
            };
            button.MouseLeave += (s, e) => button.BackColor = backgroundColor;

            if (clickHandler != null)
                button.Click += clickHandler;

            // Pintar con bordes redondeados
            button.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(button.ClientRectangle, 8))
                using (var brush = new SolidBrush(button.BackColor))
                {
                    e.Graphics.FillPath(brush, path);

                    // Texto centrado
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(button.Text, button.Font, new SolidBrush(button.ForeColor),
                        button.ClientRectangle, sf);
                }
            };

            button.Region = new Region(GetRoundedRectangle(button.ClientRectangle, 8));

            return button;
        }

        public static Button CreatePrimaryButton(string text, EventHandler clickHandler = null)
            => CreateButton(text, Colors.Primary, Color.White, clickHandler);

        public static Button CreateSuccessButton(string text, EventHandler clickHandler = null)
            => CreateButton(text, Colors.Success, Color.White, clickHandler);

        public static Button CreateDangerButton(string text, EventHandler clickHandler = null)
            => CreateButton(text, Colors.Danger, Color.White, clickHandler);

        public static Button CreateSecondaryButton(string text, EventHandler clickHandler = null)
            => CreateButton(text, Colors.Gray200, Colors.Gray700, clickHandler);
        #endregion

        #region Crear Paneles Modernos
        public static Panel CreateCard(int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Colors.Surface,
                Padding = new Padding(20)
            };

            // Pintar con sombra y bordes redondeados
            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Sombra
                using (var shadowPath = GetRoundedRectangle(new Rectangle(2, 2, width - 4, height - 4), 12))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }

                // Fondo del panel
                using (var path = GetRoundedRectangle(new Rectangle(0, 0, width - 1, height - 1), 12))
                using (var brush = new SolidBrush(Colors.Surface))
                using (var pen = new Pen(Colors.Border, 1))
                {
                    e.Graphics.FillPath(brush, path);
                    e.Graphics.DrawPath(pen, path);
                }
            };

            return panel;
        }

        public static Panel CreateGradientPanel(int x, int y, int width, int height, Color color1, Color color2)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height)
            };

            panel.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    panel.ClientRectangle, color1, color2, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                }
            };

            return panel;
        }
        #endregion

        #region Crear TextBox Moderno
        public static TextBox CreateTextBox(int x, int y, int width, string placeholder = "")
        {
            var textBox = new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Height = 35,
                Font = Fonts.Body,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White
            };

            // Panel contenedor con borde
            var container = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                BackColor = Color.White,
                Padding = new Padding(10, 8, 10, 8)
            };

            container.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = GetRoundedRectangle(container.ClientRectangle, 8))
                using (var pen = new Pen(Colors.Border, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            textBox.Location = new Point(10, 8);
            textBox.Width = width - 20;
            container.Controls.Add(textBox);

            // Placeholder effect
            if (!string.IsNullOrEmpty(placeholder))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Colors.Gray400;

                textBox.Enter += (s, e) => {
                    if (textBox.Text == placeholder)
                    {
                        textBox.Text = "";
                        textBox.ForeColor = Colors.Gray900;
                    }
                };

                textBox.Leave += (s, e) => {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Text = placeholder;
                        textBox.ForeColor = Colors.Gray400;
                    }
                };
            }

            return textBox;
        }
        #endregion

        #region Métodos Helper
        private static GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
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

        public static void MakeResponsive(Form form)
        {
            form.SizeChanged += (s, e) => {
                // Ajustar controles al tamaño de la ventana
                foreach (Control control in form.Controls)
                {
                    if (control is Panel panel && panel.Dock == DockStyle.Fill)
                    {
                        panel.Refresh();
                    }
                }
            };

            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.MinimumSize = new Size(800, 600);
        }

        public static Label CreateLabel(string text, Font font, Color color)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                AutoSize = true,
                BackColor = Color.Transparent
            };
        }

        public static DataGridView CreateModernDataGrid()
        {
            var dgv = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                Font = Fonts.Body,
                GridColor = Colors.Gray200
            };

            // Estilo del header
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Colors.Gray800;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = Fonts.BodyBold;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(10);
            dgv.ColumnHeadersHeight = 45;

            // Estilo de las filas
            dgv.RowTemplate.Height = 40;
            dgv.DefaultCellStyle.SelectionBackColor = Colors.PrimaryLight;
            dgv.DefaultCellStyle.SelectionForeColor = Colors.Gray900;
            dgv.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);

            // Alternar colores de filas
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Colors.Gray50;

            return dgv;
        }
        #endregion
    }
}
