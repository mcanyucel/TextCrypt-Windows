using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TextCrypt.model;
using TextCrypt.service;

namespace TextCrypt.viewmodel
{
    internal class MainViewModel : ObservableObject
    {
        private readonly IWindowService windowService;
        private readonly IFileService fileService;
        private readonly IEncryptionService encryptionService;
        private bool isIdle = true;
        private string displayText = string.Empty;
        private string windowTitle = "TextCrypt";
        private bool canEdit = false;
        private ObservableCollection<RecentFileItem> recentFiles = new();
        private RecentFileItem? selectedRecentFileItem = null;


        public ICommand OpenExistingFileCommand { get; }
        public ICommand CreateNewFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand StarRecentFileCommand { get; }
        public ICommand OpenRecentFileCommand { get; }


        public string DisplayText { get => displayText; set => SetProperty(ref displayText, value); }
        public string WindowTitle { get => windowTitle; set => SetProperty(ref windowTitle, $"TextCrypt {value}"); }
        public bool IsIdle { get => isIdle; set { SetProperty(ref isIdle, value); NotifyCommands(OpenExistingFileCommand, StarRecentFileCommand); } }

        public bool CanEdit { get => canEdit; set { SetProperty(ref canEdit, value); NotifyCommands(SaveFileCommand); } }

        public RecentFileItem? SelectedRecentFileItem { get => selectedRecentFileItem; set { SetProperty(ref selectedRecentFileItem, value); _ = OpenRecentFile(value); } }

        public ObservableCollection<RecentFileItem> RecentFiles { get => recentFiles; set => SetProperty(ref recentFiles, value); }

        public MainViewModel(IWindowService windowServiceProxy, IFileService fileServiceProxy, IEncryptionService encryptionServiceProxy)
        {
            windowService = windowServiceProxy;
            fileService = fileServiceProxy;
            encryptionService = encryptionServiceProxy;
            OpenExistingFileCommand = new AsyncRelayCommand(OpenExistingFile, CanOpenExistingFileCommandExecute);
            CreateNewFileCommand = new RelayCommand(CreateNewFile, CreateNewFileCanExecute);
            SaveFileCommand = new AsyncRelayCommand(SaveFile, SaveFileCanExecute);
            StarRecentFileCommand = new AsyncRelayCommand<RecentFileItem>(StarFile, StarFileCanExecute);
            OpenRecentFileCommand = new AsyncRelayCommand<RecentFileItem>(OpenRecentFile, OpenRecentFileCanExecute);
            Task.Run(async () => RecentFiles = new ObservableCollection<RecentFileItem>(await fileService.GetRecentFilesAsync()));
        }

        private bool StarFileCanExecute(RecentFileItem? recentFileItem)
        {
            return isIdle;
        }

        private async Task StarFile(RecentFileItem? recentFileItem)
        {
            if (recentFileItem != null)
            {
                recentFileItem.IsStarred = !recentFileItem.IsStarred;
                _ = await fileService.SaveRecentFilesAsync(RecentFiles.ToList());
            }
        }

        private async Task OpenRecentFile(RecentFileItem? item)
        {
            if (item == null || item.FilePath == null) return;

            IsIdle = false;

            var path = item.FilePath;

            var encryptedBytes = await fileService.ReadFileAsync(path);
            if (encryptedBytes != null)
            {
                var password = windowService.AskForPassword();

                if (password != null)
                {
                    var decryptedText = await encryptionService.DecryptAsync(encryptedBytes, password);
                    if (decryptedText == null)
                    {
                        windowService.ShowError("Invalid password or corrupted file.");
                        SelectedRecentFileItem = null;
                    }
                    else
                    {
                        canEdit = true;
                        WindowTitle = path;
                        DisplayText = decryptedText;
                    }
                }
                else
                {
                    SelectedRecentFileItem = null;
                }

            }
            IsIdle = true;
        }

        private bool OpenRecentFileCanExecute(RecentFileItem? item)
        {
            return IsIdle;
        }

        private void CreateNewFile()
        {
            DisplayText = string.Empty;
            WindowTitle = "Untitled";
            CanEdit = true;
            SelectedRecentFileItem = null;
        }

        private bool CreateNewFileCanExecute()
        {
            return isIdle;
        }

        private async Task SaveFile()
        {
            IsIdle = false;
            var password = windowService.AskForPassword();
            if (password != null)
            {
                var encryptedBytes = await encryptionService.EncryptAsync(DisplayText, password);
                if (encryptedBytes != null)
                {
                    var path = windowService.PickSaveFile();
                    if (path != null)
                    {
                        await fileService.WriteFileAsync(path, encryptedBytes);
                        WindowTitle = path;
                    }
                }
            }
            IsIdle = true;
        }

        private bool SaveFileCanExecute()
        {
            return isIdle && CanEdit;
        }

        private async Task OpenExistingFile()
        {
            IsIdle = false;
            var path = windowService.PickReadFile();
            if (path != null)
            {
                var encryptedBytes = await fileService.ReadFileAsync(path);
                if (encryptedBytes != null)
                {
                    var password = windowService.AskForPassword();

                    if (password != null)
                    {
                        var decryptedText = await encryptionService.DecryptAsync(encryptedBytes, password);
                        if (decryptedText == null)
                        {
                            windowService.ShowError("Invalid password or corrupted file.");
                        }
                        else
                        {
                            canEdit = true;
                            WindowTitle = path;
                            DisplayText = decryptedText;
                            AddToRecentFiles(path);
                        }
                    }

                }

            }
            IsIdle = true;
        }

        private void AddToRecentFiles(string filePath)
        {
            if (!RecentFiles.Any(f => f.FilePath == filePath))
            {
                RecentFiles.Add(new RecentFileItem(fileService.GetFileName(filePath), filePath, DateTime.Now, false));
                fileService.SaveRecentFilesAsync(recentFiles.ToList());
            }
        }

        private bool CanOpenExistingFileCommandExecute()
        {
            return isIdle;
        }

        private static void NotifyCommands(params ICommand[] commands)
        {
            foreach (ICommand command in commands)
            {
                ((IRelayCommand)command).NotifyCanExecuteChanged();
            }
        }
    }
}
