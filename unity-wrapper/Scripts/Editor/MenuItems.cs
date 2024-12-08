#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Itibsoft.PanelManager.Editor
{
    public static class MenuItems
    {
        [MenuItem("GameObject/Panel/View", false, 0)]
        public static void CreateGameObjectContext(MenuCommand menuCommand)
        {
            var context = menuCommand.context;

            var canvas = context as Canvas;

            if (canvas == default)
            {
                canvas = Object.FindObjectOfType<Canvas>();

                if (canvas == default)
                {
                    canvas = MenuOptionsUtils.CreateCanvas();
                }
            }
            
            var root = new GameObject("Panel", typeof(SafeArea));
            var container = new GameObject("Container");

            root.transform.SetParent(canvas.transform, false);
            container.transform.SetParent(root.transform, false);

            var rootRectTransform = root.AddComponent<RectTransform>();
            var containerRectTransform = container.AddComponent<RectTransform>();

            StretchContent(rootRectTransform);
            StretchContent(containerRectTransform);

            Selection.activeGameObject = root.gameObject;

            var scene = SceneManager.GetActiveScene();
            
            EditorSceneManager.MarkSceneDirty(scene);
            
            return;

            void StretchContent(RectTransform rectTransform)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = new Vector2(.5f, .5f);
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
#endif