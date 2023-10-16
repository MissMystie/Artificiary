using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mystie.Utils
{
    public static class GlobalHelper
    {
        public static float Ease(this float x, float a)
        {
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        public static Color GetColor(float r, float g, float b, float a)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
    }

    public static class IEnumerableExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
                return collection.Count < 1;

            return !enumerable.Any();
        }
    }

    public static class ArrayExtension
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return (array == null || array.Length == 0);
        }

        public static int LoopBack<T>(this T[] array, int x)
        {
            if (x > array.Length - 1) x = 0;
            else if (x < 0) x = array.Length - 1;
            return x;
        }
    }

    internal static class GameObjectExtension
    {
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask mask)
        {
            return (mask == (mask | (1 << gameObject.layer)));
        }
    }
}
