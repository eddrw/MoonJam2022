using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// made this using this tutorial https://www.youtube.com/watch?v=C5xnB1dZA_w
public class Dialog : MonoBehaviour
{
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private TMP_Text _textMesh;
    [SerializeField] private GameObject profileImage;
    [SerializeField] private Sprite[] sprites;

    private Dictionary<string, int> spriteDict;

    public bool IsOpen { get; private set; }

    private TypewriterEffect typewriterEffect;

    void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        CloseDialogBox();

        spriteDict = new Dictionary<string, int>();
        spriteDict.Add("billySmile", 0);
        spriteDict.Add("billyNeutral", 1);
        spriteDict.Add("billyEyesClosed", 2);
        spriteDict.Add("manletNeutral", 3);
        spriteDict.Add("manletAngry", 4);
        spriteDict.Add("manletSmile", 5);
        spriteDict.Add("henryNeutral", 6);
        spriteDict.Add("henrySmile", 7);
        spriteDict.Add("henryBigSmile", 8);
        spriteDict.Add("phantomNeutral", 9);
        spriteDict.Add("phantomSmile", 10);
        spriteDict.Add("corpaNeutral", 11);
        spriteDict.Add("corpaSad", 12);
        spriteDict.Add("corpaEyesUp", 13);
        spriteDict.Add("playMaster", 14);
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
        for (int i = 0; i < dialogObject.Dialogue.Length; i++)
        {
            string dialog = dialogObject.Dialogue[i];

            profileImage.GetComponent<Image>().sprite = sprites[spriteDict[dialogObject.ImagesToDisplay[i]]]; // i feel like this line might break
            yield return typewriterEffect.Run(dialog, _textMesh);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0)); 
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
