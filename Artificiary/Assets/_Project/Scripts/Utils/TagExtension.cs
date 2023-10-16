using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Utils
{
    internal static class TagExtension
    {
        public enum TagFilter { ALL, ANY, NONE }

        public static bool HasTags(this GameObject gameObject, IEnumerable<string> tags, TagFilter filter = TagFilter.ALL)
        {
            if (tags.IsNullOrEmpty()) return true;

            foreach (string tag in tags)
            {
                switch (filter)
                {
                    case TagFilter.NONE:
                    case TagFilter.ANY:
                        if (gameObject.HasTag(tag)) return filter == TagFilter.ANY;
                        break;
                    case TagFilter.ALL:
                        if (!gameObject.HasTag(tag)) return false;
                        break;
                }
                
            }

            return filter != TagFilter.ANY;
        }

        public static bool FilterTags(this GameObject gameObject, 
            IEnumerable<string> anyTags = null,
            IEnumerable<string> requiredTags = null,
            IEnumerable<string> ignoreTags = null)
        {
            return gameObject.HasTags(anyTags, TagFilter.ANY) 
                && gameObject.HasTags(requiredTags, TagFilter.ALL) 
                && gameObject.HasTags(ignoreTags, TagFilter.NONE);
        }
    }
}
