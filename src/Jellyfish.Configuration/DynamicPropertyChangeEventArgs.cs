// Copyright (c) Zenasoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Jellyfish.Configuration
{
    public enum PropertyChangedAction
    {
        Changed,
        Removed
    }

    public class DynamicPropertyChangedEventArgs : EventArgs
    {
        public IDynamicPropertyBase Property { get; private set; }
        public PropertyChangedAction Action { get; private set; }

        public DynamicPropertyChangedEventArgs(IDynamicPropertyBase property, PropertyChangedAction action)
        {
            this.Action = action;
            this.Property = property;
        }
    }
}