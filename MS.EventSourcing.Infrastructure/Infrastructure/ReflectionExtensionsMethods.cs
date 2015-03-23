using System;
using System.Reflection;

namespace MS.Infrastructure
{
    public static class ReflectionExtensionsMethods
    {
        /// <summary>
        /// Invokes a method with a generic signature
        /// </summary>
        /// <param name="instance">Object instance to invoke the method on</param>
        /// <param name="methodName">Name of the method to invoke</param>
        /// <param name="genericType">Type of generic parameter</param>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Method's return value if any.</returns>
        public static object InvokeGenericMethod(this object instance, string methodName, Type genericType, params object[] parameters)
        {
            var method = GetGenericMethod(instance, methodName, genericType);
            return method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Gets a method name with a generic signature
        /// </summary>
        /// <param name="instance">Object instance to invoke the method on</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="genericType">Type of generic parameter</param>
        /// <returns>Method info if it exists</returns>
        public static MethodInfo GetGenericMethod(this object instance, string methodName, Type genericType)
        {
            return instance.GetType().GetMethod(methodName).MakeGenericMethod(genericType);
        }
    }
}
