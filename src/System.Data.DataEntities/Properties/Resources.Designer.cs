﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.0
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace System.Data.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("System.Data.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 提供的名称 {0} 不正确，不能为空，且只能是字母、数字（不能是第一个字符）或下划线，最多256个字符。 的本地化字符串。
        /// </summary>
        internal static string ErrorName {
            get {
                return ResourceManager.GetString("ErrorName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 提供的命名空间 {0} 不正确，只能是字母、数字（不能是第一个字符）或下划线，最多256个字符组成的单词，使用点隔离多个单词。 的本地化字符串。
        /// </summary>
        internal static string ErrorNamespace {
            get {
                return ResourceManager.GetString("ErrorNamespace", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 类型 {0} 已经包含成员 {1}，不能添加重名的成员。 的本地化字符串。
        /// </summary>
        internal static string KeyIsExisted {
            get {
                return ResourceManager.GetString("KeyIsExisted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 类型 {0} 未能找到成员 {1}，或成员不符合需要的类型。 的本地化字符串。
        /// </summary>
        internal static string KeyNotFoundException {
            get {
                return ResourceManager.GetString("KeyNotFoundException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 对象 {0} 已经冻结，不允许再修改内容。 的本地化字符串。
        /// </summary>
        internal static string ObjectIsFrozen {
            get {
                return ResourceManager.GetString("ObjectIsFrozen", resourceCulture);
            }
        }
    }
}
