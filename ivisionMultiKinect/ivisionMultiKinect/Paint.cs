using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ivisionMultiKinect
{
    class Paint
    {
       public int Brightness(string dataset, int x)
        {
            string file = "";
            int cont = 0;
            try
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                        {
                            file = string.Concat(dataset, "\\cam0_image", i, ".png");
                        }
                        else
                        {
                            if (j == 1)
                            {
                                file = string.Concat(dataset, "\\cam1_image", i, ".png");
                            }
                            else
                            {
                                if (j == 2)
                                {
                                    file = string.Concat(dataset, "\\cam2_image", i, ".png");
                                }
                                else
                                {
                                    if (j == 3)
                                    {
                                        file = string.Concat(dataset, "\\cam3_image", i, ".png");
                                    }
                                }
                            }
                        }
                        
                        Bitmap originalImage = null;
                        using (var image = new Bitmap(file))
                        {
                            originalImage = new Bitmap(image);
                        }
                        Bitmap adjustedImage = new Bitmap(originalImage);
                        float brightness = 1.0f;
                        float contrast = 1.0f;
                        float gamma = 1.0f;

                        float adjustedBrightness = brightness - 1.0f;
                        // create matrix that will brighten and contrast the image
                        float[][] ptsArray ={
                        new float[] {contrast, 0, 0, 0, 0}, // scale red
                        new float[] {0, contrast, 0, 0, 0}, // scale green
                        new float[] {0, 0, contrast, 0, 0}, // scale blue
                        new float[] {0, 0, 0, 1.0f, 0}, // don't scale alpha
                        new float[] {adjustedBrightness, adjustedBrightness, adjustedBrightness, 0, 1}};

                        ImageAttributes imageAttributes = new ImageAttributes();
                        imageAttributes.ClearColorMatrix();
                        imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                        imageAttributes.SetGamma(gamma, ColorAdjustType.Bitmap);

                        if (j == 0)
                        {
                            file = string.Concat(dataset, "\\brilho0_image", i, ".png");
                        }
                        else
                        {
                            if (j == 1)
                            {
                                file = string.Concat(dataset, "\\brilho1_image", i, ".png");
                            }
                            else
                            {
                                if (j == 2)
                                {
                                    file = string.Concat(dataset, "\\brilho2_image", i, ".png");
                                }
                                else
                                {
                                    if (j == 3)
                                    {
                                        file = string.Concat(dataset, "\\brilho3_image", i, ".png");
                                    }
                                }
                            }
                        }

                        adjustedImage.Save(file);
                        cont++;
                    }

                }
                return cont;
            }
            catch 
            {
                return -1;
            }
        }
    }
}
