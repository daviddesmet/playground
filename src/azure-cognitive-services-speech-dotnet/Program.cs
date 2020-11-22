//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace SpeechToText
{
    class Program
    {
        // It's always a good idea to access services in an async fashion
        static async Task Main()
        {
            Console.WriteLine("Speech to Text");
            Console.WriteLine();
            Console.WriteLine("Type one of the following numbers:");
            Console.WriteLine("1. Use sample audio");
            Console.WriteLine("2. Use mic input");
            Console.WriteLine();

            Console.WriteLine("Enter 1 or 2: ");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    await RecognizeSpeechAsync();
                    break;
                case "2":
                    Console.WriteLine("Begin speaking....");
                    RecognizeSpeechUsingMicAsync().Wait();
                    Console.WriteLine("Please press <Return> to continue.");
                    Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Input not recognized! Maybe next time?");
                    break;
            }
        }

        static async Task RecognizeSpeechAsync()
        {
            // Setup the audio configuration, in this case, using a file that is in local storage.
            using (var audioInput = AudioConfig.FromWavFileInput("narration.wav"))

            // Pass the required parameters to the Speech Service which includes the configuration information
            // and the audio file name that you will use as input
            using (var recognizer = new SpeechRecognizer(GetDefaultSpeechConfig(), audioInput))
            {
                Console.WriteLine("Recognizing first result...");
                var result = await recognizer.RecognizeOnceAsync();

                ProcessResult(result);
            }
        }

        static async Task RecognizeSpeechUsingMicAsync()
        {
            // Pass the required parameters to the Speech Service which includes the configuration information
            using (var recognizer = new SpeechRecognizer(GetDefaultSpeechConfig()))
            {
                Console.WriteLine("Recognizing first result...");
                var result = await recognizer.RecognizeOnceAsync();

                ProcessResult(result);
            }
        }

        static SpeechConfig GetDefaultSpeechConfig()
        {
            // Configure the subscription information for the service to access.
            // Use either key1 or key2 from the Speech Service resource you have created
            return SpeechConfig.FromSubscription("<KEY 1>", "<LOCATION>");
        }

        static void ProcessResult(SpeechRecognitionResult result)
        {
            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    // The file contained speech that was recognized and the transcription will be output
                    // to the terminal window
                    Console.WriteLine($"We recognized: {result.Text}");
                    break;
                case ResultReason.NoMatch:
                    // No recognizable speech found in the audio file that was supplied.
                    // Out an informative message
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    // Operation was cancelled
                    // Output the reason
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                    break;
            }
        }
    }
}
