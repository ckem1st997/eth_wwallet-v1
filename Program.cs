using NBitcoin;
using Nethereum.HdWallet;
using Nethereum.Web3;

namespace eth_wwallet
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Input number thread:");
            int number = int.Parse(Console.ReadLine());

            // Biến để cache dữ liệu
            List<string> cachedData = new List<string>();
            HashSet<string> addData = new HashSet<string>();
            await AddData(Path.Combine(Environment.CurrentDirectory, "eth-list-address.txt"), addData);
            List<string> data = Wordlist.English.GetWords().ToList();
            // Tạo ra 5 Task để chạy hàm Check
            Task[] tasks = new Task[number < 1 ? 1 : number];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    // Gọi hàm Check và đợi kết quả
                    await Check(data, addData);
                });
            }

            // Đợi cho tất cả các Task hoàn thành
            await Task.WhenAll(tasks);

            Console.WriteLine("Tất cả các luồng đã hoàn thành.");
            //    await Check(data, addData);


            ////  List<string> data = await GetDataAsync(filePath1);
            //List<string> rd = new List<string>();

            //string mnemonicWords = "";
            //int count = 0;
            //int seedNum = 12;

            //Random random = new Random();
            //while (true)
            //{

            //    rd = new List<string>();
            //    var listRd = new List<int>();
            //    mnemonicWords = string.Empty;
            //    for (int i = 0; i < seedNum; i++)
            //    {
            //        bool b = true;
            //        while (b)
            //        {
            //            int randomIndex = random.Next(2048);
            //            var check = listRd.Where(x => x == randomIndex);
            //            if ((check == null || !check.Any()))
            //            {
            //                rd.Add(randomIndex.ToString());
            //                listRd.Add(randomIndex);
            //                mnemonicWords = mnemonicWords + " " + data[randomIndex];
            //                b = false;
            //            }
            //        }

            //    }
            //    mnemonicWords = mnemonicWords.Trim();
            //    if (!(!string.IsNullOrEmpty(mnemonicWords) && (mnemonicWords.Split(" ").Length == 12 || mnemonicWords.Split(" ").Length == 24))) continue;
            //    try
            //    {
            //        count++;
            //        var listAddress = new List<string>();

            //        // Tạo một ví mới từ seed
            //        Wallet wallet = new Wallet(mnemonicWords, null);
            //        string accountAddress44 = wallet.GetAccount(0).Address;
            //        if (!string.IsNullOrEmpty(accountAddress44))
            //            listAddress.Add(accountAddress44);
            //        Console.WriteLine($"[{count}]-{seedNum}");
            //        // Tạo và kiểm tra các loại địa chỉ khác nhau
            //        await DeriveAndCheckBalance(listAddress, filePath2, mnemonicWords);
            //        //if (addData.Count > 0)
            //        //    _ = Task.Run(async () =>
            //        //{
            //        //    await DeriveAndCheckBalance(listAddress, filePath2, mnemonicWords).ConfigureAwait(false);
            //        //});
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("\nException Caught!");
            //        Console.WriteLine("Message :{0} ", e.Message);
            //    }
            //}
        }

        static async Task Check(List<string> words, HashSet<string> addDataCheck)
        {

            string currentDirectory = Environment.CurrentDirectory;
            List<string> rd = new List<string>();
            string mnemonicWords = "";
            int count = 0;
            int seedNum = 12;

            Random random = new Random();
            while (true)
            {

                rd = new List<string>();
                var listRd = new List<int>();
                mnemonicWords = string.Empty;
                for (int i = 0; i < seedNum; i++)
                {
                    bool b = true;
                    while (b)
                    {
                        int randomIndex = random.Next(2048);
                        var check = listRd.Where(x => x == randomIndex);
                        if ((check == null || !check.Any()))
                        {
                            rd.Add(randomIndex.ToString());
                            listRd.Add(randomIndex);
                            mnemonicWords = mnemonicWords + " " + words[randomIndex];
                            b = false;
                        }
                    }

                }
                mnemonicWords = mnemonicWords.Trim();
                if (!(!string.IsNullOrEmpty(mnemonicWords) && (mnemonicWords.Split(" ").Length == 12 || mnemonicWords.Split(" ").Length == 24))) continue;
                try
                {
                    count++;
                    var listAddress = new List<string>();

                    // Tạo một ví mới từ seed
                    Wallet wallet = new Wallet(mnemonicWords, null);
                    string accountAddress44 = wallet.GetAccount(0).Address;
                    //if (!string.IsNullOrEmpty(accountAddress44))
                    //    listAddress.Add(accountAddress44);
                    Console.WriteLine($"[{count}]|{Task.CurrentId}-{accountAddress44}");
                    try
                    {
                        // Tạo địa chỉ từ master key và key path
                        // Kiểm tra xem địa chỉ có trong file CSV không
                        bool addressFound = false;
                        foreach (var VARIABLE in listAddress)
                        {
                            if (addDataCheck.Contains(VARIABLE))
                            {
                                addressFound = true;
                                break;
                            }
                        }
                        if (addressFound)
                        {
                            string output = $"12 Seed: {mnemonicWords} | address:{String.Join(", ", listAddress)}";
                            string filePath = Path.Combine(Environment.CurrentDirectory, "btc-wallet.txt");

                            await using (StreamWriter sw = File.AppendText(filePath))
                            {
                                await sw.WriteLineAsync(output);
                            }
                            Console.WriteLine($"Thông tin đã được ghi vào file cho địa chỉ: {String.Join(", ", listAddress)}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
        }


        //static async Task DeriveAndCheckBalance(List<string> listAddress, string csvFilePath, string mnemonicWords)
        //{
        //    try
        //    {
        //        // Tạo địa chỉ từ master key và key path
        //        // Kiểm tra xem địa chỉ có trong file CSV không
        //        bool addressFound = await AddressExistsInCsv(listAddress, csvFilePath);

        //        if (addressFound)
        //        {
        //            string output = $"12 Seed: {mnemonicWords} | address:{String.Join(", ", listAddress)}";
        //            string filePath = Path.Combine(Environment.CurrentDirectory, "btc-wallet.txt");

        //            await using (StreamWriter sw = File.AppendText(filePath))
        //            {
        //                await sw.WriteLineAsync(output);
        //            }
        //            Console.WriteLine($"Thông tin đã được ghi vào file cho địa chỉ: {String.Join(", ", listAddress)}");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("\nException Caught!");
        //        Console.WriteLine("Message :{0} ", e.Message);
        //    }
        //}


        static async Task AddData(string csvFilePath, HashSet<string> addDataCheck)
        {
            string? line = "";
            if (addDataCheck.Count < 1)
            {
                Console.WriteLine("begin aync data !");
                using (var reader = new StreamReader(csvFilePath))
                {
                    // Đọc từng dòng trong tệp
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                            addDataCheck.Add(line);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                Console.WriteLine("end aync data !");
                Console.WriteLine("data: " + addDataCheck.Count);
            }
        }

        //static async Task<bool> AddressExistsInCsv(List<string> listAddress, string csvFilePath)
        //{
        //    string? line = "";
        //    if (addData.Count < 1)
        //    {
        //        Console.WriteLine("begin aync data !");
        //        using (var reader = new StreamReader(csvFilePath))
        //        {
        //            // Đọc từng dòng trong tệp
        //            while ((line = await reader.ReadLineAsync()) != null)
        //            {
        //                if (!string.IsNullOrEmpty(line))
        //                    addData.Add(line);
        //            }
        //            reader.Close();
        //            reader.Dispose();
        //        }
        //        Console.WriteLine("end aync data !");
        //        Console.WriteLine("data: " + addData.Count);
        //    }
        //    foreach (var VARIABLE in listAddress)
        //    {
        //        if (addData.Contains(VARIABLE))
        //            return true;
        //    }
        //    return false;
        //}




        static async Task<List<string>> GetDataAsync(string filePath, List<string> cachedData)
        {
            // Nếu dữ liệu đã được cache, trả về dữ liệu từ cache
            if (cachedData != null && cachedData.Count > 0)
            {
                Console.WriteLine("Lấy dữ liệu từ cache.");
                return cachedData;
            }

            // Nếu chưa có dữ liệu trong cache, đọc từ file
            Console.WriteLine("Đọc dữ liệu từ file và cache nó.");
            cachedData = new List<string>();

            // Kiểm tra xem file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File không tồn tại.");
                return cachedData;
            }

            // Đọc file và lưu vào cache
            using (StreamReader reader = new StreamReader(filePath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    cachedData.Add(line);
                }
            }

            return cachedData;
        }

    }
}
