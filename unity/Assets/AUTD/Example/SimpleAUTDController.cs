using AUTD3Sharp;
using UnityEngine;

public class SimpleAUTDController : MonoBehaviour
{
    AUTD _autd = new AUTD();
    Link _link;
    public GameObject Target;

    void Awake()
    {
        _autd = new AUTD();

        _autd.AddDevice(gameObject.transform.position, gameObject.transform.rotation);

        string ifname = "write your interface name here";
        _link = AUTD.SOEMLink(ifname, _autd.NumDevices);
        _autd.OpenWith(_link);

        _autd.Clear();
        _autd.Calibrate();

        _autd.AppendModulationSync(AUTD.SineModulation(150)); // 150 Hz
    }

    void Update()
    {
        if (Target != null)
            _autd.AppendGainSync(AUTD.FocalPointGain(Target.transform.position));
    }

    private void OnApplicationQuit()
    {
        _autd.Dispose();
    }
}
