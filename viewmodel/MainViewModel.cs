﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public ICommand OpenExistingFileCommand { get; }
        public ICommand CreateNewFileCommand { get; }
        public ICommand SaveFileCommand { get; }

        public string DisplayText { get => displayText; set => SetProperty(ref displayText, value); }
        public string WindowTitle { get => windowTitle; set => SetProperty(ref windowTitle, $"TextCrypt {value}"); }
        public bool IsIdle { get => isIdle; set { SetProperty(ref isIdle, value); NotifyCommands(OpenExistingFileCommand); } }

        public bool CanEdit { get => canEdit; set { SetProperty(ref canEdit, value); NotifyCommands(SaveFileCommand); } }

        public MainViewModel(IWindowService windowServiceProxy, IFileService fileServiceProxy, IEncryptionService encryptionServiceProxy)
        {
            windowService = windowServiceProxy;
            fileService = fileServiceProxy;
            encryptionService = encryptionServiceProxy;
            OpenExistingFileCommand = new AsyncRelayCommand(OpenExistingFile, CanOpenExistingFileCommandExecute);
            CreateNewFileCommand = new RelayCommand(CreateNewFile, CreateNewFileCanExecute);
            SaveFileCommand = new AsyncRelayCommand(SaveFile, SaveFileCanExecute);
            
        }

        private void CreateNewFile()
        {
            DisplayText = string.Empty;
            WindowTitle = "Untitled";
            CanEdit = true;
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
                        }
                    }
                    
                }
                
            }
            IsIdle = true;
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