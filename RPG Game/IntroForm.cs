using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RPG_Game
{
    public partial class IntroForm : Form
    {
        private Form1 gameForm;

        // Declare controls as member variables
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnStart;
        // Add PictureBox for background GIF
        private PictureBox pbBackground;
        // Add label for "Press any key to start" message
        private Label lblPressAnyKey;

        public IntroForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            // Subscribe to Layout event for initial positioning with checks - Reverted to OnResize for simplicity after layout fixes
            // this.Layout += IntroForm_Layout;

            // Initialize PictureBox for background GIF
            pbBackground = new PictureBox()
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.StretchImage,
                // Removed: ImageLocation = "background.gif"
            };

            // Try loading the image directly
            try
            {
                // Load the image from the parent directory
                pbBackground.Image = Image.FromFile("../background.gif");
            }
            catch (Exception ex)
            {
                // Handle potential file loading errors (e.g., file not found, invalid format)
                MessageBox.Show($"Error loading background GIF: {ex.Message}", "Image Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Add PictureBox to controls first so it's behind other elements
            this.Controls.Add(pbBackground);

            // Create title label
            lblTitle = new Label
            {
                Text = "Stick Wars Juday",
                Font = new Font("Segoe UI", 48, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 100
            };

            // Create subtitle
            lblSubtitle = new Label
            {
                Text = "Choose Your Fighter",
                Font = new Font("Segoe UI", 24, FontStyle.Regular),
                ForeColor = Color.LightGray,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 50
            };

            // Create "Press any key to start" label
            lblPressAnyKey = new Label
            {
                Text = "PRESS ANY KEY TO CONTINUE",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 60,
                BackColor = Color.Transparent
            };

            // Create start button
            btnStart = new Button
            {
                Text = "START BATTLE",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 50),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(300, 60),
                Cursor = Cursors.Hand
            };
            btnStart.FlatAppearance.BorderColor = Color.White;
            btnStart.FlatAppearance.BorderSize = 2;
            btnStart.Click += BtnStart_Click;

            // Add hover effect for the button
            btnStart.MouseEnter += (s, e) => {
                btnStart.BackColor = Color.FromArgb(70, 70, 70);
                btnStart.ForeColor = Color.Yellow;
            };
            btnStart.MouseLeave += (s, e) => {
                btnStart.BackColor = Color.FromArgb(50, 50, 50);
                btnStart.ForeColor = Color.White;
            };

            // Add controls to form (order matters for z-index)
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(lblPressAnyKey);
            this.Controls.Add(btnStart);

            // Ensure the "Press any key to continue" label is on top
            lblPressAnyKey.BringToFront();

            // Add key press event to close form with Escape and hide message
            this.KeyPreview = true;
            this.KeyDown += (s, e) => {
                // Hide the "Press any key to start" message on any key press
                lblPressAnyKey.Hide();
                if (e.KeyCode == Keys.Escape)
                    Application.Exit();
            };
        }

        private void PositionControls()
        {
            // This check is redundant with the Layout handler, but kept for safety if called elsewhere
            if (!this.IsHandleCreated || lblTitle == null || lblSubtitle == null || btnStart == null || this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
            {
                return;
            }

            // Center title horizontally and position near the top
            lblTitle.Width = this.ClientSize.Width;
            lblTitle.Location = new Point(0, (int)(this.ClientSize.Height * 0.2));

            // Center subtitle horizontally and position below title
            lblSubtitle.Width = this.ClientSize.Width;
            lblSubtitle.Location = new Point(0, lblTitle.Bottom + 10);

            // Center "Press any key to start" label horizontally and position below subtitle
            lblPressAnyKey.Width = this.ClientSize.Width;
            lblPressAnyKey.Location = new Point(0, (int)(this.ClientSize.Height * 0.85));

            // Center button horizontally and position below subtitle
            btnStart.Location = new Point((this.ClientSize.Width - btnStart.Width) / 2, lblSubtitle.Bottom + 50);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // Hide the "Press any key to start" message if the button is clicked
            lblPressAnyKey.Hide();

            gameForm = new Form1();
            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
            this.Hide();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Removed custom background drawing
            // base.OnPaint(e);

            // // Draw a gradient background
            // using (LinearGradientBrush brush = new LinearGradientBrush(
            //     this.ClientRectangle,
            //     Color.FromArgb(20, 20, 40),
            //     Color.FromArgb(40, 20, 60),
            //     45F))
            // {
            //     e.Graphics.FillRectangle(brush, this.ClientRectangle);
            // }

            // Call base OnPaint to ensure controls are drawn
            base.OnPaint(e);
        }

        // Override OnResize to reposition controls when the form is resized
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState != FormWindowState.Minimized) // Avoid positioning when minimized
            {
                PositionControls();
            }
        }
    }
} 