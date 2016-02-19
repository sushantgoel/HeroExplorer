using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace HeroExplorer
{
    public static class MarvelFacade
    {
        private const string privateKey = "45c21982e86b90b83889b9788be4b948e2d3adb7";
        private const string publicKey = "16fff087139c6d782eae205ce12f692d";
        private const int maxChar = 1500;
        private const string imageNotAvailable = "http://i.annihil.us/u/prod/marvel/i/mg/b/40/image_not_available";

        public static async Task populateMarvelCharactersAsync(ObservableCollection<Character> marvelCharacter)
        {
            try
            {
                var chacractersDataWrapper = await getCharacterDataWrapperAsync();
                var characters = chacractersDataWrapper.data.results;

                foreach (var character in characters)
                {
                    if (character.thumbnail != null && character.thumbnail.path != "" && character.thumbnail.path != imageNotAvailable)
                    {
                        character.thumbnail.small = string.Format("{0}/standard_small.{1}", character.thumbnail.path, character.thumbnail.extension);
                        character.thumbnail.large = string.Format("{0}/portrait_xlarge.{1}", character.thumbnail.path, character.thumbnail.extension);
                        marvelCharacter.Add(character);

                    }
                }
            }
            catch (Exception)
            {

                return;
            }

        }
        private async static Task<CharacterDataWrapper> getCharacterDataWrapperAsync()
        {
            //Assemble the URL 
            Random random = new Random();
            var offset = random.Next(maxChar);

            //Get the MD5 Hash
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hash = createHash(timeStamp);

            string url = string.Format("http://gateway.marvel.com:80/v1/public/characters?limit=10&offset={0}&apikey={1}&ts={2}&hash={3}", offset, publicKey, timeStamp, hash);

            //Call out to Marvel
            HttpClient http = new HttpClient();
            var response = await http.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            //Reponse ->string/json -> deserialize
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (CharacterDataWrapper) serializer.ReadObject(ms);

            return result;
        }

        private static string createHash(string timeStamp)
        {
            var toBeHashed = timeStamp + privateKey + publicKey;
            var hashedMessage = computeMD5(toBeHashed);
            return hashedMessage;
        }

        private static string computeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
