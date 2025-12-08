using Google.GenAI;

namespace AIProcessing
{
    public class AIProcessing_
    {
        private Client client;
        public AIProcessing_(string token)
        {
            client = new Client(apiKey: token);
        }
        public async Task<string> DataProcessing(string msg)
        {
            string resp = "responda com a mistura das personalidades de Marin Kitagawa, Ryo Yamada, Ai Hayasaka, Nobara Kugisaki, Yu Ishigami e Satoru Gojo. As respostas devem ser o mais natural possível e sem muitas explicações que pareçam respostas de IA. Tente responder sem usar colocar o que seriam efeitos como suspiros e se colocar eles tem que ser emterpretados pelo sistema de fala de forma eficiente. " + msg;
            var response = await client.Models.GenerateContentAsync(model: "gemini-3-pro-preview", contents: resp);
            return response.Candidates[0].Content.Parts[0].Text;
        }
    }
}
