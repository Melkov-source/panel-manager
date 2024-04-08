﻿using System;

namespace Itibsoft.PanelManager
{
    public delegate void PanelCallbackDelegate<in TCallback>(TCallback callback) where TCallback : IPanelCallback;
    public interface IPanelController : IViewHandler
    {
        public void RegisterCallback<TCallback>(PanelCallbackDelegate<TCallback> callback) where TCallback : IPanelCallback;
        public void UnRegisterCallback<TCallback>(PanelCallbackDelegate<TCallback> callback) where TCallback : IPanelCallback;

        public void Release();
    }

    public interface IPanelController<out TPanel> : IPanelController where TPanel : IViewMono
    {
        public TPanel Panel { get; }
    }
}