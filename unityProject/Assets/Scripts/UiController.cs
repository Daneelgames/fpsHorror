using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiController : MonoBehaviour
{
    public static UiController instance;

    public Image interactionIcon;
    public Image leftWeapon;
    public TextMeshProUGUI leftWeaponAmmoText;
    public Image rightWeapon;
    public TextMeshProUGUI rightWeaponAmmoText;

    PlayerController pc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pc = GameManager.instance.pc;
        UpdateWeaponIcons();
        UpdateInteractionIcon();
    }

    public void UpdateWeaponIcons()
    {
        if (pc.weaponLeft)
        {
            leftWeapon.gameObject.SetActive(true);
            leftWeaponAmmoText.text = "" + pc.weaponLeft.ammo;
        }
        else
        {
            leftWeapon.gameObject.SetActive(false);
        }

        if (pc.weaponRight)
        {
            rightWeapon.gameObject.SetActive(true);
            rightWeaponAmmoText.text = "" + pc.weaponRight.ammo;
        }
        else
        {
            rightWeapon.gameObject.SetActive(false);
        }
    }

    public void UpdateInteractionIcon()
    {
        if (pc.actionAreaController.interactableObjectControllers.Count > 0)
            interactionIcon.gameObject.SetActive(true);
        else
            interactionIcon.gameObject.SetActive(false);
    }
}