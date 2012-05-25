﻿//-----------------------------------------------------------------------
// <copyright company="CoApp Project">
//     Copyright (c) 2010-2012 Garrett Serack and CoApp Contributors. 
//     Contributors can be discovered using the 'git log' command.
//     All rights reserved.
// </copyright>
// <license>
//     The software is licensed under the Apache 2.0 License (the "License")
//     You may not use the software except in compliance with the License. 
// </license>
//-----------------------------------------------------------------------

namespace CoApp.Autopackage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Developer.Toolkit.Publishing;
    using Developer.Toolkit.Scripting.Languages.PropertySheet;
    using Packaging.Client;
    using Properties;
    using Toolkit.Collections;
    using Toolkit.Exceptions;
    using Toolkit.Extensions;
    using Toolkit.Tasks;
    using CoAppPackageMaker.Properties;

    internal class PackageSource
    {
        internal CertificateReference Certificate;
        internal string SigningCertPassword;
        internal string SigningCertPath = string.Empty;
        internal bool Remember;

        internal PackageManager PackageManager;
        // collection of propertysheets
        internal PropertySheet[] PropertySheets;

        // all the different sets of rules 
        internal Rule[] AllRules;
        internal Rule[] DefineRules;
        internal Rule[] ApplicationRules;
        internal Rule[] AssemblyRules;
        internal Rule[] AssembliesRules;
        internal Rule[] DeveloperLibraryRules;
        internal Rule[] SourceCodeRules;
        internal Rule[] ServiceRules;
        internal Rule[] WebApplicationRules;
        internal Rule[] DriverRules;
        internal Rule[] AllRoles;

        internal IEnumerable<Rule> PackageRules;
        internal IEnumerable<Rule> MetadataRules;
        internal IEnumerable<Rule> RequiresRules;
        internal IEnumerable<Rule> ProvidesRules;
        internal IEnumerable<Rule> CompatabilityPolicyRules;
        internal IEnumerable<Rule> ManifestRules;
        internal IEnumerable<Rule> PackageCompositionRules;
        internal IEnumerable<Rule> IdentityRules;
        internal IEnumerable<Rule> SigningRules;
        internal IEnumerable<Rule> FileRules;
        internal string SourceFile;

        private AutopackageMain _mainInstance;

        public PackageSource(AutopackageMain mainInstance)
        {
            _mainInstance = mainInstance;
        }

        internal void FindCertificate()
        {
            if (string.IsNullOrEmpty(SigningCertPath))
            {
                Certificate = CertificateReference.Default;
                if (Certificate == null)
                {
                    throw new ConsoleException("No default certificate stored in the registry");
                }
            }
            else if (string.IsNullOrEmpty(SigningCertPassword))
            {
                Certificate = new CertificateReference(SigningCertPath);
            }
            else
            {
                Certificate = new CertificateReference(SigningCertPath, SigningCertPassword);
            }

            Event<Verbose>.Raise("Loaded certificate with private key {0}", Certificate.Location);
            if (Remember)
            {
                Event<Verbose>.Raise("Storing certificate details in the registry.");
                Certificate.RememberPassword();
                CertificateReference.Default = Certificate;
            }
        }

        internal void LoadPackageSourceData(string autopackageSourceFile)
        {
            // ------ Load Information to create Package 

            FindCertificate();

            SourceFile = autopackageSourceFile;

            // load up all the specified property sheets
            LoadPropertySheets(SourceFile);

            // Determine the roles that are going into the MSI, and ensure we know the basic information for the package (ver, arch, etc)
            CollectRoleRules();
        }

        internal IDictionary<string, string> MacroValues = new XDictionary<string, string>();

        internal string PostprocessValue(string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Contains("[]"))
            {
                return value.Replace("[]", "");
            }
            return value;
        }

        internal string GetMacroValue(string valuename)
        {
            if (valuename == "DEFAULTLAMBDAVALUE")
            {
                return "${packagedir}\\${each.Path}";
            }

            string defaultValue = null;

            if (valuename.Contains("??"))
            {
                var prts = valuename.Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries);
                defaultValue = prts.Length > 1 ? prts[1].Trim() : string.Empty;
                valuename = prts[0];
            }

            var parts = valuename.Split('.');
            if (parts.Length > 0)
            {
                if (parts.Length == 3)
                {
                    var result = AllRules.GetRulesByName(parts[0]).GetRulesByParameter(parts[1]).GetPropertyValue(parts[2]);
                    if (result != null)
                    {
                        return result;
                    }
                }

                if (parts.Length == 2)
                {
                    var result = AllRules.GetRulesByName(parts[0]).GetPropertyValue(parts[1]);
                    if (result != null)
                    {
                        return result;
                    }
                }

                // still not found?
                if (parts[0].Equals("package", StringComparison.InvariantCultureIgnoreCase))
                {
                    var result = this.SimpleEval(valuename.Substring(8));
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                    {
                        return result.ToString();
                    }
                }
            }

            return DefineRules.GetPropertyValue(valuename) ?? (MacroValues.ContainsKey(valuename.ToLower()) ? MacroValues[valuename.ToLower()] : Environment.GetEnvironmentVariable(valuename)) ?? defaultValue;
        }

        internal IEnumerable<object> GetFileCollection(string collectionname)
        {
            // we use this to pick up file collections.
            var fileRule = FileRules.FirstOrDefault(each => each.Parameter == collectionname);

            if (fileRule == null)
            {
                var collection = GetMacroValue(collectionname);
                if (collection != null)
                {
                    return collection.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(each => each.Trim());
                }

                Event<Error>.Raise(MessageCode.UnknownFileList, null, "Reference to unknown file list '{0}'", collectionname);
            }
            else
            {
                var list = FileList.GetFileList(collectionname, FileRules);
                return list.FileEntries.Select(each => new
                {
                    Path = each.DestinationPath,
                    Name = Path.GetFileName(each.DestinationPath),
                    Extension = Path.GetExtension(each.DestinationPath),
                    NameWithoutExtension = Path.GetFileNameWithoutExtension(each.DestinationPath),
                });
            }

            return Enumerable.Empty<object>();
        }

        // added a temp string-NOT WORKING!!
        internal void LoadPropertySheets(string autopackageSourceFile)
        {
            //
            string temporary = "Resources.template_autopkg";
            var template = PropertySheet.Parse(temporary, "autopkg-template");

            if (!File.Exists(autopackageSourceFile.GetFullPath()))
            {
                throw new ConsoleException("Can not find autopackage file '{0}'", autopackageSourceFile.GetFullPath());
            }

            var result = PropertySheet.Load(autopackageSourceFile);
            result.GetCollection += GetFileCollection;
            result.GetMacroValue += GetMacroValue;
            result.PostprocessProperty += PostprocessValue;

            PropertySheets = new[] { result, template };

            // this is the master list of all the rules from all included sheets
            AllRules = PropertySheets.SelectMany(each => each.Rules).Reverse().ToArray();

            // this is the collection of rules for all the #define category. (macros)
            DefineRules = AllRules.GetRulesById("define").GetRulesByName("*").ToArray();

            // lets generate ourselves some rule lists from the loaded propertysheets.
            FileRules = AllRules.GetRulesByName("files");

            PackageRules = AllRules.GetRulesByName("package");
            MetadataRules = AllRules.GetRulesByName("metadata");
            RequiresRules = AllRules.GetRulesByName("requires");
            ProvidesRules = AllRules.GetRulesByName("provides");

            ManifestRules = AllRules.GetRulesByName("manifest");
            CompatabilityPolicyRules = AllRules.GetRulesByName("compatability-policy");
            PackageCompositionRules = AllRules.GetRulesByName("package-composition");
            IdentityRules = AllRules.GetRulesByName("identity");
            SigningRules = AllRules.GetRulesByName("signing");
        }

        internal void CollectRoleRules()
        {
            // -----------------------------------------------------------------------------------------------------------------------------------
            // Determine the roles that are going into the MSI, and ensure we know the basic information for the package (ver, arch, etc)
            // Available Roles are:
            // application 
            // assembly (assemblies is a short-cut for making many assembly rules)
            // service
            // web-application
            // developer-library
            // source-code
            // driver

            ApplicationRules = AllRules.GetRulesByName("application").ToArray();
            AssemblyRules = AllRules.GetRulesByName("assembly").ToArray();
            AssembliesRules = AllRules.GetRulesByName("assemblies").ToArray();
            DeveloperLibraryRules = AllRules.GetRulesByName("developer-library").ToArray();
            SourceCodeRules = AllRules.GetRulesByName("source-code").ToArray();
            ServiceRules = AllRules.GetRulesByName("service").ToArray();
            WebApplicationRules = AllRules.GetRulesByName("web-application").ToArray();
            DriverRules = AllRules.GetRulesByName("driver").ToArray();
            AllRoles = ApplicationRules.Union(AssemblyRules).Union(AssembliesRules).Union(DeveloperLibraryRules).Union(SourceCodeRules).Union(ServiceRules).Union(WebApplicationRules).
                Union(DriverRules).ToArray();

            // check for any roles...
            if (!AllRoles.Any())
            {
                Event<Error>.Raise(
                    MessageCode.ZeroPackageRolesDefined, null,
                    "No package roles are defined. Must have at least one of {{ application, assembly, service, web-application, developer-library, source-code, driver }} rules defined.");
            }
        }
    }
}

