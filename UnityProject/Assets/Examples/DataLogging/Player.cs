using UnityEngine;

namespace Examples.DataLogging
{
    public class Player : MonoBehaviour
    {
        public KeyCode JumpKey;

        private void Update()
        {
            if (Input.GetKeyDown(JumpKey))
                Jump();
        }

        private void Jump()
        {
            // Log data using the singleton instance of the data logger (don't put this logic in the data logger)
            DataLogger.Instance.OnDidJump();
        }
    }
}