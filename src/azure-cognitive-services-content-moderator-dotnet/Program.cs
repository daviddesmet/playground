using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.ContentModerator;
using Microsoft.Azure.CognitiveServices.ContentModerator.Models;
using Newtonsoft.Json;

namespace ContentModerator
{
    class Program
    {
        // Your Content Moderator subscription key is found in your Azure portal resource on the 'Keys' page.
        private static readonly string SubscriptionKey = "CONTENT_MODERATOR_SUBSCRIPTION_KEY";
        // Base endpoint URL. Found on 'Overview' page in Azure resource. For example: https://westus.api.cognitive.microsoft.com
        private static readonly string Endpoint = "CONTENT_MODERATOR_ENDPOINT";

        // TEXT MODERATION
        // Name of the file that contains text
        private static readonly string TextFile = "TextFile.txt";
        // The name of the file to contain the output from the evaluation.
        private static string TextOutputFile = "TextModerationOutput.txt";

        // IMAGE MODERATION
        //The name of the file that contains the image URLs to evaluate.
        private static readonly string ImageUrlFile = "ImageFiles.txt";
        // The name of the file to contain the output from the evaluation.
        private static string ImageOutputFile = "ImageModerationOutput.json";

        // The list of URLs of the images to create review jobs for.
        private static readonly string[] IMAGE_URLS_FOR_REVIEW = new string[] { "https://moderatorsampleimages.blob.core.windows.net/samples/sample5.png" };

        // The name of the team to assign the review to. Must be the team name used to create your Content Moderator website account. 
        // If you do not yet have an account, follow this: https://docs.microsoft.com/en-us/azure/cognitive-services/content-moderator/quick-start
        // Select the gear symbol (settings)-->Credentials to retrieve it. Your team name is the Id associated with your subscription.
        private static readonly string TEAM_NAME = "CONTENT_MODERATOR_TEAM_NAME";
        // The callback endpoint for completed human reviews.
        // For example: https://westus.api.cognitive.microsoft.com/contentmoderator/review/v1.0
        // As reviewers complete reviews, results are sent using an HTTP POST request.
        private static readonly string ReviewsEndpoint = "CONTENT_MODERATOR_REVIEWS_ENDPOINT";

        // It's always a good idea to access services in an async fashion
        static async Task Main()
        {
            // Create an image review client
            ContentModeratorClient clientImage = Authenticate(SubscriptionKey, Endpoint);
            // Create a text review client
            ContentModeratorClient clientText = Authenticate(SubscriptionKey, Endpoint);
            // Create a human reviews client
            ContentModeratorClient clientReviews = Authenticate(SubscriptionKey, Endpoint);

            // Moderate text from text in a file
            await ModerateTextAsync(clientText, TextFile, TextOutputFile);

            // Moderate images from list of image URLs
            await ModerateImagesAsync(clientImage, ImageUrlFile, ImageOutputFile);

            // Create image reviews for human reviewers
            await CreateReviewsAsync(clientReviews, IMAGE_URLS_FOR_REVIEW, TEAM_NAME, ReviewsEndpoint);
        }

        public static async Task ModerateTextAsync(ContentModeratorClient client, string inputFile, string outputFile)
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("TEXT MODERATION");
            Console.WriteLine();
            // Load the input text.
            var text = File.ReadAllText(inputFile);

            // Remove carriage returns
            text = text.Replace(Environment.NewLine, " ");
            // Convert string to a byte[], then into a stream (for parameter in ScreenText()).
            var textBytes = Encoding.UTF8.GetBytes(text);
            var stream = new MemoryStream(textBytes);

            Console.WriteLine("Screening {0}...", inputFile);
            // Format text

            // Save the moderation results to a file.
            using var outputWriter = new StreamWriter(outputFile, false);
            using (client)
            {
                // Screen the input text: check for profanity, classify the text into three categories,
                // do autocorrect text, and check for personally identifying information (PII)
                outputWriter.WriteLine("Autocorrect typos, check for matching terms, PII, and classify.");

                // Moderate the text
                var screenResult = await client.TextModeration.ScreenTextAsync("text/plain", stream, "eng", true, true, null, true);
                outputWriter.WriteLine(JsonConvert.SerializeObject(screenResult, Formatting.Indented));
            }

            outputWriter.Flush();
            outputWriter.Close();

            Console.WriteLine("Results written to {0}", outputFile);
            Console.WriteLine();
        }

        public static async Task ModerateImagesAsync(ContentModeratorClient client, string urlFile, string outputFile)
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("IMAGE MODERATION");
            Console.WriteLine();
            // Create an object to store the image moderation results.
            var evaluationData = new List<EvaluationData>();

            using (client)
            {
                // Read image URLs from the input file and evaluate each one.
                using var inputReader = new StreamReader(urlFile);
                while (!inputReader.EndOfStream)
                {
                    var line = inputReader.ReadLine().Trim();
                    if (line != String.Empty)
                    {
                        Console.WriteLine("Evaluating {0}...", Path.GetFileName(line));
                        var imageUrl = new BodyModel("URL", line.Trim());

                        var imageData = new EvaluationData
                        {
                            ImageUrl = imageUrl.Value,

                            // Evaluate for adult and racy content.
                            ImageModeration = await client.ImageModeration.EvaluateUrlInputAsync("application/json", imageUrl, true)
                        };

                        await Task.Delay(1000);

                        // Detect and extract text.
                        imageData.TextDetection = await client.ImageModeration.OCRUrlInputAsync("eng", "application/json", imageUrl, true);
                        await Task.Delay(1000);

                        // Detect faces.
                        imageData.FaceDetection = await client.ImageModeration.FindFacesUrlInputAsync("application/json", imageUrl, true);
                        await Task.Delay(1000);

                        // Add results to Evaluation object
                        evaluationData.Add(imageData);
                    }
                }
            }

            // Save the moderation results to a file.
            using var outputWriter = new StreamWriter(outputFile, false);
            outputWriter.WriteLine(JsonConvert.SerializeObject(evaluationData, Formatting.Indented));

            outputWriter.Flush();
            outputWriter.Close();

            Console.WriteLine();
            Console.WriteLine("Image moderation results written to output file: " + outputFile);
            Console.WriteLine();
        }

        private static async Task CreateReviewsAsync(ContentModeratorClient client, string[] ImageUrls, string teamName, string endpoint)
        {
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("CREATE HUMAN IMAGE REVIEWS");

            // The minimum amount of time, in milliseconds, to wait between calls to the Image List API.
            const int throttleRate = 2000;
            // The number of seconds to delay after a review has finished before getting the review results from the server.
            const int latencyDelay = 45;

            // The name of the log file to create. Relative paths are relative to the execution directory.
            const string OutputFile = "OutputLog.txt";

            // The optional name of the subteam to assign the review to. Not used for this example.
            const string Subteam = null;

            // The media type for the item to review. Valid values are "image", "text", and "video".
            const string MediaType = "image";

            // The metadata key to initially add to each review item. This is short for 'score'.
            // It will enable the keys to be 'a' (adult) and 'r' (racy) in the response,
            // with a value of true or false if the human reviewer marked them as adult and/or racy.
            const string MetadataKey = "sc";
            // The metadata value to initially add to each review item.
            const string MetadataValue = "true";

            // A static reference to the text writer to use for logging.
            TextWriter writer;

            // The cached review information, associating a local content ID to the created review ID for each item.
            var reviewItems = new List<ReviewItem>();

            using var outputWriter = new StreamWriter(OutputFile, false);
            writer = outputWriter;
            WriteLine(writer, null, true);
            WriteLine(writer, "Creating reviews for the following images:", true);

            // Create the structure to hold the request body information.
            var requestInfo = new List<CreateReviewBodyItem>();

            // Create some standard metadata to add to each item.
            var metadata = new List<CreateReviewBodyItemMetadataItem>(new CreateReviewBodyItemMetadataItem[] { new CreateReviewBodyItemMetadataItem(MetadataKey, MetadataValue) });

            // Populate the request body information and the initial cached review information.
            for (var i = 0; i < ImageUrls.Length; i++)
            {
                // Cache the local information with which to create the review.
                var itemInfo = new ReviewItem()
                {
                    Type = MediaType,
                    ContentId = i.ToString(),
                    Url = ImageUrls[i],
                    ReviewId = null
                };

                WriteLine(writer, $" {Path.GetFileName(itemInfo.Url)} with id = {itemInfo.ContentId}.", true);

                // Add the item informaton to the request information.
                requestInfo.Add(new CreateReviewBodyItem(itemInfo.Type, itemInfo.Url, itemInfo.ContentId, endpoint, metadata));

                // Cache the review creation information.
                reviewItems.Add(itemInfo);
            }

            var reviewResponse = await client.Reviews.CreateReviewsWithHttpMessagesAsync("application/json", teamName, requestInfo);

            // Update the local cache to associate the created review IDs with the associated content.
            var reviewIds = reviewResponse.Body;
            for (int i = 0; i < reviewIds.Count; i++) { reviewItems[i].ReviewId = reviewIds[i]; }

            WriteLine(outputWriter, JsonConvert.SerializeObject(reviewIds, Formatting.Indented));
            await Task.Delay(throttleRate);

            // Get details of the reviews created that were sent to the Content Moderator website.
            WriteLine(outputWriter, null, true);
            WriteLine(outputWriter, "Getting review details:", true);
            foreach (var item in reviewItems)
            {
                var reviewDetail = client.Reviews.GetReviewWithHttpMessagesAsync(teamName, item.ReviewId);
                WriteLine(outputWriter, $"Review {item.ReviewId} for item ID {item.ContentId} is " + $"{reviewDetail.Result.Body.Status}.", true);
                WriteLine(outputWriter, JsonConvert.SerializeObject(reviewDetail.Result.Body, Formatting.Indented));
                await Task.Delay(throttleRate);
            }

            Console.WriteLine();
            Console.WriteLine("Perform manual reviews on the Content Moderator site.");
            Console.WriteLine("Then, press any key to continue.");
            Console.ReadKey();

            // After the human reviews, the results are confirmed.
            Console.WriteLine();
            Console.WriteLine($"Waiting {latencyDelay} seconds for results to propagate.");
            await Task.Delay(latencyDelay * 1000);

            // Get details from the human review.
            WriteLine(writer, null, true);
            WriteLine(writer, "Getting review details:", true);
            foreach (var item in reviewItems)
            {
                var reviewDetail = client.Reviews.GetReviewWithHttpMessagesAsync(teamName, item.ReviewId);
                WriteLine(writer, $"Review {item.ReviewId} for item ID {item.ContentId} is " + $"{reviewDetail.Result.Body.Status}.", true);
                WriteLine(outputWriter, JsonConvert.SerializeObject(reviewDetail.Result.Body, Formatting.Indented));

                await Task.Delay(throttleRate);
            }

            Console.WriteLine();
            Console.WriteLine("Check the OutputLog.txt file for results of the review.");

            writer = null;
            outputWriter.Flush();
            outputWriter.Close();

            Console.WriteLine("--------------------------------------------------------------");
        }

        private static ContentModeratorClient Authenticate(string key, string endpoint)
        {
            var client = new ContentModeratorClient(new ApiKeyServiceClientCredentials(key));
            client.Endpoint = endpoint;

            return client;
        }

        // Helper function that writes a message to the log file, and optionally to the console.
        // If echo is set to true, details will be written to the console.
        private static void WriteLine(TextWriter writer, string message = null, bool echo = true)
        {
            writer.WriteLine(message ?? String.Empty);
            if (echo) { Console.WriteLine(message ?? String.Empty); }
        }
    }
}
