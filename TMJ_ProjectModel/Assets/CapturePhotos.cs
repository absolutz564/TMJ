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


    public float boomerangDuration = 1.0f; // Duração da aceleração no meio (segundos)

    public string outputName = "output.mp4";
    public int framerate = 12;
    public int compression = 20;
    public bool useAudio = false;
    public float audioStartTime = 0;
    public string audioPath = "";
    // Adicione esta função ao script existente

    public async void FFMPEGConvertImagesToVideo()
    {
        await FFMPEGConvertImagesToVideoAsync();
    }

    private async Task FFMPEGConvertImagesToVideoAsync()
    {
        string tempDirectory = Path.Combine(Application.dataPath, "TempFrames");
        string outputPath = Path.Combine(Application.streamingAssetsPath, "ExportedVideos", outputName);
        string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpeg", "ffmpeg.exe");

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

            for (int i = 0; i < capturedFrames.Count; i++)
            {
                string imageName = "frame_" + i.ToString("0000") + ".png";
                string imagePath = Path.Combine(tempDirectory, imageName);
                byte[] imageBytes = capturedFrames[i].EncodeToPNG();
                File.WriteAllBytes(imagePath, imageBytes);
            }
            float fRate = capturedFrames.Count / 3;

            string imagePaths = $"-framerate {fRate} -i \"{tempDirectory}/frame_%04d.png\"";
            string command = $"{imagePaths} -c:v libx264 -pix_fmt yuv420p \"{outputPath}\"";

            ProcessStartInfo processStartInfo = new ProcessStartInfo(ffmpegPath, command);
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(ffmpegPath);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.Start();

                bool outputFileCreated = false;

                // Use Task.Run to await the WaitForExit asynchronously
                await Task.Run(() =>
                {
                    //process.WaitForExit();

                    // Check every second if the output file has been created
                    while (!outputFileCreated)
                    {
                        UnityEngine.Debug.Log("Verificando arquivo mp4.");

                        if (File.Exists(outputPath))
                        {
                            outputFileCreated = true;
                        }
                        else
                        {
                            Task.Delay(1000).Wait(); // Wait for 1 second before checking again
                        }
                    }
                });

                if (outputFileCreated)
                {
                    // Delete the temporary image files
                    foreach (var file in Directory.GetFiles(tempDirectory))
                    {
                        File.Delete(file);
                    }

                    // Delete the temporary directory
                    Directory.Delete(tempDirectory, true);

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
