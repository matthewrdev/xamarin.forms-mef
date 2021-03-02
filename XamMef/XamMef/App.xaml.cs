using System;
using MFractor.IOC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamMef
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = Resolver.Resolve<MainPage>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
