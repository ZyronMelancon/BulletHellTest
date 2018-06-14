using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawnBehaviour : MonoBehaviour
{
    public float m_force;
    Rigidbody2D self;
    public List<GameObject> bullets;

	// Use this for initialization
	void Start ()
    {
        self = GetComponent<Rigidbody2D>();
	}

    public void SetAim(float angle)
    {
        self.MoveRotation(angle);
    }

    public void Rotate(float angle)
    {
        self.rotation += angle;
    }

    public void Spin(float speed)
    {
        self.angularVelocity = speed;
    }

    public void SetForce(float force)
    {
        m_force = force;
    }

    public void Shoot(int index)
    {
        var shot = Instantiate(bullets[index], self.position, new Quaternion());
        var shotrig = shot.GetComponent<Rigidbody2D>();

        shotrig.AddForce(transform.up * m_force * shotrig.mass);
        Destroy(shot, 5);
    }

    public void ShootCircle(UnityEngine.Object parameters)
    {
    }
}
