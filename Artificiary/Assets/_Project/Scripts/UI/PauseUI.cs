using Mystie.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mystie.UI
{
    public class PauseUI : UIState
    {
        public override void DisplayState()
        {
            GameManager.Pause();
            base.DisplayState();
        }

        public override void CloseState()
        {
            base.CloseState();

            GameManager.Unpause();
        }

        public override void Pause()
        {
            Close();
        }
    }
}
