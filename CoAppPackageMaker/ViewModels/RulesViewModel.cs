using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CoApp.Autopackage;
using CoAppPackageMaker.Models;
using CoApp.Developer.Toolkit.Scripting.Languages.PropertySheet;

namespace CoAppPackageMaker.ViewModels
{

    public class RulesViewModel : ExtraPropertiesViewModelBase
        {
            private ObservableCollection<string> _ruleNames;
            
            public RulesViewModel()
            {
                _ruleNames = new ObservableCollection<string>();

                PackageSource packageSource = new PackageSource(new AutopackageMain());
                //expect to obtain form packageSource.AllRules - pakackage rule, metadata rule, provides rule ...
                foreach (Rule rule in  packageSource.AllRules)
                {
                    _ruleNames.Add(rule.Name);
                    
                }
            }

            public ObservableCollection<string> RulesNames
            {
                get { return _ruleNames; }
                set
                {
                    _ruleNames = value;
                    OnPropertyChanged("RulesNames");
                }
            }
            
        }
    }

