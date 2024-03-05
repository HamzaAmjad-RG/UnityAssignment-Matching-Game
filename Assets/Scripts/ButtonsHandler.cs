using UnityEngine;

public class ButtonsHandler : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerObject;
    private GameManger _gameManger;

    public void Start()
    {
        _gameManger = gameManagerObject.GetComponent<GameManger>();
    }

    public void OnReloadButtonPressed()
    {
        _gameManger.Reload();
    }

    public void OnHintButtonPressed()
    {
        _gameManger.GetHint();
    }
}
