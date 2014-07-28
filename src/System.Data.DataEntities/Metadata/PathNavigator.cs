using System;
using System.Collections.Generic;
using System.Globalization;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// Categories for navigation, such as providing a Path path for routing.
    /// </summary>
    /// <typeparam name="TNavigator">The type of Navigator.</typeparam>
    /// <typeparam name="TElement">Element node type.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class PathNavigator<TNavigator, TElement, TProperty>
        where TNavigator : PathNavigator<TNavigator, TElement, TProperty>
        where TElement : ElementAgent<TNavigator, TElement, TProperty>
        where TProperty : PropertyAgent<TNavigator, TElement, TProperty> {

        /// <summary>
        /// By a name corresponding to the element.
        /// </summary>
        /// <param name="name">To access the element name</param>
        /// <param name="element">If you find this element, and returns its elements, otherwise it returns null</param>
        /// <param name="errorMessage">If the entity could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Returns whether this element is successfully found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        protected abstract bool TryGetElementCore(string name, out TElement element, out string errorMessage);

        Dictionary<string, TElement> _cachedElements;
        /// <summary>
        /// By a name corresponding to the element.
        /// </summary>
        /// <param name="name">To access the element name</param>
        /// <param name="element">If you find this element, and returns its elements, otherwise it returns null</param>
        /// <param name="errorMessage">If the entity could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Returns whether this element is successfully found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public bool TryGetElement(string name, out TElement element, out string errorMessage) {
            if (name == null) {
                errorMessage = Properties.Resources.NameIsEmpty;
                element = default(TElement);
                return false;
            }

            if (_cachedElements == null) {
                _cachedElements = new Dictionary<string, TElement>();
            }
            else if (_cachedElements.TryGetValue(name, out element)) {
                errorMessage = null;
                return true;
            }

            if (TryGetElementCore(name, out element, out errorMessage)) {
                _cachedElements.Add(name, element);
                return true;
            }

            errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.NotFindElementBecause, name, errorMessage);
            return false;
        }

        /// <summary>
        /// By a name corresponding to the element.
        /// </summary>
        /// <param name="name">To access the element name</param>
        /// <returns>Find element, if not found exception will be thrown.</returns>
        public TElement GetElement(string name) {
            TElement element;
            string errorMessage;
            if (TryGetElement(name, out element, out errorMessage)) {
                return element;
            }

            throw new KeyNotFoundException(errorMessage);
        }
    }
}
