## 圣骑士莉卡物语的BepinEx插件
插件源码来自反编译F95论坛[kumarin](https://f95zone.to/threads/the-fairy-tale-of-holy-knight-ricca-two-winged-sisters-v1-3-6-mogurasoft.89059/post-8620267)
##### 声明
由于本人是初学者并不能保证代码高效，以及也没有经过原作者同意进行的反编译。欢迎提交Pr和Issues
- **使用方法**

    1.方法一:下载Releases页面附带环境的压缩包解压至游戏根目录
    
    2.方法二:下载Releases页面发布的dll移动到游戏目录内的`BepInEx/plugins`，如果没有安装BepInEx请自行搜索安装教程


- **编译方法**

    1.通过Github Action 编译
    
    fork仓库创建test分支并且开启Github Action,提交push

    2.手动编译

    下载安装 [dotnetsdk6.0](https://dotnet.microsoft.com/zh-cn/download/dotnet)

    PowerShell、MSYS2.0等Shell控制台输入

    ```
    git clone https://github.com/ShiroNeri4u/ricca_uncensor_plugin.git
    cd ricca_uncensor_plugin
    dotnet build Ricca_Uncensor_Plugin.csproj
    ```

- **已知问题**

    1.修改服装破损度时，没有对破损对象进行判断导致一些特殊boss(魅魔姐姐)，也会同时会受到锁定服装破损度的影响

    2.秒杀功能也包括敌人攻击NPC同伴