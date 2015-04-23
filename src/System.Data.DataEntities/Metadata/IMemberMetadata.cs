﻿using System;

namespace System.Data.Metadata.DataEntities {

    /// <summary>
    /// The base interface for the entity member metadata.
    /// </summary>
    public interface IMemberMetadata {

        /// <summary>
        /// Returns the distinguished name of this metadata object.
        /// </summary>
        string Name { get; }

    }
}
