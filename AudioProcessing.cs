using Google.Cloud.TextToSpeech.V1;
using System;
using System.IO;
using Raylib_cs;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace AudioSystem
{
    public class AudioSystem
    {
        private TextToSpeechClient client;
        private SynthesisInput input;
        private VoiceSelectionParams voice;
        private AudioConfig config;
        public AudioSystem(string token)
        {
            Raylib.InitAudioDevice();
            client = TextToSpeechClient.Create();
            voice = new VoiceSelectionParams
            {
                LanguageCode = "pt-BR",
                Name = "Autonoe"
            };
            config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };
        }
        public async Task Voice_System(string text)
        {
            input = new SynthesisInput { Text = text };
            var response = await client.SynthesizeSpeechAsync(input, voice, config);
            using (var output = File.Create("resp.mp3"))
            {
                response.AudioContent.WriteTo(output);
            }
        }
        public async Task Speeck()
        {

            File.Delete("resp.mp3");
        }
        public void Close()
        {
            Raylib.CloseAudioDevice();
        }
    }
}