using System.Reflection;
using System.Reflection.Emit;

namespace System.Data.DataEntities.Metadata.Clr
{
    internal static class ClrHelper
    {
        /// <summary>
        /// Creating a CLR class is a no-argument constructor function to create the.
        /// </summary>
        public static Func<object> GetCreateInstanceFunc(this Type clrType)
        {
            if (clrType.IsAbstract || clrType.IsInterface)
            {
                throw new NotSupportedException();
            }

            DynamicMethod createMethod = new DynamicMethod("CreateInstance", typeof(object), null);
            var ilGenerator = createMethod.GetILGenerator();
            var ctor = clrType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new NotSupportedException("No no-argument constructor.");
            }
            ilGenerator.Emit(OpCodes.Newobj, ctor);
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<object>)createMethod.CreateDelegate(typeof(Func<object>));
        }

        //GetValue function paramter types.
        private static readonly Type[] GetValueParameterTypes = new Type[] { typeof(object) };

        public static Func<object, object> GetGetValueFunc(this PropertyInfo property)
        {
            /*
           .method public hidebysig static object GetAge(object obj) cil managed
            {
                .maxstack 8
                L_0000: ldarg.0 
                L_0001: castclass Test.TestClass
                L_0006: callvirt instance int32 Test.TestClass::get_Age()
                L_000b: box int32
                L_0010: ret 
            }
             */
            if (!property.PropertyType.IsPublic)
            {
                return null;
            }

            var getMethod = property.GetGetMethod(false);
            if ((getMethod == null) || (!getMethod.IsPublic))
            {
                return null;
            }

            DynamicMethod dynamicGetMethod = new DynamicMethod("GetValue", typeof(object),GetValueParameterTypes);
            var ilGenerator = dynamicGetMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, property.DeclaringType);
            ilGenerator.Emit(OpCodes.Callvirt, getMethod);
            if (property.PropertyType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, property.PropertyType);
            }
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<object,object>)dynamicGetMethod.CreateDelegate(typeof(Func<object,object>));
        }

        private static readonly Type[] SetValueParameterTypes = new Type[] { typeof(object), typeof(object) };

        public static Action<object, object> GetSetValueFunc(this PropertyInfo property)
        {
            /*
            .method public hidebysig static void SetAge(object obj, object 'value') cil managed
            {
                .maxstack 8
                L_0000: ldarg.0 
                L_0001: castclass Test.TestClass
                L_0006: ldarg.1 
                L_0007: unbox.any int32
                L_000c: callvirt instance void Test.TestClass::set_MyProperty(int32)
                L_0011: ret 
            }
             */
            if (!property.PropertyType.IsPublic)
            {
                return null;
            }
            var setMethod = property.GetSetMethod(false);
            if (setMethod == null)
            {
                return null;
            }

            DynamicMethod dynamicSetMethod = new DynamicMethod("SetValue", typeof(void), SetValueParameterTypes);
            var ilGenerator = dynamicSetMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, property.DeclaringType);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            if (property.PropertyType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, property.PropertyType);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, property.PropertyType);
            }
            ilGenerator.Emit(OpCodes.Callvirt, setMethod);
            ilGenerator.Emit(OpCodes.Ret);

            return (Action<object, object>)dynamicSetMethod.CreateDelegate(typeof(Action<object, object>));
        }

        /// <summary>
        /// Get custom attribute,return defaultAttribute if not found.
        /// </summary>
        public static T GetFirstOrDefaultAttribute<T>(this ICustomAttributeProvider attProvider,bool inherit,  T defaultAttribute = null) where T : Attribute
        {
            var atts = attProvider.GetCustomAttributes(typeof(T), inherit);
            if ((atts != null) && (atts.Length > 0))
            {
                return (T)atts[0];
            }
            return defaultAttribute;
        }
    }
}
