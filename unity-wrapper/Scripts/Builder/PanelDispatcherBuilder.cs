using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Itibsoft.PanelManager
{
    public class PanelDispatcherBuilder
    {
        #region Private Fields

        private const string PANEL_DISPATCHER_NAME = "[PanelDispatcher]";
        private const string EVENT_SYSTEM_NAME = "[EventSystem]";
        
        private readonly PanelDispatcherSettings _settings;
        private EventSystem _eventSystem;

        #endregion;

        #region Initialize

        private PanelDispatcherBuilder(PanelDispatcherSettings settings = default)
        {
            settings ??= new PanelDispatcherSettings();

            _settings = settings;
        }

        #endregion

        #region Public Methods

        public static PanelDispatcherBuilder Create(PanelDispatcherSettings settings = default)
        {
            return new PanelDispatcherBuilder(settings);
        }

        public PanelDispatcherBuilder SetEventSystem(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;

            return this;
        }

        public PanelDispatcher Build()
        {
            var panelDispatcher = CreateInternal(_settings);

            return panelDispatcher;
        }

        #endregion

        #region Private Methods

        private PanelDispatcher CreateInternal(PanelDispatcherSettings settings)
        {
            var components = new[]
            {
                typeof(PanelDispatcher),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            };

            var instance = new GameObject(PANEL_DISPATCHER_NAME, components)
            {
                transform =
                {
                    position = Vector3.zero,
                    rotation = Quaternion.identity,
                    localScale = Vector3.zero
                }
            };

            var processor = (IPanelDispatcherProcessor)instance.GetComponent<PanelDispatcher>();
            
            var canvas = instance.GetComponent<Canvas>();
            var canvasScaler = instance.GetComponent<CanvasScaler>();
            var graphicRaycaster = instance.GetComponent<GraphicRaycaster>();
            
            processor.SetCanvas(canvas);
            processor.SetCanvasScaler(canvasScaler);
            processor.SetGraphicRaycaster(graphicRaycaster);

            canvas.renderMode = settings.RenderMode;
            canvas.pixelPerfect = settings.PixelPerfect;

            canvasScaler.uiScaleMode = settings.UIScaleMode;
            canvasScaler.referenceResolution = settings.ReferenceResolution;
            canvasScaler.screenMatchMode = settings.ScreenMatchMode;
            canvasScaler.matchWidthOrHeight = settings.MatchWidthOrHeight;
            canvasScaler.referencePixelsPerUnit = settings.ReferencePixelsPerUnit;

            graphicRaycaster.ignoreReversedGraphics = settings.IgnoreReversedGraphics;
            graphicRaycaster.blockingObjects = settings.BlockingObjects;

            if (_eventSystem == default)
            {
                var eventSystemInstance = CreateEventSystemInternal();
                
                eventSystemInstance.transform.SetParent(instance.transform);
            }

            Object.DontDestroyOnLoad(instance);

            return instance.GetComponent<PanelDispatcher>();
        }

        private static EventSystem CreateEventSystemInternal()
        {
            var eventSystemComponents = new Type[2];
            eventSystemComponents[0] = typeof(EventSystem);
                
#if ENABLE_INPUT_SYSTEM
            eventSystemComponents[1] = typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule);
#else
            eventSystemComponents[1] = typeof(StandaloneInputModule);
#endif
                
            var eventSystem = new GameObject(EVENT_SYSTEM_NAME, eventSystemComponents);

            return eventSystem.GetComponent<EventSystem>();
        }

        #endregion

        #region Nested Types

        public class PanelDispatcherSettings
        {
            public RenderMode RenderMode { get; set; } = RenderMode.ScreenSpaceOverlay;
            public bool PixelPerfect { get; set; } = false;

            public CanvasScaler.ScaleMode UIScaleMode { get; set; } = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            public Vector2 ReferenceResolution { get; set; } = new(1920, 1080);

            public CanvasScaler.ScreenMatchMode ScreenMatchMode { get; set; } = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            public float MatchWidthOrHeight { get; set; } = 0.5f;
            public float ReferencePixelsPerUnit { get; set; } = 100;

            public bool IgnoreReversedGraphics { get; set; } = true;

            public GraphicRaycaster.BlockingObjects BlockingObjects { get; set; } = GraphicRaycaster.BlockingObjects.None;
        }
        
        public interface IPanelDispatcherProcessor
        {
            public void SetCanvas(Canvas canvas);
            public void SetCanvasScaler(CanvasScaler canvasScaler);
            public void SetGraphicRaycaster(GraphicRaycaster graphicRaycaster);
        }

        #endregion
    }
}