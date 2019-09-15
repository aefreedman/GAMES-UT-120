using Patterns.Assets.PrototypingKit.Patterns;
using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    /// A manager class to track player input. If there's more than one input source, then this can't be a singleton.
    /// There's not actually a point to this class when using Unity's Input Manager and when the implementation
    /// is this simple, but it could expand to behave more like a real input manager if you needed to handle changing
    /// controls at runtime.
    /// </summary>
    public class InputHandler : Singleton<InputHandler>
    {
        [SerializeField] private bool UseFixedUpdate = true;

        [SerializeField] private string _verticalAxisName = "Vertical";
        [SerializeField] private string _horizontalAxisName = "Horizontal";

        public Vector2 InputAxis { get; set; }

        public static void SetInputAxisNames(string vertical, string horizontal)
        {
            Instance._verticalAxisName = vertical;
            Instance._horizontalAxisName = horizontal;
        }

        private void FixedUpdate()
        {
            if (UseFixedUpdate)
                InputAxis = new Vector2(Input.GetAxis(Instance._horizontalAxisName), Input.GetAxis(Instance._verticalAxisName));
        }

        private void Update()
        {
            if (!UseFixedUpdate)
                InputAxis = new Vector2(Input.GetAxis(Instance._horizontalAxisName), Input.GetAxis(Instance._verticalAxisName));
        }
    }
}