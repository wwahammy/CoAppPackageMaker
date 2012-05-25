using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoApp.Autopackage;
using CoApp.Packaging.Client;
using System.Security.Policy;

namespace CoAppPackageMaker.ViewModels
{
    class PackageViewModel : ExtraPropertiesViewModelBase
    {
        private string _name;
        private string _version;
        private string _architecture;
        private string _displayName;
        private Url _location;
        private Url _feed;
        private string _publisher;

        public PackageViewModel()
        {           
            PackageSource packageSource = new PackageSource(new AutopackageMain());
            //how to set?
            Architecture=  packageSource.AllRules.GetRulesByName("package").GetPropertyValue("Architecture");
             
        }

        
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        public string Architecture
        {
            get { return _architecture; }
            set
            {
                _architecture = value;
                OnPropertyChanged("Architecture");
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public Url Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }
        }
        
        public Url Feed
        {
            get { return _feed; }
            set
            {
                _feed = value;
                OnPropertyChanged("Feed");
            }
        }

        public string Publisher
        {
            get { return _publisher; }
            set
            {
                _publisher = value;
                OnPropertyChanged("Publisher");
            }
        }

        //tips, visualisation in extra class

    }
}
