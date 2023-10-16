using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mystie.Gameplay
{
    public interface IGrabbable
    {
        void Grab(InteractBehavior interactor);
    }

    public interface IInteractible
    {
        void Interact(InteractBehavior interactor);
    }
}
