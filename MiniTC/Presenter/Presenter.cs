using MiniTC.Model;
using MiniTC.View;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MiniTC.Presenter
{
    public class Presenter
    {
        // pola - przechowują referencje do widoku i modelu
        private readonly Okno _view;
        private readonly IFileSystemModel _model;

        public Presenter(Okno view, IFileSystemModel model)
        {
            _view = view;
            _model = model;

            InitializeEvents(_view.LeftPanel);
            InitializeEvents(_view.RightPanel);

            _view.CopyButton.Click += (s, e) => HandleCopy();
        }

        // ustawia eventy dla panelu
        private void InitializeEvents(PanelTC panel)
        {

            panel.DrivesComboBox.DropDown += (s, e) => LoadDrives(panel);

            panel.DrivesComboBox.SelectedIndexChanged += (s, e) => ChangeDrive(panel);
 
            panel.FilesListBox.DoubleClick += (s, e) => ChangePath(panel);

            panel.FilesListBox.SelectedIndexChanged += (s, e) => ClearOppositeSelection(panel);
        }

        // ładuje dyski do comboboxa
        private void LoadDrives(PanelTC panel) => panel.Drives = _model.GetDrives();

        private void ChangeDrive(PanelTC panel)
        {
            if (panel.DrivesComboBox.SelectedItem is string drive)
            {
                panel.CurrentPath = drive;
                RefreshDirectoryContent(panel);
            }
        }

        // zmienia ścieżkę w panelu (wchodzi do folderu lub wraca)
        private void ChangePath(PanelTC panel)
        {
            if (panel.FilesListBox.SelectedItem is not string selected) return;

            if (selected == "..")
            {
                var parent = Directory.GetParent(panel.CurrentPath);
                if (parent != null) panel.CurrentPath = parent.FullName;
            }
            else if (IsDirectory(selected))
            {
                panel.CurrentPath = Path.Combine(panel.CurrentPath, TrimDirectoryPrefix(selected));
            }
            RefreshDirectoryContent(panel);
        }

        // odświeża zawartość folderu w panelu
        private void RefreshDirectoryContent(PanelTC panel)
        {
            var dirs = _model.GetDirectories(panel.CurrentPath)
                .Select(d => "<D>" + Path.GetFileName(d))
                .ToList();

            // jak nie jesteśmy w głównym folderze to dodaje opcję "cofnij"
            if (Directory.GetParent(panel.CurrentPath) != null)
                dirs.Insert(0, "..");

            var files = _model.GetFiles(panel.CurrentPath).Select(Path.GetFileName);
            panel.DirectoryContent = dirs.Concat(files);
        }

        // obsługuje kopiowanie plików/folderów
        private void HandleCopy()
        {
            var (sourcePanel, destinationPanel, itemName) = GetCopyContext();
            if (itemName == null)
            {
                MessageBox.Show("Wybierz plik lub folder do skopiowania", "Nie wybrano", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string sourcePath = Path.Combine(sourcePanel.CurrentPath, GetRawName(itemName));
            string destinationPath = Path.Combine(destinationPanel.CurrentPath, GetRawName(itemName));

            var result = IsDirectory(itemName)
                ? _model.CopyDirectory(sourcePath, destinationPath)
                : _model.CopyFile(sourcePath, destinationPath);

            if (!result.success)
            {
                MessageBox.Show(result.errorInfo, "Błąd przy kopiowaniu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                RefreshDirectoryContent(destinationPanel);
            }
        }

        // określa kontekst kopiowania - który panel jest źródłem a który celem
        private (PanelTC source, PanelTC destination, string? item) GetCopyContext()
        {
            if (_view.LeftPanel.FilesListBox.SelectedItem is string left && left != "..")
                return (_view.LeftPanel, _view.RightPanel, left);
            if (_view.RightPanel.FilesListBox.SelectedItem is string right && right != "..")
                return (_view.RightPanel, _view.LeftPanel, right);
            return (_view.LeftPanel, _view.RightPanel, null);
        }

        // sprawdza czy element to folder (ma znacznik <D>)
        private static bool IsDirectory(string item) => item.StartsWith("<D>");
        private static string TrimDirectoryPrefix(string item) => item.Substring(3);
        private static string GetRawName(string item) => IsDirectory(item) ? TrimDirectoryPrefix(item) : item;

        private void ClearOppositeSelection(PanelTC current)
        {
            if (current == _view.LeftPanel)
                _view.RightPanel.FilesListBox.ClearSelected();
            else
                _view.LeftPanel.FilesListBox.ClearSelected();
        }
    }
}