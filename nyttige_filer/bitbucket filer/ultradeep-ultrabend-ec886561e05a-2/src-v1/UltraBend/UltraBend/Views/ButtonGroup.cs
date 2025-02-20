using System;

namespace UltraBend.Views
{
    [Flags]
    public enum ButtonGroup
    {
        Default = 1,
        BendStiffenerDesign = 2,
        Material = 4,
        Case = 8,
        Study = 16,
        LoadContour = 32
    }
}