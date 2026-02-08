using System.Linq;
using UnityEditor;
using UnityEngine;

namespace jp.lilxyzw.lilycalinventory
{
    using runtime;

    internal static class LilToonFixLightingMenu
    {
        private const string MENU_PATH = "Tools/lilToon/[Game Object] Fix Lighting (Prefab)";
        private const string MENU_PATH_GAMEOBJECT = "GameObject/lilToon/[Game Object] Fix Lighting (Prefab)";
        private const string UNDO_NAME = "Add [lilToon] Fix Lighting Prefab";
        // Keep it above most lilToon items while staying near related tools.
        private const int MENU_PRIORITY = 10;

        [MenuItem(MENU_PATH, false, MENU_PRIORITY)]
        [MenuItem(MENU_PATH_GAMEOBJECT, false, MENU_PRIORITY)]
        private static void AddFixLightingPrefab()
        {
            var avatarRoot = GetAvatarRootFromSelection();
            if(!avatarRoot) return;

            var prefab = ObjHelper.LoadAssetByGUID<GameObject>(ConstantValues.GUID_LILTOON_FIX_LIGHTING_PREFAB);
            if(!prefab)
            {
                Debug.LogError("[lilycalInventory] Failed to load [lilToon] Fix Lighting prefab.");
                return;
            }

            var existing = avatarRoot.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t != avatarRoot && PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject) == prefab);
            if(existing)
            {
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab, avatarRoot.gameObject.scene) as GameObject;
            if(!instance)
            {
                Debug.LogError("[lilycalInventory] Failed to instantiate [lilToon] Fix Lighting prefab.");
                return;
            }

            Undo.RegisterCreatedObjectUndo(instance, UNDO_NAME);
            Undo.SetTransformParent(instance.transform, avatarRoot.transform, UNDO_NAME);
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.transform.localScale = Vector3.one;
            Selection.activeGameObject = instance;
        }

        [MenuItem(MENU_PATH, true, MENU_PRIORITY)]
        [MenuItem(MENU_PATH_GAMEOBJECT, true, MENU_PRIORITY)]
        private static bool AddFixLightingPrefabValidate()
        {
            return GetAvatarRootFromSelection() != null;
        }

        private static Transform GetAvatarRootFromSelection()
        {
            if(!Selection.activeGameObject) return null;
            return Selection.activeGameObject.GetAvatarRoot();
        }
    }
}
