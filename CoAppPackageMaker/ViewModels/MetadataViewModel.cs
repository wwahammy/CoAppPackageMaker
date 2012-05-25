using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Policy;

namespace CoAppPackageMaker.ViewModels
{
    class MetadataViewModel : ExtraPropertiesViewModelBase
    {
        private string _summary;
        private string _description;
        private string _authorVersion;
        private Url _bugTracker;
        private int _stability;
        private string _licenses;
       

        public string Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                OnPropertyChanged("Summary");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string AuthorVersion
        {
            get { return _authorVersion; }
            set
            {
                _authorVersion = value;
                OnPropertyChanged("AuthorVersion");
            }
        }

        public Url BugTracker
        {
            get { return _bugTracker; }
            set
            {
                _bugTracker = value;
                OnPropertyChanged("BugTracker");
            }
        }

        public int Stability
        {
            get { return _stability; }
            set
            {
                _stability = value;
                OnPropertyChanged("Stability");
            }
        }

        public string Licenses
        {
            get { return _licenses; }
            set
            {
                _licenses = value;
                OnPropertyChanged("Licenses");
            }
        }
      

    }
}
