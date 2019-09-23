using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ChangeSprite : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Renderer;

    [SerializeField]
    private Sprite[] Sprites;

    private int _index;

    private void Start()
    {
        if (Renderer == null)
            Renderer = GetComponent<SpriteRenderer>();

        SetSprite(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NextSprite();
    }

    private void SetSprite(int toSprite)
    {
        if (toSprite > Sprites.Length - 1)
            toSprite = Sprites.Length - 1;
        
        _index = toSprite;
        Renderer.sprite = Sprites[_index];
    }

    private void NextSprite()
    {
        _index++;
        if (_index >= Sprites.Length)
            _index = 0;
        
        SetSprite(_index);
    }
}