using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.TextAnalytics;
using static System.Console;

namespace TextAnalytics
{
    public class Program
    {
        const string ANALYTICS_KEY = "REPLACE-WITH-YOUR-ANALYTICS-KEY";
        const string ANALYTICS_ENDPOINT = "REPLACE-WITH-YOUR-ANALYTICS-ENDPOINT";

        // It's always a good idea to access services in an async fashion
        public static async Task Main()
        {
            var credentials = new AzureKeyCredential(ANALYTICS_KEY);
            var endpoint = new Uri(ANALYTICS_ENDPOINT);

            var client = new TextAnalyticsClient(endpoint, credentials);

            await SentimentAnalysisExampleAsync(client);
            await SentimentAnalysisWithOpinionMiningExampleAsync(client);
            await LanguageDetectionExampleAsync(client);
            await EntityRecognitionExampleAsync(client);
            await EntityLinkingExampleAsync(client);
            await RecognizePIIExampleAsync(client);
            await KeyPhraseExtractionExampleAsync(client);

            Write("Press any key to exit.");
            ReadKey();
        }

        private static async Task SentimentAnalysisExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Sentiment analysis ******");
            WriteLine();

            var document = "I had the best day of my life. I wish you were there with me.";
            DocumentSentiment documentSentiment = await client.AnalyzeSentimentAsync(document);
            WriteLine($"Document: {document}");
            WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");

            foreach (var sentence in documentSentiment.Sentences)
            {
                WriteLine($"\tText: \"{sentence.Text}\"");
                WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
                WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
                WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
            }

            WriteLine();
        }

        private static async Task SentimentAnalysisWithOpinionMiningExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Opinion mining ******");
            WriteLine();

            var documents = new List<string>
            {
                "The food and service were unacceptable, but the concierge were nice."
            };

            AnalyzeSentimentResultCollection reviews = await client.AnalyzeSentimentBatchAsync(documents, options: new AnalyzeSentimentOptions()
            {
                AdditionalSentimentAnalyses = AdditionalSentimentAnalyses.OpinionMining
            });

            foreach (AnalyzeSentimentResult review in reviews)
            {
                WriteLine($"Document sentiment: {review.DocumentSentiment.Sentiment}\n");
                WriteLine($"\tPositive score: {review.DocumentSentiment.ConfidenceScores.Positive:0.00}");
                WriteLine($"\tNegative score: {review.DocumentSentiment.ConfidenceScores.Negative:0.00}");
                WriteLine($"\tNeutral score: {review.DocumentSentiment.ConfidenceScores.Neutral:0.00}\n");

                foreach (SentenceSentiment sentence in review.DocumentSentiment.Sentences)
                {
                    WriteLine($"\tText: \"{sentence.Text}\"");
                    WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                    WriteLine($"\tSentence positive score: {sentence.ConfidenceScores.Positive:0.00}");
                    WriteLine($"\tSentence negative score: {sentence.ConfidenceScores.Negative:0.00}");
                    WriteLine($"\tSentence neutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");

                    foreach (MinedOpinion minedOpinion in sentence.MinedOpinions)
                    {
                        WriteLine($"\tAspect: {minedOpinion.Aspect.Text}, Value: {minedOpinion.Aspect.Sentiment}");
                        WriteLine($"\tAspect positive score: {minedOpinion.Aspect.ConfidenceScores.Positive:0.00}");
                        WriteLine($"\tAspect negative score: {minedOpinion.Aspect.ConfidenceScores.Negative:0.00}");

                        foreach (OpinionSentiment opinion in minedOpinion.Opinions)
                        {
                            WriteLine($"\t\tRelated Opinion: {opinion.Text}, Value: {opinion.Sentiment}");
                            WriteLine($"\t\tRelated Opinion positive score: {opinion.ConfidenceScores.Positive:0.00}");
                            WriteLine($"\t\tRelated Opinion negative score: {opinion.ConfidenceScores.Negative:0.00}");
                        }
                    }
                }
                WriteLine($"\n");
            }

            WriteLine();
        }

        private static async Task LanguageDetectionExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Language detection ******");
            WriteLine();

            var document = "Ce document est rédigé en Français.";
            WriteLine($"Document: {document}");

            DetectedLanguage detectedLanguage = await client.DetectLanguageAsync(document);
            WriteLine("Language:");
            WriteLine($"\t{detectedLanguage.Name},\tISO-6391: {detectedLanguage.Iso6391Name}\n");

            WriteLine();
        }

        private static async Task EntityRecognitionExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Named Entity Recognition (NER) ******");
            WriteLine();

            var document = "I had a wonderful trip to Seattle last week.";
            WriteLine($"Document: {document}");

            var response = await client.RecognizeEntitiesAsync(document);
            Console.WriteLine("Named Entities:");
            foreach (var entity in response.Value)
            {
                WriteLine($"\tText: {entity.Text},\tCategory: {entity.Category},\tSub-Category: {entity.SubCategory}");
                WriteLine($"\t\tScore: {entity.ConfidenceScore:F2},\tLength: {entity.Length},\tOffset: {entity.Offset}\n");
            }

            WriteLine();
        }

        private static async Task EntityLinkingExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Entity linking ******");
            WriteLine();

            var document = "Microsoft was founded by Bill Gates and Paul Allen on April 4, 1975, " +
                "to develop and sell BASIC interpreters for the Altair 8800. " +
                "During his career at Microsoft, Gates held the positions of chairman, " +
                "chief executive officer, president and chief software architect, " +
                "while also being the largest individual shareholder until May 2014.";
            WriteLine($"Document: {document}");
            WriteLine();

            var response = await client.RecognizeLinkedEntitiesAsync(document);
            WriteLine("Linked Entities:");
            foreach (var entity in response.Value)
            {
                WriteLine($"\tName: {entity.Name},\tID: {entity.DataSourceEntityId},\tURL: {entity.Url}\tData Source: {entity.DataSource}");
                WriteLine("\tMatches:");

                foreach (var match in entity.Matches)
                {
                    WriteLine($"\t\tText: {match.Text}");
                    WriteLine($"\t\tScore: {match.ConfidenceScore:F2}");
                    WriteLine($"\t\tLength: {match.Length}");
                    WriteLine($"\t\tOffset: {match.Offset}\n");
                }
            }

            WriteLine();
        }

        private static async Task RecognizePIIExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Personally Identifiable Information recognition ******");
            WriteLine();

            var document = "A developer with SSN 859-98-0987 whose phone number is 800-102-1100 is building tools with our APIs.";
            WriteLine($"Document: {document}");
            WriteLine();

            PiiEntityCollection entities = await client.RecognizePiiEntitiesAsync(document);

            Console.WriteLine($"Redacted Text: {entities.RedactedText}");
            if (entities.Count > 0)
            {
                Console.WriteLine($"Recognized {entities.Count} PII entit{(entities.Count > 1 ? "ies" : "y")}:");
                foreach (PiiEntity entity in entities)
                {
                    Console.WriteLine($"Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
                }
            }
            else
            {
                Console.WriteLine("No entities were found.");
            }

            WriteLine();
        }

        private static async Task KeyPhraseExtractionExampleAsync(TextAnalyticsClient client)
        {
            WriteLine("****** Key phrase extraction ******");
            WriteLine();

            var document = "My cat might need to see a veterinarian.";
            WriteLine($"Document: {document}");

            var response = await client.ExtractKeyPhrasesAsync(document);

            // Printing key phrases
            Console.WriteLine("Key phrases:");

            foreach (string keyphrase in response.Value)
            {
                Console.WriteLine($"\t{keyphrase}");
            }

            WriteLine();
        }
    }
}
