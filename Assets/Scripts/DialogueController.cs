using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public TMP_Text text;
    public string[,] dialogues;
    private int index = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        index = 0;
        dialogues = new string[,] {
            { "hu tao", "asidhasoid hello traveller god im writing a fanfic" },
            { "asdasdao", "pokdsapokdsapokdsa i wanna kms" } ,
            { "ddsaoiewoiwoiwoiw", "dsadsaqqnjssaaqqkjekwjwn"} ,
            { "ddsaoi222ewoiwoiwoiw", "dsadsaqqnjksddsdsjekwjwn"} ,
            { "ddsaoiewoiwo111iwoiw", "dsadsaqqnjkrerereejekwjwn"} ,
            { "ddsao444211iewoiwoiwoiw", "dsadsa24  vcvcvcc356qq77njkjekwjwn"},
        };
        text.SetText(FormatDialogue(index));
    }

    // c sarp is fucking WACK
    public void OnButtonClick()
    {
        if (index >= dialogues.GetLength(0) - 1)
        {
            Done();
            return;
        }
        text.SetText(FormatDialogue(++index));
    }

    private string FormatDialogue(int i)
    {
        return dialogues[i, 0] + ": " + dialogues[i, 1];
    }

    private void Done()
    {
        VisualNovelHandler.FinishScene();
    }
}
