using UnityEngine;
using UnityEngine.PostProcessing;

public class IntensifyMotionBlur : MonoBehaviour {

    private PostProcessingBehaviour _postProcessing;

    void Start()
    {
        _postProcessing = GetComponent<PostProcessingBehaviour>();
    }

    void Update()
    {
        var intensity = Mathf.Max(1 - BurnObject.intensitySkewed, 0);
        _postProcessing.profile.motionBlur.enabled = intensity > 0;
        _postProcessing.profile.motionBlur.settings = new MotionBlurModel.Settings(){
            shutterAngle = 270f,
            sampleCount = 10,
            frameBlending = intensity
        };
    }

}
