using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.DataEntities.DcxmlSerialization
{
    /// <summary>
    /// Property action list.
    /// </summary>
    enum PropertyActions
    {
        /// <summary>
        /// Set value action(default).
        /// </summary>
        SetValue,
        /// <summary>
        /// Reset value action.
        /// </summary>
        Reset,
        /// <summary>
        /// Set value is null.
        /// </summary>
        SetNull
    }

    /// <summary>
    /// Collection action list.
    /// </summary>
    enum CollectionActions
    {
        /// <summary>
        /// Add new object to collection.
        /// </summary>
        Add,
        /// <summary>
        /// Edit exists object property.
        /// </summary>
        Edit,
        /// <summary>
        /// Remove a object from collection.
        /// </summary>
        Remove
    }

}

