using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public static class ApplicationUtils
    {
        public static bool IsPlayingInEditor()
        {
#if UNITY_EDITOR
            return EditorApplication.isPlaying;
#else
            return false;
#endif
        }

        public static void Quit()
        {
            if (IsPlayingInEditor() == true)
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }

        }

    }

}
