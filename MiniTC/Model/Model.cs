using System.Collections.Generic;
using System.IO;

namespace MiniTC.Model
{
    public interface IFileSystemModel
    {
        IEnumerable<string> GetDrives();
        IEnumerable<string> GetDirectories(string path);
        IEnumerable<string> GetFiles(string path);
        (bool success, string errorInfo) CopyFile(string sourcePath, string destinationPath);
        (bool success, string errorInfo) CopyDirectory(string sourcePath, string destinationPath);
    }

    public class Model : IFileSystemModel
    {
        private const string TEST_FILE_NAME = ".test";
        private const string ACCESS_DENIED_MESSAGE = "Zablokowano. Path: {0}"; // wiadomość o braku dostępu

        // funkcja która zwraca wszystkie dyski w kompie
        public IEnumerable<string> GetDrives() =>
            Directory.GetLogicalDrives().Where(drive => new DriveInfo(drive).IsReady);

        // funkcja która zwraca foldery w danej ścieżce
        public IEnumerable<string> GetDirectories(string path)
        {
            return SafeEnumerate(() => Directory.GetDirectories(path));
        }

        // funkcja która zwraca pliki w danej ścieżce
        public IEnumerable<string> GetFiles(string path)
        {
            return SafeEnumerate(() => Directory.GetFiles(path));
        }

        // bezpieczna funkcja do pobierania plików/folderów - jak coś się nie uda to nie crashuje
        private IEnumerable<string> SafeEnumerate(Func<string[]> enumeration)
        {
            try
            {
                // próbuje pobrać pliki/foldery
                return enumeration()?.Where(item =>
                {
                    try { return true; }
                    catch (IOException) { return false; }
                }) ?? Enumerable.Empty<string>();
            }
            catch (IOException)
            {
                return Enumerable.Empty<string>();
            }
        }

        // funkcja do kopiowania pojedynczego pliku
        public (bool success, string errorInfo) CopyFile(string sourcePath, string destinationPath)
        {
            if (!ValidatePaths(sourcePath, destinationPath, out var errorMessage))
                return (false, errorMessage);

            try
            {
                // pobiera folder docelowy
                var destinationDir = Path.GetDirectoryName(destinationPath);
                if (string.IsNullOrEmpty(destinationDir))
                    return (false, "Nieprawidłowa ścieżka.");

                // sprawdza czy można pisać do folderu
                if (!VerifyWriteAccess(destinationDir))
                    return (false, $"Brak permisji do folderu: {destinationDir}");

                File.Copy(sourcePath, destinationPath, overwrite: true);
                return (true, string.Empty);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException)
            {
                return (false, FormatErrorMessage(ex, sourcePath, destinationPath));
            }
        }

        // funkcja do kopiowania całego folderu
        public (bool success, string errorInfo) CopyDirectory(string sourcePath, string destinationPath)
        {
            // sprawdza ścieżki
            if (!ValidatePaths(sourcePath, destinationPath, out var errorMessage))
                return (false, errorMessage);

            try
            {
                if (!Directory.Exists(sourcePath))
                    return (false, $"Nie znaleziono źródła: {sourcePath}");

                if (Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(destinationPath), StringComparison.OrdinalIgnoreCase))
                    return (false, "Nie można skopiować w to samo miejsce.");

                if (Path.GetFullPath(destinationPath).StartsWith(Path.GetFullPath(sourcePath) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                    return (false, "Nie można skopiować do podfolderu.");

                if (Directory.Exists(destinationPath))
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    destinationPath = Path.Combine(
                        Path.GetDirectoryName(destinationPath) ?? string.Empty,
                        $"{Path.GetFileName(destinationPath)}_{timestamp}");
                }


                // sprawdza uprawnienia do zapisu
                if (!VerifyWriteAccess(destinationPath))
                    return (false, $"Brak permisji: {destinationPath}");

                var sourceInfo = new DirectoryInfo(sourcePath);

                // kopiuje wszystkie pliki z folderu
                foreach (var file in sourceInfo.EnumerateFiles())
                {
                    try
                    {
                        var targetFilePath = Path.Combine(destinationPath, file.Name);
                        var copyResult = CopyFile(file.FullName, targetFilePath);
                        if (!copyResult.success)
                        {
                            return (false, $"Nie udało się skopiować {file.Name}: {copyResult.errorInfo}");
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Błąd przy kopiowaniu {file.Name}: {ex.Message}");
                    }
                }

                // kopiuje wszystkie podfoldery (rekurencyjnie)
                foreach (var dir in sourceInfo.EnumerateDirectories())
                {
                    try
                    {
                        var targetDirPath = Path.Combine(destinationPath, dir.Name);
                        var copyResult = CopyDirectory(dir.FullName, targetDirPath);
                        if (!copyResult.success)
                        {
                            return (false, $"Błąd przy kopiowaniu {dir.Name}: {copyResult.errorInfo}");
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Błąd przy kopiowaniu {dir.Name}: {ex.Message}");
                    }
                }

                return (true, string.Empty);
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Zablokowano dostęp przy kopiowaniu. Ścieżka: {(ex.Message.Contains(sourcePath) ? sourcePath : destinationPath)}");
            }
            catch (IOException ex)
            {
                return (false, $"Błąd IO przy kopiowaniu: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Nieprawidłowość: {ex.Message}");
            }
        }
        // formatuje wiadomość błędu żeby była czytelna
        private string FormatErrorMessage(Exception ex, string sourcePath, string destinationPath)
        {
            if (ex is UnauthorizedAccessException)
                return string.Format(ACCESS_DENIED_MESSAGE,
                    ex.Message.Contains(sourcePath) ? sourcePath : destinationPath);

            return $"Operation failed: {ex.Message}";
        }

        // sprawdza czy ścieżki źródła i celu są poprawne
        private bool ValidatePaths(string source, string destination, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(source))
            {
                errorMessage = "Nie podano źródła.";
                return false;
            }
            if (string.IsNullOrEmpty(destination))
            {
                errorMessage = "Nie podano destynacji pliku.";
                return false;
            }
            return true;
        }

        // sprawdza czy można pisać do folderu - tworzy testowy plik i go usuwa
        private bool VerifyWriteAccess(string path)
        {
            try
            {
                var testPath = Path.Combine(path, TEST_FILE_NAME);
                using (File.Create(testPath)) { }
                File.Delete(testPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}