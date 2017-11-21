using UnityEngine;

public class Explosive : MonoBehaviour {

	public bool destroyOnExplosion = true;
	public GameObject explosion;

    public void Trigger(float delay = 0f)
    {
        Invoke("_Explode", delay);
    }

    private void _Explode()
	{
		Instantiate(explosion, transform.position, Quaternion.identity);
		if(destroyOnExplosion)
		{
			Destroy(gameObject);
		}
	}
}
