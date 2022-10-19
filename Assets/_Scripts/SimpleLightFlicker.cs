using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Adapted from: https://medium.com/geekculture/2d-light-flicker-in-unity-17554023693a 
public class SimpleLightFlicker : MonoBehaviour
{
    private Light2D _light;
    private bool flickIntensity = true;
    private float _baseIntensity;
    public float intensityRange = 0.2f;
    public float intensityTimeMin = .05f;
    public float intensityTimeMax = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light2D>();
        _baseIntensity = _light.intensity;
        StartCoroutine(FlickIntensity());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FlickIntensity()
    {
        float t0 = Time.time;
        float t = t0;
        WaitUntil wait = new WaitUntil(() => Time.time > t0 + t);
        yield return new WaitForSeconds(Random.Range(0.01f, 0.5f));

        while (true)
        {
            if (flickIntensity)
            {
                t0 = Time.time;
                float r = Random.Range(_baseIntensity - intensityRange, _baseIntensity + intensityRange);
                _light.intensity = r;
                t = Random.Range(intensityTimeMin, intensityTimeMax);
                yield return wait;
            }
            else yield return null;
        }
    }

    /* NOTE: The script needs to be set up for these still:
    private IEnumerator FlickPosition()
    {
        float t0 = Time.time;
        float t = t0;
        WaitUntil wait = new WaitUntil(() => Time.time > t0 + t);
        Vector3 shift = Vector3.zero;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.5f));

        while (true)
        {
            if (flickPosition)
            {
                t0 = Time.time;
                float r = Random.Range(0f, positionRadius);
                float theta = Random.Range(0.5f * (Mathf.PI - angle * Mathf.Deg2Rad),
                                           0.5f * (Mathf.PI + angle * Mathf.Deg2Rad));
                shift.x = r * Mathf.Cos(theta);
                shift.y = r * Mathf.Sin(theta);
                transform.position = _basePosition + shift;
                t = Random.Range(positionTimeMin, positionTimeMax);
                yield return wait;
            }
            else yield return null;
        }
    }

    private IEnumerator FlickColor()
    {
        float t0 = Time.time;
        float t = t0;
        WaitUntil wait = new WaitUntil(() => Time.time > t0 + t);
        yield return new WaitForSeconds(Random.Range(0.01f, 0.5f));

        while (true)
        {
            if (flickColor)
            {
                t0 = Time.time;
                Vector3ToColor(Random.insideUnitSphere * colorRadius + _colorVector);
                _light.color = _color;
                t = Random.Range(colorTimeMin, colorTimeMax);
                yield return wait;
                _light.color = _baseColor;
            }
            else yield return null;
        }
    }

    void Vector3ToColor(Vector3 v)
    {
        _color.r = v.x;
        _color.g = v.y;
        _color.b = v.z;
    }

    void ColorToVector3(Color c)
    {
        _colorVector.x = c.r;
        _colorVector.y = c.g;
        _colorVector.z = c.b;
    }
    */
}
