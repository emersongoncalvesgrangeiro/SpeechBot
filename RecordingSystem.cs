using Vosk;
using OpenTK.Audio.OpenAL;

namespace Recording
{
    public class SpeechSystem
    {
        private string path;
        private readonly Model model;
        private readonly string deviceName;
        private const int samplerate = 48000;
        private const int buffersize = 8192;

        public event Action<string> speechDetected;
        public SpeechSystem(string Token)
        {
            path = Token;
            Vosk.Vosk.SetLogLevel(-1);
            deviceName = ALC.GetStringList(GetEnumerationStringList.CaptureDeviceSpecifier).FirstOrDefault();
            model = new Model(path);
        }
        public Task Run(Action<string> callback)
        {
            return Task.Run(() => Loop(callback));
        }
        private void Loop(Action<string> callback)
        {
            ALCaptureDevice microphone = ALC.CaptureOpenDevice(deviceName, samplerate, ALFormat.Mono16, buffersize);
            ALC.CaptureStart(microphone);
            byte[] buffer = new byte[buffersize];
            while (true)
            {
                string msg = Listen(buffer, microphone);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    callback(msg);
                }
            }
        }
        private string Listen(byte[] buffer, ALCaptureDevice microphone)
        {
            var rec = new VoskRecognizer(model, samplerate);
            DateTime lastspeech = DateTime.Now;
            bool finish = false;
            var message = "";
            while (!finish)
            {
                int samples = ALC.GetInteger(microphone, AlcGetInteger.CaptureSamples);
                if (samples > 0)
                {
                    int read = Math.Min(samples, buffersize / 2);
                    ALC.CaptureSamples(microphone, buffer, read); // OK
                    int bytes = read * 2;
                    double rms = 0;
                    for (int i = 0; i < read; i++)
                    {
                        short s = BitConverter.ToInt16(buffer, i * 2);
                        rms += s * s;
                    }
                    rms = Math.Sqrt(rms / read);
                    if (rms > 500)
                    {
                        lastspeech = DateTime.Now;
                    }
                    if (rec.AcceptWaveform(buffer, bytes))
                    {
                        var text = ExtractText(rec.Result());
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            message = text;
                        }
                    }
                }
                if ((DateTime.Now - lastspeech).TotalMilliseconds > 3000)
                {
                    finish = true;
                }
                Thread.Sleep(20);
            }
            string final = ExtractText(rec.FinalResult());
            return string.IsNullOrWhiteSpace(message) ? final.ToLower() : message.ToLower();
        }

        private string ExtractText(string json)
        {
            try
            {
                var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                return obj["text"]?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}