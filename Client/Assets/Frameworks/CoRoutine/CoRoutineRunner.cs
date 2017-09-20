using System;
using UnityEngine;

namespace CoRoutine
{
    public class CoRoutineRunner : MonoBehaviour
    {
        private static CoRoutineRunner _instance;
        public static CoRoutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoRoutineRunner");
                    _instance = go.AddComponent<CoRoutineRunner>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            CoRoutineRunner[] coRoutineRunners = FindObjectsOfType<CoRoutineRunner>();
            if (coRoutineRunners != null && coRoutineRunners.Length > 1)
            {
                throw new Exception("Can only have 1 CoRoutineRunner.");
            }
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
