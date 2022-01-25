using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;

namespace AzureCognitiveService_ComputerVision
{
    class Program
    {
        // <snippet_vars>
        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "*****************************";
        static string endpoint = "https://*******************************/";

        // <snippet_analyze_url>
        // URL image used for analyzing an image (image of puppy)
        private const string ANALYZE_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png";
        //private const string ANALYZE_URL_IMAGE = "https://cdn.oggito.com/images/full/2021/1/evrim%20(1).jpg";
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Cognitive Services Computer Vision - .NET quickstart example");
            Console.WriteLine();

            // <snippet_main_calls>
            // Create a client
            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            // Analyze an image to get features and other properties.
            AnalyzeImageUrl(client, ANALYZE_URL_IMAGE).Wait();
            Console.WriteLine("Finished...");
        }
        // <snippet_auth>
        /*
         * AUTHENTICATE
         * Creates a Computer Vision client used by each example.
         */
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        // <snippet_visualfeatures>
        /* 
         * ANALYZE IMAGE - URL IMAGE
         * Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult/gory content,
         * brands, celebrities, landmarks, color scheme, and image types.
         */
        public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };
            // </snippet_visualfeatures>

            // <snippet_analyze_call>
            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);
            string resultService = JsonConvert.SerializeObject(results);
            Console.WriteLine(JsonConvert.SerializeObject(results));
            // </snippet_analyze_call>

            // <snippet_describe>
            // Sunmarizes the image content.
            Console.WriteLine("Summary:");
            foreach (var caption in results.Description.Captions)
            {
                Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
            }
            Console.WriteLine();
            // </snippet_describe>

            // <snippet_categorize>
            // Display categories the image is divided into.
            Console.WriteLine("Categories:");
            foreach (var category in results.Categories)
            {
                Console.WriteLine($"{category.Name} with confidence {category.Score}");
            }
            Console.WriteLine();
            // </snippet_categorize>

            // <snippet_tags>
            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }
            Console.WriteLine();
            // </snippet_tags>

            // <snippet_objects>
            // Objects
            Console.WriteLine("Objects:");
            foreach (var obj in results.Objects)
            {
                Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
                  $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
            }
            Console.WriteLine();
            // </snippet_objects>

            // <snippet_faces>
            // Faces
            Console.WriteLine("Faces:");
            foreach (var face in results.Faces)
            {
                Console.WriteLine($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
                  $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
                  $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
            }
            Console.WriteLine();
            // </snippet_faces>

            // <snippet_adult>
            // Adult or racy content, if any.
            Console.WriteLine("Adult:");
            Console.WriteLine($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
            Console.WriteLine($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");
            Console.WriteLine($"Has gory content: {results.Adult.IsGoryContent} with confidence {results.Adult.GoreScore}");
            Console.WriteLine();
            // </snippet_adult>

            // <snippet_brands>
            // Well-known (or custom, if set) brands.
            Console.WriteLine("Brands:");
            foreach (var brand in results.Brands)
            {
                Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
                  $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
            }
            Console.WriteLine();
            // </snippet_brands>

            // <snippet_celebs>
            // Celebrities in image, if any.
            Console.WriteLine("Celebrities:");
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Celebrities != null)
                {
                    foreach (var celeb in category.Detail.Celebrities)
                    {
                        Console.WriteLine($"{celeb.Name} with confidence {celeb.Confidence} at location {celeb.FaceRectangle.Left}, " +
                          $"{celeb.FaceRectangle.Top}, {celeb.FaceRectangle.Height}, {celeb.FaceRectangle.Width}");
                    }
                }
            }
            Console.WriteLine();
            // </snippet_celebs>


            // <snippet_landmarks>
            // Popular landmarks in image, if any.
            Console.WriteLine("Landmarks:");
            foreach (var category in results.Categories)
            {
                if (category.Detail?.Landmarks != null)
                {
                    foreach (var landmark in category.Detail.Landmarks)
                    {
                        Console.WriteLine($"{landmark.Name} with confidence {landmark.Confidence}");
                    }
                }
            }
            Console.WriteLine();
            // </snippet_landmarks>

            // <snippet_color>
            // Identifies the color scheme.
            Console.WriteLine("Color Scheme:");
            Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
            Console.WriteLine("Accent color: " + results.Color.AccentColor);
            Console.WriteLine("Dominant background color: " + results.Color.DominantColorBackground);
            Console.WriteLine("Dominant foreground color: " + results.Color.DominantColorForeground);
            Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
            Console.WriteLine();
            // </snippet_color>

            // <snippet_type>
            // Detects the image types.
            Console.WriteLine("Image Type:");
            Console.WriteLine("Clip Art Type: " + results.ImageType.ClipArtType);
            Console.WriteLine("Line Drawing Type: " + results.ImageType.LineDrawingType);
            Console.WriteLine();
            // </snippet_type>
        }
        /*
         * END - ANALYZE IMAGE - URL IMAGE
         */
    }
}
