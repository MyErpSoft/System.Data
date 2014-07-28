using System.Collections.Generic;
using System.Globalization;

namespace System.Data.DataEntities.Metadata {

    /// <summary>
    /// To provide PathNavigator element descriptions.
    /// </summary>
    /// <typeparam name="TNavigator">The type of Navigator.</typeparam>
    /// <typeparam name="TElement">Element node type.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public abstract class ElementAgent<TNavigator, TElement, TProperty>
        where TNavigator : PathNavigator<TNavigator, TElement, TProperty>
        where TElement : ElementAgent<TNavigator, TElement, TProperty>
        where TProperty : PropertyAgent<TNavigator, TElement, TProperty> {

        /// <summary>
        /// Passed in the parameter of the derived class instance.
        /// </summary>
        /// <param name="navigator">Associated instance of the Navigator.</param>
        protected ElementAgent(TNavigator navigator) {
            this._navigator = navigator;
        }

        private TNavigator _navigator;
        /// <summary>
        /// Returns the associated instance of the Navigator.
        /// </summary>
        public TNavigator Navigator { get { return _navigator; } }

        /// <summary>
        /// Passing a property path string, returns to his eventual access to elements and attributes.
        /// </summary>
        /// <param name="path">A property path string, such as R1.R2.Name</param>
        /// <param name="element">Returns the last element of this access path</param>
        /// <param name="property">Returns the access path of the last property</param>
        /// <param name="errorMessage">If the target could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Access to matches the path is complete.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public bool TryGetTarget(string path, out TElement element, out TProperty property, out string errorMessage) {
            KeyValuePair<TElement, TProperty>[] elementStack;
            if (this.TryGetTargetStack(path, out elementStack, out errorMessage)) {
                var lastElement = elementStack[elementStack.Length - 1];
                element = lastElement.Key;
                property = lastElement.Value;
                return true;
            }

            element = null;
            property = null;
            return false;
        }

        /// <summary>
        /// Passing a property path string, returned his call stack.
        /// </summary>
        /// <param name="path">A property path string, such as R1.R2.Name</param>
        /// <param name="elementStack">The access paths of stack is returned, if it fails to find a suitable property, it may be that some data is null.</param>
        /// <param name="errorMessage">If the target could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Access to matches the path is complete.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public bool TryGetTargetStack(string path, out KeyValuePair<TElement, TProperty>[] elementStack, out string errorMessage) {
            var names = Split(path);
            if ((names == null) || (names.Length == 0)) {
                elementStack = null;
                errorMessage = Properties.Resources.NameIsEmpty;
                return false;
            }

            elementStack = new KeyValuePair<TElement, TProperty>[names.Length];
            var currentElement = (TElement)this;
            for (int i = 0; i < names.Length; i++) {

                TProperty property;
                if (!currentElement.TryGetProperty(names[i], out property, out errorMessage)) {
                    errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ElementGetTargetError, this.Name, path, errorMessage);
                    return false;
                }

                elementStack[i] = new KeyValuePair<TElement, TProperty>(currentElement, property);
                if (i + 1 < names.Length) { //Not the last property to access the
                    if (property.RelationType != PropertyRelationType.Simple) { //Must be in relation to continue
                        currentElement = property.RelationTo;
                    }
                    else {
                        errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.GetTargetExistSimpleProperty, names[i]);
                        errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ElementGetTargetError, this.Name, path, errorMessage);
                        return false;
                    }
                }
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Passing a property path string, returns to his eventual access to elements and attributes.
        /// </summary>
        /// <param name="path">A property path string, such as R1.R2.Name</param>
        /// <returns>Final match elements and attributes.</returns>
        public KeyValuePair<TElement, TProperty> GetTarget(string path) {
            KeyValuePair<TElement, TProperty>[] elementStack;
            string errorMessage;
            if (this.TryGetTargetStack(path, out elementStack, out errorMessage)) {
                return elementStack[elementStack.Length - 1];
            }

            //To not report an error: the entity {0}-{1} failed because the {2}.
            throw new KeyNotFoundException(errorMessage);
        }

        /// <summary>
        /// Passing a property path string, returned his call stack.
        /// </summary>
        /// <param name="path">A property path string, such as R1.R2.Name</param>
        /// <returns>The access paths of stack is returned, if it fails to find the right property will throw an exception.</returns>
        public KeyValuePair<TElement, TProperty>[] GetTargetStack(string path) {
            KeyValuePair<TElement, TProperty>[] elementStack;
            string errorMessage;
            if (this.TryGetTargetStack(path, out elementStack, out errorMessage)) {
                return elementStack;
            }

            throw new KeyNotFoundException(errorMessage);
        }

        private Dictionary<string, TProperty> _cachedProperties;
        /// <summary>
        /// Given a property name, attempting to return to his property agent.
        /// </summary>
        /// <param name="name">Name of the property to access.</param>
        /// <param name="propertyAgent">If present, this attribute returns the property agent. Otherwise it returns null</param>
        /// <param name="errorMessage">If the property could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Whether this property was found.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public bool TryGetProperty(string name, out TProperty propertyAgent, out string errorMessage) {
            if (_cachedProperties == null) {
                _cachedProperties = new Dictionary<string, TProperty>();
            }
            else if (_cachedProperties.TryGetValue(name, out propertyAgent)) {
                errorMessage = null;
                return true;
            }

            if (this.TryGetPropertyCore(name, out propertyAgent, out errorMessage)) {
                _cachedProperties.Add(name, propertyAgent);
                return true;
            }

            errorMessage = string.Format(CultureInfo.CurrentCulture, Properties.Resources.NotFindProperty, this.Name, name) + errorMessage;
            return false;
        }

        /// <summary>
        /// Derived classes implement this method in order to return corresponds to the name property.
        /// </summary>
        /// <param name="name">Name of the property to retrieve.</param>
        /// <param name="propertyAgent">Agent returns this property if found, otherwise null</param>
        /// <param name="errorMessage">If the property could not be retrieved, the string that contains the description of the error.</param>
        /// <returns>Find a property with this name.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        protected abstract bool TryGetPropertyCore(string name, out TProperty propertyAgent, out string errorMessage);

        /// <summary>
        /// Returns the name of this element.
        /// </summary>
        public abstract string Name { get; }

        private static string[] Split(string path) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            return path.Split('.');
        }

        /// <summary>
        /// Returns the display name.
        /// </summary>
        /// <returns>一个字符串。</returns>
        public override string ToString() {
            return this.Name;
        }
    }
}
