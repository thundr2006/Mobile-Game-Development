using UnityEngine;

public class PlayerEntity : Entity {
    public override void DealDamage(float amount)
    {
        Player.Singleton.DealDamage(amount);
    }
}
