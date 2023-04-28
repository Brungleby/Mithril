
/** Serialization.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

#endregion

namespace Cuberoot.Editor
{

	[Serializable]
	public class Item : object
	{
		[SerializeField]
		public string name;

		public override string ToString() =>
			$"{name} ({GetType().Name})";
	}

	[Serializable]
	public class Sword : Item
	{
		[SerializeField]
		public int damage;

		public override string ToString() =>
			$"{base.ToString()}: {damage} damage";
	}

	[Serializable]
	public class Shield : Item
	{
		[SerializeField]
		public float health;

		public override string ToString() =>
			$"{base.ToString()}: {health} health";
	}

	[Serializable]
	[CreateAssetMenu(menuName = "Serialization Test (Container)")]
	public class Container : TypeSafeScriptableObject
	{
		[SerializeField]
		public List<Item> items;

		// [SerializeField]
		// public Item[] items;

		// [SerializeField]
		// public Item item;
	}

	[CustomEditor(typeof(Container))]
	public class ContainerEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var __target = (Container)target;

			if (GUILayout.Button("Initialize"))
			{
				var __sword = new Sword() { name = "Farts", damage = 3 };
				var __shield = new Shield() { name = "Straf", health = 30 };

				__target.items = new List<Item>();

				__target.items.Add(__sword);
				__target.items.Add(__shield);

				// __target.items = new Item[] { __sword, __shield };
			}

			if (GUILayout.Button("Update"))
			{
				var __sword = (Sword)__target.items[0];
				__sword.damage++;
				var __shield = (Shield)__target.items[1];
				__shield.health--;

				// if (__target.item == null || __target.item.GetType() != typeof(Sword))
				// 	__target.item = new Sword();
				// Sword __sword = (Sword)__target.item;
				// __sword.damage++;
			}

			if (GUILayout.Button("Save"))
			{
				EditorUtility.SetDirty(__target);
				AssetDatabase.SaveAssetIfDirty(__target);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

			}

			if (GUILayout.Button("Print Object"))
			{
				__target.OnAfterDeserialize();

				Debug.Log(__target.items.ContentsToString());
				// Debug.Log($"contents => {__target.item.ToString()}");
			}

			if (GUILayout.Button("Print JSON"))
			{
				Debug.Log(__target.GetFieldJson("items"));
				// Debug.Log(__target.GetFieldJson("item"));
			}
		}
	}
}
