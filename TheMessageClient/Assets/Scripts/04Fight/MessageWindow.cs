//ս������


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageWindow : WindowRoot
{


    private AudioSvc audioSvc;
    private NetSvc netSvc;


    public override void InitWindow()
    {
        base.InitWindow();
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;



    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
