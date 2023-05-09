using UnityEngine;

namespace Network.Administration.Selection
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
    public class Character : ScriptableObject
    {
        [SerializeField] private int id = -1;

        [SerializeField] private string displayName = "New Player";

        [SerializeField] private Sprite icon;
        
        
        public int Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
    }
}