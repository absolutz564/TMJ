using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;

public class CapturePhotos : MonoBehaviour
{
    public GameObject BtnPreview;
    public GameObject Afterphoto;
    public RawImage RawVideoPlayer;
    public RawImage webcamRawImage; // Referência à RawImage que exibe o feed da webcam
    public int numberOfPhotos = 40; // Número de fotos a serem capturadas
    public float captureInterval = 3.0f / 40.0f; // Intervalo entre as capturas em segundos

    public List<Texture2D> capturedFrames; // Lista de frames capturados

    private int photoCount = 0;


    public float boomerangDuration = 1.5f; // Duração da aceleração no meio (segundos)

    public string outputName = "output.mp4";
    public int framerate = 40;

    private Process ffmpegProcess;
    string tempDirectory;
    string outputPath;
    string ffmpegPath;

    public async void FFMPEGConvertImagesToVideo()
    {
        await FFMPEGConvertImagesToVideoAsync();
    }

    private async Task FFMPEGConvertImagesToVideoAsync()
    {
        tempDirectory = Path.Combine(Application.dataPath, "TempFrames");
        outputPath = Path.Combine(Application.streamingAssetsPath, "ExportedVideos", outputName);
        ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpegOut/Windows", "ffmpeg.exe");

        try
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            else
            {
                foreach (var file in Directory.GetFiles(tempDirectory))
                {
                    File.Delete(file);
                }
            }

            int TamanhoTotal = capturedFrames.Count;
            int TamanhoIntervalo = TamanhoTotal / 3;

            List<Texture2D> novaLista = new List<Texture2D>();

            for (int i = 0; i < TamanhoTotal; i++)
            {
                string imageName = "frame_" + i.ToString("0000") + ".png";
                string imagePath = Path.Combine(tempDirectory, imageName);
                byte[] imageBytes = capturedFrames[i].EncodeToPNG();
                File.WriteAllBytes(imagePath, imageBytes);

                if (i < TamanhoIntervalo)
                {
                    novaLista.Add(capturedFrames[i]);
                }
                else if (i < 2 * TamanhoIntervalo)
                {
                    int index = (i - TamanhoIntervalo) / 2;
                    novaLista.Add(capturedFrames[index]);
                    novaLista.Add(capturedFrames[index]);
                }
                else
                {
                    int index = i - 2 * TamanhoIntervalo;
                    novaLista.Add(capturedFrames[index]);
                }
            }


            int bitrate = 0; // Deixe o bitrate como 0 para que o libx264 determine automaticamente a taxa de bits.
            float fRate = 30;
            string imagePaths = $"-framerate {fRate} -i \"{tempDirectory}/frame_%04d.png\"";
            string command = $"{imagePaths} -c:v libx264 -profile:v high -preset slower -crf 10 -vf \"scale=1920:1080, unsharp=5:5:1.0:5:5:0.0\" -pix_fmt yuv420p \"{outputPath}\"";

            //string command = $"{imagePaths} -c:v libx264 -b:v {bitrate} -vf \"scale=1920:1080\" -pix_fmt yuv420p \"{outputPath}\"";

            ProcessStartInfo processStartInfo = new ProcessStartInfo(ffmpegPath, command);
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(ffmpegPath);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            using (ffmpegProcess = new Process())
            {
                ffmpegProcess.StartInfo = processStartInfo;

                ffmpegProcess.Start();
                string errorOutput = ffmpegProcess.StandardError.ReadToEnd();
                UnityEngine.Debug.LogError("FFmpeg Error Output: " + errorOutput);

                bool outputFileCreated = false;
                await Task.Run(() =>
                {
                    ffmpegProcess.WaitForExit(); // Aguarde o término do processo
                    outputFileCreated = File.Exists(outputPath); // Verifique se o arquivo de saída foi criado
                });

                if (outputFileCreated)
                {
                    // ... (código de limpeza)
                    UnityEngine.Debug.Log("Video conversion finished. Output path: " + outputPath);
                }
                else
                {
                    UnityEngine.Debug.LogError("Output file not created.");
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error during video conversion: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            ffmpegProcess.Kill();
            ffmpegProcess.WaitForExit();

            if (File.Exists(outputPath))
            {
                // Delete the temporary image files
                foreach (var file in Directory.GetFiles(tempDirectory))
                {
                    File.Delete(file);
                }

                // Delete the temporary directory
                Directory.Delete(tempDirectory, true);
            }
        }
    }


    public void PlayCapturedFrames(float frameInterval)
    {
        if(!Afterphoto.activeSelf)
        {
            Afterphoto.SetActive(true);
            StartCoroutine(PlayFramesWithBoomerangRoutine(frameInterval, 10));
        }
    }

    private IEnumerator PlayFramesWithBoomerangRoutine(float frameInterval, int loopCount = -1)
    {
        if (capturedFrames.Count == 0)
        {
            UnityEngine.Debug.LogWarning("No frames to play.");
            yield break;
        }

        // Número de frames para aceleração no início e no final
        int numFramesWithAcceleration = 15; // Defina o número desejado de frames para aceleração
        float maxAccelerationFactor = 2f; // Fator máximo de aceleração

        while (loopCount != 0)
        {
            // Reprodução dos frames em ordem
            for (int i = 0; i < capturedFrames.Count; i++)
            {
                // Atualizar a RawImage para mostrar o frame atual
                RawVideoPlayer.texture = capturedFrames[i];

                // Calcular a interpolação para ajustar o intervalo de frames
                float t = (float)i / (float)(capturedFrames.Count - 1);

                // Aplicar aceleração nos primeiros e últimos frames de forma mais suave
                float accelerationFactor = 1.0f;
                if (i < numFramesWithAcceleration)
                {
                    accelerationFactor = Mathf.Lerp(1.0f, maxAccelerationFactor, t); // Aceleração no início
                }
                else if (i >= capturedFrames.Count - numFramesWithAcceleration)
                {
                    float tEnd = 1.0f - (float)(capturedFrames.Count - i - 1) / (float)numFramesWithAcceleration;
                    accelerationFactor = Mathf.Lerp(1.0f, maxAccelerationFactor, tEnd); // Aceleração no final
                }

                float interpolatedFrameInterval = frameInterval * accelerationFactor;

                yield return new WaitForSeconds(interpolatedFrameInterval);
            }

            if (loopCount > 0)
            {
                loopCount--;
            }

            // Ao final da reprodução, redefinir a RawImage para nulo
            RawVideoPlayer.texture = null;

            UnityEngine.Debug.Log("Finished playing captured frames with boomerang.");

            // Aguardar um breve momento antes de começar o próximo loop
            yield return new WaitForSeconds(frameInterval);
        }
    }



    public void StartCapture()
    {
        capturedFrames = new List<Texture2D>();

        // Iniciar a captura das fotos quando o script for ativado
        StartCoroutine(CapturePhotosRoutine());
    }

    private IEnumerator CapturePhotosRoutine()
    {
        while (photoCount < numberOfPhotos)
        {
            // Aguardar o intervalo de tempo definido
            yield return new WaitForSeconds(captureInterval);

            // Capturar a foto atual da RawImage da webcam e adicionar à lista
            CapturePhotoFromWebcam();

            photoCount++;
        }

        UnityEngine.Debug.Log("Captured all photos.");

        // Duplicar a lista e adicionar os elementos de forma reversa na lista original
        List<Texture2D> duplicatedFrames = new List<Texture2D>(capturedFrames);
        duplicatedFrames.Reverse();
        capturedFrames.AddRange(duplicatedFrames);

        // Ativar o botão de visualização
        BtnPreview.SetActive(true);
        FFMPEGConvertImagesToVideo();
    }

    private void CapturePhotoFromWebcam()
    {
        if (webcamRawImage != null)
        {
            // Capturar o quadro atual da RawImage (assumindo que a webcam já está sendo exibida nela)
            Texture2D texture = new Texture2D(webcamRawImage.texture.width, webcamRawImage.texture.height, TextureFormat.RGB24, false);
            RenderTexture previousActiveRenderTexture = RenderTexture.active;
            RenderTexture.active = webcamRawImage.texture as RenderTexture;
            texture.ReadPixels(new Rect(0, 0, webcamRawImage.texture.width, webcamRawImage.texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = previousActiveRenderTexture;

            // Adicionar a textura capturada à lista
            capturedFrames.Add(texture);

            UnityEngine.Debug.Log("Captured photo " + photoCount + " and added to the list.");
        }
    }
}
