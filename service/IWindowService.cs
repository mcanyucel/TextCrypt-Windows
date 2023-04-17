using System;

namespace TextCrypt.service
{
    internal interface IWindowService
    {
        internal String? PickTextFile();
        internal String? AskForPassword();
    }
}
