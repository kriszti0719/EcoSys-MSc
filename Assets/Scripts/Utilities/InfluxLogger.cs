using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class InfluxLogger
{
    private static string url;
    private static string token;
    private static string org;
    private static string bucket;

    private static MonoBehaviour coroutineRunner;

    public static void Init(string _url, string _token, string _org, string _bucket, MonoBehaviour runner)
    {
        url = _url;
        token = _token;
        org = _org;
        bucket = _bucket;
        coroutineRunner = runner;
    }

    private static string WriteUrl =>
        $"{url}/api/v2/write?org={org}&bucket={bucket}&precision=s";

    public static void Log(string line)
    {
        if (coroutineRunner == null)
        {
            Debug.LogError("InfluxLogger: coroutineRunner missing!");
            return;
        }

        coroutineRunner.StartCoroutine(Send(line));
    }

    private static IEnumerator Send(string body)
    {
        var request = new UnityWebRequest(WriteUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(body);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Authorization", "Token " + token);
        request.SetRequestHeader("Content-Type", "text/plain");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Influx error: " + request.error);
    }
}