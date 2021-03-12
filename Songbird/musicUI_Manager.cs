using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicUI_Manager : MonoBehaviour{

    private bool triggering;
    [SerializeField]
    protected GameObject musicUIObject;
    [SerializeField]
    protected PlayerController pc = null;
    [SerializeField]
    protected MusicScripts.SongManager songManager = null;
    [SerializeField]
    protected Animator interactButtonFade = null;

    protected bool musicLocked = true;

    public bool MusicLocked
    {
        get { return musicLocked; }
        set { musicLocked = value; }
    }

    //shows objects with showonpause tag
    public void showMUI()
    {
        musicUIObject.SetActive(true);
    }

    //hides objects with showonpause tag
    public void hideMUI()
    {
        musicUIObject.SetActive(false);
    }

    //determine if in range
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            triggering = true;
            
            if (!MusicLocked)
            {
                interactButtonFade.SetBool("isOpen", true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            triggering = false;

            if (!MusicLocked)
            {
                interactButtonFade.SetBool("isOpen", false);
            }
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        hideMUI();
    }

    private void Update()
    {
        if (triggering && !MusicLocked)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!pc.isInMenu)
                {
                    pc.isInMenu = true;
                    pc.animator.SetFloat("Speed", 0);
                    showMUI();
                    songManager.SongUiOpened();
                    interactButtonFade.SetBool("isOpen", false);
                }

                else if (pc.isInMenu)
                {
                    pc.isInMenu = false;
                    hideMUI();
                    songManager.StopAllCoroutines();
                    interactButtonFade.SetBool("isOpen", true);
                }
            }
        }
    }
}

