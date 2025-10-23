using MiniTC.View;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniTC
{
    public partial class Okno : Form
    {
        private System.ComponentModel.IContainer components = null;
        private PanelTC leftPanel;
        private PanelTC rightPanel;
        private Button copyButton;

        private void InitializeComponent()
        {
            leftPanel = new PanelTC();
            rightPanel = new PanelTC();
            copyButton = new Button();
            SuspendLayout();


            leftPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            leftPanel.CurrentPath = "";
            leftPanel.Location = new Point(10, 10);
            leftPanel.Margin = new Padding(3, 2, 3, 2);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(350, 350);
            leftPanel.TabIndex = 0;
            leftPanel.Load += leftPanel_Load;

            rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            rightPanel.CurrentPath = "";
            rightPanel.Location = new Point(380, 10);
            rightPanel.Margin = new Padding(3, 2, 3, 2);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new Size(350, 350);
            rightPanel.TabIndex = 1;

            copyButton.Anchor = AnchorStyles.Bottom;
            copyButton.Location = new Point(334, 370);
            copyButton.Margin = new Padding(3, 2, 3, 2);
            copyButton.Name = "copyButton";
            copyButton.Size = new Size(120, 35);
            copyButton.TabIndex = 2;
            copyButton.Text = "Kopiuj";
            copyButton.BackColor = Color.FromArgb(70, 130, 180);
            copyButton.ForeColor = Color.White;
            copyButton.FlatStyle = FlatStyle.Flat;
            copyButton.FlatAppearance.BorderSize = 0;
            copyButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            copyButton.UseVisualStyleBackColor = false;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(750, 420);
            Controls.Add(leftPanel);
            Controls.Add(rightPanel);
            Controls.Add(copyButton);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(760, 450);
            Name = "Form";
            Text = "MiniTC - Menedżer Plików";
            BackColor = Color.FromArgb(240, 248, 255);
            Load += Form1_Load;
            ResumeLayout(false);
        }

        // zwalnia zasoby
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}