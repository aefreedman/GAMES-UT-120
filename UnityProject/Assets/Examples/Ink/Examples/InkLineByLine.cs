using Ink.Runtime;
using UnityEngine;

public class InkLineByLine : MonoBehaviour
{
    [SerializeField] private TextAsset _inkJsonAsset;
    [SerializeField] private Story _story;

    private void Start()
    {
        _story = new Story(_inkJsonAsset.text);
    }

    private void Update()
    {
        if (!Input.anyKeyDown) return;

        if (_story.currentChoices.Count > 0)
        {
            for (var i = 0; i < _story.currentChoices.Count; i++)
            {
                //KeyCode.Alpha1 = 49
                // "(KeyCode)49" casts 49 to the KeyCode enum
                if (Input.GetKeyDown((KeyCode) 49 + i))
                {
                    _story.ChooseChoiceIndex(i);
                }
            }
        }

        if (!_story.canContinue) return;

        var text = _story.Continue();
        var choiceText = "";

        if (_story.currentChoices.Count > 0)
            foreach (var t in _story.currentChoices)
            {
                choiceText += t.text + "\n";
            }

        Debug.LogFormat("{0} with choices:\n {1}", text, choiceText);
    }
}