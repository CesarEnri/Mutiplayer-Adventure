using UnityEngine;
using UnityEngine.UI;

namespace Network.Administration.Selection
{
    public class CharacterSelectButton: MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject disabledOverlay;
        [SerializeField] private Button button;

        private CharacterSelectDisplay _characterSelect;
        public Character Character { get; private set; }

        public bool IsDisabled { get; private set; }


        public void SetCharacter(CharacterSelectDisplay characterSelect, Character character)
        {
            iconImage.sprite = character.Icon;

            _characterSelect = characterSelect;
            Character = character;
        }

        public void SelectCharacter()
        {
            _characterSelect.Select(Character);
        }

        public void SetDisabled()
        {
            IsDisabled = true;
            disabledOverlay.SetActive(true);
            button.interactable = false;
        }
    }
}