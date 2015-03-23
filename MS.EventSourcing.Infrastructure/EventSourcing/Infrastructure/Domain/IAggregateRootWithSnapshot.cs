﻿using System;

namespace MS.EventSourcing.Infrastructure.Domain
{
    /// <summary>
    /// Represents an aggregate root whcih can save snapshots 
    /// </summary>
    internal interface IAggregateRootWithSnapshot
    {
        Type GetGenericType();
        object CreateGenericSnapshot();
    }
}