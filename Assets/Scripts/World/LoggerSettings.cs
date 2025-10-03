using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.World
{
    using UnityEngine;
    using Assets.Scripts.Animals.Common;

    public class LoggerSettings : MonoBehaviour
    {
        [Header("Debug Logger Settings")]
        [SerializeField] private DebugLogger.LogLevel logLevel = DebugLogger.LogLevel.Info;

        private void Awake()
        {
            DebugLogger.CurrentLevel = logLevel;
        }
    }

}
