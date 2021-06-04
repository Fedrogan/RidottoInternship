using UnityEngine;

public class ReelInfo : MonoBehaviour
{    
    [SerializeField] private int reelId;
    private bool isStopping;

    public int ReelID { get => reelId; set => reelId = value; }

    public bool IsStopping { get => isStopping; set => isStopping = value; }
}
