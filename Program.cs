using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DetectFace
{
    class Program
    {
        // key = "0123456789abcdef0123456789ABCDEF"
        private const string key = "<key>";

        // You must use the same region as you used to get your subscription
        // keys. For example, if you got your subscription keys from westus,
        // replace "westcentralus" with "westus".
        //
        // Free trial subscription keys are generated in the westcentralus
        // region. If you use a free trial subscription key, you shouldn't
        // need to change the region.
        // Specify the Azure region
        private const string endPoint =
            "https://westcentralus.api.cognitive.microsoft.com";

        // path = @"C:\Documents\LocalImage.jpg"
        private const string path = @"<LocalImage>";

        private const string remoteImageUrl =
            "https://upload.wikimedia.org/wikipedia/commons/3/37/Dagestani_man_and_woman.jpg";

        private static readonly FaceAttributeType[] faceAtt =
            { FaceAttributeType.Age, FaceAttributeType.Gender };

        static void Main(string[] args)
        {
            FaceClient faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(key),
                new System.Net.Http.DelegatingHandler[] { });
            faceClient.Endpoint = endPoint;

            Console.WriteLine("Process started ...");
            var asyc1 = DetectRemoteAsync(faceClient, remoteImageUrl);
            var async2 = DetectLocaly(faceClient, path);

            Task.WhenAll(asyc1, async2).Wait(5000);
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        // Detect faces in a remote image
        private static async Task DetectRemoteAsync(
            FaceClient faceClient, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\nInvalid remoteImageUrl:\n{0} \n", imageUrl);
                return;
            }

            try
            {
                IList<DetectedFace> faces =
                    await faceClient.Face.DetectWithUrlAsync(
                        imageUrl, true, false, faceAtt);

                GetAttributes(GetfaceAtt(faces, imageUrl), imageUrl);
            }
            catch (APIErrorException e)
            {
                Console.WriteLine(imageUrl + ": " + e.Message);
            }
        }

        // Detect faces in a local image
        private static async Task DetectLocaly(FaceClient faceClient, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine(
                    "\nUnable to open or read path:\n{0} \n", imagePath);
                return;
            }

            try
            {
                using (Stream stream = File.OpenRead(imagePath))
                {
                    IList<DetectedFace> faces =
                            await faceClient.Face.DetectWithStreamAsync(
                                stream, true, false, faceAtt);
                    GetAttributes(
                        GetfaceAtt(faces, imagePath), imagePath);
                }
            }
            catch (APIErrorException e)
            {
                Console.WriteLine(imagePath + ": " + e.Message);
            }
        }

        private static string GetfaceAtt(
            IList<DetectedFace> faces, string imagePath)
        {
            string attributes = string.Empty;

            foreach (DetectedFace face in faces)
            {
                double? age = face.faceAtt.Age;
                string gender = face.faceAtt.Gender.ToString();
                attributes += gender + " " + age + "   ";
            }

            return attributes;
        }

        // Display the face attributes
        private static void GetAttributes(string attributes, string imageUri)
        {
            Console.WriteLine(imageUri);
            Console.WriteLine(attributes + "\n");
        }
    }
}

