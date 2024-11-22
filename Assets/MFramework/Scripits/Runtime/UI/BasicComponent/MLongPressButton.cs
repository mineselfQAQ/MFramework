using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections;

namespace MFramework
{
    public class MLongPressButton : Selectable, ISubmitHandler
    {
        [Serializable]
        /// <summary>
        /// Function definition for a button click event.
        /// </summary>
        public class ButtonClickedEvent : UnityEvent { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        public float longPressDuration = 1f;
        private bool isPressing;
        private float pressTime;
        private bool longPressTriggered;
        private PointerEventData tempEventData;

        protected MLongPressButton()
        { }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            isPressing = true;
            longPressTriggered = false;
            pressTime = Time.time;
            tempEventData = eventData;
        }

        public void Update()
        {
            //长按逻辑---当按下时间超过longPressDuration时触发Press()(即onClick事件)
            if (isPressing && !longPressTriggered)
            {
                if (Time.time - pressTime >= longPressDuration)
                {
                    if (tempEventData.button != PointerEventData.InputButton.Left) return;

                    longPressTriggered = true;
                    Press();
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            isPressing = false;
            tempEventData = null;
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}
