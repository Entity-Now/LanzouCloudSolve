namespace Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("请输入蓝奏云的文件链接..");
            string lanzouResult = await GetLink();

            Console.WriteLine("是否需要下载：Y/N");
            var yesOrNo = Console.ReadLine();
            
            if (!string.IsNullOrEmpty(yesOrNo) && (yesOrNo == "Y" || yesOrNo == "y"))
            {
                await DownFile(lanzouResult);
                Console.WriteLine("下载完成");
            }
            
            Console.WriteLine("按任意键结束程序.");
            Console.ReadLine();

            static async Task<string> GetLink()
            {
                string url = Console.ReadLine();

                var Lanzou = await LanzouCloudSolve.Lanzou.GetLink(url);

                Console.WriteLine("成功获取直接链接：" + Lanzou.GetLink());

                return Lanzou.GetLink();
            }

            static async Task DownFile(string url)
            {
                var progress = new Progress<double>((val)=> Console.WriteLine($"下载进度：{val}"));
                var cancelToken = new CancellationTokenSource();

                await LanzouCloudSolve.Lanzou.DownFile(url, "MyName.txt", progress, cancelToken.Token);
            }
        }
    }
}
