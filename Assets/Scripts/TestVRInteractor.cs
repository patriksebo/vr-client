using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class TestVRInteractor : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    private Camera camera;

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    void Start()
    {
        GameObject parent = this.transform.parent.gameObject;
        camera = parent.GetComponentInChildren<Camera>();
    }
    
    void Update()
    {
        if (SteamVR_Actions._default.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            ChatManager.instance.OnButtonStart(camera);
        }
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.GetType() == typeof(Button))
            return;

        Button btn = e.target.GetComponent<Button>();
        ChatManager.instance.OnButtonClicked(btn);
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>() == null)
            return;
        
        laserPointer.color = Color.green;
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>() == null)
            return;
        
        laserPointer.color = Color.black;
    }
}
