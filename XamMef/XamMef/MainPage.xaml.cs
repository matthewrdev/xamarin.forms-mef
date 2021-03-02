using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace XamMef
{
    [Export(typeof(MainPage))]
    public partial class MainPage : ContentPage
    {
        private readonly Lazy<IMyService> myService;
        private readonly IUserDialogs userDialogs;

        [ImportingConstructor]
        public MainPage(Lazy<IMyService> myService, IUserDialogs userDialogs)
        {
            InitializeComponent();
            this.myService = myService;
            this.userDialogs = userDialogs;
        }
    }
}
