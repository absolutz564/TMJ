using UnityEngine;
using System.Collections.Generic;
using FfmpegUnity;
using System.Collections;

namespace FfmpegUnity.Sample
{
    public class VideoCreationFromTextureList : MonoBehaviour
    {
        //public List<Texture2D> capturedFrames = new List<Texture2D>();
        public string outputVideoFileName = "output_video.mp4";
        public FfmpegWriteFromTexturesCommand ffmpegCommand;
        public CapturePhotos capturePhotos;
        public void InitFrames()
        {
            if (capturePhotos.capturedFrames.Count == 0)
            {
                Debug.LogError("No captured frames to create video from.");
                return;
            }

            // Assuming you have assigned the FfmpegWriteFromTexturesCommand instance in the Inspector
            if (ffmpegCommand == null)
            {
                Debug.LogError("FfmpegWriteFromTexturesCommand is not assigned.");
                return;
            }

            StartCoroutine(ProcessTexturesAndFinish());
        }

        private IEnumerator ProcessTexturesAndFinish()
        {
            yield return StartCoroutine(ffmpegCommand.WriteTexture(capturePhotos.capturedFrames[0]));
            //foreach (Texture2D texture in capturePhotos.capturedFrames)
            //{
            //    yield return StartCoroutine(ffmpegCommand.WriteTexture(texture));
            //}

            // Now that all textures are processed, you can finish the video creation
            FinishVideoCreation();
        }

        private void FinishVideoCreation()
        {
            // This is where you might want to do any finalization or clean-up
            Debug.Log("Video creation complete.");
        }
    }
}
