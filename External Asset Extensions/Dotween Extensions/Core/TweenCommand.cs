#if IW_DOTWEEN_EXTENSIONS
using UnityEngine;

namespace IW.DotweenExtensions
{
    public abstract class TweenCommand : ScriptableObject
    {
        public abstract void ExecuteCommand(GameObject gameObject);
    }
}
#endif