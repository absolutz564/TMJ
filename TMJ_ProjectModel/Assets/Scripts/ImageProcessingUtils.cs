using UnityEngine;

public class ImageProcessingUtils
{
    public static Texture2D ApplySharpenFilter(Texture2D inputTexture, float amount = 1.0f)
    {
        int width = inputTexture.width;
        int height = inputTexture.height;

        // Cria uma cópia da textura de entrada para aplicar o filtro
        Texture2D outputTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        outputTexture.SetPixels(inputTexture.GetPixels());

        // Defina a máscara de nitidez
        float[] kernel = {
            0, -1, 0,
            -1, 5, -1,
            0, -1, 0
        };

        // Normaliza o kernel
        float kernelSum = 1.0f;
        for (int i = 0; i < kernel.Length; i++)
        {
            kernel[i] *= amount / kernelSum;
        }

        // Aplica o filtro à textura
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Color newColor = ApplyKernel(inputTexture, x, y, kernel, width, height);
                outputTexture.SetPixel(x, y, newColor);
            }
        }

        outputTexture.Apply();
        return outputTexture;
    }

    private static Color ApplyKernel(Texture2D texture, int x, int y, float[] kernel, int width, int height)
    {
        Color result = Color.black;

        int kernelSize = Mathf.FloorToInt(Mathf.Sqrt(kernel.Length));
        int halfKernelSize = kernelSize / 2;

        for (int ky = 0; ky < kernelSize; ky++)
        {
            for (int kx = 0; kx < kernelSize; kx++)
            {
                int sampleX = x + kx - halfKernelSize;
                int sampleY = y + ky - halfKernelSize;

                Color sampleColor = texture.GetPixel(sampleX, sampleY);
                float weight = kernel[ky * kernelSize + kx];

                result += sampleColor * weight;
            }
        }

        return result;
    }
}
