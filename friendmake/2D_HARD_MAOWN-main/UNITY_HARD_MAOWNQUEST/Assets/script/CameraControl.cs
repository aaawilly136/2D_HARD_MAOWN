using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region 欄位
    [Header("玩家的變形物件")]
    public Transform player;
    [Header("追蹤的速度"), Range(0, 100)]
    public float speed = 30;
    #endregion

    // 下 -0.65 上 0.65
    [Header("上下邊界")]
    public Vector2 limY = new Vector2(-0.2f, 0.44f);
    [Header("左右邊界")]
    public Vector2 limX = new Vector2(-1.17f, 106.2f);

    #region 方法
    private void Track()
    {
        Vector3 vCam = transform.position; //抓取攝影機座標
        Vector3 vPla = player.position; // 抓取玩家座標

        //利用插值讓攝影機座標朝玩家座標移動
        vCam = Vector3.Lerp(vCam, vPla, 0.5f * speed * Time.deltaTime);
        vCam.z = -10;
        //夾住 X 與 Y軸
        vCam.x = Mathf.Clamp(vCam.x, limX.x, limX.y);
        vCam.y = Mathf.Clamp(vCam.y, limY.x, limY.y);
        //更新攝影機座標
        transform.position = vCam;
    }
    #endregion
    #region 事件
    //延遲更新事件
    //在update 後執行
    //官方建議攝影機追蹤行為可在此事件呼叫執行
    private void LateUpdate()
    {
        Track();
    }

    #endregion
}
