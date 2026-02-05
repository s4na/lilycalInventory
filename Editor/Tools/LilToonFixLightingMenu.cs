using System.Linq;
using UnityEditor;
using UnityEngine;

namespace jp.lilxyzw.lilycalinventory
{
    using runtime;

    internal static class LilToonFixLightingMenu
    {
        private const string MENU_PATH = "Tools/lilToon/lilycalInventory/[lilToon] Fix Lighting (Add Prefab)";
        private const string MENU_PATH_GAMEOBJECT = "GameObject/lilToon/lilycalInventory/[lilToon] Fix Lighting (Add Prefab)";
        private const string UNDO_NAME = "Add [lilToon] Fix Lighting Prefab";
        // lilToon Fix lighting is priority 21, so use 20 to appear just above it.
        private const int MENU_PRIORITY = 20;

        [MenuItem(MENU_PATH, false, MENU_PRIORITY)]
        [MenuItem(MENU_PATH_GAMEOBJECT, false, MENU_PRIORITY)]
        private static void AddFixLightingPrefab()
        {
            var avatarRoot = GetAvatarRootFromSelection();
            if(!avatarRoot)
            {
                ErrorHelper.Report("dialog.error.avatarRootNofFound");
                return;
            }

            var prefab = ObjHelper.LoadAssetByGUID<GameObject>(ConstantValues.GUID_LILTOON_FIX_LIGHTING_PREFAB);
            if(!prefab)
            {
                Debug.LogError("[lilycalInventory] Failed to load [lilToon] Fix Lighting prefab.");
                return;
            }

            var existing = avatarRoot.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.gameObject != avatarRoot && t.name == prefab.name);
            if(existing)
            {
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab, avatarRoot.gameObject.scene) as GameObject;
            if(!instance) return;

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
