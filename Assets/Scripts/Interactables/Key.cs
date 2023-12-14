using Enums;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    [SerializeField] private KeyType keyType;
    private string onKeyObtainedMessage = "You got a key!";
    public void OnInteraction()
    {
        gameObject.SetActive(false);
        string keyName = keyType != KeyType.none ? keyType.ToString() + " " : string.Empty;
        onKeyObtainedMessage = $"You got a {keyName}key!";
        PlayerStatsManager.Instance.GetKey(keyType);
        MessageManager.Instance.OpenMessagePrompt(onKeyObtainedMessage);
    }

    public void OnPlayerEnter()
    {
    }

    public void OnPlayerExit()
    {

    }
}
