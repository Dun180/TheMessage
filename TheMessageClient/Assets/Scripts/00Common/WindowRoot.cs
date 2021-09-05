//UI窗口基类



using UnityEngine;
using UnityEngine.UI;
public class WindowRoot : MonoBehaviour
{

    public virtual void InitWindow(){}
    public void SetWindowState(bool isActive = true)
    {
        if(gameObject.activeSelf != isActive){
            SetActive(gameObject,isActive);
        }
    }

    protected void SetActive(GameObject go, bool state = true){
        go.SetActive(state);
    }

    protected void SetActive(Transform trans, bool state = true){
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true){
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true){
        img.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true){
        txt.gameObject.SetActive(state);
    }
    protected void SetText(Transform trans,int num = 0){
        SetText(trans.GetComponent<Text>(),num.ToString());
    }
    protected void SetText(Text txt,int num = 0){
        SetText(txt,num.ToString());
    }
    protected void SetText(Transform trans,string context = ""){
        SetText(trans.GetComponent<Text>(),context);
    }
    protected void SetText(Text txt,string context = ""){
        txt.text = context;
    }

    protected void SetSprite(Image image,string path)
    {
        Sprite sp = GetRes<Sprite>(path);
        image.sprite = sp;
    }

    protected T GetRes<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load(path, typeof(T)) as T;
    }
}