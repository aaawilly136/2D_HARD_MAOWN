using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    private Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DamagePlayer(int damage)
    {
        health -= damage;
        if(!ani.GetBool("死亡") && health <= 0)
        {
            ani.SetBool("死亡" , health<=0);
        }
    }
}
