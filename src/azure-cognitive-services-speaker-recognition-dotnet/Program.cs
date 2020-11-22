//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace HelloWorld
{
    class Program
    {
        // It's always a good idea to access services in an async fashion
        static async Task Main()
        {
            Console.WriteLine("Give the profile a human-readable display name:");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = "Shy person";

            // persist profileMapping if you want to store a record of who the profile is
            var profileMapping = new Dictionary<string, string>();
            await VerificationEnroll(GetDefaultSpeechConfig(), profileMapping, name);

            Console.ReadLine();
        }

        public static async Task VerificationEnroll(SpeechConfig config, Dictionary<string, string> profileMapping, string name)
        {
            using (var client = new VoiceProfileClient(config))
            using (var profile = await client.CreateProfileAsync(VoiceProfileType.TextDependentVerification, "en-us"))
            {
                using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
                {
                    Console.WriteLine($"Enrolling profile id {profile.Id} with '{name}'.");
                    // give the profile a human-readable display name
                    profileMapping.Add(profile.Id, name);

                    VoiceProfileEnrollmentResult result = null;
                    while (result is null || result.RemainingEnrollmentsCount > 0)
                    {
                        Console.WriteLine("Speak the passphrase, \"My voice is my passport, verify me.\"");
                        result = await client.EnrollProfileAsync(profile, audioInput);
                        Console.WriteLine($"Remaining enrollments needed: {result.RemainingEnrollmentsCount}");
                        Console.WriteLine("");
                    }

                    if (result.Reason == ResultReason.EnrolledVoiceProfile)
                    {
                        await SpeakerVerify(config, profile, profileMapping);
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED {profile.Id}: ErrorCode={cancellation.ErrorCode} ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
            }
        }

        public static async Task SpeakerVerify(SpeechConfig config, VoiceProfile profile, Dictionary<string, string> profileMapping)
        {
            var speakerRecognizer = new SpeakerRecognizer(config, AudioConfig.FromDefaultMicrophoneInput());
            var model = SpeakerVerificationModel.FromProfile(profile);

            Console.WriteLine("Speak the passphrase to verify: \"My voice is my passport, please verify me.\"");
            var result = await speakerRecognizer.RecognizeOnceAsync(model);
            Console.WriteLine($"Verified voice profile for speaker {profileMapping[result.ProfileId]}, score is {result.Score}");
        }

        static SpeechConfig GetDefaultSpeechConfig()
        {
            // Configure the subscription information for the service to access.
            // Use either key1 or key2 from the Speech Service resource you have created
            return SpeechConfig.FromSubscription("<KEY 1>", "<LOCATION>");
        }
    }
}
