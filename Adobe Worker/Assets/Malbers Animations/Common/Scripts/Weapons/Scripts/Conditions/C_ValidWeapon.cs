namespace MalbersAnimations.Conditions
{
    [System.Serializable]
    public class C_ValidWeapon : MWeaponConditions
    {
        public override string DisplayName => "Weapons/Is Valid Weapon";
         

        public override bool _Evaluate()
        {
            return Target != null;
        }

        private void Reset() => Name = "Is the Object a valid weapon";
    }
}
