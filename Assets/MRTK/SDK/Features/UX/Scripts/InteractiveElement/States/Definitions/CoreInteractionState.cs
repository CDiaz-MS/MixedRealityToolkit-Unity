// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// A state with already defined state settting logic within an Interactive Element is considered a Core Interaction State.
    /// </summary>
    public enum CoreInteractionState
    {
        /// <summary>
        /// Represents the Default state. The Default state is only active when all other states are not active.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Represents the Focus state.
        /// </summary>
        Focus,

        /// <summary>
        /// Represents the Touch state.
        /// </summary>
        Touch,

        /// <summary>
        /// Represents the Click state.  
        /// </summary>
        Click,

        /// <summary>
        /// Represents the Toggle state.  
        /// </summary>
        Toggle,

        /// <summary>
        /// Represents the Press state.  
        /// </summary>
        Press
    }
}
