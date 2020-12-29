//using System.Collections.Generic;//using System.Collections;//using System.IO;//using UnityEngine;//using System.Linq;//using System;//using UnityEngine.Networking;////namespace iBicha.Github//{//    public class GithubHelper//    {//        const string API_ENDPOINT = "https://api.github.com/emojis";//        const string CACHE_FILENAME = "emojiDict.json";//        const string CACHE_FOLDER = "github";////        const int CACHE_EXPIRATION_HOURS = 30 * 24; //30 days////        //Map from keyword to url//        private static Dictionary<string, object> mapKeyword = new Dictionary<string, object>();////        //Map from unicode to url//        private static Dictionary<int, string> mapUnicode = new Dictionary<int, string>();////        public static bool IsInitialized { get; private set; }//        public static string Version { get; private set; }////        public static bool cacheEnabled = true;////        public static void ClearCache()//        {//            string folder = Path.Combine(Application.temporaryCachePath, CACHE_FOLDER);//            if (Directory.Exists(folder))//            {//                Directory.Delete(folder, true);//            }////            Version = null;//            IsInitialized = false;////            Debug.Log("Github cache cleared");//        }////        public static IEnumerator Initialize(bool forceRefresh = false)//        {//            if (IsInitialized && !forceRefresh)//                yield break;////            string folder = Path.Combine(Application.temporaryCachePath, CACHE_FOLDER);////            if (Directory.Exists(folder))//            {//                var creationTime = Directory.GetCreationTime(folder);//                var difference = DateTime.Now.Subtract(creationTime);//                if (difference.TotalHours > CACHE_EXPIRATION_HOURS)//                {//                    ClearCache();//                }//            }////            if (!Directory.Exists(folder))//            {//                Directory.CreateDirectory(folder);//            }////            string filename = Path.Combine(folder, CACHE_FILENAME);//            string jsonString = null;//            if (File.Exists(filename) && !forceRefresh && cacheEnabled)//            {//                jsonString = File.ReadAllText(filename);//            }//            else//            {//                using (var www = new WWW(API_ENDPOINT))//                {//                    yield return www; //api call//                    if (!(www.isDone && string.IsNullOrEmpty(www.error)))//                    {//                        yield break;//                    }////                    jsonString = www.text;//                    if (cacheEnabled)//                        File.WriteAllText(filename, jsonString);//                }//            }////            mapKeyword = Json.Deserialize(jsonString) as Dictionary<string, object>;////            var first = (string) mapKeyword.Values.First();////            if (first.Contains("?"))//                Version = first.Substring(first.IndexOf("?", StringComparison.Ordinal) + 1);////            LoadUnicodeMap();//            IsInitialized = true;//        }//////        public static void SetGithubEmoji(EmojiTexture emojiTexture, string text, Action callback)//        {//            if (string.IsNullOrEmpty(text))//            {//                callback();//                return;//            }////            if (!IsInitialized)//            {//                callback();//                return;//            }////            int unicode;////            if (text.StartsWith(":", StringComparison.Ordinal) && text.EndsWith(":", StringComparison.Ordinal)//                                                               && mapKeyword.ContainsKey(text.Trim(':')))//            {//                text = Path.GetFileNameWithoutExtension((string) mapKeyword[text.Trim(':')]);//            }////            if (text.Length == 1)//            {//                unicode = text[0];//            }//            else if (char.IsSurrogatePair(text, 0))//            {//                unicode = char.ConvertToUtf32(text, 0);//            }//            else if (!int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out unicode))//            {//                callback();//                return;//            }////            SetGithubEmoji(emojiTexture, unicode, callback);//            callback();//        }////        public static void SetGithubEmoji(EmojiTexture emojiTexture, int unicode, Action callback)//        {//            if (!IsInitialized)//            {//                callback();//                return;//            }////            if (!mapUnicode.ContainsKey(unicode))//            {//                callback();//                return;//            }////            string text = char.ConvertFromUtf32(unicode);////            string url = mapUnicode[unicode];////            string filename = "";//            bool isLocalFile = false;//            if (cacheEnabled)//            {//                string folder = Path.Combine(Application.temporaryCachePath, CACHE_FOLDER);//                filename = Path.GetFileNameWithoutExtension(url) + ".png";//                filename = Path.Combine(folder, filename);//                if (File.Exists(filename))//                {//                    url = "file://" + filename;//                    isLocalFile = true;//                }//            }////            var req = UnityWebRequestTexture.GetTexture(url);//            emojiTexture.didDownloadTexture = false;//            req.SendWebRequest().completed += operation =>//            {//                if (!req.isHttpError && !req.isNetworkError)//                {//                    emojiTexture.texture = ((DownloadHandlerTexture) req.downloadHandler).texture;//                    emojiTexture.didDownloadTexture = true;//                    if (!isLocalFile && cacheEnabled)//                    {//                        Texture2D texture = emojiTexture;//                        File.WriteAllBytes(filename, texture.EncodeToPNG());//                    }////                }//            };//        }////        public static bool IsValid(int unicode)//        {//            if (!IsInitialized)//                return false;////            return mapUnicode.ContainsKey(unicode);//        }////        public static bool IsValid(string text)//        {//            if (!IsInitialized)//                return false;////            int unicode;////            if (text.StartsWith(":", StringComparison.Ordinal) && text.EndsWith(":", StringComparison.Ordinal)//                                                               && mapKeyword.ContainsKey(text.Trim(':')))//            {//                text = Path.GetFileNameWithoutExtension((string) mapKeyword[text.Trim(':')]);//            }////            if (!int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out unicode))//            {//                return false;//            }////            if (!mapUnicode.ContainsKey(unicode))//            {//                return false;//            }////            return true;//        }////        private static void LoadUnicodeMap()//        {//            foreach (var pair in mapKeyword)//            {//                var hex = Path.GetFileNameWithoutExtension((string) pair.Value);//                int unicode;//                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out unicode))//                {//                    if (!mapUnicode.ContainsKey(unicode))//                        mapUnicode.Add(unicode, (string) pair.Value);//                }//            }//        }//    }//}