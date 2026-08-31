using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    Animator anim;
    bool opened;

    void Start()
    {
        anim = GameObject.Find("Door").GetComponent<Animator>();
    }

    public void Interact()
    {
        Debug.Log("Door Interacted");
        if (!opened) anim.SetTrigger("Open");
        else anim.SetTrigger("Close");
        opened = !opened;
    }
}
