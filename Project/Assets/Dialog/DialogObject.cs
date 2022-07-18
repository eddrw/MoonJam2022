using UnityEngine;

[CreateAssetMenu(menuName ="Dialog/DialogObject")]

public class DialogObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] [TextArea] private string[] imagesToDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string[] Dialogue => dialogue;
    public string[] ImagesToDisplay => imagesToDisplay;
}
