﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace System {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ErrorMessageResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessageResources() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Xpandables.Properties.ErrorMessageResources", typeof(ErrorMessageResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à {0} operation failed. See inner exception..
        /// </summary>
        public static string CommandQueryHandlerFailed {
            get {
                return ResourceManager.GetString("CommandQueryHandlerFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à EnumerationType derived class expected..
        /// </summary>
        public static string EnumerationTypeDerivedClassExpected {
            get {
                return ResourceManager.GetString("EnumerationTypeDerivedClassExpected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Circular dependency found..
        /// </summary>
        public static string PropertyChangedCircularDependency {
            get {
                return ResourceManager.GetString("PropertyChangedCircularDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Property &apos;{0}&apos; of &apos;{0}&apos; can not depends on itself..
        /// </summary>
        public static string PropertyChangedCircularDependencyItself {
            get {
                return ResourceManager.GetString("PropertyChangedCircularDependencyItself", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The &apos;{0}&apos; owns a dependency on &apos;{1}&apos; which one depends on &apos;{2}&apos;..
        /// </summary>
        public static string PropertyChangedCircularDependencyMore {
            get {
                return ResourceManager.GetString("PropertyChangedCircularDependencyMore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Duplicate dependency found..
        /// </summary>
        public static string PropertyChangedDuplicateDependency {
            get {
                return ResourceManager.GetString("PropertyChangedDuplicateDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The &apos;{0}&apos; has already a dependency on &apos;{1}&apos;..
        /// </summary>
        public static string PropertyChangedDuplicateDependencyMore {
            get {
                return ResourceManager.GetString("PropertyChangedDuplicateDependencyMore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à A member expression is expected..
        /// </summary>
        public static string PropertyChangedMemberExpressionExpected {
            get {
                return ResourceManager.GetString("PropertyChangedMemberExpressionExpected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à A constant expression is expected..
        /// </summary>
        public static string PropertyChangedParameterExpressionExpected {
            get {
                return ResourceManager.GetString("PropertyChangedParameterExpressionExpected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Can not validate primitive types or string..
        /// </summary>
        public static string RequiredNestedAttributeTypeMissmatched {
            get {
                return ResourceManager.GetString("RequiredNestedAttributeTypeMissmatched", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Formatting string failed. See inner exception..
        /// </summary>
        public static string StringHelperFormattingFailed {
            get {
                return ResourceManager.GetString("StringHelperFormattingFailed", resourceCulture);
            }
        }
    }
}
