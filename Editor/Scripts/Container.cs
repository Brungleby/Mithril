
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
	public class Container : SmartObject
	{
		[SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("fart")]
		public List<Item> items;

		// [SerializeField]
		// public Item[] items;

		// [SerializeField]
		// public Item item;
	}

	[CustomEditor(typeof(Container))]
	public class ContainerEditor : SmartObject.Editor
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
			}

			if (GUILayout.Button("Update"))
			{
				var __sword = (Sword)__target.items[0];
				__sword.damage++;
				var __shield = (Shield)__target.items[1];
				__shield.health--;
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
				Debug.Log(__target.items.ContentsToString());
			}
		}
	}
}
