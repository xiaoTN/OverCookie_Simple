using System;
using System.Collections;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Object = System.Object;

namespace TN.Common
{
    public static class FileUtils
    {
        public static async UniTask<string> ReadFileAsUniTask(string filePath)
        {
            return await ReadFileAsObservable(filePath).ToUniTask();
        }

        public static IObservable<string> ReadFileAsObservable(string filePath)
        {
            return Observable.Create<string>(observer =>
            {
                CompositeDisposable disposable = new CompositeDisposable();
                UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
                UnityWebRequestAsyncOperation webResuestOperation = unityWebRequest.SendWebRequest();
                webResuestOperation.AsObservable()
                    .Subscribe(operation =>
                    {
                        if (webResuestOperation.webRequest.error.IsNullOrWhitespace() == false)
                        {
                            Debug.LogError($"读取文件失败[{filePath}]");
                            observer.OnCompleted();
                            return;
                        }

                        string context = webResuestOperation.webRequest.downloadHandler.text;
                        observer.OnNext(context);
                        observer.OnCompleted();
                    })
                    .AddTo(disposable);
                return disposable;
            });
        }

        public static string ReadString(string path)
        {
            WWW www = new WWW(path);
            while (!www.isDone)
            {
            }

            string json = www.text;
            return json;
        }

        [Obsolete("Android环境不能使用")]
        public static string ReadOneLine(string filePath, int lineCount)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (true)
                    {
                        string context = sr.ReadLine();
                        if (lineCount == 0)
                        {
                            return context;
                        }

                        lineCount--;
                    }
                }
            }
        }

        public static void WriteToFileLocalPath(string context, string localPath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, localPath);
            WriteToFile(context, fullPath);
        }

        [Obsolete("Android环境不能使用")]
        public static void WriteToFile(string context, string fullPath)
        {
            CreateDirectoryIfNotExist(fullPath);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(context);
                }
            }
        }

        /// <summary>
        /// 使用分块写入：将大文件分成较小的块，逐个块写入，而不是一次性写入整个文件。这可以减少内存使用量。例如，可以使用 FileStream 类来实现分块写入。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <param name="chunkSize"></param>
        public static async UniTask WriteToFile(string filePath, byte[] data, int chunkSize)
        {
            CreateDirectoryIfNotExist(filePath);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                try
                {
                    for (int i = 0; i < data.Length; i += chunkSize)
                    {
                        int remainingBytes = data.Length - i;
                        int bytesToWrite = Mathf.Min(chunkSize, remainingBytes);
                        fileStream.Write(data, i, bytesToWrite);
                        Debug.Log($"写入中：{i/1024f/1024f:f2}M->{(i + bytesToWrite)/1024f/1024f:f2}M");
                        await UniTask.Yield();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"写入失败,{filePath}：{e}");
                }
            }
        }

        public static void CreateDirectoryIfNotExist(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
                Debug.Log($"创建文件夹:{dirPath}");
            }
        }

        /// <summary>
        /// 通过使用Android的原生文件写入方法，可以绕过Unity的内存限制。你可以使用 AndroidJavaObject 来调用Java代码。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public static void WriteToFile(string filePath, byte[] data)
        {
            CreateDirectoryIfNotExist(filePath);
            try
            {
                AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", filePath);
                AndroidJavaObject outputStream = new AndroidJavaObject("java.io.FileOutputStream", fileObject);

                outputStream.Call("write", data);
                outputStream.Call("close");
            }
            catch (Exception e)
            {
                Debug.LogError($"写入失败,{filePath}：{e}");
            }
        }
    }
}