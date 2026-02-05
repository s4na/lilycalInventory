using System.Linq;
using UnityEditor;
using UnityEngine;

namespace jp.lilxyzw.lilycalinventory
{
    using runtime;

    internal static class LilToonFixLightingMenu
    {
        private const string MENU_PATH = "Tools/lilycalInventory/[lilToon] Fix Lighting (Add Prefab)";
        private const string MENU_PATH_GAMEOBJECT = "GameObject/lilycalInventory/[lilToon] Fix Lighting (Add Prefab)";
        private const string UNDO_NAME = "Add [lilToon] Fix Lighting Prefab";

        [MenuItem(MENU_PATH)]
        [MenuItem(MENU_PATH_GAMEOBJECT)]
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

            var instance = PrefabUtility.InstantiatePrefab(prefab, avatarRoot.scene) as GameObject;
            if(!instance) return;

            Undo.RegisterCreatedObjectUndo(instance, UNDO_NAME);
            Undo.SetTransformParent(instance.transform, avatarRoot.transform, UNDO_NAME);
            instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.transform.localScale = Vector3.one;
            Selection.activeGameObject = instance;
        }

        [MenuItem(MENU_PATH, true)]
        [MenuItem(MENU_PATH_GAMEOBJECT, true)]
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
