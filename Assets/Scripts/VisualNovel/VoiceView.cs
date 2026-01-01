using UnityEngine;
using Yarn.Unity;

public class VoicePresenter : DialoguePresenterBase
{
    [Header("Settings")]
    public AudioSource voicePlayer;
    
    public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
    {
        if (voicePlayer != null) voicePlayer.Stop();
        
        string lineID = line.TextID;
        if (!string.IsNullOrEmpty(lineID))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/line_{lineID}");
            // Debug.Log($"Finding clip: line_{lineID}");
            
            if (clip != null && voicePlayer != null)
            {
                voicePlayer.clip = clip;
                voicePlayer.Play();
            }
            else
            {
                // Debug.LogWarning($"line_{lineID} {clip == null}, {voicePlayer == null}");
            }
        }
        
        // so we dont hold up the dialogue
        return YarnTask.CompletedTask;
    }
    
    public override YarnTask OnDialogueStartedAsync()
    {
        return YarnTask.CompletedTask;
    }

    public override YarnTask OnDialogueCompleteAsync()
    {
        // Optional: Ensure audio cuts off when the conversation ends entirely.
        if (voicePlayer != null) voicePlayer.Stop();
        return YarnTask.CompletedTask;
    }
}