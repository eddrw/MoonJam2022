using System.Collections;
using UnityEngine;
using TMPro;

// made this using this tutorial https://www.youtube.com/watch?v=C5xnB1dZA_w
public class Dialog : MonoBehaviour
{
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private TMP_Text _textMesh;

    public bool IsOpen { get; private set; }

    private TypewriterEffect typewriterEffect;

    void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        CloseDialogBox();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDialog(DialogObject dialogObject)
    {
        IsOpen = true;
        _dialogBox.SetActive(true);
        StartCoroutine(StepThroughDialog(dialogObject));
    }

    private IEnumerator StepThroughDialog(DialogObject dialogObject)
    {
        foreach (string dialog in dialogObject.Dialogue)
        {
            yield return typewriterEffect.Run(dialog, _textMesh);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0)); //todo probavbly change this
        }

        CloseDialogBox();
    }

    private void CloseDialogBox()
    {
        IsOpen = false;
        _dialogBox.SetActive(false);
        _textMesh.text = string.Empty;
    }
}
