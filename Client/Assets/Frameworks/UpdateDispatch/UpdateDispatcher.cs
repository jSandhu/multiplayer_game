using System;
using UnityEngine;

namespace UpdateDispatch
{
    public class UpdateDispatcher : MonoBehaviour
    {
        public event Action Updated;
        public bool Active = true;

        private void Update()
        {
            if (Active && Updated != null)
            {
                Updated();
            }
        }
    }
}
