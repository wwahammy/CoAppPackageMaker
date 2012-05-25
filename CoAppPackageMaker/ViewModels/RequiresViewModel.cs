using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoApp.Packaging.Client;
using System.Collections.ObjectModel;

namespace CoAppPackageMaker.ViewModels
{
    class RequiresViewModel : ExtraPropertiesViewModelBase
    {
        private ObservableCollection<Package> _requiredPackages;

        public ObservableCollection<Package> RequiredPackages
        {
            get { return _requiredPackages; }
            set
            {
                _requiredPackages = value;
                OnPropertyChanged("RequiredPackages");
            }
        }
    }
}
