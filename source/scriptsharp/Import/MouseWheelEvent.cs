using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Html
{
    [Imported]
    [IgnoreNamespace]
    public class MouseWheelEvent
    {
        [ScriptName("wheelDelta")]
        public readonly int WheelDelta;

        [ScriptName("wheelDeltaX")]
        public readonly int WheelDeltaX;

        [ScriptName("HORIZONTAL_AXIS")]
        public readonly int HorizontalAxis;

        [ScriptName("axis")]
        public readonly int Axis;

        [ScriptName("detail")]
        public readonly string Detail;
    }
}
