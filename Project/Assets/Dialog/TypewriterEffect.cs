using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Coroutine Run(string textToType, TMP_Text _textLabel)
    {
        return StartCoroutine(TypeText(textToType, _textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text _textLabel)
    {
        _textLabel.text = string.Empty;

        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            _textLabel.text = textToType.Substring(0, charIndex);

            yield return null;
        }

        _textLabel.text = textToType;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
