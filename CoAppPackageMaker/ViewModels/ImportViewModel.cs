using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CoAppPackageMaker.ViewModels
{
   public class ImportViewModel : ExtraPropertiesViewModelBase
    {
        private ObservableCollection<string> _imports;

        public ObservableCollection<string> Imports
        {
            get { return _imports; }
            set { _imports = value;
            OnPropertyChanged("Imports");
            }
        }
    }
}
