using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PM.Utils
{

    public static class DebugExtensions
    {
        public static void DebugShowList<T>(List<T> list)
        {
            Debug.Log(string.Join(", ", list.Select(obj => obj.ToString())));
        }

        public static void DebugShowList<T>(IEnumerable<T> list)
        {
            Debug.Log(string.Join(", ", list.Select(obj => obj.ToString())));
        }

        public static void DebugShowList<T>(List<List<T>> lists)
        {
            string buffer = "";
            foreach (var list in lists)
                buffer += "{" + string.Join(", ", list.Select(obj => obj.ToString())) + "} ";
            Debug.Log(buffer);
        }
    }
}

