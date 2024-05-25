using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading.Tasks;
using FfmpegUnity.Sample;
using FfmpegUnity;
using NekraliusDevelopmentStudio;

public class CapturePhotos : MonoBehaviour
{
    public GameObject BtnPreview;
    public GameObject Afterphoto;
    public RawImage RawVideoPlayer;
    public RawImage RawVideoPlayerPreview;
    public RawImage webcamRawImage; // Referência à RawImage que exibe o feed da webcam
    public int numberOfPhotos = 40; // Número de fotos a serem capturadas
    public float captureInterval; // Intervalo entre as capturas em segundos

    public List<Texture2D> capturedFrames; // Lista de frames capturados

    private int photoCount = 0;


    public float boomerangDuration = 1.5f; // Duração da aceleração no meio (segundos)

    public string outputName = "output.mp4";
    public int framerate = 40;

    private Process ffmpegProcess;
    string tempDirectory;
    string outputPath;
    string ffmpegPath;

    public VideoCreationFromTextureList videoCreationFromTextures;
    public FfmpegCaptureCommand ffmpegcapture;
    public PhotoTaker videoUploader;
    public GameObject VideoUploadMessage;
    public FlashEffect flashEffect;

    public int VideoDuration;

    private void Start()
    {
        //captureInterval = VideoDuration / numberOfPhotos;
    }

    public async void FFMPEGConvertImagesToVideo()
    {
        await FFMPEGConvertImagesToVideoAsync();
    }

    private async Task FFMPEGConvertImagesToVideoAsync()
    {

        string tempDirectory = Path.Combine(Application.dataPath, "TempFrames");
        string audioFilePath = Path.Combine(Application.dataPath, "Music");
        string outputPath = Path.Combine(Application.streamingAssetsPath, "ExportedVideos", outputName);
        string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "FFmpegOut/Windows", "ffmpeg.exe");

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

            // Salvar os frames capturados como arquivos PNG na pasta temporária
            for (int i = 0; i < capturedFrames.Count; i++)
            {
                string imageName = "frame_" + i.ToString("0000") + ".png";
                string imagePath = Path.Combine(tempDirectory, imageName);
                byte[] imageBytes = capturedFrames[i].EncodeToPNG();
                File.WriteAllBytes(imagePath, imageBytes);
            }
            UnityEngine.Debug.Log("CHAMOU AQUI");

            // Calcular a taxa de quadros para um vídeo de VideoDuration
            int totalFrames = capturedFrames.Count;
            float frameRate = totalFrames / VideoDuration;

            // Configurar o comando FFmpeg para converter as imagens em um vídeo de 10 segundos
            string imagePaths = $"-framerate {frameRate} -i \"{tempDirectory}/frame_%04d.png\"";
            string audioOptions = $"-i \"{audioFilePath}\" -c:a aac -b:a 192k -ac 2 -ar 44100 -shortest";
            string command = $"{imagePaths} {audioOptions} -c:v libx264 -profile:v high -preset slower -crf {VideoDuration} -vf \"scale=1920:1080\" -pix_fmt yuv420p \"{outputPath}\"";


            ProcessStartInfo processStartInfo = new ProcessStartInfo(ffmpegPath, command)
            {
                WorkingDirectory = Path.GetDirectoryName(ffmpegPath),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process ffmpegProcess = new Process())
            {
                ffmpegProcess.StartInfo = processStartInfo;

                ffmpegProcess.Start();
                string errorOutput = await ffmpegProcess.StandardError.ReadToEndAsync();
                UnityEngine.Debug.LogError("FFmpeg Error Output: " + errorOutput);

                bool outputFileCreated = false;
                await Task.Run(() =>
                {
                    ffmpegProcess.WaitForExit(); // Aguarde o término do processo
                    outputFileCreated = File.Exists(outputPath); // Verifique se o arquivo de saída foi criado
                });

                if (outputFileCreated)
                {
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
            StartCoroutine(PlayFramesWithBoomerangRoutine(frameInterval));
        }
    }

    private IEnumerator PlayFramesWithBoomerangRoutine(float frameInterval, int loopCount = -1)
    {
        VideoUploadMessage.SetActive(true);

        if (capturedFrames.Count == 0)
        {
            UnityEngine.Debug.LogWarning("No frames to play.");
            yield break;
        }

        // Calcular o intervalo de quadro baseado na taxa de quadros desejada (fps)
        float frameTime = frameInterval; // Se frameInterval já está em segundos, não precisa mudar

        int previewCount = 1;
        while (loopCount != 0)
        {
            // Reprodução dos frames em ordem
            for (int i = 0; i < capturedFrames.Count; i++)
            {
                // Atualizar a RawImage para mostrar o frame atual
                RawVideoPlayer.texture = capturedFrames[i];
                RawVideoPlayerPreview.texture = capturedFrames[i];

                // Esperar pelo intervalo de frame especificado
                yield return new WaitForSeconds(frameTime);
            }

            if (loopCount > 0)
            {
                loopCount--;
            }

            if (previewCount > 0)
            {
                previewCount--;
                StartCoroutine(WaitStop());
                // Ao final da reprodução, redefinir a RawImage para nulo
                RawVideoPlayer.texture = null;
                RawVideoPlayerPreview.texture = null;
                RawVideoPlayerPreview.gameObject.SetActive(false);
                UnityEngine.Debug.Log("Finished playing captured frames.");
            }

            // Remover a espera após o loop completo para evitar pausas indesejadas
            // yield return new WaitForSeconds(frameInterval);
        }
    }


    IEnumerator WaitStop()
    {

        yield return new WaitForSeconds(0.85f);

        ffmpegcapture.Stop();

        string videoFilePath = Path.Combine(Application.streamingAssetsPath, "ExportedVideos", "capture.mp4");

        // Aguarda um momento para garantir que o arquivo seja fechado
        yield return new WaitForSeconds(1.0f);

        bool uploadSuccessful = false;

        while (!uploadSuccessful)
        {
            try
            {
                videoUploader.UploadVideo(videoFilePath);
                uploadSuccessful = true;
            }
            catch (IOException ex)
            {
                UnityEngine.Debug.LogWarning("Sharing violation, waiting and retrying: " + ex.Message);
            }

            yield return new WaitForSeconds(1.0f); // Aguarda 1 segundo antes de tentar novamente
        }

        UnityEngine.Debug.Log("Video upload locally complete");
    }


    public void StartCapture()
    {
        capturedFrames = new List<Texture2D>();

        // Iniciar a captura das fotos quando o script for ativado
        StartCoroutine(CapturePhotosRoutine());
    }

    private IEnumerator CapturePhotosRoutine()
    {
        flashEffect.StartFlashing();
        //flashEffect.FlashEffectUpdateLoop();
        while (photoCount < numberOfPhotos)
        {
            // Aguardar o intervalo de tempo definido
            yield return new WaitForSeconds(captureInterval);

            // Capturar a foto atual da RawImage da webcam e adicionar à lista
            CapturePhotoFromWebcam();

            photoCount++;
        }

        UnityEngine.Debug.Log("Captured all photos.");


        // Ativar o botão de visualização
        //BtnPreview.SetActive(true);
        //FFMPEGConvertImagesToVideo();
        //videoCreationFromTextures.InitFrames();
    }

    private void CapturePhotoFromWebcam()
    {
        if (webcamRawImage != null)
        {
            // Capturar o quadro atual da RawImage (assumindo que a webcam já está sendo exibida nela)
            Texture2D texture = new Texture2D(webcamRawImage.texture.width, webcamRawImage.texture.height, TextureFormat.RGBA32, false);
            RenderTexture previousActiveRenderTexture = RenderTexture.active;
            RenderTexture.active = webcamRawImage.texture as RenderTexture;
            texture.ReadPixels(new Rect(0, 0, webcamRawImage.texture.width, webcamRawImage.texture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = previousActiveRenderTexture;

            //Texture2D sharpenedTexture = ImageProcessingUtils.ApplySharpenFilter(texture, 1.0f);

            // Adicionar a textura capturada e processada à lista
            capturedFrames.Add(texture);

        }
    }
}
