using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Erwine.Leonard.T.SsmlNotePad.Common
{
    public static class FileUtility
    {
        public static bool GetLocalPath(string pathOrUri, out string localPath)
        {
            if (String.IsNullOrEmpty(pathOrUri))
            {
                localPath = pathOrUri;
                return false;
            }

            Uri uri;

            if (Uri.TryCreate(pathOrUri, UriKind.Absolute, out uri))
            {
                if (uri.Scheme == Uri.UriSchemeFile)
                    localPath = NormalizePath((String.IsNullOrWhiteSpace(uri.LocalPath)) ? pathOrUri : uri.LocalPath);
                else
                {
                    localPath = pathOrUri;
                    return false;
                }
            }
            else
                localPath = NormalizePath(pathOrUri);
            try
            {
                return true;
            }
            catch { return false; }
        }
        
        public static readonly Regex EncodedPathCharRegex = new Regex(@"_0x(?<value>[a-f\d]{4})_", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string NormalizePath(string source)
        {
            if (String.IsNullOrEmpty(source))
                return source;

            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            StringBuilder result = new StringBuilder();
            foreach (string p in source.Split(Path.PathSeparator))
            {
                if (result.Length > 0 && result[result.Length - 1] != Path.PathSeparator)
                    result.Append(Path.PathSeparator);
                string path = p.Trim().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                if (path.Length == 0)
                    continue;

                bool emitSeparator = false;
                if (path.Length > 1)
                {
                    if (path[0] == Path.DirectorySeparatorChar && path[1] == Path.DirectorySeparatorChar)
                        result.Append(Path.DirectorySeparatorChar);
                    else if (Char.IsLetter(path[0]) && path[1] == Path.VolumeSeparatorChar)
                    {
                        emitSeparator = true;
                        result.Append(path.Substring(0, 2));
                        path = path.Substring(2);
                    }
                }
                for (int i = 0; i < path.Length; i++)
                {
                    if (path[i] == Path.DirectorySeparatorChar)
                        emitSeparator = true;
                    else
                    {
                        if (emitSeparator)
                        {
                            result.Append(Path.DirectorySeparatorChar);
                            emitSeparator = false;
                        }
                        if (invalidFileNameChars.Any(c => c == path[i]) || (path.Length - i > 7 && EncodedPathCharRegex.IsMatch(path.Substring(i, 8))))
                            result.AppendFormat("_0x{x:4}_", (int)(path[i]));
                        else
                            result.Append(path[i]);
                    }
                }
            }

            return result.ToString();
        }

        public static string AsExistingDirectory(string path)
        {
            if (String.IsNullOrEmpty(path))
                return path;

            try
            {
                while (!Directory.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    if (String.IsNullOrEmpty(path))
                        break;
                }
                return path;
            }
            catch { return null; }
        }

        public static bool InvokeFileDialog(FileDialog fileDialog, Window owner, string fileName, string defaultPath)
        {
            string path;
            if (!String.IsNullOrEmpty(fileName))
            {
                if (Path.IsPathRooted(fileName))
                {
                    if (File.Exists(fileName))
                        path = Path.GetDirectoryName(fileName);
                    else
                        path = AsExistingDirectory(fileName);
                }
                else
                    path = null;
            }
            else
                path = null;
            if (String.IsNullOrEmpty(path))
            {
                path = AsExistingDirectory(defaultPath);
                if (String.IsNullOrEmpty(path))
                {
                    path = AsExistingDirectory(App.AppSettingsViewModel.LastBrowsedSubdirectory);
                    if (String.IsNullOrWhiteSpace(path))
                        path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
            }
            fileDialog.InitialDirectory = path;
            if (!String.IsNullOrWhiteSpace(fileName))
                fileDialog.FileName = Path.Combine(path, Path.GetFileName(fileName));
            bool? dialogResult = fileDialog.ShowDialog(owner ?? App.Current.MainWindow);
            if (dialogResult.HasValue && dialogResult.Value)
            {
                App.AppSettingsViewModel.LastBrowsedSubdirectory = AsExistingDirectory(fileDialog.FileName);
                return true;
            }

            return false;
        }

        public static bool InvokeSsmlFileDialog(FileDialog fileDialog, Window owner, string fileName)
        {
            fileDialog.DefaultExt = App.AppSettingsViewModel.SsmlFileExtension;
            fileDialog.Filter = String.Format("SSML Files (*{0})|*{0}|All Files (*.*)|*.*", App.AppSettingsViewModel.SsmlFileExtension);
            fileDialog.FilterIndex = 0;
            if (InvokeFileDialog(fileDialog, owner, fileName, App.AppSettingsViewModel.LastSsmlFilePath))
            {
                App.AppSettingsViewModel.LastSsmlFilePath = fileDialog.FileName;
                return true;
            }

            return false;
        }

        internal static bool InvokeWavFileDialog(FileDialog fileDialog, Window owner, string fileName)
        {
            fileDialog.DefaultExt = ".wav";
            fileDialog.Filter = "WAV Files (*.wav)|*.wav|All Files (*.*)|*.*";
            fileDialog.FilterIndex = 0;
            if (InvokeFileDialog(fileDialog, owner, fileName, App.AppSettingsViewModel.LastSavedWavPath))
            {
                App.AppSettingsViewModel.LastSavedWavPath = fileDialog.FileName;
                return true;
            }

            return false;
        }

        internal static bool InvokeAudioFileDialog(FileDialog fileDialog, Window owner, string fileName)
        {
            fileDialog.DefaultExt = ".wav";
            fileDialog.Filter = "All Audio Files (*.wav, *.mp3)|*.wav;*.mp3|WAV Files (*.wav)|*.wav|MP3 Files (*.mp3)|*.mp3|All Files (*.*)|*.*";
            fileDialog.FilterIndex = 0;
            if (InvokeFileDialog(fileDialog, owner, fileName, App.AppSettingsViewModel.LastAudioPath))
            {
                App.AppSettingsViewModel.LastAudioPath = fileDialog.FileName;
                return true;
            }

            return false;
        }

        internal static string EnsureValidExtension(string extension)
        {
            if (String.IsNullOrEmpty(extension))
                extension = " ";

            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            if (!(extension.Any(c => Char.IsWhiteSpace(c) || invalidFileNameChars.Contains(c)) || extension.Substring(1).Any(c => c == '.')))
                return (extension.StartsWith(".")) ? extension : "." + extension;

            StringBuilder sb = new StringBuilder(".");
            foreach (char c in (extension[0] == '.') ? extension.Skip(1) : extension)
            {
                if (Char.IsWhiteSpace(c) || invalidFileNameChars.Contains(c) || c == '.')
                    sb.AppendFormat("_0x{0:x}_", (int)c);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        internal static string ResolveFileUri(string relativeOrAbsoluteUriOrPath)
        {
            return ResolveFileUri(relativeOrAbsoluteUriOrPath, Path.GetDirectoryName((typeof(FileUtility)).Assembly.Location));
        }

        internal static string ResolveFileUri(string relativeOrAbsoluteUriOrPath, string baseUriPath)
        {
            if (relativeOrAbsoluteUriOrPath == null)
                throw new ArgumentNullException("relativeOrAbsoluteUriOrPath");

            if (baseUriPath == null)
                throw new ArgumentNullException("baseUriPath");

            string localPath;
            if (!GetLocalPath(relativeOrAbsoluteUriOrPath, out localPath))
            {
                try
                {
                    localPath = Path.GetFileName(NormalizePath(localPath));
                }
                catch
                {
                    throw new ArgumentException("Cannot resolve path", "relativeOrAbsoluteUriOrPath");
                }
            }

            if (Path.IsPathRooted(localPath) && Path.GetPathRoot(localPath).Length > 1)
                return Path.GetFullPath(localPath);
            
            return Path.GetFullPath(Path.Combine(baseUriPath, localPath));
        }
    }
}