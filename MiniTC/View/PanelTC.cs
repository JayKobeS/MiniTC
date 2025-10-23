using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MiniTC.View
{
    public interface IPanelView
    {
        string CurrentPath { get; set; }
        IEnumerable<string> Drives { set; }
        IEnumerable<string> DirectoryContent { set; }
        event EventHandler DriveSelected;
        event EventHandler PathSelected;
    }

    public partial class PanelTC : UserControl, IPanelView
    {
        private ComboBox drivesComboBox;
        private Label pathLabel;
        private ListBox filesListBox;
        private Label driveLabel;
        private Label pathTextLabel;

        public ComboBox DrivesComboBox => drivesComboBox;
        public Label PathLabel => pathLabel;
        public ListBox FilesListBox => filesListBox;

        public event EventHandler DriveSelected
        {
            add { DrivesComboBox.SelectedIndexChanged += value; }
            remove { DrivesComboBox.SelectedIndexChanged -= value; }
        }

        public event EventHandler PathSelected
        {
            add { FilesListBox.DoubleClick += value; }
            remove { FilesListBox.DoubleClick -= value; }
        }

        public string CurrentPath
        {
            get => pathLabel.Text;
            set => pathLabel.Text = value;
        }

        public IEnumerable<string> Drives
        {
            set
            {
                drivesComboBox.Items.Clear();
                drivesComboBox.Items.AddRange(new List<string>(value).ToArray());
            }
        }

        public IEnumerable<string> DirectoryContent
        {
            set
            {
                filesListBox.Items.Clear();
                foreach (var file in value)
                    filesListBox.Items.Add(file);
            }
        }

        public PanelTC()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            pathTextLabel = new Label();
            pathLabel = new Label();
            driveLabel = new Label();
            drivesComboBox = new ComboBox();
            filesListBox = new ListBox();
            SuspendLayout();


            pathTextLabel.Location = new Point(10, 10);
            pathTextLabel.Name = "pathTextLabel";
            pathTextLabel.Size = new Size(60, 20);
            pathTextLabel.TabIndex = 4;
            pathTextLabel.Text = "📂 Folder:";
            pathTextLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            pathTextLabel.ForeColor = Color.FromArgb(25, 25, 112);


            pathLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pathLabel.BackColor = Color.White;
            pathLabel.BorderStyle = BorderStyle.FixedSingle;
            pathLabel.Location = new Point(75, 8);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new Size(260, 25);
            pathLabel.TabIndex = 1;
            pathLabel.TextAlign = ContentAlignment.MiddleLeft;
            pathLabel.Font = new Font("Consolas", 8.5F);
            pathLabel.ForeColor = Color.FromArgb(25, 25, 112);

            driveLabel.Location = new Point(10, 45);
            driveLabel.Name = "driveLabel";
            driveLabel.Size = new Size(60, 20);
            driveLabel.TabIndex = 3;
            driveLabel.Text = "💾 Dysk:";
            driveLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            driveLabel.ForeColor = Color.FromArgb(25, 25, 112);

            drivesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            drivesComboBox.Location = new Point(75, 42);
            drivesComboBox.Name = "drivesComboBox";
            drivesComboBox.Size = new Size(80, 23);
            drivesComboBox.TabIndex = 0;
            drivesComboBox.BackColor = Color.FromArgb(230, 230, 250);
            drivesComboBox.Font = new Font("Segoe UI", 9F);

            filesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            filesListBox.ItemHeight = 18;
            filesListBox.Location = new Point(10, 75);
            filesListBox.Name = "filesListBox";
            filesListBox.Size = new Size(325, 260);
            filesListBox.TabIndex = 2;
            filesListBox.BackColor = Color.White;
            filesListBox.ForeColor = Color.FromArgb(25, 25, 112);
            filesListBox.BorderStyle = BorderStyle.FixedSingle;
            filesListBox.Font = new Font("Segoe UI", 9F);
            filesListBox.SelectionMode = SelectionMode.One;

            Controls.Add(pathTextLabel);
            Controls.Add(pathLabel);
            Controls.Add(driveLabel);
            Controls.Add(drivesComboBox);
            Controls.Add(filesListBox);

            Name = "PanelTC";
            BackColor = Color.FromArgb(248, 248, 255);
            BorderStyle = BorderStyle.FixedSingle;
            Padding = new Padding(5);
            Size = new Size(350, 350);

            ResumeLayout(false);
        }

        private void pathTextLabel_Click(object sender, EventArgs e)
        {

        }
    }
}