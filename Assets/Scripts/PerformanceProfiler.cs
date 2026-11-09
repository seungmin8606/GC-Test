using System;
using UnityEngine;
using Unity.Profiling;

public class PerformanceProfiler : MonoBehaviour
{
    [Header("Measurement Settings")]
    [SerializeField] private KeyCode startKey = KeyCode.F6;
    [SerializeField] private KeyCode stopKey = KeyCode.F7;

    [SerializeField] private float measurementDuration = 60f;
    [SerializeField] private bool autoStop = true;

    [Header("Frame Data Settings")]
    [SerializeField] private int maxRecordedFrames = 60000;

    private bool isRecording;

    private float elapsedSeconds;
    private int frameCount;

    private double frameTimeSumMs;
    private float maxFrameTimeMs;

    // 측정 중 할당을 피하기 위해 미리 배열 확보
    private float[] frameTimesMs;

    // GC Collection Count
    private int gcCountAtStart;

    // GC Alloc 측정
    private ProfilerRecorder gcAllocRecorder;
    private long gcAllocTotalBytes;
    private long gcAllocMaxBytes;
    private int gcAllocSamples;
    private int gcAllocFrames;

    private void Awake()
    {
        maxRecordedFrames = Mathf.Max(1000, maxRecordedFrames);

        frameTimesMs = new float[maxRecordedFrames];

        // Unity Profiler의 GC Allocated In Frame 카운터
        gcAllocRecorder = ProfilerRecorder.StartNew(
            ProfilerCategory.Memory,
            "GC Allocated In Frame"
        );
    }

    private void Update()
    {
        if (!isRecording)
        {
            if (Input.GetKeyDown(startKey))
            {
                StartRecording();
            }

            return;
        }

        // 종료 입력이 측정 프레임에 영향을 주지 않도록
        // 측정 종료를 먼저 처리한다.
        if (Input.GetKeyDown(stopKey))
        {
            StopRecording();
            return;
        }

        float deltaTime = Time.unscaledDeltaTime;
        float frameTimeMs = deltaTime * 1000f;

        if (frameCount >= frameTimesMs.Length)
        {
            StopRecording();
            return;
        }

        // 프레임별 데이터 저장
        frameTimesMs[frameCount] = frameTimeMs;

        frameCount++;
        elapsedSeconds += deltaTime;

        frameTimeSumMs += frameTimeMs;

        if (frameTimeMs > maxFrameTimeMs)
        {
            maxFrameTimeMs = frameTimeMs;
        }

        // GC Alloc 카운터 수집
        // LastValue는 마지막으로 완료된 Profiler 샘플이다.
        if (gcAllocRecorder.Valid &&
            gcAllocRecorder.Count > 0)
        {
            long allocatedBytes = gcAllocRecorder.LastValue;

            if (allocatedBytes >= 0)
            {
                gcAllocTotalBytes += allocatedBytes;
                gcAllocSamples++;

                if (allocatedBytes > 0)
                {
                    gcAllocFrames++;
                }

                if (allocatedBytes > gcAllocMaxBytes)
                {
                    gcAllocMaxBytes = allocatedBytes;
                }
            }
        }

        // 지정된 시간에 도달하면 자동 종료
        if (autoStop &&
            elapsedSeconds >= measurementDuration)
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        elapsedSeconds = 0f;
        frameCount = 0;

        frameTimeSumMs = 0.0;
        maxFrameTimeMs = 0f;

        gcAllocTotalBytes = 0;
        gcAllocMaxBytes = 0;
        gcAllocSamples = 0;
        gcAllocFrames = 0;

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

        // 기본 성능 지표
        double averageFPS = frameCount / elapsedSeconds;
        double averageFrameTimeMs =
            frameTimeSumMs / frameCount;

        // GC Collection Count
        int gcCountAtEnd = GC.CollectionCount(0);
        int gcCollectionCount =
            gcCountAtEnd - gcCountAtStart;

        // 백분위수 계산
        // 측정이 끝난 뒤에만 정렬한다.
        Array.Sort(frameTimesMs, 0, frameCount);

        float p95FrameTime = GetPercentile(0.95f);
        float p99FrameTime = GetPercentile(0.99f);

        // 가장 느린 1% 프레임의 평균 시간 기반 FPS
        int slowFrameCount =
            Mathf.Max(1, Mathf.CeilToInt(frameCount * 0.01f));

        double slowFrameTimeSum = 0.0;

        for (int i = frameCount - slowFrameCount;
             i < frameCount;
             i++)
        {
            slowFrameTimeSum += frameTimesMs[i];
        }

        double averageSlowFrameTime =
            slowFrameTimeSum / slowFrameCount;

        double onePercentLowFPS =
            averageSlowFrameTime > 0.0
                ? 1000.0 / averageSlowFrameTime
                : 0.0;

        // 기본 결과
        string result =
            "[Performance Result]" +
            "\nDuration: " + elapsedSeconds.ToString("F2") + " s" +
            "\nTotal Frames: " + frameCount +
            "\nAverage FPS: " + averageFPS.ToString("F2") +
            "\nAverage Frame Time: " +
                averageFrameTimeMs.ToString("F3") + " ms" +
            "\nMax Frame Time: " +
                maxFrameTimeMs.ToString("F3") + " ms" +
            "\nP95 Frame Time: " +
                p95FrameTime.ToString("F3") + " ms" +
            "\nP99 Frame Time: " +
                p99FrameTime.ToString("F3") + " ms" +
            "\n1% Low FPS: " +
                onePercentLowFPS.ToString("F2") +
            "\nGC Collection Count: " + gcCollectionCount;

        // GC Alloc 결과
        if (gcAllocSamples > 0)
        {
            double averageGCAlloc =
                (double)gcAllocTotalBytes / gcAllocSamples;

            double totalGCAllocKB =
                gcAllocTotalBytes / 1024.0;

            double averageGCAllocKB =
                averageGCAlloc / 1024.0;

            double gcAllocKBPerSecond =
                totalGCAllocKB / elapsedSeconds;

            result +=
                "\nGC Alloc Samples: " + gcAllocSamples +
                "\nGC Alloc Total: " +
                    totalGCAllocKB.ToString("F2") + " KB" +
                "\nGC Alloc Average: " +
                    averageGCAllocKB.ToString("F4") + " KB/frame" +
                "\nGC Alloc Rate: " +
                    gcAllocKBPerSecond.ToString("F2") + " KB/s" +
                "\nGC Alloc Max Sample: " +
                    gcAllocMaxBytes + " B" +
                "\nGC Alloc Nonzero Samples: " +
                    gcAllocFrames;
        }
        else
        {
            result +=
                "\nGC Alloc: N/A (Profiler counter unavailable)";
        }

        Debug.Log(result);
    }

    private float GetPercentile(float percentile)
    {
        if (frameCount == 0)
        {
            return 0f;
        }

        int index = Mathf.CeilToInt(
            frameCount * percentile
        ) - 1;

        index = Mathf.Clamp(index, 0, frameCount - 1);

        return frameTimesMs[index];
    }

    private void OnDestroy()
    {
        gcAllocRecorder.Dispose();
    }
}