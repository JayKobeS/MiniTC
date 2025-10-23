using MiniTC.Model;
using MiniTC.Presenter;
using MiniTC.View;
using System;
using System.Windows.Forms;

namespace MiniTC
{
    public interface IMainView
    {
        PanelTC LeftPanel { get; }
        PanelTC RightPanel { get; }
        Button CopyButton { get; }
        event EventHandler CopyButtonClick;
    }

    public partial class Okno : Form, IMainView
    {

        public PanelTC LeftPanel => leftPanel;
        public PanelTC RightPanel => rightPanel;
        public Button CopyButton => copyButton;


        public event EventHandler CopyButtonClick
        {
            add => copyButton.Click += value;
            remove => copyButton.Click -= value;
        }

        public Okno()
        {
            InitializeComponent();
            SetupLayoutEvents();
        }

        private void SetupLayoutEvents()
        {
            this.Resize += (s, e) => AdjustLayout();
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            const int padding = 25;
            const int spacing = 15;
            int totalWidth = this.ClientSize.Width - 2 * padding - spacing;
            int panelWidth = totalWidth / 2;
            int panelHeight = this.ClientSize.Height - 2 * padding - copyButton.Height - spacing;

            // ustawia pozycjê i rozmiar lewego panelu
            leftPanel.Left = padding;
            leftPanel.Top = padding;
            leftPanel.Width = panelWidth;
            leftPanel.Height = panelHeight;

            rightPanel.Left = leftPanel.Right + spacing;
            rightPanel.Top = padding;
            rightPanel.Width = panelWidth;
            rightPanel.Height = panelHeight;

            copyButton.Left = (this.ClientSize.Width - copyButton.Width) / 2;
            copyButton.Top = this.ClientSize.Height - copyButton.Height - padding;
        }

        private void leftPanel_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}