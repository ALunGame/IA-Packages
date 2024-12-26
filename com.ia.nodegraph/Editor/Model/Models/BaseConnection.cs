using System;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Model
{
    public partial class BaseConnection
    {
        /// <summary>
        /// from节点GUID
        /// </summary>
        internal string from;
        internal string fromPortName;

        /// <summary>
        /// to节点GUID
        /// </summary>
        internal string to;
        internal string toPortName;
    }
}
