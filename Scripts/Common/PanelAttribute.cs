﻿using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace Itibsoft.PanelManager
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class PanelAttribute : PreserveAttribute
    {
        public PanelType PanelType;
        public string AssetId;
    }
}