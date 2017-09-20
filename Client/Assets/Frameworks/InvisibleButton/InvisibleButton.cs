using UnityEngine;
using UnityEngine.UI;

namespace InvisButton
{
    [RequireComponent(typeof(Button))]
    public class InvisibleButton : Text
    {
        protected override void Awake()
        {
            base.Awake();
        }
    }
}
