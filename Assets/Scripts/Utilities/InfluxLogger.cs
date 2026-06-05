using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
    public static void UploadCsvInEditMode(string fileName)
    {
        string targetBucket = "ecosys_csv";
        string baseUrl = url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;

        string filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".logs", "InfluxDB", fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"[InfluxImport] A fájl nem található: {filePath}");
            return;
        }

        string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
        if (lines.Length <= 1)
        {
            Debug.LogError("[InfluxImport] A fájl üres vagy csak fejlécet tartalmaz.");
            return;
        }

        string[] headers = lines[0].Split(';');
        int stepIdx = System.Array.IndexOf(headers, "Step");
        int runIdIdx = System.Array.IndexOf(headers, "RunId");
        int aiModeIdx = System.Array.IndexOf(headers, "AiMode");

        if (stepIdx == -1 || runIdIdx == -1 || aiModeIdx == -1)
        {
            Debug.LogError("[InfluxImport] Hiányzó kötelező oszlopok! (Step, RunId, AiMode szükséges)");
            return;
        }

        string runId = lines[1].Split(';')[runIdIdx];

        string deleteUrl = $"{baseUrl}/api/v2/delete?org={org}&bucket={targetBucket}";
        string deleteJson = "{\"start\":\"1970-01-01T00:00:00Z\",\"stop\":\"2100-01-01T00:00:00Z\",\"predicate\":\"run_id=\\\"" + runId + "\\\"\"}";

        using (var deleteRequest = new UnityWebRequest(deleteUrl, "POST"))
        {
            deleteRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(deleteJson));
            deleteRequest.downloadHandler = new DownloadHandlerBuffer();
            deleteRequest.SetRequestHeader("Authorization", "Token " + token);
            deleteRequest.SetRequestHeader("Content-Type", "application/json");

            deleteRequest.SendWebRequest();
            while (!deleteRequest.isDone) { }

            if (deleteRequest.result != UnityWebRequest.Result.Success)
                Debug.LogWarning($"[InfluxImport] Törlési megjegyzés: {deleteRequest.error}");
        }

        bool isDeath = fileName.Contains("Death") || headers.Contains("Species");
        string measurement = isDeath ? "death" : "snapshot";
        StringBuilder lpPayload = new StringBuilder();

        for (int i = 1; i < lines.Length; i++)
        {
            string currentLine = lines[i].Replace("\r", "").Trim();
    
            if (string.IsNullOrWhiteSpace(currentLine)) continue;
    
            string[] cols = currentLine.Split(';');
            if (cols.Length != headers.Length) continue;

            string rowRunId = cols[runIdIdx];
            string rowAiMode = cols[aiModeIdx];
            int rowStep = int.Parse(cols[stepIdx]);

            System.DateTime baseDt = System.DateTime.ParseExact(rowRunId, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);
            long epochSeconds = (long)(baseDt.ToUniversalTime() - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds + rowStep;

            List<string> tags = new List<string> { "run_id=" + rowRunId, "ai_mode=" + rowAiMode };
            List<string> fields = new List<string>();

            if (isDeath)
            {
                int speciesIdx = System.Array.IndexOf(headers, "Species");
                int causeIdx = System.Array.IndexOf(headers, "Cause");
                if (speciesIdx != -1) tags.Add("species=" + cols[speciesIdx]);
                if (causeIdx != -1) tags.Add("cause=" + cols[causeIdx]);

                for (int j = 0; j < headers.Length; j++)
                {
                    if (j == stepIdx || j == runIdIdx || j == aiModeIdx || j == speciesIdx || j == causeIdx) continue;
                    string header = headers[j];
                    string val = cols[j];

                    if (header == "FCM_Weights")
                    {
                        if (val != "None" && !string.IsNullOrEmpty(val))
                        {
                            string[] weights = val.Split('/');
                            for (int w = 0; w < weights.Length; w++) fields.Add($"fcm_w_{w}={weights[w]}");
                        }
                        continue;
                    }

                    string fieldName = header.ToLower();
                    if (val.ToLower() == "true" || val.ToLower() == "false") fields.Add($"{fieldName}={val.ToLower()}");
                    else if (float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float num)) fields.Add($"{fieldName}={val}");
                    else fields.Add($"{fieldName}=\"{val}\"");
                }
            }
            else
            {
                for (int j = 0; j < headers.Length; j++)
                {
                    if (j == stepIdx || j == runIdIdx || j == aiModeIdx) continue;
                    string header = headers[j];
                    string val = cols[j];

                    string fieldName = FormatSnapshotHeader(header);
                    if (float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float num)) fields.Add($"{fieldName}={val}");
                    else fields.Add($"{fieldName}=\"{val}\"");
                }
            }

            fields.Add($"step={rowStep}");
            lpPayload.AppendLine($"{measurement},{string.Join(",", tags)} {string.Join(",", fields)} {epochSeconds}");
        }

        string writeUrl = $"{baseUrl}/api/v2/write?org={org}&bucket={targetBucket}&precision=s";
        using (var writeRequest = new UnityWebRequest(writeUrl, "POST"))
        {
            writeRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(lpPayload.ToString()));
            writeRequest.downloadHandler = new DownloadHandlerBuffer();
            writeRequest.SetRequestHeader("Authorization", "Token " + token);
            writeRequest.SetRequestHeader("Content-Type", "text/plain");

            writeRequest.SendWebRequest();
            while (!writeRequest.isDone) { }
            if (writeRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[InfluxImport] Hiba: {writeRequest.error} | {writeRequest.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"[InfluxImport] SIKER! A(z) {fileName} adatai törlés után feltöltve az '{targetBucket}' bucketbe (Edit Mode)!");
            }
        }
    }

    private static string FormatSnapshotHeader(string header)
    {
        if (header == "BunnyCount") return "bunny_count";
        if (header == "FoxCount") return "fox_count";
        if (header == "BunnyAvgAge") return "bunny_avg_age";
        if (header == "BunnyAvgSpeed") return "bunny_avg_speed";
        if (header == "BunnyMaxGen") return "bunny_max_generation";
        if (header == "FoxAvgAge") return "fox_avg_age";
        if (header == "FoxAvgSpeed") return "fox_avg_speed";
        if (header == "FoxMaxGen") return "fox_max_generation";
        if (header.StartsWith("BunnyState_")) return "bunny_state_" + header.Substring(11);
        if (header.StartsWith("FoxState_")) return "fox_state_" + header.Substring(9);
        if (header == "BunnyAvgW_Hunger") return "bunny_avg_w_hunger";
        if (header == "BunnyAvgW_Thirst") return "bunny_avg_w_thirst";
        if (header == "BunnyAvgW_Energy") return "bunny_avg_w_energy";
        if (header == "BunnyAvgW_Mate") return "bunny_avg_w_mate";
        if (header == "FoxAvgW_Hunger") return "fox_avg_w_hunger";
        if (header == "FoxAvgW_Thirst") return "fox_avg_w_thirst";
        if (header == "FoxAvgW_Energy") return "fox_avg_w_energy";
        if (header == "FoxAvgW_Mate") return "fox_avg_w_mate";
        if (header.StartsWith("BunnyAvgFcmW_")) return "bunny_avg_fcm_w_" + header.Substring(13);
        if (header.StartsWith("FoxAvgFcmW_")) return "fox_avg_fcm_w_" + header.Substring(11);
        return header.ToLower();
    }
}