using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Network.Administration.Selection
{
    public class CharacterSelectButton: MonoBehaviour
    {
        [SerializeField] private Image iconImage;


        private CharacterSelectDisplay _characterSelect;
        private Character _character;
        

        private void SetCharacter(CharacterSelectDisplay characterSelect, Character character)
        {
            iconImage.sprite = character.Icon;

            _characterSelect = characterSelect;
            _character = character;
        }

        public void SelectCharacter()
        {
            _characterSelect.Select(_character);
        }
    }
}