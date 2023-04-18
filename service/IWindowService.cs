using System;

namespace TextCrypt.service
{
    internal interface IWindowService
    {
        internal String? PickReadFile();
        internal String? AskForPassword();

        internal String? PickSaveFile();

        internal void ShowError(String message);

        internal bool Verify(String message);
    }
}
