using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class MatrixBlender : MonoBehaviour
{
    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }

    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            Camera.main.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
            yield return 1;
        }
        Camera.main.projectionMatrix = dest;
    }
    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float startDuration, float endDuration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < startDuration)
        {
            Camera.main.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / startDuration);
            startDuration = Mathf.Lerp(startDuration, endDuration, (Time.time - startTime) / 100f);
            yield return 1;
        }
        Camera.main.projectionMatrix = dest;
    }

    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(Camera.main.projectionMatrix, targetMatrix, duration));
    }
    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float startDuration, float endDuration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(Camera.main.projectionMatrix, targetMatrix, startDuration, endDuration));
    }
    public Coroutine BlendToMatrix()
    {
        StopAllCoroutines();
        return null;
    }
}

