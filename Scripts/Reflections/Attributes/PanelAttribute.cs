﻿using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace Itibsoft.PanelManager
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public class PanelAttribute : PreserveAttribute, IPanelMeta
    {
        public PanelType PanelType { get; set; }
        public int Order { get; set; }
        public string AssetId { get; set; }
    }
}