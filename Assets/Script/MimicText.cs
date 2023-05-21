using UnityEngine;
using TMPro;

public class MimicText : MonoBehaviour
{
    public TextMeshProUGUI MainText;
    public TextMeshProUGUI ToMimicText;

    // Update is called once per frame
    void Update()
    {
        ToMimicText.text = MainText.text;
    }
}
