using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TextCrypt.service;
using TextCrypt.service.implementation;
using TextCrypt.viewmodel;

namespace TextCrypt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public new static App Current => (App)Application.Current;

        public App()
        {
            ServiceProvider = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add service injection here
            services.AddTransient<MainViewModel>();
            services.AddSingleton<IWindowService, WindowsWindowService>();
            services.AddSingleton<IFileService, WindowsFileService>();
            services.AddSingleton<IEncryptionService, WindowsEncryptionService>();

            return services.BuildServiceProvider();

        }
    }
}
