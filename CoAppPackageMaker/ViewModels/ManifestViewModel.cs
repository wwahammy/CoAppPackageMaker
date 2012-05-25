using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CoAppPackageMaker.ViewModels
{
    class ManifestViewModel : ExtraPropertiesViewModelBase
    {
        private ObservableCollection<string> _assembly;
        private ObservableCollection<string> _include;

        public ManifestViewModel(string name) { 
        
        }
        

        public ObservableCollection<string> Assembly
        {
            get { return _assembly; }
            set
            {
                _assembly = value;
                OnPropertyChanged("Assembly");
            }
        }

        public ObservableCollection<string> Include
        {
            get { return _include; }
            set
            {
                _include = value;
                OnPropertyChanged("Include");
            }
        }
    }
}
