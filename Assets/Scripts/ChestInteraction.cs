using UnityEngine;

public class ChestInteraction : MonoBehaviour, IInteractable
{
    Animator anim;
    bool opened;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Interact()
    {
        if (!opened) anim.SetTrigger("Open");
        else anim.SetTrigger("Close");
        opened = !opened;
    }

}
