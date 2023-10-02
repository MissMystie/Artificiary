using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.UI
{
    public class OverlayUI : UIState
    {
        public override void Escape()
        {
            Pause();
        }

        public override void Pause()
        {
            manager.SetState(pauseState);
        }
    }
}
