using AUTD3Sharp;
using UnityEngine;

public class SimpleAUTDController : MonoBehaviour
{
    AUTD _autd = new AUTD();
    public GameObject Target;

    void Awake()
    {
        _autd = new AUTD();

        _autd.AddDevice(gameObject.transform.position, gameObject.transform.rotation);

        _autd.Open(LinkType.SOEM, "\\Device\\NPF_{161CCDCC-4E53-4982-9F4D-CF129EEA2AE8}");
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
