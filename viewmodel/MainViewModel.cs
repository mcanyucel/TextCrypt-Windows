using CommunityToolkit.Mvvm.ComponentModel;
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

        public ICommand OpenExistingFileCommand { get; }
        public string DisplayText { get => displayText; set => SetProperty(ref displayText, value); }
        public string WindowTitle { get => windowTitle; set => SetProperty(ref windowTitle, $"TextCrypt {value}"); }
        public bool IsIdle { get => isIdle; set { SetProperty(ref isIdle, value); NotifyCommands(OpenExistingFileCommand); } }

        public MainViewModel(IWindowService windowServiceProxy, IFileService fileServiceProxy, IEncryptionService encryptionServiceProxy)
        {
            windowService = windowServiceProxy;
            fileService = fileServiceProxy;
            encryptionService = encryptionServiceProxy;
            OpenExistingFileCommand = new AsyncRelayCommand(OpenExistingFile, CanOpenExistingFileCommandExecute);
            
        }

        private async Task OpenExistingFile()
        {
            IsIdle = false;
            var path = windowService.PickTextFile();
            if (path != null)
            {
                var encryptedBytes = await fileService.ReadFileAsync(path);
                if (encryptedBytes != null)
                {
                    var password = windowService.AskForPassword();
                    
                    if (password != null)
                    {
                        WindowTitle = path;
                        var decryptedText = await encryptionService.DecryptAsync(encryptedBytes, password);
                        if (decryptedText == null)
                        {
                            DisplayText = "Invalid password or corrupted file.";
                        }
                        else
                        {
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
