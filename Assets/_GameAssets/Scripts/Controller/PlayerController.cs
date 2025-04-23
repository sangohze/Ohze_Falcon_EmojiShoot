using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelData;

public class PlayerController : Singleton<PlayerController>
{
    public WeaponType _playerWeapon;

    [SerializeField] List<GameObject> _listWeapon = new List<GameObject>();
    public void CheckWeaponInLevel()
    {
        for (int i = 0; i < _listWeapon.Count; i++)
        {
            if (i == (int)_playerWeapon)
            {
                _listWeapon[i].SetActive(true);
            }
            else
            {
                _listWeapon[i].SetActive(false);
            }
        }
        GamePlayController.I.InitWeaponLogic(_playerWeapon);
    }

    private void CheckSkinWeapon()
    {

    }    

}
