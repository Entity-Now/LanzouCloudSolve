# LanzouCloudSolve

LanzouCloudSolve是一款用c#开发的蓝奏云直链解析库，可用于获取蓝奏云文件的直接下载链接。

## 下载


## 使用说明

`LanzouCloudSolve` 命名空间下的 `Lanzou` 类包含以下方法：
| 方法 | 说明 |
| ---- | ---- |
| GetLink | 获取蓝奏云链接的下载直链 |
| DownFile | 用于下载任意网络文件 |

### 示例代码

获取下载直链
```cs
Console.WriteLine("请输入蓝奏云的文件链接..");
string url = Console.ReadLine();

var Lanzou = await LanzouCloudSolve.Lanzou.GetLink(url);

Console.WriteLine("成功获取直接链接：" + Lanzou.GetLink());
```

完整使用案例
```cs
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
```

## 免责声明

此项目只用于学习目的，不支持任何非法用途。使用本程序下载文件时，请确保您遵守相关法律法规和下载网站的使用条款。本项目不承担任何因使用本程序而导致的任何形式的损失或损害。