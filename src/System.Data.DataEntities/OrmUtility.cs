using System.ComponentModel;
using System.Data.DataEntities.Metadata;
using System.Data.DataEntities.Metadata.Clr;
using System.Reflection;

namespace System.Data.DataEntities
{
    /// <summary>
    /// Provides static functions that access to the ORM entity.
    /// </summary>
    internal static class OrmUtility
    {
        /// <summary>
        /// Verifies that the string is composed of letters or numbers (allow underscore). Separation of and supports the use of a namespace
        /// </summary>
        internal static bool VerifyNameWithNamespace(string str)
        {
            char item;
            int wordStartIndex = 0;  //Start position of a Word
            int endIndex = str.Length;
            int wordSize = 0;

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            for (int i = 0; i < endIndex; i++)
            {
                item = str[i];
                if (!((item >= 'a' && item <= 'z') ||
                    (item >= 'A' && item <= 'Z') ||
                    (item == '_')))
                {
                    if (item >= '0' && item <= '9')
                    {
                        //Cannot start with a number
                        if (i == wordStartIndex)
                        {
                            return false;
                        }
                    }
                    else if (item == '.')
                    {
                        //Using split words should not be empty, the last may not be
                        //A Word cannot be longer than 256 characters.
                        if ((wordSize == 0) || (i == endIndex) || (wordSize > 256))
                        {
                            return false;
                        }
                        wordStartIndex = i + 1;
                        wordSize = 0;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }

                wordSize++;
            }

            return true;
        }

        /// <summary>
        /// Verifies that the string is composed of letters or numbers (allow underscore)
        /// </summary>
        internal static bool VerifyName(string str)
        {
            char item;
            int wordStartIndex = 0;  //Start position of a Word
            int endIndex = str.Length;
            int wordSize = 0;

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            if (endIndex > 256)
            {
                return false;
            }

            for (int i = 0; i < endIndex; i++)
            {
                item = str[i];
                if (!((item >= 'a' && item <= 'z') ||
                    (item >= 'A' && item <= 'Z') ||
                    (item == '_')))
                {
                    if (item >= '0' && item <= '9')
                    {
                        //Cannot start with a number
                        if (i == wordStartIndex)
                        {
                            return false;
                        }
                    }
                    return false;
                }

                wordSize++;
            }

            return true;
        }

        internal static void ThrowArgumentNullException(string paramName)
        {
            throw new ArgumentNullException(paramName);
        }

        internal static void ThrowArgumentException(string message)
        {
            throw new ArgumentException(message);
        }

        internal static void ThrowInvalidOperationException(string message)
        {
            throw new InvalidOperationException(message);
        }
        public static IEntityType GetEntityType(this Type classType)
        {
            #region Parameter checking
            if (null == classType)
            {
                throw new ArgumentNullException("classType");
            }
            #endregion

            if (classType.IsDefined(typeof(DataObjectAttribute),true))
            {
                return EntityType.GetEntityType(classType);
            }

            //Looking for static fields, static fields should contain a class named +Type.
            string fieldName = classType.Name + "Type";
            FieldInfo typeField = classType.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            //And return types supported IEntity type interface.
            if (typeField == null)
            {
                throw new ArgumentOutOfRangeException("classType", classType, string.Format(
                    "Class {0} must provide a tag Data object attribute or static fields (name {1}), and returns type supports IEntity type interface.",
                    classType.FullName, fieldName));
            }

            if ((typeof(IEntityType).IsAssignableFrom(typeField.FieldType)))
            {
                throw new ArgumentOutOfRangeException("classType", classType, string.Format(
                    "Although the class {0} provides a public static field (name {1}), but the return type does not support IEntity type interface.",
                    classType.FullName, fieldName));
            }

            IEntityType result = (IEntityType)typeField.GetValue(null);
            if (result == null)
            {
                throw new ArgumentOutOfRangeException("classType", classType, string.Format(
                    "Although the class {0} provides a public static field (name {1}), but the return value is null.",
                    classType.FullName, fieldName));
            }

            return result;
        }

        /// <summary>
        /// Returns a displayable name of the entity, for example, Customer (PK=23)
        /// </summary>
        /// <param name="entity">The entity to be displayed, and can be null</param>
        /// <param name="dt">Type of entity, if the entity is not null, this parameter cannot be null.</param>
        /// <returns>To display name.</returns>
        internal static string GetEntityDisplayName(this object entity,IEntityType dt) {
            if (entity == null) {
                return "<Null>";
            }
            if (dt == null) {
                OrmUtility.ThrowArgumentNullException("dt");
            }

            object oid = null;
            if (dt.PrimaryKey != null) {
                oid = dt.PrimaryKey.GetValue(entity);
            }
            if (oid == null) {
                oid = System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(entity);
                return dt.Name + "(oid=" + oid.ToString() + ")";
            }
            else {
                return dt.Name + "(pk=" + oid.ToString() + ")";
            }
        }
    }
}
