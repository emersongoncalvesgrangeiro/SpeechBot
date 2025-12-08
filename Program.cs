using dotenv.net;
using Recording;
using AIProcessing;

namespace Main
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            bool start = false;
            string msg2 = "";
            int value = 0;
            DotEnv.Load();
            string Key = Environment.GetEnvironmentVariable("Gemini");
            string path = Environment.GetEnvironmentVariable("local");
            string TTSKey = Environment.GetEnvironmentVariable("TTS");
            SpeechSystem speech = new SpeechSystem(path);
            AIProcessing_ aI = new AIProcessing_(Key);
            string response = "";
            Console.Clear();
            await speech.Run(async (msg) =>
            {
                if (msg.Contains("iniciar"))
                {
                    start = true;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("🗸");
                    msg = "";
                }
                else if (msg.Contains("parar"))
                {
                    start = false;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("🗙");
                }
                else if (msg.Contains("limpar"))
                {
                    Console.Clear();
                }
                if (start && msg != "" && msg != "parar" && msg != "limpar" && msg != "<unk>" && value == 0)
                {
                    msg2 = msg;
                    Console.WriteLine($"➯ {msg}");
                    response = await aI.DataProcessing(msg);
                    Console.WriteLine($"\t➯ {response}");
                }
                if (msg2 == msg)
                {
                    value = 1;
                }
                else
                {
                    value = 0;
                }
                Console.WriteLine(start ? "escutando" : "não escutando");
            });
        }
    }
}