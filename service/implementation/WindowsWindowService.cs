using Microsoft.Win32;
using System.Windows;

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

        string? IWindowService.PickReadFile()
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

        string? IWindowService.PickSaveFile()
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Binary Files (*.bin)|*.bin"
            };
            if (sfd.ShowDialog() == true)
            {
                return sfd.FileName;
            }
            else
            {
                return null;
            }
        }

        void IWindowService.ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        bool IWindowService.Verify(string message)
        {
            return MessageBox.Show(message, "Verify", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
