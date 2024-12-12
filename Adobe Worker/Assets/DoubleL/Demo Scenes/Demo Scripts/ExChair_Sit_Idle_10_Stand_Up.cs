using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubleL
{
    public class ExChair_Sit_Idle_10_Stand_Up : MonoBehaviour
    {
        public GameObject handWeapon;
        public GameObject floorWeapon;

        private void Start()
        {
            InitWeapon();
        }

        public void HideFloorWeapon()
        {
            handWeapon.SetActive(true);
            floorWeapon.SetActive(false);
        }

        public void InitWeapon()
        {
            handWeapon.SetActive(false);
            floorWeapon.SetActive(true);
        }
    }
}
