using UnityEngine;
using UnityEngine.SceneManagement; //scencemanger使用場景管理器

public class MenuManger : MonoBehaviour
{

    public void DelayStartGame()
    {
        
        SceneManager.LoadScene("遊戲畫面");
        Player.life = 2;
        
    }
    /// <summary>
    /// 開始遊戲
    /// </summary>
    public void Gamestart()
    {
        //延遲呼叫("方法名稱" ,延遲時間)
        Invoke("DelayStartGame", 1.1f);
    }

    /// <summary>
    /// 離開遊戲
    /// </summary>
    public void QuitGame()
    {
        Invoke("DelayQuitGame", 1.3f);

    }
    public void DelayQuitGame()
    {
        Application.Quit();
    }

}
