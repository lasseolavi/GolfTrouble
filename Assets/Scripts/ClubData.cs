using UnityEngine;

[CreateAssetMenu(fileName = "NewClub", menuName = "Golf/Club")]
public class ClubData : ScriptableObject
{
    public string clubName = "Club";
    public float launchAngle = 35f;     // degrees
    public float powerMultiplier = 3f;  // how "hard" the club hits for a given drag
    public float maxDragDistance = 4f;  // distance cap of the drag
    public Color color = Color.gray;
    public float length = 3f;
}