using System;
using UnityEngine;

public class PerformanceProfiler : MonoBehaviour
{
    [Header("Measurement Keys")]
    [SerializeField] private KeyCode startKey = KeyCode.F6;
    [SerializeField] private KeyCode stopKey = KeyCode.F7;

    private bool isRecording;

    private float elapsedSeconds;
    private int frameCount;

    private float frameTimeSumMs;
    private float maxFrameTimeMs;

    private int gcCountAtStart;

    private void Update()
    {
        // 측정 중이 아닐 때는 시작 키를 확인한다.
        if (!isRecording)
        {
            if (Input.GetKeyDown(startKey))
            {
                StartRecording();
            }

            return;
        }

        // 측정 중에는 프레임 통계를 기록한다.
        float deltaTime = Time.unscaledDeltaTime;
        float frameTimeMs = deltaTime * 1000f;

        elapsedSeconds += deltaTime;
        frameCount++;

        frameTimeSumMs += frameTimeMs;

        if (frameTimeMs > maxFrameTimeMs)
        {
            maxFrameTimeMs = frameTimeMs;
        }

        // 종료 키를 누르면 결과를 한 번 출력한다.
        if (Input.GetKeyDown(stopKey))
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        elapsedSeconds = 0f;
        frameCount = 0;

        frameTimeSumMs = 0f;
        maxFrameTimeMs = 0f;

        gcCountAtStart = GC.CollectionCount(0);

        isRecording = true;
    }

    private void StopRecording()
    {
        isRecording = false;

        if (frameCount == 0 || elapsedSeconds <= 0f)
        {
            Debug.LogWarning("측정 데이터가 부족합니다.");
            return;
        }

        float averageFPS = frameCount / elapsedSeconds;
        float averageFrameTimeMs = frameTimeSumMs / frameCount;

        int gcCountAtEnd = GC.CollectionCount(0);
        int gcCountDelta = gcCountAtEnd - gcCountAtStart;

        Debug.Log(
            $"[Profiler] Duration: {elapsedSeconds:F1}s" +
            $" | Average FPS: {averageFPS:F1}" +
            $" | Average Frame Time: {averageFrameTimeMs:F2} ms" +
            $" | Max Frame Time: {maxFrameTimeMs:F2} ms" +
            $" | GC Gen0 Count: {gcCountDelta}"
        );
    }
}