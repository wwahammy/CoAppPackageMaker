using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoAppPackageMaker.ViewModels
{


    class PackageComposition : ExtraPropertiesViewModelBase
    {

     private string _symlinks;
     private string _fileCopy;

        public string Symlinks
        {
            get { return _symlinks; }
            set
            {
                _symlinks = value;
                OnPropertyChanged("Symlinks");
            }
        }

        public string FileCopy
        {
            get { return _fileCopy; }
            set
            {
               _fileCopy = value;
                OnPropertyChanged("FileCopy");
            }
        }
    }
}
