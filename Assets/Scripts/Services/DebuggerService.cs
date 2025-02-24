using BlockAndGun.Interfaces;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class DebuggerService : IDebuggerService
    {
        public int messageCount;
        public void DebugMessage(string message)
        {
            Debug.Log(message);
            messageCount++;
        }
    }
}