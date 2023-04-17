using Microsoft.Win32;


namespace TextCrypt.service.implementation
{
    internal class WindowsWindowService : IWindowService
    {
        string? IWindowService.AskForPassword()
        {
            var pwd = new PasswordDialog();
            if (pwd.ShowDialog() == true)
            {
                return pwd.Password;
            }
            else
            {
                return null;
            }
        }

        string? IWindowService.PickTextFile()
        {
            var opf = new OpenFileDialog
            {
                Filter = "Binary Files (*.bin)|*.bin"
            };
            if (opf.ShowDialog() == true)
            {
                return opf.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}
